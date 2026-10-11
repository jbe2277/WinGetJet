using System.Globalization;

namespace WinGetJet.Views;

public static class ViewConvert
{
    public static string Format(string format, object? value) => string.Format(CultureInfo.CurrentCulture, format, value);

    public static string Format(string format, object? value1, object? value2) => string.Format(CultureInfo.CurrentCulture, format, value1, value2);

    public static string UpdateAvailable(bool isUpdateAvailable) => isUpdateAvailable ? "X" : "";
}