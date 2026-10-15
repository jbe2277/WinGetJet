using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using UITest;

namespace WinGetJet.Tests.Views;

public class ShellWindow(FrameworkAutomationElementBase element) : Window(element)
{
    public CheckBox ShowOnlyUpdatesCheckBox => this.Find(x => x.ByControlType(ControlType.CheckBox)
        .And(x.ByName("Show only apps with updates"))).AsCheckBox();
}