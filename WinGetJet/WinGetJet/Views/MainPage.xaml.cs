using System.Collections.ObjectModel;
using System.Diagnostics;
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
        Loaded += async (_, _) =>
        {
            try
            {
                var packages = await packageService.GetInstalledPackagesAsync();
                foreach (var app in packages.OrderBy(a => a.Name))
                    Apps.Add(new AppModel(app.Id, app.Name, app.Version, app.ProductCodes));
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log them, show a message to the user, etc.)
                Debug.WriteLine($"Error retrieving installed packages: {ex.Message}");
            }
        };
    }
}