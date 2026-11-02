using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinGetJet.ViewModels;

namespace WinGetJet.Views;

public sealed partial class UpdateView : ContentDialog, IUpdateView
{
    public UpdateView()
    {
        InitializeComponent();
    }

    public UpdateViewModel ViewModel => (UpdateViewModel)DataContext;

    public async Task<bool> ShowDialogAsync(object owner)
    {
        XamlRoot = ((UIElement)owner).XamlRoot;
        return await ShowAsync() == ContentDialogResult.Primary;
    }
}
