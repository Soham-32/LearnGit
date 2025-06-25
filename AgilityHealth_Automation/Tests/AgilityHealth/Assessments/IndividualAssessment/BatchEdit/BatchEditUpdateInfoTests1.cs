using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditUpdateInfoTests1 : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;
        private static User _individualAssessReviewer1;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                
                _individualAssessReviewer1 = TestEnvironment.UserConfig.GetUserByDescription("member");
                var member = setup.GetCompanyMember(Company.Id, _individualAssessReviewer1.Username);
                
                _team = GetTeamForBatchEdit(setup, "BatchEditViewer");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewer_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentResponse.IndividualViewers.Add(member.ToAddUserRequest());
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
        public void BatchEdit_EditInfo_ParticipantsReviewers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var iIAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            //click checkbox to invite their own reviewers
            batchEditParticipantReviewerPage.ClickInviteOwnReviewersCheckbox();
            batchEditParticipantReviewerPage.ClickSaveButton();
            iIAssessmentDashboardPage.WaitUntilLoaded();

            batchEditParticipantReviewerPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            //assert
            var checkBox= batchEditParticipantReviewerPage.IsParticipantCheckboxChecked(SharedConstants.ParticipantAllowInvite);
            Assert.IsTrue(checkBox, "'Invite their own reviewers' checkbox is not checked");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_EditInfo_Viewers()
        {
            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditViewersPage = new BatchEditViewersPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var iIAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickViewersTab();

            //delete viewer
            batchEditViewersPage.DeleteIndividualAndAggregateEmail();
            batchEditViewersPage.ClickSaveButton();
            iIAssessmentDashboardPage.WaitUntilLoaded();

            batchEditViewersPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickViewersTab();

            var viewerText = string.Join("", batchEditViewersPage.GetIndividualAggregateViewer());

            //assert
            Log.Info("Assert: Verify that the viewer has been deleted");
            Assert.AreEqual("Enter viewer's email", viewerText, "The individual and aggregate viewer field box is not empty");
        }
    }
}