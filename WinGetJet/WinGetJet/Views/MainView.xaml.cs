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

    private void AppsListViewSelectionChanged(object sender, SelectionChangedEventArgs e) => ViewModel.SelectedApp = ((ListView)sender).SelectedItem as AppModel;

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        StatusTextBlock.Text = "Retrieving installed packages...";
        try
        {
            var packages = await packageService.GetInstalledPackagesAsync();
            foreach (var app in packages.OrderBy(a => a.Name))
                ViewModel.AppsSource.Add(new AppModel(app.Id, app.Name, app.Version, app.ProductCodes));

            await Parallel.ForEachAsync(ViewModel.AppsView, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                async (app, cancellation) =>
                {
                    var details = await packageService.LoadDetailsAsync(app.Id, cancellation);
                    if (details is not null)
                    {
                        await TaskHelper.Run(async () =>
                        {
                            app.Available = details.Available;
                            app.Source = details.Source;
                            app.IsUpdateAvailable = details.IsUpdateAvailable;
                            app.Publisher = details.Publisher;
                            app.Author = details.Author;
                            app.Description = details.Description;
                            app.PackageUrl = details.PackageUrl;
                            app.License = details.License;
                            app.IconUrl = details.IconUrl;
                            app.PublisherUrl = details.PublisherUrl;
                            app.PublisherSupportUrl = details.PublisherSupportUrl;
                            app.LicenseUrl = details.LicenseUrl;
                            app.Copyright = details.Copyright;
                            app.ReleaseNotes = details.ReleaseNotes;
                            app.ReleaseNotesUrl = details.ReleaseNotesUrl;
                            app.Documentations = [.. details.Documentations.Select(d => new AppDocumentation(d.Label, d.Url))];
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
}