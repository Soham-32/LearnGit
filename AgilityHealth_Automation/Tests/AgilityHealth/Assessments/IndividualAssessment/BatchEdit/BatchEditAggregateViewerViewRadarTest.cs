using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
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
    public class BatchEditAggregateViewerViewRadarTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;
        private static User _individualAssessViewer1;
        public static MemberResponse Member;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _individualAssessViewer1 = TestEnvironment.UserConfig.GetUserByDescription("member");
                Member = setup.GetCompanyMember(Company.Id, _individualAssessViewer1.Username);

                _team = GetTeamForBatchEdit(setup, "BatchEditViewerViews");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditViewerViews_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
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
        [TestCategory("KnownDefect")]//Bug Id: 52899
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void BatchEdit_Add_AggregateViewer_ViewRadar()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditViewersPage = new BatchEditViewersPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickViewersTab();
            batchEditViewersPage.WaitUntilViewersPageLoaded();
            batchEditViewersPage.InputAggregateEmail(Member.Email);
            batchEditViewersPage.ClickSaveButton();
            assessmentDashboard.WaitUntilLoaded();
            topNav.LogOut();
            login.LoginToApplication(_individualAssessViewer1.Username, _individualAssessViewer1.Password);
            var teamAssessmentDashboardUrl = dashboardPage.GetTeamAssessmentDashboardUrl(_team.Name);
            teamAssessmentDashboard.NavigateToUrl(teamAssessmentDashboardUrl);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();
            individualAssessmentDashboard.WaitUntilLoaded();
            Log.Info($"Assert: Verify viewer {_individualAssessViewer1.Username} able to access only batch radar");
            Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent(_assessment.AssessmentName + " - Roll up"),
                $"Individual Roll up doesn't exist with name : {_assessment.AssessmentName}");
            Assert.IsFalse(individualAssessmentDashboard.IsAssessmentPresent(_assessment.AssessmentName + " - " + _team.Members[0].FullName()),
                $"Individual radar shows when it should not : {_assessment.AssessmentName}");
        }
    }
}