using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit
{
    class IaBatchEditPage : BasePage
    {
        public IaBatchEditPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Assessment Options
        private readonly By PointOfContactTextbox = By.Id("poc");

        private readonly By PointOfContactEmailTextbox = By.Id("pocemail");

        private readonly By AllowInviteOthersCheckbox = By.Id("CanInviteOthers");
        private readonly By AllowViewResultsCheckbox = By.Id("CanViewResults");
        private readonly By NotifyRevieweesToInviteOthersButton = By.Id("revieweeBtn");
        private readonly By AssessmentNameTextbox = By.Id("IndividualAssessmentNameUpdate");
        private readonly By PublishButton = By.Id("publishBtn");
        private readonly By UpdateButton = By.Id("draftButton");

        // reviewees
        private static By RevieweeEditLink(string revieweeName) =>
            By.XPath($"//span[text() = '{revieweeName}']/parent::td/following-sibling::td[3]//img[@title='Edit']");
        private readonly By SendToAllReviewersButton = By.Id("send_all_reviewers");
        private readonly By SendToAllRevieweesButton = By.Id("send_all_reviewees");

        public void EditBatchAssessment(CreateIndividualAssessmentRequest assessment)
        {
            Log.Step(nameof(IaBatchEditPage), "Enter updated information for assessment");
            if (!string.IsNullOrEmpty(assessment.AssessmentName))
            {
                Wait.UntilElementClickable(AssessmentNameTextbox).SetText(assessment.AssessmentName);
            }

            if (!string.IsNullOrEmpty(assessment.PointOfContact))
            {
                Wait.UntilElementClickable(PointOfContactTextbox).SetText(assessment.PointOfContact);
            }

            if (!string.IsNullOrEmpty(assessment.PointOfContactEmail))
            {
                Wait.UntilElementClickable(PointOfContactEmailTextbox).SetText(assessment.PointOfContactEmail);
            }

            if (assessment.AllowInvite)
            {
                Wait.UntilElementClickable(AllowInviteOthersCheckbox).Check();
            }

            if (assessment.AllowResultView)
            {
                Wait.UntilElementClickable(AllowViewResultsCheckbox).Check();
            }
        }

        public void NotifyRevieweesToInviteOthers()
        {
            Log.Step(nameof(IaBatchEditPage), "Click on the 'Notify Reviewees to Invite Others' button");
            Wait.UntilElementClickable(NotifyRevieweesToInviteOthersButton).Click();
        }

        internal void PublishAssessment()
        {
            Log.Step(nameof(IaBatchEditPage), "Click the 'Publish' button");
            Wait.UntilElementClickable(PublishButton).Click();
            Wait.UntilJavaScriptReady();
        }

        internal void UpdateAssessment()
        {
            Log.Step(nameof(IaBatchEditPage), "Click on the 'Update' button");
            Wait.UntilElementClickable(UpdateButton).Click();
            Wait.UntilJavaScriptReady();
        }

        internal void ClickRevieweeEditLink(string revieweeName)
        {
            Log.Step(nameof(IaBatchEditPage), $"Click on the 'Edit' button for reviewee <{revieweeName}>");
            Wait.UntilElementClickable(RevieweeEditLink(revieweeName)).Click();
            Wait.UntilJavaScriptReady();
            Driver.SwitchToFrame(By.XPath("//iframe[@title = 'Edit Individual Assessment']"));
        }

        internal void ClickSendToAllReviewersButton()
        {
            Log.Step(nameof(IaBatchEditPage), "Click on the 'Send to All Reviewers' button");
            Wait.UntilElementClickable(SendToAllReviewersButton).Click();
            Wait.UntilJavaScriptReady();
        }

        internal void ClickSendToAllRevieweesButton()
        {
            Log.Step(nameof(IaBatchEditPage), "Click on the 'Send to All Reviewees' button");
            Wait.UntilElementClickable(SendToAllRevieweesButton).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}