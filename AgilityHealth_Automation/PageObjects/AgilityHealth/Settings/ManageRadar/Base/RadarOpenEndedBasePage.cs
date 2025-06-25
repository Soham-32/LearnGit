using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarOpenEndedBasePage : RadarGridBasePage
    {
        public RadarOpenEndedBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddEditOpenEndedPopupOpenQuestionDropdown = By.XPath("//span[@aria-owns='CompetencyType_listbox']");
        private static By AddEditOpenEndedPopupSelectOpenQuestionDropdown(string excludeRole) => By.XPath($"//ul[@id='CompetencyType_listbox']//li[@role='option'][normalize-space()='{excludeRole}']");
        private static By AddEditOpenEndedPopupTextIframe => By.XPath("//div[normalize-space()='Text']//following-sibling::div//iframe");
        private static By AddEditOpenEndedPopupTextBodyIframe => By.XPath("//body");
        private static readonly By AddEditOpenEndedPopupAdvanceOptionsButton = By.XPath("//span[@id='openAdvancedOptions']");
        private static By AddEditOpenEndedPopupOrderTextbox => By.XPath("//div[normalize-space()='Order']//following-sibling::div//input");
        private static By AddEditOpenEndedPopupExcludeTextbox => By.XPath("//div[normalize-space()='Exclude']//following-sibling::div//input");

        private readonly By TranslatedAddEditOpenEndedPopupOpenQuestionDropdown = By.XPath("//span[@aria-owns='TranslatedCompetencyType_listbox']/span");
        private static By TranslatedAddEditOpenEndedPopupTextIframe => By.XPath("//div[normalize-space()='Text' and contains(@class,'translated')]//following-sibling::div//iframe");

        internal void EnterOpenEndedInfo(RadarOpenEndedResponse radarOpenEnded)
        {
            Log.Step(nameof(RadarOpenEndedBasePage), "Enter Open Ended Info");
            
            //Open Questions 
            SelectItem(AddEditOpenEndedPopupOpenQuestionDropdown, AddEditOpenEndedPopupSelectOpenQuestionDropdown(radarOpenEnded.OpenQuestions));

            //Text
            EnterTextDescription(radarOpenEnded.Text);

            //Advanced Options
            Wait.UntilElementClickable(AddEditOpenEndedPopupAdvanceOptionsButton).Click();
            Wait.UntilJavaScriptReady();

            //Order
            if (!string.IsNullOrEmpty(radarOpenEnded.Order))
                Wait.UntilElementClickable(AddEditOpenEndedPopupOrderTextbox).SendKeys(radarOpenEnded.Order);

            //Exclude
            if (!string.IsNullOrEmpty(radarOpenEnded.Exclude))
                Wait.UntilElementClickable(AddEditOpenEndedPopupExcludeTextbox).SetText(radarOpenEnded.Exclude);

            //Company
            if (string.IsNullOrEmpty(radarOpenEnded.Company)) return;
            SelectCompanyFilter(radarOpenEnded.CompanyFilter);
            SelectCompany(radarOpenEnded.Company);
        }

        internal void EnterTranslatedOpenEndedInfo(OpenEnded radarOpenEnded)
        {
            Log.Step(nameof(RadarOpenEndedBasePage), "Enter Open Ended Info");
            EnterTranslatedTextDescription(radarOpenEnded.Text);
        }

        public bool IsTranslatedOpenQuestionEnabled()
        {
            return !Wait.UntilElementClickable(TranslatedAddEditOpenEndedPopupOpenQuestionDropdown).GetAttribute("class")
                .Contains("disabled");
        }

        private void EnterTextDescription(string text)
        {
            Log.Step(nameof(RadarOpenEndedBasePage), "Enter value in 'Text' description");
            Driver.SwitchToFrame(AddEditOpenEndedPopupTextIframe);
            Wait.UntilElementClickable(AddEditOpenEndedPopupTextBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }

        private void EnterTranslatedTextDescription(string text)
        {
            Log.Step(nameof(RadarOpenEndedBasePage), "Enter value in 'Text' description");
            Driver.SwitchToFrame(TranslatedAddEditOpenEndedPopupTextIframe);
            Wait.UntilElementClickable(AddEditOpenEndedPopupTextBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }
    }
}