using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManagePrivateKey
{
    internal class ManagePrivateAppKeyPage : BasePage
    {
        public ManagePrivateAppKeyPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By PrivateAddAppKeyButton = By.Id("add_app");

        // Create private app popup
        private readonly By CreatePrivateAppNameTextBox = By.Id("app_name");
        private readonly By CreatePrivateAppCreateButton = By.Id("create_app");
        private readonly By CreatePrivateAppCloseButton = By.Id("cancel_app");
        private readonly By PrivateAppKey = By.Id("app_privateKeyId");

        // Private App Key Grid
        private static By PrivateAppDeleteButton(string appName) => By.XPath($"//td[text() = '{appName}']/..//a[contains(@class, 'k-grid-delete')] | //font[text() = '{appName}']/ancestor::td/following-sibling::td/a");
        private static readonly By PrivateAppKeyDeleteButton = By.XPath("//td//a[contains(text(), 'Delete')]");
        private static readonly By PrivateAppKeyTableRow = By.XPath("//*[@id='PrivateAppKeyGrid']/table/tbody");
        private static By PrivateAppUserAccess(string appName) =>
            By.XPath($"//*[@id='PrivateAppKeyGrid']/table/tbody/tr/td[contains(text(),'{appName}')]/following-sibling::td[1] | //*[@id='PrivateAppKeyGrid']/table/tbody/tr/td//font[contains(text(),'{appName}')]/ancestor::td/following-sibling::td[1]");
        private static By PrivateAppCreatedBy(string appName) =>
            By.XPath($"//*[@id='PrivateAppKeyGrid']/table/tbody/tr/td[contains(text(),'{appName}')]/following-sibling::td[2] | //*[@id='PrivateAppKeyGrid']/table/tbody/tr/td//font[contains(text(),'{appName}')]/ancestor::td/following-sibling::td[2]");
        private static By PrivateAppExpiryAt(string appName) =>
            By.XPath($"//*[@id='PrivateAppKeyGrid']/table/tbody/tr/td[contains(text(),'{appName}')]/following-sibling::td[3] | //*[@id='PrivateAppKeyGrid']/table/tbody/tr/td//font[contains(text(),'{appName}')]/ancestor::td/following-sibling::td[3]");


        // Method
        public void ClickOnPrivateAddAppKeyButton()
        {
            Log.Info("Click on 'Private app Add App Key' button");
            Wait.UntilElementClickable(PrivateAddAppKeyButton).Click();
            Wait.UntilJavaScriptReady();
        }

        // Create private app popup
        public void EnterPrivateAppKeyName(string appName)
        {
            Log.Info("Enter private app key name");
            Wait.UntilElementClickable(CreatePrivateAppNameTextBox).SetText(appName);
        }

        public void ClickOnCreatePrivateAppCloseButton()
        {
            Log.Info("Click on 'Private App Close' button");
            Wait.UntilElementClickable(CreatePrivateAppCloseButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnCreatePrivateAppCreateButton()
        {
            Log.Info("Click on 'Private App Create' button");
            Wait.UntilElementClickable(CreatePrivateAppCreateButton).Click();
            Wait.UntilElementVisible(PrivateAppKey);
        }

        public string GetPrivateAppKey()
        {
            Log.Info("Get private app key");
            return Wait.UntilElementVisible(PrivateAppKey).GetText();
        }

        public bool IsPrivateAppKeyDisplayed()
        {
            return Driver.IsElementDisplayed(PrivateAppKey);
        }

        // Private App Key Grid

        public void ClickOnPrivateAppDeleteButton(string appName)
        {
            Log.Info("Click on 'Private App Delete' button");
            Wait.UntilElementClickable(PrivateAppDeleteButton(appName)).Click();
            Driver.AcceptAlert();
        }
        public void DeleteAllPrivateAppKeys()
        {
            var privateAppKeyDeleteButtonCount = Driver.FindElements(PrivateAppKeyDeleteButton).Count;
            for (int i = 0; i < privateAppKeyDeleteButtonCount; i++)
            {
                Log.Info("Click on 'Private App Delete' button");
                Wait.UntilElementClickable(PrivateAppKeyDeleteButton).Click();
                Driver.AcceptAlert();
            }
        }

        public string GetPrivateAppCreatedBy(string appName)
        {
            Log.Info("Get 'Private App CreateBy' name");
            return Wait.UntilElementExists(PrivateAppCreatedBy(appName)).GetText();
        }

        public string GetPrivateAppUserAccess(string appName)
        {
            Log.Info("Get 'Private App User Access' type");
            return Wait.UntilElementExists(PrivateAppUserAccess(appName)).GetText();
        }

        public string GetPrivateAppExpiryAt(string appName)
        {
            Log.Info("Get 'Private App ExpiryAt' date");
            return Wait.UntilElementExists(PrivateAppExpiryAt(appName)).GetText();
        }

        public bool IsPrivateAppDisplayed()
        {
            return Driver.IsElementDisplayed(PrivateAppKeyTableRow);
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/privateappkey/company/{companyId}");
            Wait.UntilJavaScriptReady();
        }
    }
}