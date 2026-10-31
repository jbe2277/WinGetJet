using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinGetJet.Views;

public sealed partial class AppIcon : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(AppIcon), new PropertyMetadata(null, IconChanged));

    public static readonly DependencyProperty IsUpdateAvailableProperty =
        DependencyProperty.Register(nameof(IsUpdateAvailable), typeof(bool), typeof(AppIcon), new PropertyMetadata(false));

    public AppIcon()
    {
        InitializeComponent();
    }

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsUpdateAvailable
    {
        get => (bool)GetValue(IsUpdateAvailableProperty);
        set => SetValue(IsUpdateAvailableProperty, value);
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private static void IconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((AppIcon)d).PlaceholderIcon.Visibility = e.NewValue is null ? Visibility.Visible : Visibility.Collapsed;
    }

    private void RootGridSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var size = Math.Min(e.NewSize.Width, e.NewSize.Height);
        UpdateBadge.Width = UpdateBadge.Height = Math.Round(size * 0.45);
        var offset = -Math.Round(size * 0.08);
        UpdateBadge.Margin = new Thickness(0, 0, offset, offset);
    }
}
