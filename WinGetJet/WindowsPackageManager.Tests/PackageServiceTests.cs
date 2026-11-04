using Microsoft.Management.Deployment;

namespace WindowsPackageManager.Tests;

public class PackageServiceTests
{
    CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task WinGetAppTest()
    {
        const string id = @"ARP\Machine\X64\Git_is1";

        var service = new PackageService();
        var packages = await service.GetInstalledPackagesAsync(Cancellation);
        var package = packages.First(x => x.Id == id);
        Assert.Equal("Git", package.Name);
        var info = await service.LoadDetailsAsync(package.Id, cancellation: Cancellation);
        Assert.NotNull(info);
        Assert.Equal(id, info.Id);
        Assert.Equal("Git.Git", info.RemoteId);
        var details = info.VersionDetails;
        Assert.Equal("The Git Development Community", details.Publisher.Text);
        Assert.Equal(new Uri("https://gitforwindows.org/"), details.Publisher.Url);
        Assert.Equal(new Uri("https://github.com/git-for-windows/git/issues"), details.PublisherSupportUrl);
        Assert.StartsWith("Git is a free and open source distributed version control system", details.Description);
        Assert.Equal(new Uri("https://gitforwindows.org/"), details.PackageUrl);
        Assert.Equal("GPL-2.0", details.License.Text);
        Assert.Equal(new Uri("https://github.com/git-for-windows/build-extra/blob/HEAD/LICENSE.txt"), details.License.Url);
        Assert.Equal("Copyright (C) 1989, 1991 Free Software Foundation, Inc.", details.Copyright);
        Assert.False(string.IsNullOrEmpty(details.ReleaseNotes.Text));
        Assert.StartsWith("https://github.com/git-for-windows/git/releases/tag/v", details.ReleaseNotes.Url?.ToString());
        var documentation = Assert.Single(details.Documentations);
        Assert.Equal("Wiki", documentation.Text);
        Assert.Equal(new Uri("https://github.com/git-for-windows/git/wiki"), documentation.Url);
        Assert.Contains("git", details.Tags);
        Assert.Contains("vcs", details.Tags);
        Assert.Equal(PackageInstallerType.Inno, details.InstallRequirements.InstallerType);
    }
    
    [Fact]
    public async Task LaodAllDetailsTest()
    {
        var service = new PackageService();
        var packages = await service.GetInstalledPackagesAsync(Cancellation);

        // Verify that no exceptions are thrown when loading details for all packages
        var details = await Task.WhenAll(packages.Select(x => service.LoadDetailsAsync(x.Id, cancellation: Cancellation)));
    }
}
