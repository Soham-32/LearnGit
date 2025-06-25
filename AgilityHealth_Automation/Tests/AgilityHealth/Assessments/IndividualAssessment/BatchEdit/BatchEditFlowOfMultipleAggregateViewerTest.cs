using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class BatchEditFlowOfMultipleAggregateViewerTest : IndividualAssessmentBaseTest
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
                var member2 = setup.GetCompanyMember(Company.Id, SharedConstants.Viewer.Email);
                
                _team = GetTeamForBatchEdit(setup, "BatchEditViewer");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewer_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentResponse.AggregateViewers.Add(member.ToAddUserRequest());
                _assessmentResponse.AggregateViewers.Add(member2.ToAddUserRequest());
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
        public void BatchEdit_ViewersTab_AggregateViewer_Multiple()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditViewersPage = new BatchEditViewersPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickViewersTab();
            var aggregateViewer = batchEditViewersPage.GetAggregateViewer();

            //assert
            Log.Info("Assert: Verify that the viewers added to the assessment are shown on the UI");
            Assert.That.ListsAreEqual(_assessmentResponse.AggregateViewers.Select(i => i.Email).ToList(), aggregateViewer, "The list of aggregate viewers do not match");
        }
    }
}