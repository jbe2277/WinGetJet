using Microsoft.UI.Xaml;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Waf.Applications;
using System.Waf.Foundation;
using WindowsPackageManager;
using WinGetJet.Models;
using WinGetJet.Services;
using WinGetJet.ViewModels;
using WinGetJet.Views;

namespace WinGetJet;

public partial class App : Application
{
    private static readonly (string loggerNamePattern, LogLevel minLevel)[] logSettings =
    [
        ("WGJ", LogLevel.Info),
    ];

    private readonly SemaphoreSlim loadDetailsSemaphore = new(16);
    private readonly PackageService packageService = new();
    private readonly AsyncDelegateCommand refreshCommand;
    private ShellWindow shellWindow = null!;
    private ShellViewModel shellViewModel = null!;
    private MainViewModel mainViewModel = null!;


    public App()
    {
        LogManager.Setup().LoadConfiguration(c =>
        {
            c.Configuration.DefaultCultureInfo = CultureInfo.InvariantCulture;
            var layout = "${date:universalTime=true:format=yyyy-MM-dd HH\\:mm\\:ss.ff} [${level:format=FirstCharacter}] ${processid} ${logger} ${message} ${exception}";
            var fileTarget = c.ForTarget("fileTarget").WriteTo(new AtomicFileTarget
            {
                FileName = LogInfo.LogBaseFileName,
                Layout = layout,
                ArchiveAboveSize = 5_000_000,  // 5 MB
                MaxArchiveFiles = 2,
            }).WithAsync(AsyncTargetWrapperOverflowAction.Block);

            var traceTarget = c.ForTarget("traceTarget").WriteTo(new TraceTarget
            {
                Layout = layout,
                RawWrite = true
            }).WithAsync(AsyncTargetWrapperOverflowAction.Block);

            foreach (var (loggerNamePattern, minLevel) in logSettings)
            {
                c.ForLogger(loggerNamePattern).FilterMinLevel(minLevel).WriteTo(fileTarget).WriteTo(traceTarget);
            }
        });
        refreshCommand = new AsyncDelegateCommand(Refresh);
        InitializeComponent();
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Log.Default.Info("{0} {1} is starting; OS: {2}; .NET: {3}", ApplicationInfo.ProductName, ApplicationInfo.Version, Environment.OSVersion, Environment.Version);

        shellWindow = new ShellWindow();
        shellWindow.Closed += ShellWindowClosed;
        shellViewModel = new ShellViewModel(shellWindow);
        shellViewModel.Initialize();

        var mainView = new MainView();
        mainViewModel = new MainViewModel(mainView)
        {
            RefreshCommand = refreshCommand
        };
        mainViewModel.Initialize();

        shellWindow.ContentPage = mainView;
        shellWindow.Activate();

        mainViewModel.StatusText = "Retrieving installed packages...";
        mainViewModel.AppsSource.CollectionChanged += AppsSourceCollectionChanged;
        mainViewModel.AppsSource.CollectionItemChanged += AppsSourceCollectionItemChanged;
        try
        {
            var timeStamp = Stopwatch.GetTimestamp();
            var packages = await packageService.GetInstalledPackagesAsync();
            foreach (var app in packages.OrderBy(a => a.Name))
                mainViewModel.AppsSource.Add(new AppModel(app.Id, app.Name, app.ProductCodes, app.InstalledVersion));
            mainViewModel.SelectedApp = mainViewModel.AppsView.FirstOrDefault();
            Log.Default.Info($"Retrieved {mainViewModel.AppsView.Count} installed packages in {Stopwatch.GetElapsedTime(timeStamp).TotalSeconds:F2} seconds.");

            mainViewModel.StatusText = $"Retrieved {mainViewModel.AppsView.Count} installed packages.";
        }
        catch (Exception ex)
        {
            mainViewModel.StatusText = $"Error retrieving installed packages: {ex.Message}";
        }
    }

    private async Task Refresh()
    {
        packageService.InvalidateCache();
        var packages = await packageService.GetInstalledPackagesAsync();
        var appModels = packages.OrderBy(a => a.Name).Select(x => new AppModel(x.Id, x.Name, x.ProductCodes, x.InstalledVersion)).ToArray();
        CollectionHelper.Merge(mainViewModel.AppsSource, appModels, AppModelComparer.Default, null, null, null, moveAction: mainViewModel.AppsSource.Move);        
        foreach (var app in mainViewModel.AppsSource)
        {
            app.InstalledVersion = packages.FirstOrDefault(p => p.Id == app.Id)?.InstalledVersion;            
            LoadDetails(app);
        }
    }

    private void AppsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is not (NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace) || e.NewItems is null) return;
        foreach (var app in e.NewItems.OfType<AppModel>()) LoadDetails(app);
    }

    private async void LoadDetails(AppModel app)
    {
        await loadDetailsSemaphore.WaitAsync();
        try
        {
            var details = await packageService.LoadDetailsAsync(app.Id, null, CancellationToken.None);
            if (details is not null)
            {
                app.IsUpdateAvailable = details.IsUpdateAvailable;

                var availableVersions = details.AvailableVersions.Where(x => x.Kind == PackageVersionKind.Available).ToArray();
                if (!app.AvailableVersions.SequenceEqual(availableVersions))
                {
                    app.AvailableVersions = availableVersions;
                    app.SelectedVersion = app.AvailableVersions.FirstOrDefault();
                }
                
                if (!details.VersionDetails.Version.Equals(app.DefaultVersionDetails?.Version))
                {
                    app.DefaultVersionDetails = CreateAppVersionModel(details.VersionDetails);
                    app.SelectedVersionDetails = app.DefaultVersionDetails;
                    if (app.DefaultVersionDetails.Version.Version is not null) app.VersionDetailsCache[app.DefaultVersionDetails.Version.Version] = app.DefaultVersionDetails;
                }
                
                app.Icon ??= await IconService.ResolveIconAsync(app);
            }
        }
        catch (Exception ex)
        {
            Log.Default.Error(ex, "Failed to load details for {0}", app.Id);
        }
        finally
        {
            loadDetailsSemaphore.Release();
        }
    }

    private async void AppsSourceCollectionItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppModel.SelectedVersion))
        {
            var source = (AppModel)sender!;
            if (source.SelectedVersion?.Version is not null)
            {
                if (!source.VersionDetailsCache.TryGetValue(source.SelectedVersion.Version, out var versionDetails))
                {
                    var details = await packageService.LoadDetailsAsync(source.Id, new(PackageVersionKind.Available, source.SelectedVersion.Version));
                    versionDetails = details?.VersionDetails is null ? null : CreateAppVersionModel(details.VersionDetails);
                    source.VersionDetailsCache[source.SelectedVersion.Version] = versionDetails;
                }
                source.SelectedVersionDetails = versionDetails;
            }
        }
    }

    private void ShellWindowClosed(object sender, WindowEventArgs args)
    {
        Log.Default.Info("{0} closed", ApplicationInfo.ProductName);
    }

    private static AppVersionModel CreateAppVersionModel(PackageVersionDetails versionDetails)
    {
        return new AppVersionModel(
            Version: versionDetails.Version,
            Source: versionDetails.Source,
            Publisher: new(versionDetails.Publisher.Text, versionDetails.Publisher.Url),
            Author: versionDetails.Author,
            Description: versionDetails.Description,
            PackageUrl: versionDetails.PackageUrl,
            License: new(versionDetails.License.Text, versionDetails.License.Url),
            IconUrl: versionDetails.IconUrl,
            PublisherSupportUrl: versionDetails.PublisherSupportUrl,
            Copyright: versionDetails.Copyright,
            ReleaseNotes: new(versionDetails.ReleaseNotes.Text, versionDetails.ReleaseNotes.Url),
            Documentations: [.. versionDetails.Documentations.Select(d => new AppTextLink(d.Text, d.Url))],
            Tags: versionDetails.Tags);
    }
}
