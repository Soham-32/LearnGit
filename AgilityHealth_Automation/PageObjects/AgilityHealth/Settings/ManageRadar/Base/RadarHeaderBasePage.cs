using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarHeaderBasePage : BasePage
    {
        public RadarHeaderBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        internal readonly By HeaderCompanyDropdown = By.XPath("//span[normalize-space()='Company']/following-sibling::span/span[contains(@class,'k-header')]");
        internal readonly By HeaderLanguageDropdown = By.XPath("//span[normalize-space()='Language']/following-sibling::span/span[contains(@class,'k-header')]");
        private static By SelectLanguageFromHeaderLanguageDropDown(string language) => By.XPath($"//div[@id='Survey_LanguageCode-list']//ul//li[@role='option'][normalize-space()='{language}']");
        private readonly By WarningMessage = By.XPath("//p[@class='caution-message']");

        //Survey Bar
        private static By WizardStepper(string step) => By.XPath($"//div[@class='wizard']//a[contains(.,'{step}')]");

        //Header Company and Language Dropdown All List
        private readonly By HeaderRadarCompanyAllValue = By.XPath("//ul[@id='companyIdFilterDropbox_listbox']//li[@role='option']");
        private readonly By HeaderLanguageAllValue = By.XPath("//ul[@id='Survey_LanguageCode_listbox']//li[@role='option']");

        // Previous and Next Button
        private readonly By BackToButton = By.Id("prev");
        private readonly By ContinueToButton = By.Id("next");

        public List<string> GetHeaderLanguageDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All Language value from Language dropdown");
            Wait.UntilElementClickable(HeaderLanguageDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getHeaderLanguageAllValue = Driver.GetTextFromAllElements(HeaderLanguageAllValue).ToList();
            Wait.UntilElementClickable(HeaderLanguageDropdown).Click();
            return getHeaderLanguageAllValue;
        }

        public List<string> GetHeaderCompanyDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'Company' value from Header Company dropdown");
            Wait.UntilElementClickable(HeaderCompanyDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getHeaderCompanyAllValue = Driver.GetTextFromAllElements(HeaderRadarCompanyAllValue).ToList();
            Wait.UntilElementClickable(HeaderCompanyDropdown).Click();
            return getHeaderCompanyAllValue;
        }

        public void SelectRadarLanguage(string language)
        {
            Log.Step(nameof(RadarHeaderBasePage), $"Select {language}  from Header Language dropdown");
            SelectItem(HeaderLanguageDropdown, SelectLanguageFromHeaderLanguageDropDown(language));
        }

        public void SelectWizardStep(string step)
        {
            Log.Step(nameof(RadarHeaderBasePage), $"Select {step} step from Survey bar");
            Wait.UntilElementClickable(WizardStepper(step)).Click();
        }

        public void ClickOnBackToButton()
        {
            Log.Step(nameof(RadarHeaderBasePage), "Click on 'Back To' Button");
            Wait.UntilElementClickable(BackToButton).Click();
        }

        public void ClickOnContinueToButton()
        {
            Log.Step(nameof(RadarHeaderBasePage), "Click on 'Continue To' Button");
            Wait.UntilElementClickable(ContinueToButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void CloseCurrentWindowAndSwitchToFirstWindow()
        {
            Log.Step(nameof(RadarHeaderBasePage), "Close current window and Switch back to First window");
            Driver.Close();
            Driver.SwitchToFirstWindow();
        }
        public bool IsHeaderCompanyDropdownEnabled()
        {
            return !Convert.ToBoolean(Wait.UntilElementClickable(HeaderCompanyDropdown).GetAttribute("aria-disabled"));
        }

        public bool IsWarningMessagePresent()
        {
            return Driver.IsElementPresent(WarningMessage);
        }

        public string GetWarningMessage()
        {
            return Wait.UntilElementClickable(WarningMessage).Text;
        }
    }
}