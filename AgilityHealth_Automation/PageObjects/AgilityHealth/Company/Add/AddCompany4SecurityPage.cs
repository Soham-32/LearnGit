using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add
{
    internal class AddCompany4SecurityPage : SecurityBasePage
    {
        public AddCompany4SecurityPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new AddCompanyHeaderWidget(driver, log);
        }

        public AddCompanyHeaderWidget Header { get; set; }

        private readonly By CreateCompanyButton = AutomationId.Equals("btnSaveClose");

        public void ClickCreateCompanyButton()
        {
            Log.Step(nameof(AddCompany4SecurityPage), "Click 'Create Company' button");
            Wait.UntilElementClickable(CreateCompanyButton).Click();
        }

    }
}
