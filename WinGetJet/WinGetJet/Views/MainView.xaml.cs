using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WindowsPackageManager;
using WinGetJet.ViewModels;

namespace WinGetJet.Views;

public sealed partial class MainView : Page, IMainView
{
    public MainView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public MainViewModel ViewModel => (MainViewModel)DataContext;

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += ViewModelPropertyChanged;
    }

    private void ViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // ComboBox resets SelectedItem when ItemsSource changes; re-apply the selection afterwards
        if (e.PropertyName == nameof(MainViewModel.SelectedApp))
            DispatcherQueue.TryEnqueue(() => VersionComboBox.SelectedItem = ViewModel.SelectedApp?.SelectedVersion);
    }

    private void VersionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // ComboBox sets SelectedItem to null when ItemsSource changes; do not propagate this to the model
        if (VersionComboBox.SelectedItem is PackageVersionKey version && ViewModel.SelectedApp is { } app) app.SelectedVersion = version;
    }
}