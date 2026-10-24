using Microsoft.UI.Xaml;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.Globalization;
using System.Waf.Applications;
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

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Log.Default.Info("{0} {1} is starting; OS: {2}; .NET: {3}", ApplicationInfo.ProductName, ApplicationInfo.Version, Environment.OSVersion, Environment.Version);

        var shellWindow = new ShellWindow();
        shellWindow.Closed += ShellWindowClosed;
        var shellViewModel = new ShellViewModel(shellWindow);
        shellViewModel.Initialize();
        
        var mainView = new MainView();
        var mainViewModel = new MainViewModel(mainView);
        mainViewModel.Initialize();

        shellWindow.ContentPage = mainView;
        shellWindow.Activate();
    }

    private void ShellWindowClosed(object sender, WindowEventArgs args)
    {
        Log.Default.Info("{0} closed", ApplicationInfo.ProductName);
    }
}
