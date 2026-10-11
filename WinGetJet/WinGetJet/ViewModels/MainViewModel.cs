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
    }

    public ObservableList<AppModel> AppsSource => appsSource;

    public ObservableListViewCore<AppModel> AppsView { get; }

    public string FilterText { get; set => SetProperty(ref field, value); } = "";

    private bool Filter(AppModel app)
    {
        return string.IsNullOrEmpty(FilterText)
            || (!string.IsNullOrEmpty(app.Name) && app.Name.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase));
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(FilterText))
        {
            AppsView.Update();
        }
    }
}
