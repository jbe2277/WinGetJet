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

    public bool IsLoading { get; set => SetProperty(ref field, value); } = true;

    public string? ErrorMessage { get; set => SetProperty(ref field, value); }

    public PackageInstallRequirements? Requirements { get; set => SetProperty(ref field, value); }

    public bool HasAgreements => Requirements?.Agreements.Count > 0;

    public bool IsElevationRequired => Requirements?.Elevation is PackageElevation.Required or PackageElevation.ElevatesSelf;

    public bool AgreementsAccepted { get; set => SetProperty(ref field, value); }

    public bool CanUpdate => !IsLoading && Requirements is { IsInstallerAvailable: true } && (!HasAgreements || AgreementsAccepted);

    public Task<bool> ShowDialogAsync(object owner) => ViewCore.ShowDialogAsync(owner);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(Requirements))
        {
            RaisePropertyChanged(nameof(HasAgreements));
            RaisePropertyChanged(nameof(IsElevationRequired));
        }
        if (e.PropertyName is nameof(IsLoading) or nameof(Requirements) or nameof(AgreementsAccepted)) RaisePropertyChanged(nameof(CanUpdate));
    }
}
