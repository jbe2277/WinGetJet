using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinGetJet.Views;

public enum AppIconSize
{
    Small,
    Large
}

public sealed partial class AppIcon : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(AppIcon), new PropertyMetadata(null));

    public static readonly DependencyProperty IsUpdateAvailableProperty =
        DependencyProperty.Register(nameof(IsUpdateAvailable), typeof(bool), typeof(AppIcon), new PropertyMetadata(false));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(AppIconSize), typeof(AppIcon), new PropertyMetadata(AppIconSize.Small, OnSizeChanged));

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

    public AppIconSize Size
    {
        get => (AppIconSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((AppIcon)d).ApplySize();

    private void ApplySize()
    {
        var large = Size == AppIconSize.Large;
        RootGrid.Width = RootGrid.Height = large ? 48 : 24;
        IconImage.Width = IconImage.Height = large ? 48 : 24;
        PlaceholderIcon.FontSize = large ? 40 : 20;
        UpdateBadge.Width = UpdateBadge.Height = large ? 20 : 14;
        UpdateBadge.CornerRadius = new CornerRadius(large ? 10 : 6);
        UpdateBadge.Margin = large ? new Thickness(0, 0, -4, -4) : new Thickness(0, 0, -3, -3);
        UpdateGlyph.FontSize = large ? 12 : 9;
    }
}
