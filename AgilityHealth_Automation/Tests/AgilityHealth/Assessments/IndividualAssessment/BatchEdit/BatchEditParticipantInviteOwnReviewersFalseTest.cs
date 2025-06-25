using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos;
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
    public class BatchEditParticipantInviteOwnReviewersFalseTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static User _member;
        private static CreateIndividualAssessmentRequest _assessmentResponse;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _member = TestEnvironment.UserConfig.GetUserByDescription("member");

            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
              
                _team = GetTeamForBatchEdit(setup, "BatchEditInviteOwnReviewerFalse", 1, _member);
                _assessmentResponse = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                    Company.Id, User.CompanyName, _team.Uid, $"BatchEditIAInvite_{Guid.NewGuid()}");
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentResponse.AllowInvite = true;

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
        public void BatchEdit_Participant_InviteOwnReviewer_False()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);

            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();

            //go to participant/reviewer tab
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            //click on invite own reviewer so check mark is there
            batchEditParticipantReviewerPage.ClickInviteOwnReviewersCheckbox();
            
            //assert
            Log.Info("Assert: Verify that check box is inactive  after click for 'Invite their own reviewers'");
            var checkBox1 = batchEditParticipantReviewerPage.IsParticipantCheckboxChecked(SharedConstants.ParticipantAllowInvite);
            Assert.IsFalse(checkBox1, "'Invite their own reviewers' checkbox is checked");
            batchEditParticipantReviewerPage.ClickSaveButton();
            iAssessmentDashboardPage.WaitUntilLoaded();

            //return to participant/reviewer tab to verify check box is true
            batchEditParticipantReviewerPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            
            //assert
            Log.Info("Assert: Verify that check box is still inactive for 'Invite their own reviewers' after returning to page");
            var checkBox2= batchEditParticipantReviewerPage.IsParticipantCheckboxChecked(SharedConstants.ParticipantAllowInvite);
            Assert.IsFalse(checkBox2, "'Invite their own reviewers' checkbox is checked");
            
            //search for email to invite a reviewer
            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(_member.Username, _member.Password);
            iAssessmentDashboardPage.NavigateToPage(teamId.ToInt());
            var radar = iAssessmentDashboardPage.EditIndividual_IsLinkHrefPresent(_assessment.AssessmentName, _member.FullName);
            
            //assert
            Log.Info("Assert: Verify that edit link does not exist");
            Assert.IsFalse(radar, $"The edit link for {_assessment.AssessmentName} - {_member.FullName} exists");
        }
    }
}