using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings
{
    public class SettingsV2Page : SettingsBasePage
    {
        public SettingsV2Page(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private static By SettingOptions(string option) => By.XPath($"//button[contains(text(),'{option}')] | //button//font[contains(text(),'{option}')] | //div//h2[contains(text(), '{option}')]/..//following-sibling::div");
        private readonly By CompanyDropdown = By.XPath("//div[@id='CompanySelect']//select");
        private readonly By GrowthItemCustomSettingManageSettingsButton = By.XPath("//button[@label='Custom Settings']");
        private readonly By SettingsTitle = By.XPath("//h2[text()='Settings']");

        public void SelectSettingsOption(string option)
        {
            Log.Step(nameof(SettingsV2Page), $"Click on the <{option}> option");
            Wait.HardWait(2000); // Wait till page loaded.
            Wait.UntilElementVisible(SettingOptions(option));
            Wait.UntilElementClickable(SettingOptions(option)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnBusinessOutcomesManageSettingsButton()
        {
            SelectSettingsOption("Manage Settings");
            Wait.HardWait(1000);
        }

        public void ClickOnManageFeatureButton()
        {
            SelectSettingsOption("Manage Features");
        }
        public void ClickOnCustomGrowthItemManageSettingsButton()
        {
            Log.Step(nameof(SettingsV2Page), "Click on the 'Manage Settings' for Growth item custom settings");
            Wait.UntilElementClickable(GrowthItemCustomSettingManageSettingsButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsSettingOptionPresent(string option)
        {
            return Driver.IsElementPresent(SettingOptions(option));
        }
        public void WaitUntilPageLoaded(string option)
        {
            Wait.UntilElementVisible(SettingOptions(option));
        }
        public void SelectCompany(string companyName)
        {
            Log.Step(nameof(SettingsV2Page), $"Select company <{companyName}> in the dropdown");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CompanyDropdown).SelectDropdownValueByVisibleText(companyName);
            Wait.UntilJavaScriptReady();
        }

        public string GetSettingsTitle()
        {
            return Wait.UntilElementVisible(SettingsTitle).GetText();
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/V2/settings/company/{companyId}", SettingOptions("Manage Profile"));
        }

        public void NavigateToSettingsPageForV2ForProd(string env, int id)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/V2/settings/company/{id}");
        }

    }
}