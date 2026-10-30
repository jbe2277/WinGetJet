using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;

namespace WinGetJet.Tests.Views;

public class MainView(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public Label AppsFoundTextBlock => this.Find("AppsFoundTextBlock").AsLabel();

    public CheckBox ShowOnlyUpdatesCheckBox => this.Find("ShowOnlyUpdatesCheckBox").As<CheckBox>();

    public Button RefreshButton => this.Find("RefreshButton").As<Button>();

    public ListBox AppsListView => this.Find("AppsListView").As<ListBox>();

    public DetailsView DetailsView => this.Find("DetailsView").As<DetailsView>();
}


public class AppsListViewItem(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public Label AppNameTextBlock => this.Find("AppNameTextBlock").AsLabel();

    public Label AppVersionAndPublisherTextBlock => this.Find("AppVersionAndPublisherTextBlock").AsLabel();
}