using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using WinGetJet.Tests.Views;
using Xunit;

namespace WinGetJet.Tests.Tests;

public class WinGetJetTest : UITest
{
    [Fact]
    public void SearchForGitAndVerifyDetails()
    {
        Launch();
        var window = GetShellWindow();
        window.SetState(WindowVisualState.Maximized);

        Thread.Sleep(8000);
        window.CaptureToFile(GetScreenshotFile("Launch"));

        window.SearchBox.TextBox.Enter("git");
        Thread.Sleep(1000);
        window.CaptureToFile(GetScreenshotFile("Launch2"));
        var mainView = window.MainView;
        
        var itemsCount = mainView.AppsListView.Items.Length;
        Assert.Contains(itemsCount.ToString(), mainView.AppsFoundTextBlock.Text);
        
        var selectedApp = mainView.AppsListView.SelectedItem.As<AppsListViewItem>()!;
        Assert.Equal("Git", selectedApp.AppNameTextBlock.Text);
        Assert.Contains("The Git Development Community", selectedApp.AppVersionAndPublisherTextBlock.Text);

        var detailsView = mainView.DetailsView;
        Assert.Equal("Git", detailsView.AppNameTextBlock.Text);
        Assert.Equal("The Git Development Community", detailsView.PublisherHeaderTextBlock.Text);

        Assert.NotEmpty(detailsView.InstalledVersionTextBlock.Text);
        Assert.NotEmpty(detailsView.VersionComboBox.SelectedItem?.Text ?? "");

        Assert.Equal("The Git Development Community", detailsView.PublisherTextBlock.Text);
        Assert.False(detailsView.PublisherHyperlinkButton.IsOffscreen);

        Assert.Equal("GPL-2.0", detailsView.LicenseTextBlock.Text);
        Assert.False(detailsView.LicenseHyperlinkButton.IsOffscreen);

        Assert.Contains("Free Software Foundation, Inc.", detailsView.CopyrightTextBlock.Text);

        Assert.False(detailsView.HomepageHyperlinkButton.IsOffscreen);
        Assert.False(detailsView.SupportHyperlinkButton.IsOffscreen);
        Assert.Equal("Wiki", detailsView.DocumentationHyperlinkButtons.Single().Name);

        Assert.NotEmpty(detailsView.ReleaseNotesTextBlock.TextBox.Text);
        Assert.False(detailsView.ReleaseNotesHyperlinkButton.IsOffscreen);

        Assert.Contains("Git is a free", detailsView.DescriptionTextBlock.TextBox.Text);

        Assert.Contains("git", detailsView.TagsTextBlock.TextBox.Text);
    }
}