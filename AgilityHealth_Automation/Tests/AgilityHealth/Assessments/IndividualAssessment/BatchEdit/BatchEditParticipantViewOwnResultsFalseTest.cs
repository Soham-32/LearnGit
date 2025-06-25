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
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditParticipantViewOwnResultsFalseTest : IndividualAssessmentBaseTest
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
               
                _team = GetTeamForBatchEdit(setup, "BatchEditViewOwnResultsFalse",1, _member);
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewResults_");
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
        public void BatchEdit_Participant_ViewOwnResults_False()
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

            //click on view own results so check mark is not there
            batchEditParticipantReviewerPage.ClickViewOwnResultsCheckbox();
            Assert.IsFalse(batchEditParticipantReviewerPage.IsParticipantCheckboxChecked(SharedConstants.ParticipantAllowResultView),"'View Own Results' checkbox is checked");
            batchEditParticipantReviewerPage.ClickSaveButton();
            iAssessmentDashboardPage.WaitUntilLoaded();

            //return to participant/reviewer tab to verify check box is false
            batchEditParticipantReviewerPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();
            
            //assert
            Log.Info("Assert: Verify that check box is inactive for 'View Own Results'");
            Assert.IsFalse(batchEditParticipantReviewerPage.IsParticipantCheckboxChecked(SharedConstants.ParticipantAllowResultView),"'View Own Results' checkbox is checked");
            
            //search for radar
            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(_member.Username, _member.Password);
            iAssessmentDashboardPage.NavigateToPage(teamId.ToInt());
            var radar = iAssessmentDashboardPage.IsAssessmentPresent($"{_assessment.AssessmentName} - {_member.FullName}");
            
            //assert
            Log.Info("Assert: Verify that radar is not accessible");
            Assert.IsFalse(radar, "The radar is accessible when it shouldn't be");
        }
    }
}