using System.ComponentModel;
using System.Waf.Applications;
using System.Waf.Foundation;
using WinGetJet.Models;

namespace WinGetJet.ViewModels;

public interface IMainView : IView { }

public class MainViewModel : ViewModelCore<IMainView>
{
    private readonly ObservableList<AppModel> appsSource = [];

    public MainViewModel(IMainView view) : base(view, false)
    {
        AppsView = new(appsSource, null, Filter, null);

        AppsView.PropertyChanged += AppsViewPropertyChanged;
        AppsView.CollectionItemChanged += AppsViewCollectionItemChanged;
        UpdateCountProperty();
    }

    public ObservableList<AppModel> AppsSource => appsSource;

    public ObservableListViewCore<AppModel> AppsView { get; }

    public string FilterText { get; set => SetProperty(ref field, value); } = "";

    public int UpdateCount { get; set => SetProperty(ref field, value); }

    public bool ShowOnlyUpdates { get; set => SetProperty(ref field, value); }

    public AppModel? SelectedApp { get; set => SetProperty(ref field, value); }

    public string? StatusText { get; set => SetProperty(ref field, value); }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(FilterText) or nameof(ShowOnlyUpdates)) AppsView.Update();
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

    private void AppsViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppsView.Count)) UpdateCountProperty();
    }

    private void AppsViewCollectionItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppModel.IsUpdateAvailable)) UpdateCountProperty();
    }
}
