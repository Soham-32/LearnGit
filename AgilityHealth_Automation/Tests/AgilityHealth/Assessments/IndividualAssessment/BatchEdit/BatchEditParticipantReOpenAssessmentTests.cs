using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditParticipantReOpenAssessmentTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
               
                _team = GetTeamForBatchEdit(setup, "BatchEditReOpen");

                _reviewer = MemberFactory.GetReviewer();
                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
                
                _assessmentResponse = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                    Company.Id, User.CompanyName, _team.Uid, $"BatchEditReviewDone_{Guid.NewGuid()}");
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentResponse.Members.First().Reviewers.Add(reviewerResponse.ToAddIndividualMemberRequest());
            
                _assessment = setup.CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_Participant_ReOpenAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            var participantEmail = _assessmentResponse.Members[0].Email;

            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(
                SharedConstants.IaEmailParticipantSubject, participantEmail, "inbox"));
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            batchEditAssessmentPage.ReopenParticipantAssessment();
            batchEditAssessmentPage.WaitUntilParticipantCompleteMarkDisappear(participantEmail);

            Log.Info("Assert: Verify check mark disappears and assessment gets reopened");
            Assert.IsFalse(batchEditParticipantReviewerPage.DoesParticipantCompleteCheckMarkDisplay(participantEmail), "A check mark should not present. The survey has not been reopened");
            Assert.IsFalse(batchEditParticipantReviewerPage.DoesParticipantReopenAssessmentIconDisplay(), "Reopen assessment icon should not present");
        }
    }
}