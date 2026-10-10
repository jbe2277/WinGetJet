using System.Waf.Applications;

namespace WinGetJet.ViewModels;

public interface IShellView : IView { }

public class ShellViewModel : ViewModelCore<IShellView>
{
    public ShellViewModel(IShellView view) : base(view, false)
    {
    }
}
