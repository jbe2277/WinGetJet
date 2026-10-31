using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Waf.Foundation;
using WinGetJet.ViewModels;

namespace WinGetJet.Views;

public sealed partial class ShellWindow : Window, IShellView
{
    private readonly ThrottledAction updateFilterAction;

    public ShellWindow()
    {
        updateFilterAction = new ThrottledAction(UpdateFilter, ThrottledActionMode.InvokeOnlyIfIdleForDelayTime, TimeSpan.FromMilliseconds(300));
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

    private void UpdateFilter()
    {
        ((MainViewModel)((MainView)ContentPage!).DataContext).FilterText = SearchBox.Text;
    }

    private void SearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason is not AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
            updateFilterAction.InvokeAccumulated();
    }

    private void AppTitleBarSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SearchBox.Width = Math.Clamp(e.NewSize.Width * 0.4, 200, 400);
    }
}
