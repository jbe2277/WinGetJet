using Microsoft.UI.Xaml;
using WinGetJet.Views;

namespace WinGetJet;

public partial class App : Application
{
    private Window? window;
    
    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        window = new ShellWindow();
        window.Activate();
    }
}
