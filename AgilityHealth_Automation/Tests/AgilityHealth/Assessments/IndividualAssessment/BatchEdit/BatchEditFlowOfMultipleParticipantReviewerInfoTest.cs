using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class BatchEditFlowOfMultipleParticipantReviewerInfoTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;
        private static ReviewerResponse _reviewerResponse1;
        private static ReviewerResponse _reviewerResponse2;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var reviewer1 = MemberFactory.GetReviewer();
                _reviewerResponse1 = setup.CreateReviewer(reviewer1).GetAwaiter().GetResult();
                var reviewer2 = MemberFactory.GetReviewer();
                _reviewerResponse2 = setup.CreateReviewer(reviewer2).GetAwaiter().GetResult();
                
                _team = GetTeamForBatchEdit(setup, "BatchEditParticipantReviewer", 2);
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAParticipantReviewer_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentResponse.Members.First().Reviewers.Add(_reviewerResponse1.ToAddIndividualMemberRequest());
                _assessmentResponse.Members.First().Reviewers.Add(_reviewerResponse2.ToAddIndividualMemberRequest());
            
                _assessment = setup
                    .CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
                _assessmentResponse.BatchId = _assessment.BatchId;
                _assessment = setup
                    .CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("TalentDevelopment")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_FlowOfParticipantReviewersInfo_Multiple()
        { 
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewerResponse1.Email);

            //assert
            Log.Info("Assert: Verify that the participant and reviewer are shown on the UI");
            foreach (var participant in _assessment.Participants)
            {
                Assert.IsTrue(batchEditParticipantReviewerPage.DoesParticipantDisplay(participant.Email), $"The participant <{participant.Email}> is not shown");
            }
            Assert.IsTrue(batchEditParticipantReviewerPage.DoesReviewerDisplay(_reviewerResponse1.Email), $"The reviewer <{_reviewerResponse1.Email}> is not shown");
            Assert.IsTrue(batchEditParticipantReviewerPage.DoesReviewerDisplay(_reviewerResponse2.Email), $"The reviewer <{_reviewerResponse2.Email}> is not shown");
        }
    }
}