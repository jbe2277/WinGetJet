using System.Waf.Foundation;

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

    public string? Available { get; set => SetProperty(ref field, value); }

    public string? Source { get; set => SetProperty(ref field, value); }

    public bool IsUpdateAvailable { get; set => SetProperty(ref field, value); }

    public AppTextLink Publisher { get; set => SetProperty(ref field, value); } = AppTextLink.Empty;

    public string? Author { get; set => SetProperty(ref field, value); }

    public string? Description { get; set => SetProperty(ref field, value); }

    public Uri? PackageUrl { get; set => SetProperty(ref field, value); }

    public AppTextLink License { get; set => SetProperty(ref field, value); } = AppTextLink.Empty;

    public Uri? IconUrl { get; set => SetProperty(ref field, value); }

    public Uri? PublisherSupportUrl { get; set => SetProperty(ref field, value); }

    public string? Copyright { get; set => SetProperty(ref field, value); }

    public AppTextLink ReleaseNotes { get; set => SetProperty(ref field, value); } = AppTextLink.Empty;

    public IReadOnlyList<AppTextLink> Documentations { get; set => SetProperty(ref field, value); } = [];

    public IReadOnlyList<string> Tags { get; set => SetProperty(ref field, value); } = [];

    public object? Icon { get; set => SetProperty(ref field, value); }
}
