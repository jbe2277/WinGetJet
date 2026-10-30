using FlaUI.Core.Definitions;
using Xunit;

namespace WinGetJet.Tests.Tests;

public class WinGetJetTest : UITest
{
    [Fact]
    public void LaunchShowsMainView()
    {
        Launch();
        var window = GetShellWindow();
        window.SetState(WindowVisualState.Maximized);

        Thread.Sleep(8000);
        window.CaptureToFile(GetScreenshotFile("Launch"));

        window.SearchBox.TextBox.Enter("git");

        Thread.Sleep(8000);
        window.CaptureToFile(GetScreenshotFile("Launch2"));
    }
}