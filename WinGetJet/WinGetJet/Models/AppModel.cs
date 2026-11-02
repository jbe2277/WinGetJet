using System.Waf.Foundation;
using WindowsPackageManager;

namespace WinGetJet.Models;

public class AppModel(string id, string name, IReadOnlyList<string> productCodes, string? installedVersion) : Model
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public IReadOnlyList<string> ProductCodes { get; } = productCodes;

    public string? InstalledVersion { get; set => SetProperty(ref field, value); } = installedVersion;

    public bool IsUpdateAvailable { get; set => SetProperty(ref field, value); }

    public IReadOnlyList<PackageVersionKey> AvailableVersions { get; set => SetProperty(ref field, value); } = [];

    public PackageVersionKey? SelectedVersion { get; set => SetProperty(ref field, value); }

    public AppVersionModel? DefaultVersionDetails { get; set => SetProperty(ref field, value); }

    public AppVersionModel? SelectedVersionDetails { get; set => SetProperty(ref field, value); }

    public Dictionary<string, AppVersionModel?> VersionDetailsCache { get; } = [];

    public object? Icon { get; set => SetProperty(ref field, value); }
}

public record AppVersionModel(PackageVersionKey Version, string? Source, PackageTextLink Publisher, string? Author, string? Description, Uri? PackageUrl,
    PackageTextLink License, Uri? IconUrl, Uri? PublisherSupportUrl, string? Copyright, PackageTextLink ReleaseNotes,
    IReadOnlyList<PackageTextLink> Documentations, IReadOnlyList<string> Tags, IReadOnlyList<PackageAgreement> Agreements,
    PackageInstallRequirements InstallRequirements);
