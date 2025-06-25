using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit
{
    public class EditHeaderPage : HeaderBasePage
    {
        public EditHeaderPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators 
        private static By AssessmentTabs(string stageName) => By.XPath($"//button[text()='{stageName}'] | //button//font[text()='{stageName}'] ");
        private static readonly By EditPulseCheckTitle = By.XPath("//div//p[contains(normalize-space(),'Edit Pulse Check')]");
        private static readonly By PublishButton = AutomationId.Equals("publishEditBtn");
        private readonly By DeleteButton = AutomationId.Equals("deleteAssessmentBtn");
        private readonly By DeleteAssessmentPopupDeleteButton = By.XPath("//div//h2[@id='alertDialogTitle']/..//button[contains(normalize-space(),'Delete')]");
        private readonly By DeleteAssessmentPopupCancelButton = By.XPath("//button[text()='Cancel']");
        private readonly By SaveButtonToolTip = By.XPath("//div[@role='tooltip']//div");
        private readonly By LoadingTeamsText = By.XPath("//*[text()='Loading teams...']");


        //Method
        private void ClickOnAssessmentTabs(string assessmentStage)
        {
            Log.Step(GetType().Name, $"Click on {assessmentStage}");
            Driver.JavaScriptScrollToElement(AssessmentTabs(assessmentStage), false).Click();
            Wait.HardWait(7000); // Wait till page loads successfully.
            if (assessmentStage == "Edit Recipients")
            {
                Wait.UntilElementNotExist(LoadingTeamsText);
            }
        }

        public void ClickOnEditPulseCheckTab() => ClickOnAssessmentTabs("Edit Pulse Check");

        public void ClickOnEditQuestionsTab() => ClickOnAssessmentTabs("Edit Questions");

        public void ClickOnEditRecipientsTab() => ClickOnAssessmentTabs("Edit Recipients");

        public bool IsEditPulseCheckTitleDisplayed()
        {
            return Driver.IsElementDisplayed(EditPulseCheckTitle);
        }

        public void ClickOnPublishButton()
        {
            Log.Step(GetType().Name, "Click on Publish button");
            Wait.UntilElementClickable(PublishButton).Click();
        }

        public bool IsPublishButtonDisplayed()
        {
            return Driver.IsElementDisplayed(PublishButton, 10);
        }

        public void ClickDeleteButton()
        {
            Log.Step(GetType().Name, "Click on Delete button");
            Wait.UntilElementClickable(DeleteButton).Click();
        }

        public void ClickOnDeleteAssessmentPopupDeleteButton()
        {
            Log.Step(GetType().Name, "Click on OK button on delete popup");
            Wait.UntilElementClickable(DeleteAssessmentPopupDeleteButton).Click();
            Wait.UntilElementNotExist(DeleteAssessmentPopupDeleteButton);
        }

        public void ClickDeleteAssessmentPopupCancelButton()
        {
            Log.Step(GetType().Name, "Click on Cancel button on delete popup");
            Wait.UntilElementClickable(DeleteAssessmentPopupCancelButton).Click();
            Wait.UntilElementNotExist(DeleteAssessmentPopupCancelButton);
        }

        public bool IsDeleteVisible()
        {
            return Driver.IsElementDisplayed(DeleteButton);
        }

        public string GetSaveButtonTooltip()
        {
            Log.Step(GetType().Name, "Get save button tool tip");
            return Wait.UntilElementExists(SaveButtonToolTip).GetText();
        }
    }
}