using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinGetJet.ViewModels;

namespace WinGetJet.Views;

public sealed partial class UninstallView : ContentDialog, IUninstallView
{
    public UninstallView()
    {
        InitializeComponent();
    }

    public UninstallViewModel ViewModel => (UninstallViewModel)DataContext;

    public async Task<bool> ShowDialogAsync(object owner)
    {
        XamlRoot = ((UIElement)owner).XamlRoot;
        return await ShowAsync() == ContentDialogResult.Primary;
    }
}
