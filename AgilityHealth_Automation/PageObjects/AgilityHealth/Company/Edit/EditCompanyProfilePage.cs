using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit
{
    public class EditCompanyProfilePage : CompanyProfileBase
    {
        public EditCompanyProfilePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditCompanyHeaderWidget(driver, log);
        }

        public EditCompanyHeaderWidget Header { get; set; }

        public new void WaitUntilLoaded()
        {
            Wait.UntilAttributeNotEquals(CompanyNameTextBox, "value", "");
        }
    }
}