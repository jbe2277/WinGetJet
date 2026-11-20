using System.Waf.Foundation;

namespace WinGetJet.Models;

public enum InstallationOperation { None, Upgrade, Uninstall }

public enum InstallationOperationStatus { None, Pending, InProgress, Completed }

public enum InstallationResultStatus
{
    Success,
    BlockedByPolicy,
    CatalogError,
    InternalError,
    InvalidOptions,
    DownloadError,
    InstallError,
    ManifestError,
    NoApplicableInstallers,
    NoApplicableUpgrade,
    PackageAgreementsNotAccepted
}

public class InstallationOperationState : Model
{
    public InstallationOperation Operation { get; set => SetProperty(ref field, value); }

    public InstallationOperationStatus Status { get; set => SetProperty(ref field, value); }

    public ulong BytesToDownload { get; set => SetProperty(ref field, value); }

    public ulong BytesDownloaded { get; set => SetProperty(ref field, value); }

    public double DownloadProgress { get; set => SetProperty(ref field, value); }

    public double InstallProgress { get; set => SetProperty(ref field, value); }

    public InstallationResultStatus Result { get; set => SetProperty(ref field, value); }

    public Exception? ResultException { get; set => SetProperty(ref field, value); }

    public uint ResultErrorCode { get; set => SetProperty(ref field, value); }

    public bool IsRebootRequired { get; set => SetProperty(ref field, value); }

    public void Reset()
    {
        BytesToDownload = 0;
        BytesDownloaded = 0;
        DownloadProgress = 0;
        InstallProgress = 0;
        ResultException = null;
        ResultErrorCode = 0;
        IsRebootRequired = false;
    }
}
