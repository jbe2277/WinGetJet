using Microsoft.Management.Deployment;
using UITests;
using Xunit;

namespace WinGetJet.Tests.Tests;

public class UpgradeTests : UITest
{
    [Fact]
    public async Task InstallNotepadPlusPlus()
    {
        var manager = new PackageManager();
        var catalogs = manager.GetPackageCatalogs();
        var catalogOptions = new CreateCompositePackageCatalogOptions
        {
            CompositeSearchBehavior = CompositeSearchBehavior.AllCatalogs,
            InstalledScope = PackageInstallScope.Any
        };
        for (int i = 0; i < catalogs.Count; i++) catalogOptions.Catalogs.Add(catalogs[i]);

        var connect = await manager.CreateCompositePackageCatalog(catalogOptions).ConnectAsync();
        Assert.Equal(ConnectResultStatus.Ok, connect.Status);
        var catalog = connect.PackageCatalog;

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

        installedResult = await catalog.FindPackagesAsync(findOptions);
        var installedVersion = installedResult.Matches[0].CatalogPackage.InstalledVersion;
        Assert.NotNull(installedVersion);
        Assert.Equal(versions[1].Version, installedVersion.Version);


        var upgradeOptions = new InstallOptions
        {
            PackageInstallMode = PackageInstallMode.Silent,
            PackageVersionId = versions[0]
        };
        var upgradeResult = await manager.UpgradePackageAsync(package, upgradeOptions);
        Assert.Equal(InstallResultStatus.Ok, upgradeResult.Status);

        installedResult = await catalog.FindPackagesAsync(findOptions);
        installedVersion = installedResult.Matches[0].CatalogPackage.InstalledVersion;
        Assert.NotNull(installedVersion);
        Assert.Equal(versions[1].Version, installedVersion.Version);


        var uninstallOptions = new UninstallOptions { PackageUninstallMode = PackageUninstallMode.Silent };
        var uninstallResult = await manager.UninstallPackageAsync(package, uninstallOptions);
        Assert.Equal(UninstallResultStatus.Ok, uninstallResult.Status);

        installedResult = await catalog.FindPackagesAsync(findOptions);
        Assert.Null(installedResult.Matches[0].CatalogPackage.InstalledVersion);
    }
}
