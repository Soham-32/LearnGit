using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.PowerBi
{
    public class AuditRefreshPage : BasePage
    {
        public AuditRefreshPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Locators
        private readonly By EmailTextBox = By.Id("email");
        private readonly By PasswordTextBox = By.Id("i0118");
        private readonly By LoginButton = By.Id("idSIButton9");
        private readonly By SubmitButton = By.Id("submitBtn");
        private readonly By CompaniesDropdown = By.XPath("//div[@class='slicer-dropdown-menu']");
        private static By CompanyCheckbox(string companyName) => By.XPath($"//span[text()='{companyName}']/preceding-sibling::div");
        private static By RefreshDates(string companyName) => By.XPath($"//div[@role='gridcell' and @column-index='0' and text()='{companyName}']/following-sibling::div[@column-index='3']");
        #endregion

        #region Methods
        public void EnterEmail(string email)
        {
            Log.Step(nameof(AuditRefreshPage), "Enter Email.");
            Wait.UntilElementClickable(EmailTextBox).Click();
            Wait.UntilElementExists(EmailTextBox).SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            Log.Step(nameof(AuditRefreshPage), "Enter Password.");
            Wait.UntilElementClickable(PasswordTextBox).Click();
            Wait.UntilElementExists(PasswordTextBox).SendKeys(password);
        }

        public void ClickOnLoginButton()
        {
            Log.Step(nameof(AuditRefreshPage), "Click on the 'Login' Button.");
            Wait.UntilElementClickable(LoginButton).Click();
        }

        public void ClickOnSubmitButton()
        {
            Log.Step(nameof(AuditRefreshPage), "Click on the 'Submit' Button.");
            Wait.UntilElementClickable(SubmitButton).Click();
        }

        public void LoginToPowerBiDashboard(string email, string password)
        {
            EnterEmail(email);
            ClickOnSubmitButton();
            EnterPassword(password);
            ClickOnLoginButton();
        }
        public void SelectCompanyFromCompanyDropdown(string companyName)
        {
            Log.Step(nameof(AuditRefreshPage), $"Select the company from company dropdown '{companyName}'.");
            Wait.UntilElementClickable(CompaniesDropdown).Click();
            Driver.JavaScriptScrollToElement(CompanyCheckbox(companyName));
            Wait.UntilElementClickable(CompanyCheckbox(companyName)).Click();
            Wait.HardWait(2000);//Need to wait till all data is loaded after selecting company.
        }

        public List<string> GetRefreshedDateAndTime(string companyName)
        {
            Log.Step(nameof(AuditRefreshPage), $"Get refreshed date and time '{companyName}'.");
            var latestDateAndTime = Driver.GetTextFromAllElements(RefreshDates(companyName)).ToList();
            Wait.UntilElementVisible(RefreshDates(companyName)).GetText();
            return latestDateAndTime;
        }

        //Url
        public void NavigateToPage()
        {
            Log.Step(nameof(AuditRefreshPage), "Navigate to Power bi login page");
            NavigateToUrl(Constants.PowerBiUri);
        }

        public void NavigateToReportsPage()
        {
            Log.Step(nameof(AuditRefreshPage), "Navigate to 'Audit dashboard' page");
            NavigateToUrl($"{Constants.PowerBiUri}/groups/me/reports/4e8c82f0-b938-43cd-beab-5a30460b69d2/ReportSection?ctid=d1885df8-e520-4380-ad17-2339021d3049&experience=power-bi");
            Wait.HardWait(3000);//Need to wait to load the audit dashboard page
        }
    }
    #endregion
}
