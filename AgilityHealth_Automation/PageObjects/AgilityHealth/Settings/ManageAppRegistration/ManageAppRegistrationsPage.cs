using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.OAuth;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageAppRegistration
{
    internal class ManageAppRegistrationsPage : BasePage
    {
        public ManageAppRegistrationsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By AddAppButton = By.Id("add_app");
        private static By RowValueByColumnName(int rowIndex, string column) => By.XPath($"//*[@id = 'AppRegistrationGrid']//tbody/tr[{rowIndex}]/td[count(//div[@id='AppRegistrationGrid']//thead//th[@data-title='{column}']/preceding-sibling::th)+1]");
        private static By DeleteButton(string appName) =>
            By.XPath($"//td[text() = '{appName}']/..//a[contains(@class, 'k-grid-delete')]");
        private readonly By AllRows = By.XPath("//*[@id = 'AppRegistrationGrid']//tbody/tr");

        // create popup
        private readonly By AppNameTextBox = By.Id("app_name");
        private readonly By CreateButton = By.Id("create_app");
        private readonly By ClientId = By.Id("app_clientId");
        private readonly By Secret = By.Id("app_secret");
        private readonly By CloseButton = By.Id("cancel_app");

        // Methods
        public List<AppRegistrationResponse> GetAppRegistrations()
        {
            Wait.UntilJavaScriptReady();
            var appRegistrations = new List<AppRegistrationResponse>();
            var totalRows = Wait.UntilAllElementsLocated(AllRows);
            for (var i = 1; i < totalRows.Count + 1; i++)
            {
                var app = new AppRegistrationResponse
                {
                    AppName = Wait.UntilElementVisible(RowValueByColumnName(i, "App Name")).GetText(),
                    ClientId = Wait.UntilElementVisible(RowValueByColumnName(i, "ClientId")).GetText(),
                    Access = Wait.UntilElementVisible(RowValueByColumnName(i, "Access")).GetText(),
                    CreatedBy = Wait.UntilElementVisible(RowValueByColumnName(i, "Created By")).GetText()
                };
                appRegistrations.Add(app);
            }

            return appRegistrations;
        }

        public void AddNewAppRegistration(string appName)
        {
            Wait.UntilElementClickable(AddAppButton).Click();
            Wait.UntilElementClickable(AppNameTextBox).SetText(appName);
            Wait.UntilElementClickable(CreateButton).Click();
        }

        public void ClickDeleteButton(string appName)
        {
            Wait.UntilElementClickable(DeleteButton(appName)).Click();
            Driver.AcceptAlert();
        }

        public AddAppRegistrationResponse GetNewAppRegistrationInfoFromPopup()
        {
            return new AddAppRegistrationResponse
            {
                AppName = Wait.UntilElementVisible(AppNameTextBox).GetText(),
                ClientId = Wait.UntilElementVisible(ClientId).GetText(),
                Secret = Wait.UntilElementVisible(Secret).GetText()
            };
        }

        public void ClickCloseButtonOnCreatePopup()
        {
            Wait.UntilElementClickable(CloseButton).Click();
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/appregistration/company/{companyId}");
        }

    }
}