using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentIconForDraftAssessment : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static ReviewerResponse _reviewer;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessmentResponse;


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                
                var reviewerRequest = MemberFactory.GetReviewer();
                _reviewer = setup.CreateReviewer(reviewerRequest).GetAwaiter().GetResult();

                _team = GetTeamForBatchEdit(setup, "EditIA");
                var assessmentRequest = IndividualAssessmentFactory.GetDraftIndividualAssessment(
                    Company.Id, User.CompanyName, _team.Uid, $"ReviewerIA_{Guid.NewGuid()}");
                assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                assessmentRequest.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());
                _assessmentResponse = setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
                assessmentRequest.BatchId = _assessmentResponse.BatchId;
                _assessmentResponse = setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_IconDraftAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);

            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessmentResponse.BatchId, teamId);
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewer.Email);

            Assert.IsFalse(batchEditParticipantReviewerPage.DoesResendIconDisplay(), "Resend icon should not display");
            Assert.IsFalse(batchEditParticipantReviewerPage.DoesAccessLinkButtonDisplay(_reviewer.Email),
                "Access Link icon should not display");
        }
    }
}
