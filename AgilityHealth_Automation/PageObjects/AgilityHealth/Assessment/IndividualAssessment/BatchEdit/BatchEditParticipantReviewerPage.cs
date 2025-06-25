using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit
{
    public class BatchEditParticipantReviewerPage : BatchEditBase
    {
        public BatchEditParticipantReviewerPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private static By EditReviewerButton(string email) => By.XPath(
            $"//div[@automation-id='reviewerEmail']//p[text()='{email}']/ancestor::div[@automation-id='reviewerEmail']/following-sibling::div//button[@automation-id='reviewerEditPencil']");
        private static By ReviewerRole(string email) => By.XPath($"//div[@automation-id='reviewerEmail']//p[text()='{email}']/ancestor::div//p[@automation-id='reviewerRoles']");
        private readonly By InviteOwnReviewersCheckbox = AutomationId.Equals("allowInviteCheckbox");
        private readonly By ViewOwnResultsCheckbox = AutomationId.Equals("allowResultViewCheckbox");
        private readonly By ParticipantEmailField = AutomationId.Equals("participantEmail");

        public void ClickInviteOwnReviewersCheckbox()
        {
            Log.Step(nameof(BatchEditParticipantReviewerPage), "Click checkbox to invite own reviewer");
            Wait.UntilElementClickable(InviteOwnReviewersCheckbox).Check();
        }
        public void ClickViewOwnResultsCheckbox()
        {
            Log.Step(nameof(BatchEditParticipantReviewerPage), "Click checkbox to invite own reviewer");
            Wait.UntilElementClickable(ViewOwnResultsCheckbox).Check();
        }

        public bool IsParticipantCheckboxChecked(string checkbox)
        {
            Log.Step(nameof(BatchEditParticipantReviewerPage), $"Verify if <{checkbox}> is checked");
            switch (checkbox)
            {
                case "allowInviteCheckbox":
                    return Wait.UntilElementClickable(InviteOwnReviewersCheckbox).GetElementAttribute("class").Contains("Mui-checked");
                case "allowResultViewCheckbox":
                    return Wait.UntilElementClickable(ViewOwnResultsCheckbox).GetElementAttribute("class").Contains("Mui-checked");
                default:
                    return false;
            }
        }

        public void ClickOnEditReviewerButton(string email)
        {
            Log.Step(nameof(BatchEditParticipantReviewerPage), "Click on edit reviewer button");
            Wait.UntilElementClickable(EditReviewerButton(email)).Click();
            Wait.HardWait(2000);
        }

        public string GetReviewerRole(string email)
        {
            Log.Step(nameof(BatchEditParticipantReviewerPage), "Get the selected role from the batch edit page");
            return Wait.UntilElementVisible(ReviewerRole(email)).GetText();
        }

        public void WaitUntilLoaded()
        {
            Log.Step(nameof(BatchEditParticipantReviewerPage), "Wait for page to load");
            Wait.UntilElementVisible(ParticipantEmailField);
            Wait.UntilJavaScriptReady();
        }
    }
}
