using Microsoft.UI.Xaml;

namespace WinGetJet.Views;

public sealed partial class ShellWindow : Window
{
    public ShellWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon("Assets/AppIcon.ico");

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        RootFrame.Navigate(typeof(MainPage));
    }
}
