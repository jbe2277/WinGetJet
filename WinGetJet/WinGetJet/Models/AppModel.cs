using System.Waf.Foundation;
using WindowsPackageManager;

namespace WinGetJet.Models;

public record AppTextLink(string? Text, Uri? Url)
{
    public static AppTextLink Empty { get; } = new(null, null);
}

public class AppModel(string Id, string Name, string? Version, IReadOnlyList<string> ProductCodes) : Model
{
    public string Id { get; } = Id;

    public string Name { get; } = Name;

    public string? Version { get; } = Version;

    public IReadOnlyList<string> ProductCodes { get; } = ProductCodes;

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
