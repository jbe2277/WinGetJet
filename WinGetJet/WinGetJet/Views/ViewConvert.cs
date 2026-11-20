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

    public static bool IsNotEmpty(string? value) => !string.IsNullOrEmpty(value);

    public static Visibility IsOperationVisible(Models.InstallationOperationStatus status) 
        => status == Models.InstallationOperationStatus.None ? Visibility.Collapsed : Visibility.Visible;

    public static Visibility IsOperationInProgress(Models.InstallationOperationStatus status) 
        => status is Models.InstallationOperationStatus.Pending or Models.InstallationOperationStatus.InProgress ? Visibility.Visible : Visibility.Collapsed;

    public static Visibility IsOperationCompleted(Models.InstallationOperationStatus status) 
        => status == Models.InstallationOperationStatus.Completed ? Visibility.Visible : Visibility.Collapsed;

    public static double ToPercent(double progress) => progress * 100;

    public static string DownloadInfo(ulong bytesDownloaded, ulong bytesToDownload, double progress)
        => bytesToDownload == 0 ? Format("Download: {0:P0}", progress) 
            : string.Format(CultureInfo.CurrentCulture, "Download: {0:P0} ({1:N1} / {2:N1} MB)", progress, bytesDownloaded / 1048576d, bytesToDownload / 1048576d);

    public static string ResultInfo(Models.InstallationResultStatus result, uint errorCode, Exception? exception, bool isRebootRequired)
    {
        var text = result == Models.InstallationResultStatus.Success ? "Update completed successfully." : Format("Update failed: {0}", result);
        if (errorCode != 0) text += Format(" (Error code: 0x{0:X8})", errorCode);
        if (exception is not null) text += Environment.NewLine + exception.Message;
        if (isRebootRequired) text += Environment.NewLine + "A reboot is required to complete the installation.";
        return text;
    }

    public static string? GetSourceDisplayName(string? source) => source switch
    {
        "winget" => "WinGet",
        "msstore" => "Microsoft Store",
        _ => source
    };
}