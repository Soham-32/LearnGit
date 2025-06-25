using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
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
    public class BatchEditDeleteParticipant : IndividualAssessmentBaseTest
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

                _team = GetTeamForBatchEdit(setup, "BatchEditIAReviewer", 2);
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAReviewer_");
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Critical")]
        public void BatchEdit_Delete_Participant()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);

            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            batchEditParticipantReviewerPage.DeleteParticipant(_assessmentResponse.Members[0].Email);
            batchEditParticipantReviewerPage.AcceptDeleting();
            batchEditParticipantReviewerPage.ClickSaveButton();
            assessmentDashboard.WaitUntilLoaded();

            batchEditParticipantReviewerPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            Log.Info("Assert: Verify that deleted participant doesn't display participants list");
            Assert.IsFalse(batchEditAssessmentPage.DoesParticipantDisplay(_assessmentResponse.Members[0].Email),
                $"Deleted participant {_assessmentResponse.Members[0].Email} should not display in participants list");
        }
    }
}