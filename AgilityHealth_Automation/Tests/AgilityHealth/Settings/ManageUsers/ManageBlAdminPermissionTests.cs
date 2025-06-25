using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.EnterpriseTeam;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManageBlAdminPermissionTests : BaseTest
    {

        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User BusinessLineAdminUser => SiteAdminUserConfig.GetUserByDescription("business line admin 3");
        
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48956
        [TestCategory("SiteAdmin")]
        public void ManageUsers_Permission_BlAdmin_AddEditTags()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamProfilePage = new CreateTeamPage(Driver, Log);
            var multiTeamProfilePage = new CreateMultiTeamPage(Driver, Log);
            var createEnterpriseTeamPage = new CreateEnterpriseTeamPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);
            const string expectedTag = "Automation";
            const string teams = "Teams";
            const string multiTeams = "Multi Teams";
            const string enterpriseTeams = "Enterprise Teams";
            const string addTeamTags = "Add Team Tags";
            const string editTeamTags = "Edit Team Tags";
            const string addMultiTeamTags = "Add Multi Team Tags";
            const string editMultiTeamTags = "Edit Multi Team Tags";
            const string addEnterpriseTeamTags = "Add Enterprise Team Tags";
            const string editEnterpriseTeamTags = "Edit Enterprise Team Tags";

            Log.Info($"Login as {User.Username} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(Company.Id);

            Log.Info("Turn On 'MtEt Team' feature and click on update button");
            manageFeaturesPage.TurnOnMtEtTeamFeature();
            manageFeaturesPage.TurnOnEnableLanguageSelection();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to the 'Manage User' page and select 'Business Line Admin' tab");
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("Edit Permission for 'Business Line admin' user");
            manageUserPage.EditPermission(teams, addTeamTags);
            manageUserPage.EditPermission(teams, editTeamTags);

            manageUserPage.EditPermission(multiTeams, addMultiTeamTags);
            manageUserPage.EditPermission(multiTeams, editMultiTeamTags);

            manageUserPage.EditPermission(enterpriseTeams, addEnterpriseTeamTags);
            manageUserPage.EditPermission(enterpriseTeams, editEnterpriseTeamTags);

            topNav.LogOut();

            Log.Info($"Login as {BusinessLineAdminUser.Username} user");
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);

            //Team
            Log.Info("Create a team and verify the 'Team Tags' section");
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is not displayed");

            var today = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            var teamInfo = new TeamInfo()
            {
                TeamName = RandomDataUtil.GetTeamName(),
                WorkType = SharedConstants.NewTeamWorkType,
                PreferredLanguage = "English",
                Methodology = "Scrum",
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                Tags = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Business Lines", SharedConstants.TeamTag) }
            };
            teamProfilePage.EnterTeamInfo(teamInfo);

            Assert.AreEqual(expectedTag, teamProfilePage.GetSelectedTag(), "Tag is not matched");
            teamProfilePage.ClickCreateTeamAndAddTeamMembers();

            Log.Info($"Edit the {teamInfo.TeamName} and verify the 'Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.SearchTeam(teamInfo.TeamName);
            dashBoardPage.ClickTeamEditButton(teamInfo.TeamName);
            Assert.IsTrue(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is not displayed");
            Assert.AreEqual(expectedTag, teamProfilePage.GetSelectedTag(), "Tag is not matched");

            //MultiTeam
            Log.Info("Create a multi team and verify the 'Multi Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.MultiTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(teamProfilePage.IsTagsSectionDisplayed(), "'Multi Team Tags' section is not displayed");

            var multiTeamInfo = new MultiTeamInfo()
            {
                TeamName = "MultiTeam" + RandomDataUtil.GetTeamName(),
                TeamType = "Program Team",
                AssessmentType = SharedConstants.TeamAssessmentType,
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };
            multiTeamProfilePage.EnterMultiTeamInfo(multiTeamInfo);
            Assert.AreEqual(expectedTag, teamProfilePage.GetSelectedTag(), "Tag is not matched");
            multiTeamProfilePage.ClickCreateTeamAndAddSubTeam();

            Log.Info($"Edit the {multiTeamInfo.TeamName} and verify the 'Multi Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.SearchTeam(multiTeamInfo.TeamName);
            dashBoardPage.ClickTeamEditButton(multiTeamInfo.TeamName);
            Assert.IsTrue(teamProfilePage.IsTagsSectionDisplayed(), "'Multi Team Tags' section is not displayed");
            Assert.AreEqual(expectedTag, teamProfilePage.GetSelectedTag(), "Tag is not matched");

            //Enterprise
            Log.Info("Create a enterprise team and verify the 'Enterprise Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.EnterpriseTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(teamProfilePage.IsTagsSectionDisplayed(), "Enterprise Team Tags' section is not displayed");

            var enterpriseTeamInfo = new EnterpriseTeamInfo
            {
                TeamName = "ET_" + RandomDataUtil.GetTeamName(),
                TeamType = "Portfolio Team",
                ExternalIdentifier = "",
                Department = "IT",
                DateEstablished = DateTime.Now,
                AgileAdoptionDate = DateTime.Now,
                Description = "test description for enterprise team",
                TeamBio = $"{RandomDataUtil.GetTeamBio()} enterprise team",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };
            createEnterpriseTeamPage.EnterEnterpriseTeamInfo(enterpriseTeamInfo);
            Assert.AreEqual(expectedTag, teamProfilePage.GetSelectedTag(), "Tag is not matched");
            createEnterpriseTeamPage.GoToAddSubteams();

            Log.Info($"Edit the {enterpriseTeamInfo.TeamName} and verify the 'Enterprise Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.SearchTeam(enterpriseTeamInfo.TeamName);
            dashBoardPage.ClickTeamEditButton(enterpriseTeamInfo.TeamName);
            Assert.IsTrue(teamProfilePage.IsTagsSectionDisplayed(), "'Multi Team Tags' section is not displayed");
            Assert.AreEqual(expectedTag, teamProfilePage.GetSelectedTag(), "Tag is not matched");
            topNav.LogOut();

            Log.Info($"Login as {User.Username} and navigate to the 'Manage User' page then select 'Business Line Admin' tab");
            login.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("Edit Permission for 'Business Line admin' user");
            manageUserPage.EditPermission(teams, addTeamTags, false);
            manageUserPage.EditPermission(teams, editTeamTags, false);

            manageUserPage.EditPermission(multiTeams, addMultiTeamTags, false);
            manageUserPage.EditPermission(multiTeams, editMultiTeamTags, false);

            manageUserPage.EditPermission(enterpriseTeams, addEnterpriseTeamTags, false);
            manageUserPage.EditPermission(enterpriseTeams, editEnterpriseTeamTags, false);
            topNav.LogOut();

            Log.Info($"Login as {BusinessLineAdminUser.Username}");
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);

            Log.Info("Create a team and verify the 'Team Tags' section");
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is displayed");

            Log.Info($"Edit the {SharedConstants.RadarTeam} and verify the 'Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.SearchTeam(SharedConstants.RadarTeam);
            dashBoardPage.ClickTeamEditButton(SharedConstants.RadarTeam);
            Assert.IsFalse(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is displayed");

            Log.Info("Create a multi team and verify the 'Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.MultiTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is displayed");

            Log.Info($"Edit the {SharedConstants.MultiTeamForGrowthJourney} and verify the 'Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickTeamEditButton(SharedConstants.MultiTeamForGrowthJourney);
            Assert.IsFalse(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is displayed");

            Log.Info("Create a enterprise team and verify the 'Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.EnterpriseTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is displayed");

            Log.Info($"Edit the {SharedConstants.EnterpriseTeamForGrowthJourney} and verify the 'Team Tags' section");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickTeamEditButton(SharedConstants.EnterpriseTeamForGrowthJourney);
            Assert.IsFalse(teamProfilePage.IsTagsSectionDisplayed(), "'Team Tags' section is displayed");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48956
        [TestCategory("SiteAdmin")]
        public void ManageUsers_Permission_BLAdmin_AddEditDelete_TeamAdmin()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var blAdminUser = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);
            var teamAdminUser = new ManageUserPage(Driver, Log, UserType.TeamAdmin);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            const string sectionName = "Users";

            Log.Info($"Navigate to the login page and login as {User.FullName}");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(Company.Id);

            Log.Info("Turn On 'Enable Language Selection' feature and click on update button");
            manageFeaturesPage.TurnOnEnableLanguageSelection();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to 'Manage Users' page and turn ON user permission for View/Add/Edit/Delete team admins.");
            blAdminUser.NavigateToPage(Company.Id);
            blAdminUser.SelectTab();

            blAdminUser.EditPermission(sectionName, "View Team Admins");
            blAdminUser.EditPermission(sectionName, "Add Team Admins");
            blAdminUser.EditPermission(sectionName, "Edit Team Admins");
            blAdminUser.EditPermission(sectionName, "Delete Team Admins");


            Log.Info($"Login as a '{BusinessLineAdminUser.FullName}' and Navigate to 'Manage Users' page and add team admin without assigning team.");
            topNav.LogOut();
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);
            teamAdminUser.NavigateToPage(Company.Id);
            Assert.IsTrue(teamAdminUser.IsUserTabDisplayed(),"Failure !! 'Team Admins' tab is not displayed.");

            //Add team admin
            teamAdminUser.SelectTab();
            teamAdminUser.ClickOnAddNewUserButton();

            var teamAdminInfo = new TeamAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_TA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            teamAdminUser.EnterTeamAdminInfo(teamAdminInfo);
            teamAdminUser.ClickSaveAndCloseButton();
            teamAdminUser.SearchUserOnUserTab(teamAdminInfo.Email);

            Assert.IsTrue(teamAdminUser.IsUserDisplayed(teamAdminInfo.Email),
                $"Failure !! Team Admin with email '{teamAdminInfo.Email}' is not displayed");

            //Edit team admin
            Log.Info("On 'Manage Users' page , Edit team admin.");
            teamAdminUser.ClickOnEditUserIcon(teamAdminInfo.Email);

            var teamAdminInfoEdited = new TeamAdminInfo
            {
                FirstName = "FirstNameEdited" + RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_TA_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
                Team = SharedConstants.Team
            };

            teamAdminUser.EditTeamAdminInfo(teamAdminInfoEdited);
            teamAdminUser.ClickSaveAndCloseButton();
            teamAdminUser.ClearSearchUserBox();
            teamAdminUser.SearchUserOnUserTab(teamAdminInfoEdited.Email);

            Assert.IsTrue(teamAdminUser.IsUserDisplayed(teamAdminInfoEdited.Email),
                $"Failure !! Team Admin with email '{teamAdminInfoEdited.Email}' is not displayed");

            //Delete team admin
            Log.Info("On 'Manage Users' page , Delete team admin.");
            var alertMessage = teamAdminUser.ClickOnDeleteUserIcon(teamAdminInfoEdited.Email);

            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(teamAdminUser.IsUserDisplayed(teamAdminInfoEdited.Email),
                $"Failure !! Team Admin with First name: {teamAdminInfoEdited.FirstName} and Last name: {teamAdminInfoEdited.LastName} is displayed");
        }
    }
}
