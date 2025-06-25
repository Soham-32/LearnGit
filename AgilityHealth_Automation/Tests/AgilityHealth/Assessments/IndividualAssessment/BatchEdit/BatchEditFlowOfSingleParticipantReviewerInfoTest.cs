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
    public class BatchEditFlowOfSingleParticipantReviewerInfoTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;
        private static ReviewerResponse _reviewerResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var reviewer = MemberFactory.GetReviewer();
           
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForBatchEdit(setup, "BatchEditViewer");
                
                _reviewerResponse = setup.CreateReviewer(reviewer).GetAwaiter().GetResult();
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewer_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentResponse.Members.First().Reviewers.Add(_reviewerResponse.ToAddIndividualMemberRequest());
            
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_FlowOfParticipantReviewersInfo_Single()
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
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewersWithReviewerEmail(_reviewerResponse.Email);
            batchEditParticipantReviewerPage.WaitUntilAddReviewerButtonShows(_assessment.Participants.First().Email);

            //assert
            Log.Info("Assert: Verify that the participant and reviewer are shown on the UI");
            Assert.IsTrue(batchEditParticipantReviewerPage.DoesParticipantDisplay(_assessment.Participants.First().Email), $"The participant <{_assessment.Participants.First().Email}> is not shown");
            Assert.IsTrue(batchEditParticipantReviewerPage.DoesReviewerDisplay(_reviewerResponse.Email), $"The reviewer <{_reviewerResponse.Email}> is not shown");
        }
    }
}