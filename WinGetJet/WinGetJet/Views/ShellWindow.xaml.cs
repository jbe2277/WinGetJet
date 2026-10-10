using Microsoft.UI.Xaml;

namespace WinGetJet.Views;

public sealed partial class ShellWindow : Window
{
    public ShellWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");

        RootFrame.Navigate(typeof(MainPage));
    }
}
