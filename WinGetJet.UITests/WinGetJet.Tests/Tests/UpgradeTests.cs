using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using Microsoft.Management.Deployment;
using UITests;
using WinGetJet.Tests.Views;
using Xunit;

namespace WinGetJet.Tests.Tests;

public class UpgradeTests : UITest
{
    public static bool IsGitHubActions { get; } = string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase);

    public static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact(Skip = "Runs only on GitHub Actions", SkipUnless = nameof(IsGitHubActions))]
    public async Task InstallUpgradeUninstallNotepadPlusPlus()
    {
        const string packageId = "Notepad++.Notepad++";

        var manager = new PackageManager();
        var package = await FindPackageAsync(manager, packageId);
        var versions = package.AvailableVersions.ToArray();
        Log.WriteLine(("Latest version:", $"{versions[0].Version}"));
        Log.WriteLine(("2nd latest version:", $"{versions[1].Version}"));

        // Verify that the package is not installed before starting the test
        package = await FindPackageAsync(manager, packageId);
        Assert.Null(package.InstalledVersion);

        var installOptions = new InstallOptions
        {
            PackageInstallMode = PackageInstallMode.Silent,
            PackageVersionId = versions[1]
        };
        var installResult = await manager.InstallPackageAsync(package, installOptions);
        Assert.Equal(InstallResultStatus.Ok, installResult.Status);        
        package = await FindPackageAsync(manager, packageId);
        var installedVersion = package.InstalledVersion;
        Assert.NotNull(installedVersion);
        Assert.Equal(versions[1].Version, installedVersion.Version);


        Launch();
        var window = GetShellWindow();
        window.SetState(WindowVisualState.Maximized);
        var mainView = window.MainView;
        var detailsView = mainView.DetailsView;

        Retry.WhileTrue(() => string.IsNullOrEmpty(detailsView.AppNameTextBlock.Text), throwOnTimeout: true);
        Thread.Sleep(1000);
        
        window.SearchBox.TextBox.Enter("Notepad++ Team");
        Retry.WhileFalse(() => detailsView.AppNameTextBlock.Text.StartsWith("Notepad++"), throwOnTimeout: true);
        Thread.Sleep(1000);
        window.CaptureToFile(GetScreenshotFile("Launch"));

        detailsView.UpdateButton.Click();
        var updateView = window.FirstModalWindow().As<UpdateView>();
        Assert.Equal("Notepad++ Team", updateView.PublisherTextBlock.Text);
        Assert.Equal(versions[1].Version, updateView.InstalledVersionTextBlock.Text);
        Assert.Equal(versions[0].Version, updateView.TargetVersionTextBlock.Text);
        Assert.Equal("WinGet", updateView.SourceTextBlock.Text);
        Assert.False(updateView.ElevationInfoBar.IsOffscreen);
        Assert.True(updateView.UpdateButton.IsEnabled);
        Assert.True(updateView.CancelButton.IsEnabled);
        Thread.Sleep(1000);
        window.CaptureToFile(GetScreenshotFile("UpdateView"));

        var upgradeOptions = new InstallOptions
        {
            PackageInstallMode = PackageInstallMode.Silent,
            PackageVersionId = versions[0]
        };
        var upgradeResult = await manager.UpgradePackageAsync(package, upgradeOptions);
        Assert.Equal(InstallResultStatus.Ok, upgradeResult.Status);

        package = await FindPackageAsync(manager, packageId);
        installedVersion = package.InstalledVersion;
        Assert.NotNull(installedVersion);
        Assert.Equal(versions[0].Version, installedVersion.Version);


        var uninstallOptions = new UninstallOptions { PackageUninstallMode = PackageUninstallMode.Silent };
        var uninstallResult = await manager.UninstallPackageAsync(package, uninstallOptions);
        Assert.Equal(UninstallResultStatus.Ok, uninstallResult.Status);

        // NSIS uninstaller (Notepad++) returns before uninstall is finished -> retry until ARP entry is removed
        await Assert.RetryAsync(async () => (await FindPackageAsync(manager, packageId)).InstalledVersion, Assert.Null, cancellationToken: Cancellation);
    }

    private async Task<CatalogPackage> FindPackageAsync(PackageManager manager, string packageId)
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
        var catalog = catalogRef.PackageCatalog;

        var findOptions = new FindPackagesOptions { ResultLimit = 1 };
        findOptions.Selectors.Add(new PackageMatchFilter
        {
            Field = PackageMatchField.Id,
            Option = PackageFieldMatchOption.Equals,
            Value = packageId
        });
        var result = await catalog.FindPackagesAsync(findOptions);
        return result.Matches[0].CatalogPackage;
    }
}
