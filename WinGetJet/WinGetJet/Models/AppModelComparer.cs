namespace WinGetJet.Models;

public sealed class AppModelComparer : IEqualityComparer<AppModel>
{
    public static AppModelComparer Default { get; } = new();

    public bool Equals(AppModel? x, AppModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Id == y.Id && x.Version == y.Version;
    }

    public int GetHashCode(AppModel obj) => (obj.Id, obj.Version).GetHashCode();    
}
