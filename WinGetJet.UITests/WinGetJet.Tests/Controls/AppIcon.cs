using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;

namespace WinGetJet.Tests.Controls;

public class AppIcon(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public AutomationElement UpdateBadge => this.Find("UpdateBadge");
}
