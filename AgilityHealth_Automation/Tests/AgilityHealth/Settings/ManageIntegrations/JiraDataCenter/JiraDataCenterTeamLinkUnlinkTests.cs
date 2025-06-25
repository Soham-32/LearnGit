using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Newtonsoft.Json;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraDataCenterIntegration;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraDataCenter
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraDataCenterTeamLinkUnlinkTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static bool _classInitFailed;
        private static readonly string JiraCenterInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraDataCenterIntegrationInfo.json");
        private static readonly JiraDataCenterIntegration JiraDataCenter = JsonConvert.DeserializeObject<JiraDataCenterIntegration>(JiraCenterInfo);

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("TestIntegration");
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamResponse.TeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyLinkUnlinkTeamWithJiraDataCenterBoard()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var linkTeamPage = new LinkTeamBasePage(Driver, Log);
            var manageIntegrationsPage = new ManageIntegrationsPage(Driver, Log);
            var teamMetricsPage = new EditMetricsBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Integrations' page and Click on the 'Manage Button'.");
            manageIntegrationsPage.NavigateToPage(Company.Id);
            manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraDataCenter);

            Log.Info($"Unlink the already linked team and verify that the team - {_teamResponse.Name} is successfully linked to the Jira board - {JiraDataCenter.JiraBoard}.");
            linkTeamPage.UnlinkAlreadyLinkedTeam(JiraDataCenter.JiraBoard);
            linkTeamPage.LinKTeam(JiraDataCenter.JiraProjectName, JiraDataCenter.JiraBoard, _teamResponse.Name);

            Assert.AreEqual(JiraDataCenter.JiraProjectName, linkTeamPage.GetLinkedJiraProjectName("Static Jira Project For Testing (Do Not Touch)"), "Project name does not match");
            Assert.AreEqual(JiraDataCenter.JiraBoard, linkTeamPage.GetLinkedJiraBoardName(JiraDataCenter.JiraBoard), $"The linked board name is not matched with the selected board {JiraDataCenter.JiraBoard}.");
            Assert.AreEqual(_teamResponse.Name, linkTeamPage.GetLinkedTeamName(_teamResponse.Name), $"The linked team name is not matched with the selected team {_teamResponse.Name}.");

            Log.Info($"Verify that the linked team - {_teamResponse.Name} and Jira board - {JiraDataCenter.JiraBoard} are not available in the 'Select Team' and 'Select Board' dropdown lists respectively.");
            var listOfTeam = linkTeamPage.GetAgilityHealthTeamsList();
            Assert.IsFalse(listOfTeam.Contains(_teamResponse.Name), $"Team name {_teamResponse.Name} is still present in the 'Select Team' list");
            var listOfBoard = linkTeamPage.GetJiraBoardList();
            Assert.IsFalse(listOfBoard.Contains(JiraDataCenter.JiraBoard), $"Jira Board name {JiraDataCenter.JiraBoard} is still present in the 'Select JiraBoard' list");

            Log.Info("Navigate to the 'Team Metrics' page and verify that the 'Jira' logo, linked 'Team' & 'Jira Board' name and 'Get Data' button are present.");
            teamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            Assert.IsTrue(teamMetricsPage.IsJiraLogoPresent(), "The 'Jira' logo is not present.");
            Assert.AreEqual(_teamResponse.Name, teamMetricsPage.GetTeamName(_team.Name), "The 'Team' name does not match.");
            Assert.AreEqual(JiraDataCenter.JiraBoard, teamMetricsPage.GetJiraBoardName(_team.Name), "The 'Jira Board' name does not match.");
            Assert.IsTrue(teamMetricsPage.IsGetDataButtonPresent(), "The 'Get Data' button is not present.");

            Log.Info("Navigate back to the 'Manage Integrations' page and click on the 'Manage' button for the 'Jira Cloud'.");
            manageIntegrationsPage.NavigateToPage(Company.Id);
            manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraDataCenter);

            Log.Info($"Unlink the Team {_teamResponse.Name} linked with {JiraDataCenter.JiraBoard} Board.");
            linkTeamPage.UnlinkAlreadyLinkedTeam(JiraDataCenter.JiraBoard);
            Assert.IsFalse(linkTeamPage.IsUnlinkButtonEnabled(), "The Unlink button is not present");

            Log.Info($"Verify that the unlinked Team {_teamResponse.Name} and Jira Board {JiraDataCenter.JiraBoard} are available in the 'Select Team' and 'Select Board' dropdown lists respectively.");
            linkTeamPage.SelectJiraProject(JiraDataCenter.JiraProjectName);
            listOfTeam = linkTeamPage.GetAgilityHealthTeamsList();
            Assert.IsTrue(listOfTeam.Contains(_teamResponse.Name), $"Team name {_teamResponse.Name} is not present in the 'Select Team' list");
            listOfBoard = linkTeamPage.GetJiraBoardList();
            Assert.IsTrue(listOfBoard.Contains(JiraDataCenter.JiraBoard), $"Jira Board name {JiraDataCenter.JiraBoard} is not present in the 'Select Jira Board' list");

            Log.Info("Navigate to the 'Team Metrics' page and verify that the 'Jira' logo and 'Get Data' button are not present.");
            teamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            Assert.IsFalse(teamMetricsPage.IsJiraLogoPresent(), "The 'Jira' logo is present");
            Assert.IsFalse(teamMetricsPage.IsGetDataButtonPresent(), "The 'Get Data' button is present.");
        }
    }
}