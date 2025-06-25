using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar
{
    internal class ManageRadarPage : RadarGridBasePage
    {
        public ManageRadarPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By CreateNewRadarButton = By.Id("create-btn");
        private static By PreviewAssessmentButton(string radarName) => By.XPath($"//tr/td[normalize-space()='{radarName}']/parent::tr/td/a[text()='Preview']");
        private static By BlankRadarButton(string radarName) => By.XPath($"//tr/td[normalize-space()='{radarName}']/parent::tr/td/a[text()='Radar']");
        private static By EditRadarIcon(string radarName) => By.XPath($"//tr/td[normalize-space()='{radarName}']/parent::tr/td/a[contains(@class,'edit')]");
        private static By DeleteRadarIcon(string radarName) => By.XPath($"//tr/td[normalize-space()='{radarName}']/parent::tr/td/a[contains(@class,'delete')]");

        // Create Radar popup
        private readonly By CreateRadarPopupCreateRadarRadioButton = By.Id("blank");
        private readonly By CreateRadarPopupCopyExistingRadarRadioButton = By.Id("copy");
        private readonly By CreateRadarPopupCreateRadarButton = By.XPath("//input[@value='Create Radar']");
        private readonly By CreateRadarPopupSelectAssessmentDropdown = By.XPath("//div[@id='survey_div']//span[@class='k-icon k-i-arrow-s']");
        private static By CreateRadarPopupSelectAssessmentName(string item) => By.XPath($"//ul[@id='NewSurveyId_listbox']/li[normalize-space()='{item}']");

        // Delete Assessment PopUp
        private readonly By DeleteAssessmentPopUpWarningMessage = By.Id("delete_text");
        private readonly By DeleteAssessmentPopUpDeleteButton = By.Id("remove_survey");
        private readonly By DeleteAssessmentPopUpCancelButton = By.CssSelector("#cancel_survey");
        private readonly By DeleteAssessmentPopUpCloseButton = By.XPath("//span[@id='delete_survey_dialog_wnd_title']//parent::div//span[@role='presentation'][normalize-space()='Close']");

        public void ClickOnCreateNewRadarButton()
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Create New Radar' Button");
            Wait.UntilElementClickable(CreateNewRadarButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnRadarEditIcon(string radarName)
        {
            Log.Step(nameof(ManageRadarPage), $"Click on {radarName} radar 'Edit' Icon");
            Driver.JavaScriptScrollToElement(EditRadarIcon(radarName));
            Wait.UntilElementClickable(EditRadarIcon(radarName)).Click();
        }
        public void ClickOnDeleteRadarIcon(string radarName)
        {
            Log.Step(nameof(ManageRadarPage), $"Click on {radarName} radar 'Delete' Icon");
            Wait.UntilElementClickable(DeleteRadarIcon(radarName)).Click();
        }
        public void ClickOnPreviewRadarButton(string radarName)
        {
            Log.Step(nameof(ManageRadarPage), $"Click on {radarName} radar 'Preview' Radar Button");
            Wait.UntilElementClickable(PreviewAssessmentButton(radarName)).Click();
        }
        public void ClickOnBlankRadarButton(string radarName)
        {
            Log.Step(nameof(ManageRadarPage), $"Click on {radarName} radar 'Blank' Radar Button");
            Wait.UntilElementClickable(BlankRadarButton(radarName)).Click();
        }

        //Create Radar popup
        public void CreateRadarPopupClickOnCreateRadarRadioButton() 
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Create' Radio Button from create radar Popup");
            Wait.UntilElementClickable(CreateRadarPopupCreateRadarRadioButton).Click();
        }
        public void CreateRadarPopupClickOnCopyExistingRadarRadioButton()
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Copy Existing Radar' Radio Button from create radar Popup");
            Wait.UntilElementClickable(CreateRadarPopupCopyExistingRadarRadioButton).Click();
        }
        public void CreateRadarPopupSelectAssessment(string radarName)
        {
            Log.Step(nameof(ManageRadarPage), $"Select '{radarName}' Assessment from create radar Popup");
            SelectItem(CreateRadarPopupSelectAssessmentDropdown, CreateRadarPopupSelectAssessmentName(radarName));
        }

        public void CreateRadarPopupClickOnCreateRadarButton()
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Create Radar' Button from create radar Popup");
            Wait.UntilElementClickable(CreateRadarPopupCreateRadarButton).Click();
        }
 
        public bool IsCreateRadarPopupCreateRadarButtonEnabled()
        {
            return Driver.IsElementEnabled(CreateRadarPopupCreateRadarButton);
        }

        // Delete Radar Popup
        public string DeleteAssessmentPopupGetWarningMessage()
        {
            Log.Step(nameof(ManageRadarPage), " Get 'Warning Message' Text from Delete assessment Popup");
            return Wait.UntilElementVisible(DeleteAssessmentPopUpWarningMessage).GetText();
        }

        public void DeleteAssessmentPopUpClickOnDeleteButton()
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Delete' Button from Delete assessment Popup");
            Wait.UntilElementClickable(DeleteAssessmentPopUpDeleteButton).Click();
            Wait.HardWait(3000); // Wait until Delete Assessment popup closed

        }
        public void DeleteAssessmentPopUpClickOnCloseButton()
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Close' Icon from Delete assessment Popup");
            Wait.UntilElementClickable(DeleteAssessmentPopUpCloseButton).Click();
        }
        public void DeleteAssessmentPopUpClickOnCancelButton()
        {
            Log.Step(nameof(ManageRadarPage), "Click on 'Cancel' Button from Delete assessment Popup");
            Wait.UntilElementClickable(DeleteAssessmentPopUpCancelButton).Click();
        }
        public bool IsRadarPresent(string radarName)
        {
            return Driver.IsElementPresent(PreviewAssessmentButton(radarName));
        }
        public void NavigateToPage()
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/surveys", CreateNewRadarButton);
        }
    }
}