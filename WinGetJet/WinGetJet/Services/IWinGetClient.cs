using WindowsPackageManager;

namespace WinGetJet.Services;

internal interface IWinGetClient
{
    void InvalidateCache();

    Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation = default);

    Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey? version = null, CancellationToken cancellation = default);

    Task UpgradeAsync(string id, PackageVersionKey version, bool acceptPackageAgreements = false, IProgress<double>? progress = null, CancellationToken cancellation = default);
}

internal class WinGetClient(PackageService packageService) : IWinGetClient
{
    public void InvalidateCache() => packageService.InvalidateCache();

    public Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation) => packageService.GetInstalledPackagesAsync(cancellation);

    public Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey? version, CancellationToken cancellation) => packageService.LoadDetailsAsync(id, version, cancellation);

    public Task UpgradeAsync(string id, PackageVersionKey version, bool acceptPackageAgreements = false, IProgress<double>? progress = null, CancellationToken cancellation = default) => packageService.UpgradeAsync(id, version, acceptPackageAgreements, progress, cancellation);
}
