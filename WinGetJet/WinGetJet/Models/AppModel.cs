using System.Waf.Foundation;
using WindowsPackageManager;

namespace WinGetJet.Models;

public record AppTextLink(string? Text, Uri? Url)
{
    public static AppTextLink Empty { get; } = new(null, null);
}

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

public record AppVersionModel(PackageVersionKey Version, string? Source, AppTextLink Publisher, string? Author, string? Description, Uri? PackageUrl,
    AppTextLink License, Uri? IconUrl, Uri? PublisherSupportUrl, string? Copyright, AppTextLink ReleaseNotes,
    IReadOnlyList<AppTextLink> Documentations, IReadOnlyList<string> Tags);
