using Microsoft.UI.Xaml;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
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

    private readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private readonly PackageService packageService = new();
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
        mainViewModel = new MainViewModel(mainView);
        mainViewModel.Initialize();

        shellWindow.ContentPage = mainView;
        shellWindow.Activate();

        mainViewModel.StatusText = "Retrieving installed packages...";
        mainViewModel.AppsSource.CollectionItemChanged += AppsSourceCollectionItemChanged;
        try
        {
            var timeStamp = Stopwatch.GetTimestamp();
            var packages = await packageService.GetInstalledPackagesAsync();
            foreach (var app in packages.OrderBy(a => a.Name))
                mainViewModel.AppsSource.Add(new AppModel(app.Id, app.Name, app.Version, app.ProductCodes));
            mainViewModel.SelectedApp = mainViewModel.AppsView.FirstOrDefault();
            Log.Default.Info($"Retrieved {mainViewModel.AppsView.Count} installed packages in {Stopwatch.GetElapsedTime(timeStamp).TotalSeconds:F2} seconds.");

            timeStamp = Stopwatch.GetTimestamp();
            await Parallel.ForEachAsync(mainViewModel.AppsView, new ParallelOptions { MaxDegreeOfParallelism = 16 },
                async (app, cancellation) =>
                {
                    var details = await packageService.LoadDetailsAsync(app.Id, null, cancellation);
                    if (details is not null)
                    {
                        await TaskHelper.Run(async () =>
                        {
                            app.AvailableVersions = details.AvailableVersions.Where(x => x.Kind == PackageVersionKind.Available).ToArray();
                            app.SelectedVersion = app.AvailableVersions.FirstOrDefault();
                            app.IsUpdateAvailable = details.IsUpdateAvailable;
                            app.DefaultVersionDetails = CreateAppVersionModel(details.VersionDetails);
                            app.SelectedVersionDetails = app.DefaultVersionDetails;
                            if (app.DefaultVersionDetails.Version.Version is not null) app.VersionDetailsCache[app.DefaultVersionDetails.Version.Version] = app.DefaultVersionDetails;
                            app.Icon = await IconService.ResolveIconAsync(app);
                        }, uiScheduler, cancellation);
                    }
                });
            Log.Default.Info($"Retrieved details for {mainViewModel.AppsView.Count} installed packages in {Stopwatch.GetElapsedTime(timeStamp).TotalSeconds:F2} seconds.");

            mainViewModel.StatusText = $"Retrieved {mainViewModel.AppsView.Count} installed packages.";
        }
        catch (Exception ex)
        {
            mainViewModel.StatusText = $"Error retrieving installed packages: {ex.Message}";
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
