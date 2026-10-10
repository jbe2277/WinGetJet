using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using WinGetJet.Models;

namespace WinGetJet.Services;

internal static class IconService
{
    public static ImageSource? ResolveIcon(AppModel app)
    {
        if (app.IconUrl is not null)
        {
            return new BitmapImage
            {
                DecodePixelWidth = 32,
                UriSource = app.IconUrl
            };
        }
        return null;
    }
}
