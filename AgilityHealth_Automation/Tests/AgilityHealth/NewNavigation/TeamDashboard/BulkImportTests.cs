using AgilityHealth_Automation.Base;
using AtCommon.Api;
using AtCommon.Dtos.Bulk;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("BulkImport"), TestCategory("NewNavigation")]
    public class BulkImportTests : BaseTest
    {
        private static readonly string TemplatePath = $@"{new FileUtil().GetBasePath()}Resources\TestData\BulkImport\BulkImportTemplate.xlsx";

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BulkImport_DownloadTemplate()
        {

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            const string fileName = "BulkImportTemplate.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SelectBulkMgmt("Download Template");

            var downloadFile = FileUtil.WaitUntilFileDownloaded(fileName, 30);
            Assert.That.ExcelAreEqual(downloadFile, TemplatePath);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BulkImport_Export()
        {
            const string fileName = "CompanyExport.xlsx";
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SelectBulkMgmt("Export");

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"<{fileName}> file not downloaded successfully");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id:- 45959, 47736, 45521
        [TestCategory("CompanyAdmin")]
        public async Task BulkImport_GenerateExternalIdentifiers()
        {
            var team = TeamFactory.GetNormalTeam("bulk");
            var caClient = await ClientFactory.GetAuthenticatedClient(User.Username,
                User.Password, TestEnvironment.EnvironmentName);
            var createTeamResponse = await caClient.PostAsync<TeamResponse>(
                RequestUris.Teams(), team);
            createTeamResponse.EnsureSuccess();

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamProfileTabPage = new TeamProfileTabPage(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SelectBulkMgmt("Generate Ext. Identifiers");

            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.SearchTeam(team.Name);
            teamDashboardPage.ClickOnTeamName(team.Name);

            var actualExternalIdentifier = teamProfileTabPage.GetTeamInfo().ExternalIdentifier;
            Assert.IsFalse(string.IsNullOrWhiteSpace(actualExternalIdentifier),
                $"External Identifier not generated for team <{team.Name}>");

        }

        [TestMethod]
        [TestCategory("KnownDefect")]   //Bug:34664
        [TestCategory("CompanyAdmin")]
        public void BulkImport_Import()
        {
            // generate an excel
            var teams = new List<AddTeam> { TeamFactory.GetTeamForBulkImport() };
            var members = new List<AddMembers>
            {
                MemberFactory.GetMemberForBulkImport(teams.First().ExternalIdentifier),
                MemberFactory.GetStakeholderForBulkImport(teams.First().ExternalIdentifier)
            };
            var users = new List<AddUser> { UserFactory.GetBlAdminUserForBulkImport() };

            const string fileName = "BulkImportTemplate.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            var newFile = $@"{FileUtil.GetDownloadPath()}\BulkImportTemplate{CSharpHelpers.Random8Number()}.xlsx";
            ExcelUtil.GenerateImportFile(TemplatePath, newFile, teams, members, users);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamProfileTabPage = new TeamProfileTabPage(Driver, Log);
            var importPopup = new ImportTeamsPopupPage(Driver, Log);
            var teamMembersTabPage = new TeamMembersTabPage(Driver, Log);
            var stakeholdersTabPage = new StakeholdersTabPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SelectBulkMgmt("Import");

            importPopup.UploadFile(newFile);
            Assert.AreEqual("SUCCEEDED", importPopup.GetStatusResult(),
                "Status Result does not match.");

            importPopup.ClickCloseButton();

            Log.Info("Verify Team added");
            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.SearchTeam(teams[0].Name);
            Assert.IsTrue(teamDashboardPage.IsTeamDisplayed(teams[0].Name),
                $"Team <{teams[0].Name}> not found on the dashboard after import.");
            int teamId = teamDashboardPage.GetTeamIdFromLink(teams[0].Name);
            teamProfileTabPage.NavigateToPage(Company.Id, teamId);
            var actualTeamProfile = teamProfileTabPage.GetTeamInfo();
            Assert.AreEqual(teams[0].Name, actualTeamProfile.TeamName, "Team Name doesn't match");
            Assert.AreEqual(teams[0].Type, actualTeamProfile.WorkType, "Work Type does not match.");
            Assert.AreEqual(
                teams[0].Tags.Single(t => t.Category == "Methodology").Tags.First(),
                actualTeamProfile.Methodology, "Methodology does not match.");
            Assert.AreEqual(teams[0].ExternalIdentifier, actualTeamProfile.ExternalIdentifier,
                "External Identifier does not match.");
            Assert.AreEqual(teams[0].Department, actualTeamProfile.DepartmentAndGroup, "Department doesn't match");
            Assert.AreEqual(teams[0].Bio, actualTeamProfile.TeamBioOrBackground, "Team BIO doesn't match");

            Log.Info("Verify Team Member added");
            teamProfileTabPage.ClickOnTeamMembersTab();
            var expectedMember = members.First(m => !m.IsStakeholder);
            var actualTeamMember = teamMembersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(expectedMember.Email);
            Assert.AreEqual(expectedMember.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(expectedMember.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(expectedMember.Email.ToLower(), actualTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(expectedMember.Roles.ToList(), actualTeamMember.Role);
            Assert.That.ListsAreEqual(expectedMember.ParticipantGroups.ToList(), actualTeamMember.ParticipantGroup);

            Log.Info("Verify Stakeholder added");
            teamProfileTabPage.ClickOnStakeholderTab();
            var expectedStakeholder = members.First(m => m.IsStakeholder);
            var actualStakeholder = stakeholdersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(expectedStakeholder.Email);
            Assert.AreEqual(expectedStakeholder.FirstName, actualStakeholder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(expectedStakeholder.LastName, actualStakeholder.LastName, "Lastname doesn't match");
            Assert.AreEqual(expectedStakeholder.Email, actualStakeholder.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(expectedStakeholder.Roles.ToList(), actualStakeholder.Role);

            Log.Info("Verify User added");
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            var userImagePath = manageUserPage.GetAvatar(users[0].Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, users[0].FirstName, users[0].LastName, users[0].Email, today),
                "New user <{users[0].Email}> not found after import");

        }
    }
}