using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings
{
    public class SettingsPage : SettingsBasePage
    {
        public SettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private static By SettingOptions(string option) => By.XPath($"//a[@class='ease btn green-btn'][contains(normalize-space(),'{option}')] | //div//h2[contains(text(), '{option}')]/../a");
        private static By SettingOptionsDescription(string option) => By.XPath($"//h2[text()='{option}']/following-sibling::p");
        
        private readonly By ManageRadarsButton = By.XPath("//h2[text()='Manage Radars']");
        public void SelectSettingsOption(string option)
        {
            Log.Step(nameof(SettingsPage), $"Select setting option <{option}>");
            Wait.UntilElementClickable(SettingOptions(option)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsSettingOptionPresent(string option)
        {
            return Driver.IsElementPresent(SettingOptions(option));
        }

        public string GetSettingOptionDescription(string option)
        {
            Log.Step(nameof(SettingsPage), $"Get <{option}> setting option description");
            return Wait.UntilElementVisible(SettingOptionsDescription(option)).GetText();
        }

        public string GetManageRadarsButtonTitle()
        {
            return Wait.UntilElementVisible(ManageRadarsButton).GetText();
        }

        public void NavigateToPage(int companyId = 0)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/settings/{companyId}", SettingOptions("View My Profile"));
        }

        public string GetPageUrl(int companyId)
        {
            return $"{BaseTest.ApplicationUrl}/settings?companyId={companyId}";
        }

        public void NavigateToSettingsPageForV1ForProd(string env)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/Settings");
        }
    }
}