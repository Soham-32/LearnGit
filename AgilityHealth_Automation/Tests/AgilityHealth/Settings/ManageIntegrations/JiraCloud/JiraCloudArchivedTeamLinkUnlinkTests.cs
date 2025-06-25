using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraCloudIntegration;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Newtonsoft.Json;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraCloud
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraCloudArchivedTeamLinkUnlinkTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static bool _classInitFailed;
        private static SetUpMethods _setupUi;
        private static readonly string JiraCloudInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraCloudIntegrationInfo.json");
        private static readonly JiraCloudIntegration JiraCloudData = JsonConvert.DeserializeObject<JiraCloudIntegration>(JiraCloudInfo);

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("TestIntegration");
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamResponse.TeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _setupUi = new SetUpMethods(testContext, TestEnvironment);
                _setupUi.JiraIntegrationLinkTeamToBoard(Constants.PlatformJiraCloud, JiraCloudData.JiraBoard, JiraCloudData.JiraProjectName, _teamResponse, Company.Id);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyArchivedTeamLinkUnlinkWithJiraBoard()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var linkTeamPage = new LinkTeamBasePage(Driver, Log);
            var manageIntegrationsPage = new ManageIntegrationsPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Archive the AH team that is linked with the Jira Board.");
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_teamResponse.Name);
            dashBoardPage.DeleteTeam(_teamResponse.Name, RemoveTeamReason.ArchiveOther);

            Log.Info($"Navigate to the 'Manage Integrations' page and Verify that the team - {_teamResponse.Name} linked to the Jira board - {JiraCloudData.JiraBoard} is not displayed on the 'Link Team to Board' page.");
            manageIntegrationsPage.NavigateToPage(Company.Id);
            manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraCloud);
            Assert.IsFalse(linkTeamPage.IsUnlinkButtonEnabled(), "The 'Unlink' button is present.");

            Log.Info("Restore the Archived team and verify that the team is present in the 'Active' state.");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.FilterTeamStatus("Archived");
            dashBoardPage.RestoreTeam(_teamResponse.Name);
            dashBoardPage.SearchTeam(_teamResponse.Name);
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_teamResponse.Name), $"{_teamResponse.Name} is not displayed at 'Active' state");

            Log.Info("Navigate back to the 'Manage Integrations' page and click on the 'Manage' button for the 'Jira Cloud'.");
            manageIntegrationsPage.NavigateToPage(Company.Id);
            manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraCloud);

            Log.Info($"Verify that the Team {_teamResponse.Name} and Jira Board {JiraCloudData.JiraBoard} are available in the 'Select Team' and 'Select Board' dropdown lists respectively.");
            var listOfTeam = linkTeamPage.GetAgilityHealthTeamsList();
            Assert.IsTrue(listOfTeam.Contains(_teamResponse.Name), $"Team name {_teamResponse.Name} is not present in the 'Select Team' list");
            var listOfBoard = linkTeamPage.GetJiraBoardList();
            Assert.IsTrue(listOfBoard.Contains(JiraCloudData.JiraBoard), $"Jira Board name {JiraCloudData.JiraBoard} is not present in the 'Select Jira Board' list");
        }
    }
}
