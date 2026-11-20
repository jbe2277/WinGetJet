using Microsoft.Management.Deployment;
using WinGetJet.Models;

namespace WinGetJet.Services;

internal static class InstallationOperationStateHelper
{
    public static InstallationResultStatus ToModel(this InstallResultStatus status) => status switch
    {
        InstallResultStatus.Ok => InstallationResultStatus.Success,
        InstallResultStatus.BlockedByPolicy => InstallationResultStatus.BlockedByPolicy,
        InstallResultStatus.CatalogError => InstallationResultStatus.CatalogError,
        InstallResultStatus.InternalError => InstallationResultStatus.InternalError,
        InstallResultStatus.InvalidOptions => InstallationResultStatus.InvalidOptions,
        InstallResultStatus.DownloadError => InstallationResultStatus.DownloadError,
        InstallResultStatus.InstallError => InstallationResultStatus.InstallError,
        InstallResultStatus.ManifestError => InstallationResultStatus.ManifestError,
        InstallResultStatus.NoApplicableInstallers => InstallationResultStatus.NoApplicableInstallers,
        InstallResultStatus.NoApplicableUpgrade => InstallationResultStatus.NoApplicableUpgrade,
        InstallResultStatus.PackageAgreementsNotAccepted => InstallationResultStatus.PackageAgreementsNotAccepted,
        _ => throw new NotSupportedException($"Unsupported InstallResultStatus: {status}"),
    };
}
