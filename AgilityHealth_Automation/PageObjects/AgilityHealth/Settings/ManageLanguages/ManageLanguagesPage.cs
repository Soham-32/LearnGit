using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageLanguages
{
    internal class ManageLanguagesPage : BasePage
    {
        public ManageLanguagesPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By PageDescription = By.XPath("//div[@class='pg-title']//p");
        private readonly By InactiveLanguagesFilterBox = By.Id("InactiveLanguagesFilterBox");
        private readonly By ActiveLanguagesFilterBox = By.Id("ActiveLanguagesFilterBox");
        private readonly By AllInactiveLanguages = By.XPath("//div[@id='gridInactiveLanguages']//table/tbody/tr/td");
        private readonly By AllActiveLanguages = By.XPath("//div[@id='gridActiveLanguages']//table/tbody/tr/td");
        private static By SpecificLanguage(string language) => By.XPath($"//table/tbody/tr/td[contains(normalize-space(),'{language}')]");


        //Methods
        public string GetPageDescription()
        {
            Log.Step(nameof(ManageLanguagesPage), "Get Manage Language page description");
            return Wait.UntilElementVisible(PageDescription).GetText();
        }

        public List<string> GetAllInactiveLanguages()
        {
            Log.Step(nameof(ManageLanguagesPage), "Get all the languages present in 'Inactive' Grid");
            return Driver.GetTextFromAllElements(AllInactiveLanguages).ToList();
        }

        public List<string> GetAllActiveLanguages()
        {
            Log.Step(nameof(ManageLanguagesPage), "Get all the languages present in 'Active' Grid");
            return Driver.GetTextFromAllElements(AllActiveLanguages).ToList();
        }

        public void SelectLanguage(string language)
        {
            Log.Step(nameof(ManageLanguagesPage), "Select a language");
            Wait.UntilElementVisible(SpecificLanguage(language)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SearchLanguageInInactiveFilterBox(string language)
        {
            Log.Step(nameof(ManageLanguagesPage), $"Search {language} language in Inactive Filter textbox");
            Wait.UntilElementClickable(InactiveLanguagesFilterBox).SendKeys(language);
        }
        public void SearchLanguageInActiveFilterBox(string language)
        {
            Log.Step(nameof(ManageLanguagesPage), $"Search {language} language in Active Filter textbox");
            Wait.UntilElementClickable(ActiveLanguagesFilterBox).SendKeys(language);
        }
        public bool IsInactiveLanguagesFilterBoxPresent()
        {
            return Driver.IsElementPresent(InactiveLanguagesFilterBox);
        }
        public bool IsActiveLanguagesFilterBoxPresent()
        {
            return Driver.IsElementPresent(ActiveLanguagesFilterBox);
        }
        public bool IsLanguageClickable(string language)
        {
            return Wait.UntilElementClickable(SpecificLanguage(language)) != null;
        }

        public void NavigateToPage()
        {
            Log.Step(nameof(ManageLanguagesPage), "Navigate to the Manage Language page");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/languages", InactiveLanguagesFilterBox);
        }
    }
}