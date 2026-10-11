using System.Globalization;

namespace WinGetJet.Views;

public static class ViewConvert
{
    public static string Format(string format, object? value) => string.Format(CultureInfo.CurrentCulture, format, value);
}