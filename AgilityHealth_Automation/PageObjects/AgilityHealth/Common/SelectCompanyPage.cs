using AgilityHealth_Automation.Base;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Common
{
    public class SelectCompanyPage : BasePage
    {
        public SelectCompanyPage(IWebDriver driver) : base(driver) { }

        // Locators
        private readonly By SelectCompanies = By.XPath("//form[@action]//span[text()='Select a Company']");
        private static By CompanyListItem(string companyName) => By.XPath($"//ul[@id='company_listbox']//li[text()='{companyName}']");
        private readonly By SelectButton = By.XPath("//input[@type='submit']");

        // Methods
        public void SelectCompany(string companyName)
        {
            Log.Step(nameof(SelectCompanyPage), "Select a Company from dropdown");
            SelectItem(SelectCompanies, CompanyListItem(companyName));
            Wait.UntilElementClickable(SelectButton).Click();
            Wait.HardWait(4000); //Need to wait to team dashboard to load
        }

    }
}
