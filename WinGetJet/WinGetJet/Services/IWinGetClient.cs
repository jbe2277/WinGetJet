using WindowsPackageManager;

namespace WinGetJet.Services;

internal interface IWinGetClient
{
    void InvalidateCache();

    Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation = default);

    Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey? version = null, CancellationToken cancellation = default);
}

internal class WinGetClient(PackageService packageService) : IWinGetClient
{
    public void InvalidateCache() => packageService.InvalidateCache();

    public Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation) => packageService.GetInstalledPackagesAsync(cancellation);

    public Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey? version, CancellationToken cancellation) => packageService.LoadDetailsAsync(id, version, cancellation);
}

internal class MockWinGetClient : IWinGetClient
{
    public void InvalidateCache() { }

    public Task<IReadOnlyList<PackageInfo>> GetInstalledPackagesAsync(CancellationToken cancellation) => Task.FromResult<IReadOnlyList<PackageInfo>>([]);

    public Task<PackageDetails?> LoadDetailsAsync(string id, PackageVersionKey? version, CancellationToken cancellation) => Task.FromResult<PackageDetails?>(null);
}