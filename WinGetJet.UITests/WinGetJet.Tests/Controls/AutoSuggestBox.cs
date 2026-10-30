using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;

namespace WinGetJet.Tests.Controls;

public class AutoSuggestBox(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public TextBox TextBox => this.Find("TextBox").AsTextBox();
}
