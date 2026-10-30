using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;

namespace WinGetJet.Tests.Controls;

public class ExpandableTextBlock(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public TextBox TextBox => this.Find("ContentTextBlock").AsTextBox();

    public Button ExpandButton => this.Find("ExpandButton").AsButton();
}
