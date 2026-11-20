using WindowsPackageManager;
using WinGetJet.Models;

namespace WinGetJet.Services;

internal static class AppVersionModelHelper
{
    public static AppVersionModel CreateModel(this PackageVersionDetails x)
    {
        return new AppVersionModel(x.Version, x.Source, x.Publisher, x.Author,
            x.Description, x.PackageUrl, x.PrivacyUrl, x.License, x.IconUrl,
            x.PublisherSupportUrl, x.Copyright, x.ReleaseNotes,
            x.Documentations, x.Tags, x.Agreements, x.InstallRequirements);
    }
}
