using System.ComponentModel;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;
using WinGetJet.Models;

namespace WinGetJet.ViewModels;

public interface IMainView : IView { }

public class MainViewModel : ViewModelCore<IMainView>
{
    private readonly ObservableList<AppModel> appsSource = [];

    public MainViewModel(IMainView view) : base(view, false)
    {
        AppsView = new(appsSource, null, Filter, null);
        appsSource.CollectionItemChanged += AppsSourceCollectionItemChanged;

        AppsView.PropertyChanged += AppsViewPropertyChanged;
        AppsView.CollectionItemChanged += AppsViewCollectionItemChanged;
        UpdateCountProperty();
    }

    public ObservableList<AppModel> AppsSource => appsSource;

    public ObservableListViewCore<AppModel> AppsView { get; }

    public ICommand RefreshCommand { get; internal set; } = DelegateCommand.DisabledCommand;

    public string FilterText { get; set => SetProperty(ref field, value); } = "";

    public int UpdateCount { get; set => SetProperty(ref field, value); }

    public bool ShowOnlyUpdates { get; set => SetProperty(ref field, value); }

    public AppModel? SelectedApp { get; set => SetProperty(ref field, value); }

    public string? StatusText { get; set => SetProperty(ref field, value); }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(FilterText) or nameof(ShowOnlyUpdates)) 
        { 
            var selectedApp = SelectedApp;
            AppsView.Update();
            if (selectedApp is not null && AppsView.Contains(selectedApp))
                SelectedApp = selectedApp;
            else
                SelectedApp = AppsView.FirstOrDefault();
        }
    }

    private bool Filter(AppModel app)
    {
        return
            (string.IsNullOrEmpty(FilterText)
                || ContainsFilter(app.Name)
                || ContainsFilter(app.DefaultVersionDetails?.Publisher.Text)
                || ContainsFilter(app.DefaultVersionDetails?.Description)
                || app.DefaultVersionDetails?.Tags.Any(x => ContainsFilter(x)) == true
            )
            && (!ShowOnlyUpdates || app.IsUpdateAvailable);

        bool ContainsFilter(string? text) => !string.IsNullOrEmpty(text) && text.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase);
    }

    private void UpdateCountProperty() => UpdateCount = AppsView.Count(x => x.IsUpdateAvailable);

    private void AppsSourceCollectionItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppModel.IsUpdateAvailable)) AppsView.Update();
    }

    private void AppsViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppsView.Count)) UpdateCountProperty();
    }

    private void AppsViewCollectionItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppModel.IsUpdateAvailable)) UpdateCountProperty();
    }
}
