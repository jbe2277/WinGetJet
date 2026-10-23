using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinGetJet.Views;

public sealed partial class ExpandableTextBlock : UserControl
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ExpandableTextBlock),
        new PropertyMetadata(null, (d, _) => ((ExpandableTextBlock)d).OnTextChanged()));

    public static readonly DependencyProperty CollapsedMaxLinesProperty = DependencyProperty.Register(nameof(CollapsedMaxLines), typeof(int), typeof(ExpandableTextBlock),
        new PropertyMetadata(5, (d, _) => ((ExpandableTextBlock)d).SetExpanded(false)));

    private bool isExpanded;

    public ExpandableTextBlock()
    {
        InitializeComponent();
        SetExpanded(false);
    }

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public int CollapsedMaxLines
    {
        get => (int)GetValue(CollapsedMaxLinesProperty);
        set => SetValue(CollapsedMaxLinesProperty, value);
    }

    private void OnTextChanged()
    {
        ContentTextBlock.Text = Text ?? "";
        SetExpanded(false);
    }

    private void ContentTextTrimmedChanged(TextBlock sender, IsTextTrimmedChangedEventArgs args)
    {
        if (!isExpanded) ExpandButton.Visibility = sender.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ExpandButtonClick(object sender, RoutedEventArgs e) => SetExpanded(!isExpanded);

    private void SetExpanded(bool expanded)
    {
        isExpanded = expanded;
        ContentTextBlock.MaxLines = expanded ? 0 : CollapsedMaxLines;
        ExpandButton.Content = expanded ? "Show less" : "Show more";
        ExpandButton.Visibility = expanded || ContentTextBlock.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }
}
