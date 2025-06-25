using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add
{
    internal class AddCompany1CompanyProfilePage : CompanyProfileBase
    {
        public AddCompany1CompanyProfilePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new AddCompanyHeaderWidget(driver, log);
        }

        public AddCompanyHeaderWidget Header { get; set; }

        private readonly By NextButton = AutomationId.Equals("btnNext");
        
        
        public bool IsCompanyNameVisible()
        {
            return Driver.GetElementCount(CompanyNameTextBox) > 0 && Wait.UntilElementExists(CompanyNameTextBox).Displayed;
        }

        public void ClickNextButton()
        {
            Wait.UntilElementClickable(NextButton).Click();
        }

    }
}
