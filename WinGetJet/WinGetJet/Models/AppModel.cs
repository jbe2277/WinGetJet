using System.Waf.Foundation;

namespace WinGetJet.Models;

public record AppDocumentation(string Label, string Url);

public class AppModel(string Id, string Name, string? Version, IReadOnlyList<string> ProductCodes) : Model
{
    public string Id { get; } = Id;

    public string Name { get; } = Name;

    public string? Version { get; } = Version;

    public IReadOnlyList<string> ProductCodes { get; } = ProductCodes;

    public string? Available { get; set => SetProperty(ref field, value); }

    public string? Source { get; set => SetProperty(ref field, value); }

    public bool IsUpdateAvailable { get; set => SetProperty(ref field, value); }

    public string? Publisher { get; set => SetProperty(ref field, value); }

    public string? Author { get; set => SetProperty(ref field, value); }

    public string? Description { get; set => SetProperty(ref field, value); }

    public string? PackageUrl { get; set => SetProperty(ref field, value); }

    public string? License { get; set => SetProperty(ref field, value); }

    public Uri? IconUrl { get; set => SetProperty(ref field, value); }

    public string? PublisherUrl { get; set => SetProperty(ref field, value); }

    public string? PublisherSupportUrl { get; set => SetProperty(ref field, value); }

    public string? LicenseUrl { get; set => SetProperty(ref field, value); }

    public string? Copyright { get; set => SetProperty(ref field, value); }

    public string? ReleaseNotes { get; set => SetProperty(ref field, value); }

    public string? ReleaseNotesUrl { get; set => SetProperty(ref field, value); }

    public IReadOnlyList<AppDocumentation> Documentations { get; set => SetProperty(ref field, value); } = [];

    public IReadOnlyList<string> Tags { get; set => SetProperty(ref field, value); } = [];

    public object? Icon { get; set => SetProperty(ref field, value); }
}
