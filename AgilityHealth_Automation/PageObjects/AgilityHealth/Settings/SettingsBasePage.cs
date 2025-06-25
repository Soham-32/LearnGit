using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings
{
    public class SettingsBasePage : BasePage
    {
        public SettingsBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Key Customer Verification

        #region Settings Elements
        
        private readonly By PageTitle = By.XPath("//h2 | //h1");
        private readonly By CompanySelectionDropdown = By.XPath("//span[contains(text(), 'Select')]");
        private static By CompanyName(string name) => By.XPath($"//ul[@id='Companies_listbox']//li[text()='{name}']");
        private readonly By GoButton = By.XPath("//input[@id='company'] | //span[text()='Apply']");
        #endregion

        //Key Customer Verification

        #region Settings Elements

        public void SelectCompanyFromSettings(string companyName)
        {
            Log.Step(nameof(SettingsV2Page), $"Select company <{companyName}> from the dropdown");
            SelectItem(CompanySelectionDropdown, CompanyName(companyName));
            Wait.UntilElementClickable(GoButton).Click();
        }

        public string GetPageTitle()
        {
            return Wait.UntilElementVisible(PageTitle).GetText();
        }

        public void ClickOnGoButton()
        {
            Log.Step(nameof(SettingsV2Page), "Click on 'Go' button");
            Wait.UntilElementClickable(GoButton).Click();
        }
        #endregion
    }
}
