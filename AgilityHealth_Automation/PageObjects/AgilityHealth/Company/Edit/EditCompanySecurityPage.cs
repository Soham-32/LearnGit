using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit
{
    internal class EditCompanySecurityPage : SecurityBasePage
    {
        public EditCompanySecurityPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditCompanyHeaderWidget(driver, log);
        }

        public EditCompanyHeaderWidget Header { get; set; }

        // Locators

        //Key-customer verification
        #region Elements
        private readonly By SessionTimeoutFieldText = By.XPath("//label[text()='Session Timeout (Minutes)']");
        #endregion

        // Methods

        //Key-customer verification
        #region Elements
        public string GetSessionTimeoutFieldText()
        {
            Log.Step(nameof(SecurityBasePage), "Get the 'SessionTimeout' field text");
            return Wait.UntilElementVisible(SessionTimeoutFieldText).GetText();
        }
        #endregion
    }
}