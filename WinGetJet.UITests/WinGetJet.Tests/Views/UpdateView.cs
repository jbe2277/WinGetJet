using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using UITests;

namespace WinGetJet.Tests.Views;

public class UpdateView(FrameworkAutomationElementBase element) : AutomationElement(element)
{
    public Label PublisherTextBlock => this.Find("PublisherTextBlock").AsLabel();
    public Label InstalledVersionTextBlock => this.Find("InstalledVersionTextBlock").AsLabel();
    public Label TargetVersionTextBlock => this.Find("TargetVersionTextBlock").AsLabel();
    public Label SourceTextBlock => this.Find("SourceTextBlock").AsLabel();

    public AutomationElement ErrorInfoBar => this.Find("ErrorInfoBar");
    public AutomationElement ElevationInfoBar => this.Find("ElevationInfoBar");

    public AutomationElement AgreementsList => this.Find("AgreementsList");
    public CheckBox AcceptAgreementsCheckBox => this.Find("AcceptAgreementsCheckBox").AsCheckBox();

    public Button UpdateButton => this.Find("PrimaryButton").AsButton();
    public Button CancelButton => this.Find("CloseButton").AsButton();
}
