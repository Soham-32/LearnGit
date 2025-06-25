using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AngleSharp.Text;
using AtCommon.Api;
using AtCommon.Dtos.Bulk;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams.BulkImport
{
    [TestClass]
    [TestCategory("BulkImport"), TestCategory("Dashboard")]
    public class BulkImportTests : BaseTest
    {
        private static readonly string TemplatePath = $@"{new FileUtil().GetBasePath()}Resources\TestData\BulkImport\BulkImportTemplate.xlsx";

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BulkImport_DownloadTemplate()
        {
            
            var login = new LoginPage(Driver, Log);
            var dashboard = new TeamDashboardPage(Driver, Log);

            const string fileName = "BulkImportTemplate.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashboard.ClickOnBulkDownloadTemplate();

            var downloadFile = FileUtil.WaitUntilFileDownloaded(fileName, 30);
            Assert.That.ExcelAreEqual(downloadFile, TemplatePath);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BulkImport_Export()
        {
            const string fileName = "CompanyExport.xlsx";
            var login = new LoginPage(Driver, Log);
            var dashboard = new TeamDashboardPage(Driver, Log);
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashboard.ClickOnBulkExport();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), 
                $"<{fileName}> file not downloaded successfully");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task BulkImport_GenerateExternalIdentifiers()
        {
            // create a new team without ext identifier
            var team = TeamFactory.GetNormalTeam("bulk");
            var caClient = await ClientFactory.GetAuthenticatedClient(User.Username, 
                User.Password, TestEnvironment.EnvironmentName);
            var createTeamResponse = await caClient.PostAsync<TeamResponse>(
                RequestUris.Teams(), team);
            createTeamResponse.EnsureSuccess();

            var login = new LoginPage(Driver, Log);
            var dashboard = new TeamDashboardPage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashboard.ClickOnBulkGenerateExtIdentifiers();

            dashboard.MoveToLogo();
            dashboard.GridTeamView();
            dashboard.SearchTeam(team.Name);
            dashboard.ClickTeamEditButton(team.Name);

            var actualExternalIdentifier = editTeamProfilePage.GetExternalIdentifier();
            Assert.IsFalse(string.IsNullOrWhiteSpace(actualExternalIdentifier), 
                $"External Identifier not generated for team <{team.Name}>");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug: 34664
        [TestCategory("CompanyAdmin")]
        public void BulkImport_Import()
        {
            // generate an excel
            var teams = new List<AddTeam> {TeamFactory.GetTeamForBulkImport()};
            var members = new List<AddMembers>
            {
                MemberFactory.GetMemberForBulkImport(teams.First().ExternalIdentifier),
                MemberFactory.GetStakeholderForBulkImport(teams.First().ExternalIdentifier)
            };
            var users = new List<AddUser> {UserFactory.GetBlAdminUserForBulkImport()};

            const string fileName = "BulkImportTemplate.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            var newFile = $@"{FileUtil.GetDownloadPath()}\BulkImportTemplate{CSharpHelpers.Random8Number()}.xlsx";
            ExcelUtil.GenerateImportFile(TemplatePath, newFile, teams, members, users);

            var login = new LoginPage(Driver, Log);
            var dashboard = new TeamDashboardPage(Driver, Log);
            var importPopup = new ImportTeamsPopup(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);
            var editTeamMembersPage = new TeamMemberCommon(Driver, Log);
            var editStakeholdersPage = new EditTeamStakeHolderPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            dashboard.ClickOnBulkImport();

            importPopup.UploadFile(newFile);
            Assert.AreEqual("SUCCEEDED", importPopup.GetStatusResult(), 
                "Status Result does not match.");

            importPopup.ClickCloseButton();

            Log.Info("Verify Team added");
            dashboard.GridTeamView();
            dashboard.SearchTeam(teams[0].Name);
            Assert.IsTrue(dashboard.DoesTeamDisplay(teams[0].Name), 
                $"Team <{teams[0].Name}> not found on the dashboard after import.");
            var teamId = dashboard.GetTeamIdFromLink(teams[0].Name).ToInt();
            editTeamProfilePage.NavigateToPage(teamId);
            var actualTeamProfile = editTeamProfilePage.GetTeamInfo();
            Assert.AreEqual(teams[0].Name, actualTeamProfile.TeamName, "Team Name doesn't match");
            Assert.AreEqual(teams[0].Type, actualTeamProfile.WorkType, "Work Type does not match.");
            Assert.AreEqual(
                teams[0].Tags.Single(t => t.Category == "Methodology").Tags.First(), 
                actualTeamProfile.Methodology, "Methodology does not match.");
            Assert.AreEqual(teams[0].ExternalIdentifier, editTeamProfilePage.GetExternalIdentifier(),
                "External Identifier does not match.");
            Assert.AreEqual(teams[0].Department, actualTeamProfile.Department, "Department doesn't match");
            // BUG 26699
            //Assert.AreEqual(teams[0].FormationDate.Value.ToString("MMMM yyyy"), 
            //    actualTeamProfile.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(teams[0].AgileAdoptionDate?.ToString("MMMM yyyy"), 
                actualTeamProfile.AgileAdoptionDate, "Agile Adoption Date doesn't match");
            Assert.AreEqual(teams[0].Description, actualTeamProfile.Description, "Description doesn't match");
            Assert.AreEqual(teams[0].Bio, actualTeamProfile.TeamBio, "Team BIO doesn't match");

            editTeamMembersPage.NavigateToPage(teamId);
            Log.Info("Verify Team Member added");
            var actualTeamMember = editTeamMembersPage.GetTeamMemberInfoFromGrid(1);
            var expectedMember = members.First(m => !m.IsStakeholder);
            Assert.AreEqual(expectedMember.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(expectedMember.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(expectedMember.Email.ToLower(), actualTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(expectedMember.Roles.ToList(), actualTeamMember.Role.SplitCommas());
            Assert.That.ListsAreEqual(expectedMember.ParticipantGroups.ToList(), actualTeamMember.ParticipantGroup.SplitCommas());
            
            Log.Info("Verify Stakeholder added");
            editStakeholdersPage.NavigateToPage(teamId);
            var actualStakeholder = editStakeholdersPage.GetStakeHolderInfoFromGrid(1);
            var expectedStakeholder = members.First(m => m.IsStakeholder);
            Assert.AreEqual(expectedStakeholder.FirstName, actualStakeholder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(expectedStakeholder.LastName, actualStakeholder.LastName, "Lastname doesn't match");
            Assert.AreEqual(expectedStakeholder.Email, actualStakeholder.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(expectedStakeholder.Roles.ToList(), actualStakeholder.Role.SplitCommas());
            
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