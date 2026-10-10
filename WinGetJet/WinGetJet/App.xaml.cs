using Microsoft.UI.Xaml;
using WinGetJet.ViewModels;
using WinGetJet.Views;

namespace WinGetJet;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var shellWindow = new ShellWindow();
        var shellViewModel = new ShellViewModel(shellWindow);
        shellViewModel.Initialize();
        
        var mainView = new MainView();
        var mainViewModel = new MainViewModel(mainView);
        mainViewModel.Initialize();

        shellWindow.ContentPage = mainView;
        shellWindow.Activate();
    }
}
