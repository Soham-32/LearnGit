using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add
{
    internal class AddCompanyHeaderWidget : BasePage
    {
        public AddCompanyHeaderWidget(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By DeleteButton = AutomationId.Equals("btnDelete");
        private readonly By SaveAsDraftButton = AutomationId.Equals("btnSaveDraft");
        private readonly By CloseButton = AutomationId.Equals("btnClose");

        public void ClickDeleteButton()
        {
            Log.Step(nameof(AddCompanyHeaderWidget), "Click 'Delete' button");
            Wait.UntilElementClickable(DeleteButton).Click();
        }

        public void ClickSaveAsDraftButton()
        {
            Log.Step(nameof(AddCompanyHeaderWidget), "Click 'Save As Draft' button");
            Wait.UntilElementClickable(SaveAsDraftButton).Click();
        }

        public void ClickCloseButton()
        {
            Log.Step(nameof(AddCompanyHeaderWidget), "Click 'Close' button");
            Wait.UntilElementClickable(CloseButton).Click();
        }
    }
}
