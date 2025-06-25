using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings
{
    public class ManageIntegrationsPage : BasePage
    {
        public ManageIntegrationsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Add Connection page
        private static By LinkButton(string platform) => By.XPath($"//div[text()='{platform}']//ancestor::div[contains(@class,'MuiGrid-root MuiGrid-item')]//button[text()='Link']");
        private static By ManageButton(string platform) => By.XPath($"//div[text()='{platform}']//ancestor::div[contains(@class,'MuiGrid-root MuiGrid-item')]//button[text()='Manage']");
        private readonly By LoadingSpinner = By.XPath("//p[contains(text(), 'Loading')]//img[contains(@src, 'load') and contains(@src, '.gif')]");
        private readonly By AddConnectionTitle = By.XPath("//h1[text() = 'Add Connection']");

        //Add Connection page
        public bool IsLinkButtonPresent(string platform)
        {
            return Driver.IsElementPresent(LinkButton(platform));
        }

        public void ClickOnLinkButton(string platform)
        {
            Log.Step(nameof(ManageIntegrationsPage), $"Click on the 'Link' button for the '{platform}'.");
            Wait.UntilElementClickable(LinkButton(platform)).Click();
        }

        public void ClickOnManageButton(string platform)
        {
            Log.Step(nameof(ManageIntegrationsPage), $"Click on the 'Manage' button for the '{platform}'.");
            Wait.UntilElementClickable(ManageButton(platform)).Click();
            Wait.UntilElementNotExist(LoadingSpinner, 20);
        }

        //Delete Instance
        public void WaitUntilAddConnectionPageLoaded()
        {
            Log.Step(nameof(ManageIntegrationsPage), "Wait until Add connection page is loaded");
            Wait.UntilElementNotExist(LoadingSpinner);
            Wait.UntilElementVisible(AddConnectionTitle);
        }

        public bool IsManageButtonPresent(string platform)
        {
            return Driver.IsElementPresent(ManageButton(platform));
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/v2/integrations/company/{companyId}");
        }
    }
}
