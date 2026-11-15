using Microsoft.Management.Deployment;
using Windows.System;
using WindowsPackageManager;

namespace WinGetJet.Services;

internal class MockWinGetClient : IWinGetClient
{
    private readonly List<PackageInfo> installedPackages = 
    [
        new PackageInfo(Id: "Git.Git", Name: "Git", ProductCodes: ["Git_is1"], InstalledVersion: "2.55.0.5")
    ];

    private readonly List<PackageDetails> packageDetails = 
    [
        new PackageDetails(
            Id: "Git.Git",
            RemoteId: "Git.Git",
            AvailableVersions: [
                new(PackageVersionKind.Installed, "2.55.0.5"),
                new(PackageVersionKind.DefaultInstall, "2.56.0.4"),
                new(PackageVersionKind.Available, "2.56.0.4")
            ],
            IsUpdateAvailable: true,
            VersionDetails: new PackageVersionDetails(
                Version: new(PackageVersionKind.DefaultInstall, "2.56.0.4"),
                Source: "winget",
                Publisher: new("The Git Development Community", new Uri("https://gitforwindows.org/")),
                Author: "The Git Development Community",
                Description: """
                    Git is a free and open source distributed version control system designed to handle everything from small to very large projects with speed and efficiency.
                    Git for Windows focuses on offering a lightweight, native set of tools that bring the full feature set of the Git SCM to Windows while providing appropriate user interfaces for experienced Git users and novices alike.
                    """,
                PackageUrl: new Uri("https://gitforwindows.org/"),
                PrivacyUrl: null,
                License: new("GPL-2.0", new Uri("https://git-scm.com/about/free-and-open-source")),
                IconUrl: null,
                PublisherSupportUrl: new Uri("https://github.com/git-for-windows/git/issues"),
                Copyright: "Copyright (C) 1989, 1991 Free Software Foundation, Inc.",
                ReleaseNotes: new("""
                    Changes since Git for Windows v2.56.0(3) (October 22nd 2026)
                    This is another urgent security bug fix release, addressing CVE-2026-111497.
                    Bug Fixes
                    - The executables included in the 32-bit MinGit had corrupt code-signatures, which has been fixed.
                    - Upgrades the included brotli component to v1.2.1 to address CVE-2026-111497.
                    """, new Uri("https://github.com/git-for-windows/git/releases/tag/v2.56.0.windows.4")),
                Documentations: [new("Wiki", new Uri("https://github.com/git-for-windows/git/wiki"))],
                Tags: ["git", "vcs"],
                Agreements: [],
                InstallRequirements: new(
                    InstallerType: PackageInstallerType.Inno,
                    Architecture: ProcessorArchitecture.X64,
                    Scope: PackageInstallerScope.System,
                    Elevation: ElevationRequirement.ElevatesSelf,
                    IsInstallerAvailable: true)
            )
        )
    ];

    public void InvalidateCache() { }

    public Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation) => Task.FromResult<IReadOnlyList<PackageInfo>>(installedPackages);

    public Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey? version, CancellationToken cancellation) => Task.FromResult(packageDetails.FirstOrDefault(x => x.Id == id));

    public async Task UpgradeAsync(string id, PackageVersionKey version, bool acceptPackageAgreements = false, IProgress<double>? progress = null, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        var packageIndex = installedPackages.FindIndex(x => x.Id == id);
        var detailsIndex = packageDetails.FindIndex(x => x.Id == id);
        if (packageIndex < 0 || detailsIndex < 0) throw new InvalidOperationException($"Package not found: {id}");

        var details = packageDetails[detailsIndex];
        var targetVersion = details.AvailableVersions.FirstOrDefault(x => version.Version is null
            ? x.Kind == PackageVersionKind.DefaultInstall
            : x.Kind == PackageVersionKind.Available && x.Version == version.Version)
            ?? throw new InvalidOperationException($"Package version not found: {version.Version}");
        if (!acceptPackageAgreements && details.VersionDetails.Agreements.Count > 0)
            throw new InvalidOperationException("Package agreements must be accepted before upgrading.");

        progress?.Report(0);
        for (int step = 1; step <= 20; step++)
        {
            await Task.Delay(100, cancellation);
            if (step < 20) progress?.Report(step / 20.0);
        }

        cancellation.ThrowIfCancellationRequested();
        installedPackages[packageIndex] = installedPackages[packageIndex] with { InstalledVersion = targetVersion.Version };
        packageDetails[detailsIndex] = details with
        {
            AvailableVersions = details.AvailableVersions.Select(x => x.Kind == PackageVersionKind.Installed
                ? new PackageVersionKey(PackageVersionKind.Installed, targetVersion.Version)
                : x).ToArray(),
            IsUpdateAvailable = details.AvailableVersions.Any(x => x.Kind == PackageVersionKind.DefaultInstall && x.Version != targetVersion.Version)
        };
        progress?.Report(1);
    }
}
