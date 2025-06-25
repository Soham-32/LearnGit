using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
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
    public class BatchEditDeleteReviewer : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static IndividualAssessmentResponse _assessmentResponse;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static CreateReviewerRequest _reviewer;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "BatchEdit");

                _reviewer = MemberFactory.GetReviewer();
                var reviewerResponse = setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();

                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEdit_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentResponse = individualDataResponse.Item3;
                _assessmentRequest.Members = _team.Members.Select(m => MemberResponseExtensions.ToAddIndividualMemberRequest(m)).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(reviewerResponse.ToAddIndividualMemberRequest());
                _assessmentRequest.BatchId = _assessmentResponse.BatchId;

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType)
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
        public void BatchEdit_Delete_Reviewer()
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
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewer.Email);

            batchEditParticipantReviewerPage.DeleteReviewer(_reviewer.Email);
            batchEditParticipantReviewerPage.AcceptDeleting();

            Assert.AreEqual(_team.Members[0].FullName() + " (0)", batchEditParticipantReviewerPage.GetParticipantNameByEmail(_team.Members[0].Email), "Participant name should be displayed properly with count of reviewer");

            batchEditParticipantReviewerPage.ClickSaveButton();
            assessmentDashboard.WaitUntilLoaded();

            batchEditParticipantReviewerPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            Log.Info("Assert: Verify that deleted reviewer doesn't display in reviewers list");
            Assert.IsFalse(batchEditParticipantReviewerPage.DoesReviewerDisplay(_reviewer.Email),
                $"Deleted reviewer {_reviewer.Email} should not display in reviewers list");
        }
    }
}