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

    private void SearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason is not AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
        {
            ((MainViewModel)((MainView)ContentPage!).DataContext).FilterText = SearchBox.Text;
        }
    }

    private void AppTitleBarSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SearchBox.Width = Math.Clamp(e.NewSize.Width * 0.4, 200, 400);
    }
}
