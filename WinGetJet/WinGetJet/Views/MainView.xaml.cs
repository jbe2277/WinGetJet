using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Waf.Foundation;
using WindowsPackageManager;
using WinGetJet.Models;
using WinGetJet.Services;
using WinGetJet.ViewModels;

namespace WinGetJet.Views;

public sealed partial class MainView : Page, IMainView
{
    private readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private readonly PackageService packageService = new();

    public MainView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public MainViewModel ViewModel => (MainViewModel)DataContext;

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += ViewModelPropertyChanged;
        StatusTextBlock.Text = "Retrieving installed packages...";
        try
        {
            var packages = await packageService.GetInstalledPackagesAsync();
            foreach (var app in packages.OrderBy(a => a.Name))
                ViewModel.AppsSource.Add(new AppModel(app.Id, app.Name, app.Version, app.ProductCodes));
            ViewModel.SelectedApp = ViewModel.AppsView.FirstOrDefault();

            await Parallel.ForEachAsync(ViewModel.AppsView, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                async (app, cancellation) =>
                {
                    var details = await packageService.LoadDetailsAsync(app.Id, new(PackageVersionKind.DefaultInstall), cancellation);
                    if (details is not null)
                    {
                        await TaskHelper.Run(async () =>
                        {
                            app.AvailableVersions = details.AvailableVersions;
                            app.SelectedVersion = details.AvailableVersions.FirstOrDefault();
                            app.Source = details.Source;
                            app.IsUpdateAvailable = details.IsUpdateAvailable;
                            app.Publisher = new(details.Publisher.Text, details.Publisher.Url);
                            app.Author = details.Author;
                            app.Description = details.Description;
                            app.PackageUrl = details.PackageUrl;
                            app.License = new(details.License.Text, details.License.Url);
                            app.IconUrl = details.IconUrl;
                            app.PublisherSupportUrl = details.PublisherSupportUrl;
                            app.Copyright = details.Copyright;
                            app.ReleaseNotes = new(details.ReleaseNotes.Text, details.ReleaseNotes.Url);
                            app.Documentations = [.. details.Documentations.Select(d => new AppTextLink(d.Text, d.Url))];
                            app.Tags = details.Tags;
                            app.Icon = await IconService.ResolveIconAsync(app);
                        }, uiScheduler, cancellation);
                    }
                });

            StatusTextBlock.Text = $"Retrieved {ViewModel.AppsView.Count} installed packages.";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error retrieving installed packages: {ex.Message}";
        }
    }

    private void ViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // ComboBox resets SelectedItem when ItemsSource changes; re-apply the selection afterwards
        if (e.PropertyName == nameof(MainViewModel.SelectedApp))
        {
            DispatcherQueue.TryEnqueue(() => VersionComboBox.SelectedItem = ViewModel.SelectedApp?.SelectedVersion);
            SetReleaseNotesExpanded(false);
            SetDescriptionExpanded(false);
        }
    }

    private void DescriptionTextTrimmedChanged(TextBlock sender, IsTextTrimmedChangedEventArgs args)
    {
        if (DescriptionTextBlock.MaxLines != 0)
            DescriptionExpandButton.Visibility = sender.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void DescriptionExpandButtonClick(object sender, RoutedEventArgs e) => SetDescriptionExpanded(DescriptionTextBlock.MaxLines != 0);

    private void SetDescriptionExpanded(bool expanded)
    {
        DescriptionTextBlock.MaxLines = expanded ? 0 : 5;
        DescriptionExpandButton.Content = expanded ? "Show less" : "Show more";
        DescriptionExpandButton.Visibility = expanded || DescriptionTextBlock.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ReleaseNotesTextTrimmedChanged(TextBlock sender, IsTextTrimmedChangedEventArgs args)
    {
        if (ReleaseNotesTextBlock.MaxLines != 0)
            ReleaseNotesExpandButton.Visibility = sender.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ReleaseNotesExpandButtonClick(object sender, RoutedEventArgs e) => SetReleaseNotesExpanded(ReleaseNotesTextBlock.MaxLines != 0);

    private void SetReleaseNotesExpanded(bool expanded)
    {
        ReleaseNotesTextBlock.MaxLines = expanded ? 0 : 5;
        ReleaseNotesExpandButton.Content = expanded ? "Show less" : "Show more";
        ReleaseNotesExpandButton.Visibility = expanded || ReleaseNotesTextBlock.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void VersionComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // ComboBox sets SelectedItem to null when ItemsSource changes; do not propagate this to the model
        if (VersionComboBox.SelectedItem is PackageVersionKey version && ViewModel.SelectedApp is { } app) app.SelectedVersion = version;
    }
}