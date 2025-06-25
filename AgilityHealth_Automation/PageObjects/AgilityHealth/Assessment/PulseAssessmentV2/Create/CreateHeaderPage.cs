using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create
{
    public class CreateHeaderPage : HeaderBasePage
    {
        public CreateHeaderPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By CreatePulseCheckButton = By.XPath("//button//span[contains(normalize-space(),'Create Pulse Check')]");
        private readonly By SelectQuestionButton = By.XPath("//span[contains(normalize-space(),'Select Questions')]/ancestor::button");
        private readonly By SelectRecipientsButton = By.XPath("//span[contains(normalize-space(),'Select Recipients')]/ancestor::button");
        private readonly By ReviewAndPublishButton = By.XPath("//span[contains(normalize-space(),'Review & Publish')]/ancestor::button");

        //Methods
        public void ClickOnCreatePulseCheckStepper()
        {
            Log.Step(nameof(CreateHeaderPage), "Click on 'Create Pulse Check' button");
            Wait.UntilElementClickable(CreatePulseCheckButton).Click();
        }

        public void ClickOnSelectQuestionsStepper()
        {
            Log.Step(nameof(CreateHeaderPage), "Click on 'Select Questions' button");
            Wait.UntilElementExists(SelectQuestionButton).Click();
        }

        public void ClickOnSelectRecipientsStepper()
        {
            Log.Step(nameof(CreateHeaderPage), "Click on 'Select Recipients' button");
            Driver.JavaScriptScrollToElement(SelectRecipientsButton, false);
            Wait.UntilElementClickable(SelectRecipientsButton).Click();
            Wait.HardWait(2000); //It takes time load recipients tab 
        }

        public void ClickOnReviewAndPublishStepper()
        {
            Log.Step(nameof(CreateHeaderPage), "Click on 'Create Pulse Check' button");
            Wait.UntilElementClickable(ReviewAndPublishButton).Click();
        }



        public bool IsSelectQuestionsStepperEnabled()
        {
            Wait.HardWait(1000);
            return Wait.UntilElementExists(SelectQuestionButton).Enabled;
        }

        public bool IsSelectRecipientsStepperEnabled()
        {
            Wait.HardWait(1000);
            return Wait.UntilElementExists(SelectRecipientsButton).Enabled;
        }
        public bool IsSelectReviewAndPublishStepperEnabled()
        {
            Wait.HardWait(1000);
            return Wait.UntilElementExists(ReviewAndPublishButton).Enabled;
        }
    }
}