using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WindowsPackageManager;
using WinGetJet.Models;
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

    internal static string VersionAndPublisher(object? version, string? publisher) => 
        string.IsNullOrEmpty(publisher) ? $"{version}" : $"{version}   |   {publisher}";

    internal static Visibility IsOperationVisible(InstallationOperationStatus status) => 
        status != InstallationOperationStatus.None ? Visibility.Visible : Visibility.Collapsed;

    internal static Visibility IsOperationInProgress(InstallationOperationStatus status) => 
        status is not InstallationOperationStatus.Pending and not InstallationOperationStatus.InProgress ? Visibility.Collapsed : Visibility.Visible;

    internal static Visibility IsOperationCompleted(InstallationOperationStatus status) => 
        status == InstallationOperationStatus.Completed ? Visibility.Visible : Visibility.Collapsed;

    internal static string DownloadInfo(ulong bytesDownloaded, ulong bytesToDownload, double progress) => 
        bytesToDownload == 0 
        ? $"Download: {progress:P0}"
        : $"Download: {progress:P0} ({bytesDownloaded / 1_000_000:N1} / {bytesToDownload / 1_000_000:N1} MB)";

    internal static Visibility IsDownloadVisible(InstallationOperation operation) =>
        operation == InstallationOperation.Uninstall ? Visibility.Collapsed : Visibility.Visible;

    internal static string InstallInfo(InstallationOperation operation, double progress) =>
        operation == InstallationOperation.Uninstall ? $"Uninstall: {progress:P0}" : $"Install: {progress:P0}";

    internal static string ResultInfo(InstallationOperation operation, InstallationResultStatus result, uint errorCode, Exception? exception, bool isRebootRequired)
    {
        var name = operation == InstallationOperation.Uninstall ? "Uninstall" : "Update";
        var text = result == InstallationResultStatus.Success ? $"{name} completed successfully." : $"{name} failed: {result}";
        if (errorCode != 0) text += $" (Error code: 0x{errorCode:X8})";
        if (exception is not null) text += Environment.NewLine + exception.Message;
        if (isRebootRequired) text += Environment.NewLine + "A reboot is required to complete the installation.";
        return text;
    }
}
