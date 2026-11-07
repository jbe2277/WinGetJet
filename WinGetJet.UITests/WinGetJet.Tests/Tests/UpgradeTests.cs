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
    }
}
