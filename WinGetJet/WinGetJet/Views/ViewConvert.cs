using Microsoft.UI.Xaml;
using System.Globalization;

namespace WinGetJet.Views;

public static class ViewConvert
{
    public static string Format(string format, object? value) => string.Format(CultureInfo.CurrentCulture, format, value);

    public static string Format(string format, object? value1, object? value2) => string.Format(CultureInfo.CurrentCulture, format, value1, value2);

    public static string VersionAndPublisher(object? version, string? publisher)
        => string.IsNullOrEmpty(publisher) ? Format("{0}", version) : Format("{0}   |   {1}", version, publisher);

    public static Visibility BoolToVisibility(bool value) => value ? Visibility.Visible : Visibility.Collapsed;

    public static Visibility IsVisible(object? value) => value switch
    {
        null => Visibility.Collapsed,
        string s => string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible,
        System.Collections.ICollection c => c.Count == 0 ? Visibility.Collapsed : Visibility.Visible,
        _ => Visibility.Visible
    };

    public static Visibility IsAnyVisible(object? value1, object? value2) => IsVisible(value1) == Visibility.Visible ? Visibility.Visible : IsVisible(value2);

    public static string Join(IEnumerable<string>? values) => values is null ? "" : string.Join(", ", values);

    public static bool HasItems(System.Collections.IEnumerable? value) => value switch
    {
        null => false,
        System.Collections.ICollection c => c.Count > 0,
        _ => value.GetEnumerator().MoveNext()
    };

    public static Visibility IsNotVisible(object? value) => IsVisible(value) == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

    public static string? GetSourceDisplayName(string? source) => source switch
    {
        "winget" => "WinGet",
        "msstore" => "Microsoft Store",
        _ => source
    };
}