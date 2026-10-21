using Microsoft.Management.Deployment;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsPackageManager;

public record PackageInfo(string Id, string Name, string? Version, IReadOnlyList<string> ProductCodes);

public record PackageDetails(string Id, IReadOnlyList<PackageVersionKey> AvailableVersions, string? Source, bool IsUpdateAvailable,
    PackageTextLink Publisher, string? Author, string? Description, Uri? PackageUrl, PackageTextLink License, Uri? IconUrl,
    Uri? PublisherSupportUrl, string? Copyright, PackageTextLink ReleaseNotes,
    IReadOnlyList<PackageTextLink> Documentations, IReadOnlyList<string> Tags);

public record PackageTextLink(string? Text, Uri? Url);

public enum PackageVersionKind { Installed, DefaultInstall, Available }
public record PackageVersionKey(PackageVersionKind Kind, string? Version = null);

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
    public async Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey version, CancellationToken cancellation = default)
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
        var availableVersionsRaw = package.AvailableVersions.ToArray();
        var packageVersion = version.Kind switch
        {
            PackageVersionKind.Installed => package.InstalledVersion,
            PackageVersionKind.DefaultInstall => package.DefaultInstallVersion,
            PackageVersionKind.Available => package.GetPackageVersionInfo(availableVersionsRaw.First(x => x.Version == version.Version)),
            _ => throw new NotSupportedException($"Unsupported version kind: {version.Kind}")
        };
        var availableVersions = availableVersionsRaw.Select(v => new PackageVersionKey(PackageVersionKind.Available, v?.Version));
        if (package.DefaultInstallVersion is not null) availableVersions = availableVersions.Prepend(new(PackageVersionKind.DefaultInstall, package.DefaultInstallVersion.Version));
        if (package.InstalledVersion is not null) availableVersions = availableVersions.Prepend(new(PackageVersionKind.Installed, package.InstalledVersion.Version));

        CatalogPackageMetadata? metadata = null;
        try
        {
            metadata = (packageVersion ?? package.InstalledVersion)?.GetCatalogPackageMetadata();
        }
        catch (COMException)
        {
            // Some package sources don't implement IPackageVersionInfo4
        }

        var iconUrl = metadata?.Icons.ToArray().FirstOrDefault(x => !string.IsNullOrEmpty(x.Url))?.Url;
        return new PackageDetails(id,
            AvailableVersions: availableVersions.ToArray(),
            Source: packageVersion?.PackageCatalog?.Info?.Name,
            IsUpdateAvailable: package.IsUpdateAvailable,
            Publisher: new(metadata?.Publisher, ToUri(metadata?.PublisherUrl)),
            Author: metadata?.Author,
            Description: metadata?.Description,
            PackageUrl: ToUri(metadata?.PackageUrl),
            License: new(metadata?.License, ToUri(metadata?.LicenseUrl)),
            IconUrl: ToUri(iconUrl),
            PublisherSupportUrl: ToUri(metadata?.PublisherSupportUrl),
            Copyright: metadata?.Copyright,
            ReleaseNotes: new(metadata?.ReleaseNotes, ToUri(metadata?.ReleaseNotesUrl)),
            Documentations: metadata?.Documentations.ToArray().Select(d => new PackageTextLink(d.DocumentLabel, ToUri(d.DocumentUrl))).ToArray() ?? [],
            Tags: metadata?.Tags.ToArray() ?? []);
    }

    /// <summary>Upgrades the package to the specified version. DefaultInstall upgrades to the latest version.</summary>
    public async Task UpgradeAsync(string id, PackageVersionKey version, IProgress<double>? progress = null, CancellationToken cancellation = default)
    {
        if (version.Kind == PackageVersionKind.Installed) throw new ArgumentException("Cannot upgrade to the installed version.", nameof(version));
        var manager = await packageManager.Value.ConfigureAwait(false);
        var catalog = await compositeCatalog.Value.ConfigureAwait(false);

        var findOptions = new FindPackagesOptions { ResultLimit = 1 };
        findOptions.Selectors.Add(new PackageMatchFilter
        {
            Field = PackageMatchField.Id,
            Option = PackageFieldMatchOption.Equals,
            Value = id
        });
        var findResult = await catalog.FindPackagesAsync(findOptions).AsTask(cancellation).ConfigureAwait(false);
        if (findResult.Status != FindPackagesResultStatus.Ok || findResult.Matches.Count == 0)
            throw new InvalidOperationException($"Package not found: {id}");

        var package = findResult.Matches[0].CatalogPackage;
        var options = new InstallOptions
        {
            PackageInstallMode = PackageInstallMode.Silent,
            AcceptPackageAgreements = true
        };
        if (version.Kind == PackageVersionKind.Available)
            options.PackageVersionId = package.AvailableVersions.ToArray().First(x => x.Version == version.Version);

        var operation = manager.UpgradePackageAsync(package, options);
        if (progress is not null) operation.Progress = (_, p) => progress.Report(p.DownloadProgress * 0.5 + p.InstallationProgress * 0.5);
        var result = await operation.AsTask(cancellation).ConfigureAwait(false);
        if (result.Status != InstallResultStatus.Ok)
            throw new InvalidOperationException($"Upgrade failed: {result.Status} (0x{result.ExtendedErrorCode?.HResult:X8})");
    }

    private static Uri? ToUri(string? url) => Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : null;

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
