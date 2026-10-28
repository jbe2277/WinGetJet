using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.FileProperties;
using WinGetJet.Models;

namespace WinGetJet.Services;

internal static class IconService
{
    private const int IconSize = 48;
    private static readonly PackageManager packageManager = new();

    public static async Task<ImageSource?> ResolveIconAsync(AppModel app)
    {
        if (app.DefaultVersionDetails?.IconUrl is not null)
        {
            return new BitmapImage { DecodePixelWidth = IconSize, UriSource = app.DefaultVersionDetails.IconUrl };
        }

        foreach (var productCode in app.ProductCodes)
        {
            var icon = await TryGetPackageLogoAsync(productCode) ?? await TryGetDisplayIconAsync(productCode);
            if (icon is not null) return icon;
        }
        return null;
    }

    // MSIX / Store apps: product code = package family name
    private static async Task<ImageSource?> TryGetPackageLogoAsync(string packageFamilyName)
    {
        try
        {
            var package = packageManager.FindPackagesForUser("", packageFamilyName).FirstOrDefault();
            if (package is null) return null;

            using var stream = await package.GetLogoAsRandomAccessStreamReference(new Size(IconSize, IconSize)).OpenReadAsync();
            var bitmap = new BitmapImage { DecodePixelWidth = IconSize };
            await bitmap.SetSourceAsync(stream);
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    // Win32 apps: product code = ARP (Uninstall) registry key name
    private static async Task<ImageSource?> TryGetDisplayIconAsync(string productCode)
    {
        var iconPath = GetDisplayIconPath(productCode);
        if (iconPath is null || !File.Exists(iconPath)) return null;

        try
        {
            var file = await StorageFile.GetFileFromPathAsync(iconPath);
            using var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, IconSize, ThumbnailOptions.UseCurrentScale);
            if (thumbnail is null) return null;

            var bitmap = new BitmapImage { DecodePixelWidth = IconSize };
            await bitmap.SetSourceAsync(thumbnail);
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetDisplayIconPath(string productCode)
    {
        (RegistryHive Hive, RegistryView View)[] locations =
        [
            (RegistryHive.LocalMachine, RegistryView.Registry64),
            (RegistryHive.LocalMachine, RegistryView.Registry32),
            (RegistryHive.CurrentUser, RegistryView.Default),
        ];

        foreach (var (hive, view) in locations)
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, view);
            using var key = baseKey.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{productCode}");
            if (key?.GetValue("DisplayIcon") is not string displayIcon || string.IsNullOrWhiteSpace(displayIcon)) continue;

            // Format: "C:\path\app.exe",0  → strip icon index and quotes
            var path = displayIcon;
            var comma = path.LastIndexOf(',');
            if (comma > 0 && int.TryParse(path[(comma + 1)..], out _)) path = path[..comma];
            return Environment.ExpandEnvironmentVariables(path.Trim().Trim('"'));
        }
        return null;
    }
}