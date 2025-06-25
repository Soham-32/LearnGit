using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add
{
    internal class AddCompany3SubscriptionPage : SubscriptionBasePage
    {
        public AddCompany3SubscriptionPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new AddCompanyHeaderWidget(driver, log);
        }

        public AddCompanyHeaderWidget Header { get; set; }

        private readonly By BackButton = AutomationId.Equals("btnBack");
        private readonly By NextButton = AutomationId.Equals("btnNext");

        public void ClickNextButton()
        {
            Log.Step(nameof(AddCompany3SubscriptionPage), "Click 'Next' button");
            Wait.UntilElementClickable(NextButton).Click();
        }

        public void ClickBackButton()
        {
            Log.Step(nameof(AddCompany3SubscriptionPage), "Click 'Back' button");
            Wait.UntilElementClickable(BackButton).Click();
        }

    }
}
