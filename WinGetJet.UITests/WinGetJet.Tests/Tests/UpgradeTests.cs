using Microsoft.Management.Deployment;
using UITests;
using Xunit;

namespace WinGetJet.Tests.Tests;

public class UpgradeTests : UITest
{
    public static bool IsGitHubActions { get; } = string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase);

    [Fact(Skip = "Runs only on GitHub Actions", SkipUnless = nameof(IsGitHubActions))]
    public async Task InstallNotepadPlusPlus()
    {
        var manager = new PackageManager();
        var catalog = await ConnectAsync(manager);

        var findOptions = new FindPackagesOptions { ResultLimit = 1 };
        findOptions.Selectors.Add(new PackageMatchFilter
        {
            Field = PackageMatchField.Id,
            Option = PackageFieldMatchOption.Equals,
            Value = "Notepad++.Notepad++"
        });
        var result = await catalog.FindPackagesAsync(findOptions);
        var package = result.Matches[0].CatalogPackage;
        var versions = package.AvailableVersions.ToArray();
        Log.WriteLine(("Latest version:", $"{versions[0].Version}"));
        Log.WriteLine(("2nd latest version:", $"{versions[1].Version}"));


        // Verify that the package is not installed before starting the test
        var installedResult = await catalog.FindPackagesAsync(findOptions);
        Assert.Null(installedResult.Matches[0].CatalogPackage.InstalledVersion);

        var installOptions = new InstallOptions
        {
            PackageInstallMode = PackageInstallMode.Silent,
            PackageVersionId = versions[1]
        };
        var installResult = await manager.InstallPackageAsync(package, installOptions);
        Assert.Equal(InstallResultStatus.Ok, installResult.Status);        
        catalog = await ConnectAsync(manager);
        installedResult = await catalog.FindPackagesAsync(findOptions);
        package = installedResult.Matches[0].CatalogPackage;
        var installedVersion = package.InstalledVersion;
        Assert.NotNull(installedVersion);
        Assert.Equal(versions[1].Version, installedVersion.Version);


        var upgradeOptions = new InstallOptions
        {
            PackageInstallMode = PackageInstallMode.Silent,
            PackageVersionId = versions[0]
        };
        var upgradeResult = await manager.UpgradePackageAsync(package, upgradeOptions);
        Assert.Equal(InstallResultStatus.Ok, upgradeResult.Status);

        catalog = await ConnectAsync(manager);
        installedResult = await catalog.FindPackagesAsync(findOptions);
        package = installedResult.Matches[0].CatalogPackage;
        installedVersion = package.InstalledVersion;
        Assert.NotNull(installedVersion);
        Assert.Equal(versions[0].Version, installedVersion.Version);


        var uninstallOptions = new UninstallOptions { PackageUninstallMode = PackageUninstallMode.Silent };
        var uninstallResult = await manager.UninstallPackageAsync(package, uninstallOptions);
        Assert.Equal(UninstallResultStatus.Ok, uninstallResult.Status);

        catalog = await ConnectAsync(manager);
        installedResult = await catalog.FindPackagesAsync(findOptions);
        Assert.Null(installedResult.Matches[0].CatalogPackage.InstalledVersion);
    }

    private async Task<PackageCatalog> ConnectAsync(PackageManager manager)
    {
        var options = new CreateCompositePackageCatalogOptions
        {
            CompositeSearchBehavior = CompositeSearchBehavior.AllCatalogs,
            InstalledScope = PackageInstallScope.Any
        };
        var catalogs = manager.GetPackageCatalogs();
        for (int i = 0; i < catalogs.Count; i++) options.Catalogs.Add(catalogs[i]);
        var catalogRef = await manager.CreateCompositePackageCatalog(options).ConnectAsync();
        Assert.Equal(ConnectResultStatus.Ok, catalogRef.Status);
        return catalogRef.PackageCatalog;
    }
}
