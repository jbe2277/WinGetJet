using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;
using WinGetJet.Tests.Controls;

namespace WinGetJet.Tests.Views;

public class DetailsView(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public Label NoAppSelectedTextBlock => this.Find("NoAppSelectedTextBlock").AsLabel();

    public AppIcon AppIcon => this.Find("AppIcon").As<AppIcon>();
    public Label AppNameTextBlock => this.Find("AppNameTextBlock").AsLabel();
    public Label PublisherHeaderTextBlock => this.Find("PublisherHeaderTextBlock").AsLabel();

    public Label InstalledVersionTextBlock => this.Find("InstalledVersionTextBlock").AsLabel();
    public ComboBox VersionComboBox => this.Find("VersionComboBox").AsComboBox();
    public Label SourceTextBlock => this.Find("SourceTextBlock").AsLabel();

    public Button UpdateButton => this.Find("UpdateButton").AsButton();
    public Button UninstallButton => this.Find("UninstallButton").AsButton();

    public Button PublisherHyperlinkButton => this.Find("PublisherHyperlinkButton").AsButton();
    public Label PublisherTextBlock => this.Find("PublisherTextBlock").AsLabel();

    public Button LicenseHyperlinkButton => this.Find("LicenseHyperlinkButton").AsButton();
    public Label LicenseTextBlock => this.Find("LicenseTextBlock").AsLabel();

    public Label CopyrightTextBlock => this.Find("CopyrightTextBlock").AsLabel();

    public Button HomepageHyperlinkButton => this.Find("HomepageHyperlinkButton").AsButton();
    public Button SupportHyperlinkButton => this.Find("SupportHyperlinkButton").AsButton();
    public IReadOnlyList<Button> DocumentationHyperlinkButtons => this.FindAll("DocumentationHyperlinkButton").Select(x => x.AsButton()).ToArray();

    public Button ReleaseNotesHyperlinkButton => this.Find("ReleaseNotesHyperlinkButton").AsButton();
    public ExpandableTextBlock ReleaseNotesTextBlock => this.Find("ReleaseNotesTextBlock").As<ExpandableTextBlock>();

    public ExpandableTextBlock DescriptionTextBlock => this.Find("DescriptionTextBlock").As<ExpandableTextBlock>();

    public ExpandableTextBlock TagsTextBlock => this.Find("TagsTextBlock").As<ExpandableTextBlock>();
}
