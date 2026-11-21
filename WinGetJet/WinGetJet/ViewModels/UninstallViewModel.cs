using System.Waf.Applications;
using WinGetJet.Models;

namespace WinGetJet.ViewModels;

public interface IUninstallView : IView
{
    Task<bool> ShowDialogAsync(object owner);
}

public class UninstallViewModel(IUninstallView view) : ViewModelCore<IUninstallView>(view, false)
{
    public AppModel? App { get; set => SetProperty(ref field, value); }

    public Task<bool> ShowDialogAsync(object owner) => ViewCore.ShowDialogAsync(owner);
}
