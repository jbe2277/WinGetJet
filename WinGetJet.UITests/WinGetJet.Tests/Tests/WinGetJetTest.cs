using Xunit;

namespace WinGetJet.Tests.Tests;

public class WinGetJetTest : UITest
{
    [Fact]
    public void LaunchShowsMainView()
    {
        Launch();
        var window = GetShellWindow();

        Assert.Equal("WinGetJet", window.Title);
        Assert.False(window.ShowOnlyUpdatesCheckBox.IsOffscreen);
        Assert.True(window.ShowOnlyUpdatesCheckBox.IsEnabled);
    }
}