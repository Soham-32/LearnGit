using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit
{
    public class EditCompanyHeaderWidget : BasePage
    {
        public EditCompanyHeaderWidget(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By SaveButton = AutomationId.Equals("btnEditSave");
        private readonly By CancelButton = AutomationId.Equals("btnEditCancel");
        private readonly By CloseButton = AutomationId.Equals("btnClose");
        private readonly By RadarSelectionTab = By.XPath("//button[contains(normalize-space(), 'Radar Selection')]");
        private readonly By SubscriptionTab = By.XPath("//button[text() = 'Subscription'] | //font[text() = 'Subscription']");
        private readonly By SecurityTab = By.XPath("//button[text() = 'Security']");
        // save as draft popup
        private readonly By SaveAsDraftButton = AutomationId.Equals("btnSaveDraft");
        private readonly By CancelPopupCancelButton = AutomationId.Equals("btnDelete");

        public void ClickSaveButton()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Save' button");
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.HardWait(3000); //wait till company added
        }

        public void ClickCancelButton()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Cancel' button");
            Wait.UntilElementClickable(CancelButton).Click();
        }

        public void ClickCloseButton()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Close' button");
            Wait.UntilElementClickable(CloseButton).Click();
        }

        public void ClickSaveAsDraftButton()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Save As Draft' button");
            Wait.UntilElementVisible(SaveAsDraftButton);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SaveAsDraftButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickCancelButtonOnPopup()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Cancel' button on the popup");
            Wait.UntilElementVisible(CancelPopupCancelButton);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CancelPopupCancelButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnRadarSelectionTab()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Radar Selection' tab");
            Wait.UntilElementClickable(RadarSelectionTab).Click();
        }

        public void ClickOnSubscriptionTab()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Subscription' tab");
            Wait.UntilElementClickable(SubscriptionTab).Click();
        }

        public void ClickOnSecurityTab()
        {
            Log.Step(nameof(EditCompanyHeaderWidget), "Click 'Security' tab");
            Wait.UntilElementClickable(SecurityTab).Click();
        }
    }
}
