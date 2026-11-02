using System.ComponentModel;
using System.Waf.Applications;
using WindowsPackageManager;
using WinGetJet.Models;

namespace WinGetJet.ViewModels;

public interface IUpdateView : IView
{
    Task<bool> ShowDialogAsync(object owner);
}

public class UpdateViewModel(IUpdateView view) : ViewModelCore<IUpdateView>(view, false)
{
    public AppModel? App { get; set => SetProperty(ref field, value); }

    public PackageVersionKey? TargetVersion { get; set => SetProperty(ref field, value); }

    public AppVersionModel? TargetVersionDetails { get; set => SetProperty(ref field, value); }

    public string? ErrorMessage { get; set => SetProperty(ref field, value); }

    public bool HasAgreements => TargetVersionDetails?.Agreements.Count > 0;

    public bool IsElevationRequired => TargetVersionDetails?.InstallRequirements.Elevation is PackageElevation.Required or PackageElevation.ElevatesSelf;

    public bool AgreementsAccepted { get; set => SetProperty(ref field, value); }

    public bool CanUpdate => TargetVersionDetails?.InstallRequirements is { IsInstallerAvailable: true } && (!HasAgreements || AgreementsAccepted);

    public Task<bool> ShowDialogAsync(object owner) => ViewCore.ShowDialogAsync(owner);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(TargetVersionDetails))
        {
            RaisePropertyChanged(nameof(HasAgreements));
            RaisePropertyChanged(nameof(IsElevationRequired));
        }
        if (e.PropertyName is nameof(TargetVersionDetails) or nameof(AgreementsAccepted)) RaisePropertyChanged(nameof(CanUpdate));
    }
}
