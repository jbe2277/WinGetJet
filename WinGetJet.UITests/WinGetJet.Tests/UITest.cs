using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;
using WinGetJet.Tests.Views;

namespace WinGetJet.Tests;

public abstract class UITest() : UITestBase(
        Environment.GetEnvironmentVariable("UITestWindowsAppId") ?? "E57BA85C-C29A-4E37-AC9B-24BCE7ED79B5_1z32rh13vfry6!App",
        Environment.GetEnvironmentVariable("UITestOutputPath") ?? "TestResults/WinGetJet/")
{
    public Application Launch()
    {
        Log.WriteLine("");
        Log.WriteLine($"Launch:          {WindowsAppId}");
        return App = Application.LaunchStoreApp(WindowsAppId);
    }

    public ShellWindow GetShellWindow() => (App!.GetMainWindow(Automation) ?? throw new ElementFoundException("MainWindow not found")).As<ShellWindow>();
}
