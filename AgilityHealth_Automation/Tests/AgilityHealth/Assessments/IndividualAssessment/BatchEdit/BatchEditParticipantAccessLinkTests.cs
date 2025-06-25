using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditParticipantAccessLinkTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
               
                _team = GetTeamForBatchEdit(setup, "BatchEditCopyLink", 2);
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditCopyLink_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void BatchEdit_Participant_CopyAssessmentLink()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            batchEditParticipantReviewerPage.ClickOnParticipantAccessLinkButton(_assessmentResponse.Members[0].Email);

            Assert.AreEqual("Link copied to clipboard", batchEditParticipantReviewerPage.GetAccessLinkTooltip(), $"Access link tooltip should display with correct message. Expected: 'Link copied to clipboard' - Actual '{batchEditParticipantReviewerPage.GetAccessLinkTooltip()}'");

            surveyPage.NavigateToUrl(CSharpHelpers.GetClipboard());

            Log.Info("Verify that user gets navigated to correct survey screen");
            Assert.AreEqual("Assessment - AH", Driver.Title, $"Survey screen should display. Expected: Assessment AH - Actual: {Driver.Title}");
            Assert.AreEqual($"Hello, {_assessmentResponse.Members[0].FullName()}", surveyPage.GetSurveyIdentity(), $"Survey screen should display for the correct user. Expected Hello, {_assessmentResponse.Members[0].FullName()} - Actual: {surveyPage.GetSurveyIdentity()}");
        }
    }
}
