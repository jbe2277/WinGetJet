using Microsoft.UI.Xaml;
using System.Globalization;

namespace WinGetJet.Views;

public static class ViewConvert
{
    public static string Format(string format, object? value) => string.Format(CultureInfo.CurrentCulture, format, value);

    public static string Format(string format, object? value1, object? value2) => string.Format(CultureInfo.CurrentCulture, format, value1, value2);

    public static string UpdateAvailable(bool isUpdateAvailable) => isUpdateAvailable ? "X" : "";

    public static Visibility IsVisible(object? value) => value switch
    {
        null => Visibility.Collapsed,
        string s => string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible,
        System.Collections.ICollection c => c.Count == 0 ? Visibility.Collapsed : Visibility.Visible,
        _ => Visibility.Visible
    };

    public static string Join(IEnumerable<string>? values) => values is null ? "" : string.Join(", ", values);

    public static Visibility IsNotVisible(object? value) => IsVisible(value) == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
}