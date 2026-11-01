using System.ComponentModel;
using System.Waf.Applications;

namespace WinGetJet.ViewModels;

public interface IShellView : IView 
{
    object? ContentPage { get; set; }
}

public class ShellViewModel : ViewModelCore<IShellView>
{
    public ShellViewModel(IShellView view) : base(view, false)
    {
    }

    public IViewModelCore? ContentViewModel { get; set => SetProperty(ref field, value); }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(ContentViewModel))
        {
            ViewCore.ContentPage = ContentViewModel?.View;
            ContentViewModel?.Initialize();
        }
    }
}
