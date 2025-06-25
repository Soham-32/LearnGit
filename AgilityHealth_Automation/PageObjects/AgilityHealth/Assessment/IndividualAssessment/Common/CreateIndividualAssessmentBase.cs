using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common
{
    public class CreateIndividualAssessmentBase : IndividualAssessmentBase
    {
        public CreateIndividualAssessmentBase(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Top navigation
        private readonly By AssessmentTitle = By.TagName("h1");
        private readonly By BackButton = AutomationId.Equals("stepBack");
        private readonly By DeleteButton = AutomationId.Equals("deleteAssessmentBtn");
        private readonly By SaveAsDraftButton = AutomationId.Equals("saveAsDraftBtn");
        private readonly By NextButton = AutomationId.Equals("stepNext");
        private readonly By AddReviewersNavigationButton = By.XPath("//button//p[text()='Add/Remove Reviewers']");
        private readonly By InviteViewersNavigationButton = By.XPath("//button//p[text()='Invite Viewers']");
        private readonly By ReviewAndPublishNavigationButton = By.XPath("//button//p[text()='Review & Publish']");

        public void ClickAddReviewersButton()
        {
            Log.Step(GetType().Name, "Click on 'Add Reviewers' step");
            Wait.UntilElementClickable(AddReviewersNavigationButton).Click();
        }

        public void ClickInviteReviewersButton()
        {
            Log.Step(GetType().Name, "Click on 'Invite Viewers' step");
            Wait.UntilElementClickable(InviteViewersNavigationButton).Click();
        }

        public void ClickReviewAndPublishButton()
        {
            Log.Step(GetType().Name, "Click on 'Review & Publish' step");
            Wait.UntilElementClickable(ReviewAndPublishNavigationButton).Click();
        }

        public void ClickNextButton()
        {
            Log.Step(GetType().Name, "Click the 'Next' button");
            Wait.UntilElementVisible(AssessmentTitle);
            Driver.JavaScriptScrollToElement(NextButton, false);
            Wait.UntilElementClickable(NextButton).Click();
            Wait.UntilElementVisible(AssessmentTitle);
            Wait.HardWait(2000); //Taking time to load page
        }

        public void ClickBackButton()
        {
            Log.Step(GetType().Name, "Click the 'Back' button");
            Wait.UntilElementClickable(BackButton).Click();
        }

        public void ClickDeleteButton()
        {
            Log.Step(GetType().Name, "Click the 'Delete' button");
            Wait.UntilElementClickable(DeleteButton).Click();
        }

        public void ClickSaveAsDraftButton()
        {
            Log.Step(GetType().Name, "Click the 'Save As Draft' button");
            Wait.UntilElementClickable(SaveAsDraftButton).Click();
        }

        public bool IsSaveAsDraftButtonEnabled()
        {
            return Wait.UntilElementExists(SaveAsDraftButton).Enabled;
        }

        public bool IsDeleteButtonEnabled()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementEnabled(DeleteButton, 10);
        }

        public bool IsNextButtonEnabled()
        {
            return Wait.UntilElementExists(NextButton).Enabled;
        }
    }
}
