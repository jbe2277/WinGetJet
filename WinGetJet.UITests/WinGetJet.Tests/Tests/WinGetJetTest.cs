using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
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
        var mainView = window.MainView;
        var detailsView = mainView.DetailsView;

        Retry.WhileTrue(() => string.IsNullOrEmpty(detailsView.AppNameTextBlock.Text), throwOnTimeout: true);
        Thread.Sleep(1000);
        window.CaptureToFile(GetScreenshotFile("Launch"));

        window.SearchBox.TextBox.Enter("git");
        Retry.WhileFalse(() => "Git" == detailsView.AppNameTextBlock.Text, throwOnTimeout: true);
        Thread.Sleep(1000);
        window.CaptureToFile(GetScreenshotFile("Launch2"));
        
        var itemsCount = mainView.AppsListView.Items.Length;  // Virtualization will only load the visible items
        var appsFoundCount = int.Parse(mainView.AppsFoundTextBlock.Text.Split(" ")[0]);
        Assert.True(appsFoundCount >= itemsCount);

        var selectedApp = mainView.AppsListView.SelectedItem.As<AppsListViewItem>()!;
        Assert.Equal("Git", selectedApp.AppNameTextBlock.Text);
        Assert.Contains("The Git Development Community", selectedApp.AppVersionAndPublisherTextBlock.Text);

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