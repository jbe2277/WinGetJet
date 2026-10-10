using System.Collections.ObjectModel;
using System.Waf.Applications;
using WinGetJet.Models;

namespace WinGetJet.ViewModels;

public interface IMainView : IView { }

public class MainViewModel : ViewModelCore<IMainView>
{
    public MainViewModel(IMainView view) : base(view, false)
    {
    }

    public ObservableCollection<AppModel> Apps { get; } = [];
}
