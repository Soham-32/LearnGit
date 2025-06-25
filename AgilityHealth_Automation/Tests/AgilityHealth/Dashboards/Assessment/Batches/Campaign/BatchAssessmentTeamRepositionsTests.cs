using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.Batches.Campaign
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class BatchAssessmentTeamRepositionsTests : BaseTest
    {
        private const string AutomationTeamBatchAssessment = Constants.TeamForBatchAssessment;
        private const string AutomationNormalTeam = SharedConstants.Team;
        private static readonly BatchAssessment BatchAssessment = BatchFactory.GetValidBatchDetails();

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BatchAssessment_Selected_Unselected_Teams_Repositions()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            Log.Info("Login into the Application");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Batch Dashboard page");
            dashBoardPage.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

            Log.Info("Click on '+' icon from Batch dashboard page");
            batchesTabPage.ClickPlusButton();

            Log.Info($" Enter values to create Batch Assessment by selecting '{AutomationTeamBatchAssessment}' & '{AutomationNormalTeam}' teams");
            createBatchAssessmentPopupPage.EnterBatchAssessment(BatchAssessment);

            Log.Info($"Get Team column list and Indexes of '{AutomationTeamBatchAssessment}' & '{AutomationNormalTeam}' teams ");
            var selectedTeamList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupColumnValueList("Team");
            Assert.AreEqual(1, selectedTeamList.IndexOf(AutomationTeamBatchAssessment), $"'{AutomationTeamBatchAssessment}' team is not at 2nd Position");
            Assert.AreEqual(0, selectedTeamList.IndexOf(AutomationNormalTeam), $"{AutomationNormalTeam} team is not at 1st Position");

            Log.Info("Click on 'Send and Launch Now' button to create a Batch Assessment");
            createBatchAssessmentPopupPage.ClickSendAndLaunchNowButton();
            createBatchAssessmentPopupPage.ClickOnCreateEditBatchAssessmentPopupYesProceedButton();
            createBatchAssessmentPopupPage.ClickOkButton();

            Log.Info($"Click on {BatchAssessment.BatchName} batch 'Edit' button and get Team column list");
            batchesTabPage.SearchBatchName(BatchAssessment.BatchName);
            batchesTabPage.ClickBatchEditButton(BatchAssessment.BatchName);
            var editedTeamList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupColumnValueList("Team");

            Log.Info($"Verify that '{AutomationTeamBatchAssessment}' & '{AutomationNormalTeam}' team retain their indexes");
            Assert.AreEqual(1, editedTeamList.IndexOf(AutomationTeamBatchAssessment), $"'{AutomationTeamBatchAssessment}' team is not at 2nd Position");
            Assert.AreEqual(0, editedTeamList.IndexOf(AutomationNormalTeam), $"'{AutomationNormalTeam}' team is not at 1st Position");

            Log.Info($"Unselect '{AutomationNormalTeam}' and get Team column list");
            createBatchAssessmentPopupPage.SelectTeamName(AutomationNormalTeam, false);
            var unselectedTeamList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupColumnValueList("Team");

            Log.Info($"Verify that '{AutomationTeamBatchAssessment}' & '{AutomationNormalTeam}' team Indexes");
            Assert.AreEqual(0, unselectedTeamList.IndexOf(AutomationTeamBatchAssessment), $"'{AutomationTeamBatchAssessment}' team is not at 1st Position");
            Assert.AreEqual(1, unselectedTeamList.IndexOf(AutomationNormalTeam), $"'{AutomationNormalTeam}' team is not at 2nd Position");
        }
    }
}
