using Microsoft.Management.Deployment;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsPackageManager;

public record PackageInfo(string Id, string Name, string? Version, IReadOnlyList<string> ProductCodes);

public record PackageDetails(string Id, string? Available, string? Source, bool IsUpdateAvailable,
    string? Publisher, string? Author, string? Description, string? PackageUrl, string? License, string? IconUrl);


public class PackageService
{
    private readonly Lazy<Task<PackageManager>> packageManager = new(() => Task.Run(() => new PackageManager()));
    private readonly Lazy<Task<PackageCatalog>> compositeCatalog;

    public PackageService()
    {
        compositeCatalog = new(GetCompositeCatalogAsync);
    }

    /// <summary>Fast: local installed catalog only, no remote correlation.</summary>
    public async Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation = default)
    {
        var manager = await packageManager.Value.ConfigureAwait(false);
        var catalogRef = manager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
        var connectResult = await catalogRef.ConnectAsync().AsTask(cancellation).ConfigureAwait(false);
        if (connectResult.Status != ConnectResultStatus.Ok)
            throw new InvalidOperationException($"Failed to connect to local catalog: {connectResult.Status}");

        var findResult = await connectResult.PackageCatalog.FindPackagesAsync(new FindPackagesOptions()).AsTask(cancellation).ConfigureAwait(false);
        if (findResult.Status != FindPackagesResultStatus.Ok)
            throw new InvalidOperationException($"Failed to find packages: {findResult.Status}");

        var matches = findResult.Matches;
        var results = new PackageInfo[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            var package = matches[i].CatalogPackage;
            Trace.WriteLine($"{package.Id}: ProductCodes: {string.Join(",", package.InstalledVersion?.ProductCodes?.ToArray() ?? [])}");
            results[i] = new PackageInfo(package.Id, package.Name, package.InstalledVersion?.Version, package.InstalledVersion?.ProductCodes?.ToArray() ?? []);
        }
        return results;
    }

    /// <summary>Slow: correlates a single package with remote sources and fills details.</summary>
    public async Task<PackageDetails?> LoadDetailsAsync(string id, CancellationToken cancellation = default)
    {
        var catalog = await compositeCatalog.Value.ConfigureAwait(false);

        var options = new FindPackagesOptions { ResultLimit = 1 };
        options.Selectors.Add(new PackageMatchFilter
        {
            Field = PackageMatchField.Id,
            Option = PackageFieldMatchOption.Equals,
            Value = id
        });

        var findResult = await catalog.FindPackagesAsync(options).AsTask(cancellation).ConfigureAwait(false);
        if (findResult.Status != FindPackagesResultStatus.Ok || findResult.Matches.Count == 0)
            return null;

        var package = findResult.Matches[0].CatalogPackage;

        CatalogPackageMetadata? metadata = null;
        try
        {
            metadata = (package.DefaultInstallVersion ?? package.InstalledVersion)?.GetCatalogPackageMetadata();
        }
        catch (COMException)
        {
            // Some package sources don't implement IPackageVersionInfo4
        }

        return new PackageDetails(id,
            Available: package.DefaultInstallVersion?.Version,
            Source: package.DefaultInstallVersion?.PackageCatalog?.Info?.Name,
            IsUpdateAvailable: package.IsUpdateAvailable,
            Publisher: metadata?.Publisher,
            Author: metadata?.Author,
            Description: metadata?.Description,
            PackageUrl: metadata?.PackageUrl,
            License: metadata?.License,
            IconUrl: metadata?.Icons?.ToArray()?.FirstOrDefault(x => !string.IsNullOrEmpty(x.Url))?.Url);
    }

    private async Task<PackageCatalog> GetCompositeCatalogAsync()
    {
        var options = new CreateCompositePackageCatalogOptions
        {
            CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs,
            InstalledScope = PackageInstallScope.Any
        };
        var manager = await packageManager.Value.ConfigureAwait(false);
        var catalogs = manager.GetPackageCatalogs();
        for (int i = 0; i < catalogs.Count; i++)
            options.Catalogs.Add(catalogs[i]);

        var connect = await manager.CreateCompositePackageCatalog(options).ConnectAsync().AsTask().ConfigureAwait(false);
        if (connect.Status != ConnectResultStatus.Ok)
            throw new InvalidOperationException($"Failed to connect to composite catalog: {connect.Status}");

        return connect.PackageCatalog;
    }
}
