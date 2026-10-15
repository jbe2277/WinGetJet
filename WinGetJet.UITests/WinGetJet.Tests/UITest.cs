using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;
using WinGetJet.Tests.Views;

namespace WinGetJet.Tests;

public abstract class UITest() : UITestBase("WinGetJet.exe",
        Environment.GetEnvironmentVariable("UITestExePath") ?? "../WinGetJet/WinGetJet/bin/x64/Debug/net10.0-windows10.0.26100.0/win-x64/",
        Environment.GetEnvironmentVariable("UITestOutputPath") ?? "TestResults/WinGetJet/")
{
    public Application Launch()
    {
        Log.WriteLine("");
        if (!File.Exists(Executable))
        {
            throw new FileNotFoundException("Build WinGetJet first or set UITestExePath to the directory containing WinGetJet.exe.", Executable);
        }
        Log.WriteLine($"Launch:          {Executable}");
        return App = Application.Launch(new System.Diagnostics.ProcessStartInfo(Executable)
        {
            WorkingDirectory = Path.GetDirectoryName(Executable)!
        });
    }

    public ShellWindow GetShellWindow() => (App!.GetMainWindow(Automation) ?? throw new ElementFoundException("MainWindow not found")).As<ShellWindow>();
}
