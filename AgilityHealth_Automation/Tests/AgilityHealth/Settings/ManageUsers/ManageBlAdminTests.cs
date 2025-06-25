using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManageBlAdminTests : BaseTest
    {
        // set the team name so it can be used by all the test methods
        public static string NormalTeamName = "BlAdminTeam" + RandomDataUtil.GetTeamName();
        public static UserType UserType = UserType.BusinessLineAdmin;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var team = new AddTeamWithMemberRequest
            {
                Name = NormalTeamName,
                AgileAdoptionDate = DateTime.Now,
                FormationDate = DateTime.Now,
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest {Category = "Work Type", Tags = new List<string> {SharedConstants.NewTeamWorkType}},
                    new TeamTagRequest {Category = "Methodology", Tags = new List<string> {"Scrum"}}
                }
            };

            var setup = new SetupTeardownApi(TestEnvironment);
            setup.CreateTeam(team).GetAwaiter().GetResult();

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageUsers_AddEditDeleteBLAdmin_TeamAccess()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType);
            var loginPage = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnSettingsLink();

            settingsPage.SelectSettingsOption("View Users");

            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var blAdminInfo = new BlAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_BL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                NotifyUser = true,
                Tag = SharedConstants.MultiTeam,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };

            manageUserPage.EnterBlAdminInfo(blAdminInfo,User.IsBusinessLineAdmin());
            manageUserPage.ClickSaveAndCloseButton();
            
            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            var userImagePath = manageUserPage.GetAvatar( blAdminInfo.Email);

            Log.Info($"New user email: {blAdminInfo.Email}");
            Assert.IsTrue(manageUserPage.IsUserExist( userImagePath, blAdminInfo.FirstName, blAdminInfo.LastName, blAdminInfo.Email, today),
                "Failure !! Business Line Admin with First name: " + blAdminInfo.FirstName + " and Last name: " + blAdminInfo.LastName + " is not created");
            
            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", blAdminInfo.Email, "Inbox"));


            loginPage.SetUserPassword(SharedConstants.CommonPassword);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(blAdminInfo.Tag), $"Team '{blAdminInfo.Tag}' does not show on Teams Dashboard");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageUsers_AddBLAdmin_WithNoTag()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType);
            var loginPage = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var sectionName = "Teams";
            var featureName = "Add Team Tags";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnSettingsLink();
            settingsPage.SelectSettingsOption("View Users");
            manageUserPage.SelectTab();

            Log.Info("Create a new Bl user with no tags");
            manageUserPage.ClickOnAddNewUserButton();

            var blAdminInfo = new BlAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_BL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                NotifyUser = true,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };

            manageUserPage.EnterBlAdminInfo(blAdminInfo, User.IsBusinessLineAdmin());
            manageUserPage.ClickSaveAndCloseButton();

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);

            var userImagePath = manageUserPage.GetAvatar(blAdminInfo.Email);

            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, blAdminInfo.FirstName, blAdminInfo.LastName, blAdminInfo.Email, today),
                "Failure !! Business Line Admin with First name: " + blAdminInfo.FirstName + " and Last name: " + blAdminInfo.LastName + " is not created");

            Log.Info($"Turn off '{featureName}' feature.");
            manageUserPage.EditPermission(sectionName, featureName,false);
            
            manageUserPage.EditPermission("Teams", "Edit Team Tags", false);


            Log.Info("Logout and login as new BL user and verify no team is displayed");
            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", blAdminInfo.Email, "Inbox"));

            loginPage.SetUserPassword(SharedConstants.CommonPassword);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            Log.Info($"Verify that no team is displayed for user {blAdminInfo.FirstName}");
            Assert.IsTrue(dashBoardPage.DoesAnyTeamDisplay(), "Few Teams are shown Dashboard");

            Log.Info("Create a new team as a new BL user");
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();

            var teamInfo = new TeamInfo
            {
                TeamName = NormalTeamName + "01",
                WorkType = SharedConstants.NewTeamWorkType,
                Methodology = "Scrum"
            };

            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();
            finishAndReviewPage.ClickOnGoToTeamDashboard();

            dashBoardPage.GridTeamView();

            Log.Info($"Verify team: {teamInfo.TeamName} is created successfully");
            Assert.AreEqual(teamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, dashBoardPage.GetCellValue(1, "Work Type"), "Work Type doesn't match");
        }
    }
}
