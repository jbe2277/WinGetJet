using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WindowsPackageManager;
using WinGetJet.Models;

namespace WinGetJet.Views;

public sealed partial class MainPage : Page
{
    private readonly PackageService packageService = new();
    public ObservableCollection<AppModel> Apps { get; } = [];

    public MainPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        StatusTextBlock.Text = "Retrieving installed packages...";
        try
        {
            var packages = await packageService.GetInstalledPackagesAsync();
            foreach (var app in packages.OrderBy(a => a.Name))
                Apps.Add(new AppModel(app.Id, app.Name, app.Version, app.ProductCodes));

            await Parallel.ForEachAsync(Apps, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                async (app, cancellation) =>
                {
                    var details = await packageService.LoadDetailsAsync(app.Id, cancellation);
                    if (details is not null)
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
                        //app.Icon = IconService.ResolveIcon(app);
                    }
                });

            StatusTextBlock.Text = $"Retrieved {Apps.Count} installed packages.";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Error retrieving installed packages: {ex.Message}";
        }
    }
}