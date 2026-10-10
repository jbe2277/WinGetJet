using System.Waf.Foundation;

namespace WinGetJet.Models;

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

    public string? IconUrl { get; set => SetProperty(ref field, value); }
}
