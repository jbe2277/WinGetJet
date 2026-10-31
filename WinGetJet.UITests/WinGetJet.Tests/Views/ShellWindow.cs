using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using UITests;
using WinGetJet.Tests.Controls;

namespace WinGetJet.Tests.Views;

public class ShellWindow(FrameworkAutomationElementBase element) : Window(element)
{
    public MainView MainView => this.Find("MainView").As<MainView>();

    public AutoSuggestBox SearchBox => this.Find("SearchBox").As<AutoSuggestBox>();

    public void SetState(WindowVisualState state) => Patterns.Window.Pattern.SetWindowVisualState(state);
}