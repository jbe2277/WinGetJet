using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinGetJet.ViewModels;

namespace WinGetJet.Views;

public sealed partial class ShellWindow : Window, IShellView
{
    public ShellWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon("Assets/AppIcon.ico");

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
    }

    public ShellViewModel ViewModel => (ShellViewModel)DataContext!;

    public object? DataContext { get; set; }

    public Page? ContentPage 
    { 
        get => RootFrame.Content as Page;
        set => RootFrame.Content = value;
    }
}
