using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AngleSharp.Common;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base
{
    public class HeaderBasePage : BasePage
    {
        public HeaderBasePage(IWebDriver driver, ILogger log) : base(driver, log) { }


        //Locators
        private readonly By NextButton = AutomationId.Equals("stepNext");
        private readonly By BackButton = AutomationId.Equals("stepBack");
        private readonly By CloseIcon = AutomationId.Equals("headerCloseBtn");
        private readonly By ExitPulseAssessmentPopupExitButton = By.XPath("//button[text()='Exit']");
        private readonly By ExitPulseAssessmentPopupCancelButton = By.XPath("//button[text()='Cancel']");
        private readonly By SaveAsDraftButton = AutomationId.Equals("saveAsDraftBtn");
        private readonly By PublishPopupPublishButton = AutomationId.Equals("confirmPublishDialog_publishBtn");
        private readonly By PublishPopupCancelButton = By.XPath("//*[local-name()='svg' and @data-testid='CloseIcon']");
        private readonly By PublishPopupSaveAsDraftButton = AutomationId.Equals("confirmPublishDialog_saveAsDraftBtn");


        //Methods
        public void ClickOnNextButton()
        {
            Log.Step(GetType().Name, "Click on 'Next' button");
            Driver.JavaScriptScrollToElement(Wait.UntilAllElementsLocated(NextButton).GetItemByIndex(1)).Click();
            Wait.HardWait(2000);// After clicking on next API is getting called to get data, which is taking sometime.
        }

        public bool IsNextButtonEnabled()
        {
            return Wait.UntilElementVisible(NextButton).Enabled;
        }

        public void ClickOnBackButton()
        {
            Log.Step(GetType().Name, "Click on 'Back' button");
            Driver.JavaScriptScrollToElement(Wait.UntilAllElementsLocated(BackButton).GetItemByIndex(1)).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsBackButtonEnabled()
        {
            return Wait.UntilElementVisible(BackButton).Enabled;// We are using this method as we want to get bool value for that element is enabled or not
        }

        public void ClickOnCloseIcon()
        {
            Log.Step(GetType().Name, "Click on 'Close' button");
            Wait.UntilElementClickable(CloseIcon).Click();
        }
        
        public void ClickOnExitButtonOfExitPulseAssessmentPopup()
        {
            Log.Step(GetType().Name, "Click on 'Exit' button from close popup");
            Wait.UntilElementClickable(ExitPulseAssessmentPopupExitButton).Click();
            Wait.HardWait(5000);//  Wait until team assessment dashboard loaded successfully.
        }

        public void ClickOnCancelButtonOfExitPulseAssessmentPopup()
        {
            Log.Step(GetType().Name, "Click on 'Cancel' button from Close popup");
            Wait.UntilElementClickable(ExitPulseAssessmentPopupCancelButton).Click();
        }

        public void ClickSaveAsDraftButton()
        {
            Log.Step(GetType().Name, "Click on 'Save as Draft' button");
            Wait.UntilElementClickable(SaveAsDraftButton).Click();
            Wait.HardWait(2000); //  Wait until team assessment dashboard loaded successfully.
        }

        public bool IsSaveEnabled()
        {
            return Driver.IsElementEnabled(SaveAsDraftButton);
        }

        public void ClickOnPublishPopupPublishButton()
        {
            Log.Step(GetType().Name, "Click on 'Publish' button from publish popup");
            Wait.UntilElementClickable(PublishPopupPublishButton).Click();
        }

        public void ClickOnPublishPopupCancelButton()
        {
            Log.Step(GetType().Name, "Click on 'Cancel' button from publish popup");
            Wait.UntilElementClickable(PublishPopupCancelButton).Click();
        }

        public void ClickOnPublishPopupSaveAsDraftButton()
        {
            Log.Step(GetType().Name, "Click on 'Save as Draft' button from publish popup");
            Wait.UntilElementClickable(PublishPopupSaveAsDraftButton).Click();
        }
    }
}
