using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.IndividualAssessments;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    public class CreateIndividualAssessment2AddReviewersPage : CreateIndividualAssessmentBase
    { 
        public CreateIndividualAssessment2AddReviewersPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            
        }

        private readonly By InviteOwnReviewersCheckbox = AutomationId.Equals("allowInviteCheckbox", "input");
        private readonly By ViewOwnResultsCheckbox = AutomationId.Equals("allowResultViewCheckbox", "input");
        private readonly By ParticipantName = AutomationId.Equals("participantName");
        private readonly By ParticipantEmailField = AutomationId.Equals("participantEmail");
        private readonly By CreateAndAddAnotherParticipantButton = AutomationId.Equals("createAddAnotherParticipantBtn");

        public void CheckToInviteOwnReviewers(CreateIndividualAssessmentRequest request)
        {
            Log.Step(nameof(CreateIndividualAssessment2AddReviewersPage), 
                "Check the 'Invite their own Reviewers' checkbox");
            Wait.UntilElementExists(InviteOwnReviewersCheckbox).Check(request.AllowInvite);
        }

        public void CheckboxForViewOwnResults(CreateIndividualAssessmentRequest request)
        {
            Log.Step(nameof(CreateIndividualAssessment2AddReviewersPage), 
                "Check the 'View their own results' checkbox");
            Wait.UntilElementExists(ViewOwnResultsCheckbox).Check(request.AllowResultView);
        }

        public int GetCountOfParticipants()
        {
            return Wait.UntilAllElementsLocated(ParticipantName).Count;
        }

        
        public void ClickCreateAndAddAnotherParticipantButton()
        {
            Log.Step(nameof(CreateIndividualAssessment2AddReviewersPage), 
                "Click the 'Select and Add Another' button on Select New Participant popup");
            Wait.UntilElementClickable(CreateAndAddAnotherParticipantButton).Click();
        }

        public void WaitUntilLoaded()
        {
            Log.Step(nameof(CreateIndividualAssessment2AddReviewersPage), "Wait for page to load");
            Wait.UntilElementVisible(ParticipantEmailField);
            Wait.UntilJavaScriptReady();
        }
    }
}
