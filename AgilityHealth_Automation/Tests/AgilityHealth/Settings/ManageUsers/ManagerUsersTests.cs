using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManagerUsersTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        public static UserType UserType;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetUpMethods(_, TestEnvironment);
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                setup.TurnOnOffDisableFileUploadRequiredFeature(Company.Id, false);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical"), TestCategory("Smoke"), TestCategory("CompanyAdmin")]
        public void ManageUsers_AddEditDeleteCompanyAdmin()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);
            var loginPage = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.ClickOnAddNewUserButton();

            var companyAdminInfo = new CompanyAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_CA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                FeatureAdmin = false,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            Log.Info($"New user email: {companyAdminInfo.Email}");

            manageUserPage.EnterCompanyAdminInfo(companyAdminInfo);
            manageUserPage.ClickSaveAndCloseButton();

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(companyAdminInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            Assert.AreEqual(companyAdminInfo.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(companyAdminInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, companyAdminInfo.FirstName, companyAdminInfo.LastName, companyAdminInfo.Email, today),
                "Failure !! Company Admin with First name: " + companyAdminInfo.FirstName + " and Last name: " + companyAdminInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(companyAdminInfo.Email), "Created User date is not matched");

            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Confirm your account", companyAdminInfo.Email));

            loginPage.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();

            manageUserPage.NavigateToPage(Company.Id);

            manageUserPage.ClickOnEditUserIcon(companyAdminInfo.Email);
            var companyAdminInfoEdited = new CompanyAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_CA_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "English",
                FeatureAdmin = false,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            manageUserPage.EditCompanyAdminInfo(companyAdminInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();
            Log.Info($"Edited user email: {companyAdminInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(companyAdminInfoEdited.Email);
            Assert.AreEqual(companyAdminInfoEdited.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "Updated 'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(companyAdminInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, companyAdminInfoEdited.FirstName, companyAdminInfoEdited.LastName, companyAdminInfoEdited.Email, today),
                $"Failure !! Company Admin with First name: {companyAdminInfoEdited.FirstName} and Last name: {companyAdminInfoEdited.LastName} is not created");

            Driver.NavigateToPage(ApplicationUrl);
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            var teamNames = dashBoardPage.GetAllTeamsNames();
            var applicableTeams = Constants.AllTestTeams.Except(Constants.TestTeamsNotApplicableToCompanyAdmin).ToList();
            foreach (var expectedTeam in applicableTeams)
            {
                CollectionAssert.Contains(teamNames, expectedTeam,
                    $"The new user should have access to '{expectedTeam}', but it was not found on the Team Dashboard");
            }

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            Assert.AreEqual($"{_team.Name} Team Assessments", teamAssessmentDashboard.GetAssessmentDashboardTitle(),
                "Failure !! Company Admin doesn't have access to team within his company");

            topNav.LogOut();

            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);

            //Edit
            var actualLastLogin = manageUserPage.GetLastLogin(companyAdminInfoEdited.FirstName, companyAdminInfoEdited.LastName, companyAdminInfoEdited.Email);

            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(companyAdminInfoEdited.Email), "Created User date is not matched");
            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 5);

            manageUserPage.ClickOnEditUserIcon(companyAdminInfoEdited.Email);

            manageUserPage.CheckUncheckAhFCheckboxValue(false);
            manageUserPage.CheckUncheckAhTrainerCheckboxValue(false);
            manageUserPage.ClickSaveAndCloseButton();

            manageUserPage.ClickOnEditUserIcon(companyAdminInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            manageUserPage.ClickCancelButton();
            EditUserAndVerifyPopupFromGlobalSearch(companyAdminInfoEdited.Email, UserType.CompanyAdmin);

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(companyAdminInfoEdited.Email);

            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(companyAdminInfoEdited.Email),
                $"Failure !! Company Admin with First name: {companyAdminInfoEdited.FirstName} and Last name: {companyAdminInfoEdited.LastName} is not deleted");

        }

        [TestMethod]
        [TestCategory("Critical"), TestCategory("Smoke"), TestCategory("CompanyAdmin")]
        public void ManageUsers_AddEditDeleteTeamAdmin()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.TeamAdmin);
            var loginPage = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var teamAdminInfo = new TeamAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_TA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                Team = SharedConstants.Team,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            manageUserPage.EnterTeamAdminInfo(teamAdminInfo);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {teamAdminInfo.Email}");
            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(teamAdminInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            Assert.AreEqual(teamAdminInfo.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();
            var userImagePath = manageUserPage.GetAvatar(teamAdminInfo.Email);

            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, teamAdminInfo.FirstName, teamAdminInfo.LastName, teamAdminInfo.Email, today),
                "Failure !! Team Admin with First name: " + teamAdminInfo.FirstName + " and Last name: " + teamAdminInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(teamAdminInfo.Email), "Created User date is not matched");

            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", teamAdminInfo.Email, "Inbox"));

            loginPage.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            Assert.AreEqual(1, dashBoardPage.TotalTeam(), "Team count doesn't match");

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            Assert.AreEqual($"{_team.Name} Team Assessments", teamAssessmentDashboard.GetAssessmentDashboardTitle(),
                "Failure !! Team Admin doesn't have access to assigned team");

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnEditUserIcon(teamAdminInfo.Email);

            var teamAdminInfoEdited = new TeamAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_TA_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "English",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
            };

            manageUserPage.EditTeamAdminInfo(teamAdminInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"Edited user email: {teamAdminInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(teamAdminInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            Assert.AreEqual(teamAdminInfoEdited.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "Updated 'Preferred Language' is not matched");
            userImagePath = manageUserPage.GetAvatar(teamAdminInfoEdited.Email);
            manageUserPage.ClickCancelButton();

            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, teamAdminInfoEdited.FirstName, teamAdminInfoEdited.LastName, teamAdminInfoEdited.Email, today),
                $"Failure !! Team Admin with First name: {teamAdminInfoEdited.FirstName} and Last name: {teamAdminInfoEdited.LastName} is not created");

            var actualLastLogin = manageUserPage.GetLastLogin(teamAdminInfoEdited.FirstName, teamAdminInfoEdited.LastName, teamAdminInfoEdited.Email);
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(teamAdminInfoEdited.Email), "Created User date is not matched");

            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 10);

            EditUserAndVerifyPopupFromGlobalSearch(teamAdminInfoEdited.Email, UserType.TeamAdmin);

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(teamAdminInfoEdited.Email);

            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(teamAdminInfoEdited.Email),
                $"Failure !! Team Admin with First name: {teamAdminInfoEdited.FirstName} and Last name: {teamAdminInfoEdited.LastName} is not deleted");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUsers_AddEditDeleteOL_NoTeamNameSubteam()
        {
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            var loginPage = new LoginPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var organizationalLeadersInfo = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_OL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                NotifyUser = true,
                CanSeeTeamName = false,
                CanViewSubteams = false,
                Team = Constants.MultiTeamForBenchmarking,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            manageUserPage.EnterOlInfo(organizationalLeadersInfo);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {organizationalLeadersInfo.Email}");

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, organizationalLeadersInfo.FirstName, organizationalLeadersInfo.LastName, organizationalLeadersInfo.Email, today),
                "Failure !! OL Admin with First name: " + organizationalLeadersInfo.FirstName + " and Last name: " + organizationalLeadersInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(organizationalLeadersInfo.Email), "Created User date is not matched");

            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", organizationalLeadersInfo.Email, "Inbox"));

            loginPage.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();

            dashBoardPage.GridTeamView();

            Assert.AreEqual(1, dashBoardPage.TotalTeam(), "Team count doesn't match");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(Constants.MultiTeamForBenchmarking), $"{Constants.MultiTeamForBenchmarking} Team doesn't exists.");

            dashBoardPage.GoToTeamAssessmentDashboard(1);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamAssessmentType);

            radarPage.Filter_OpenFilterSidebar();

            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist("Team 1"), "Team 1 doesn't exists");

            topNav.LogOut();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageUserPage.NavigateToPage(Company.Id);

            manageUserPage.SelectTab();
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            var organizationalLeadersInfoEdited = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                CanSeeTeamName = false,
                CanViewSubteams = false,
                Email = Constants.UserEmailPrefix + "_OL_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
            };

            manageUserPage.EditOlInfo(organizationalLeadersInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {organizationalLeadersInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(organizationalLeadersInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, organizationalLeadersInfoEdited.FirstName, organizationalLeadersInfoEdited.LastName, organizationalLeadersInfoEdited.Email, today),
                $"Failure !! Organizational Leaders with First name: {organizationalLeadersInfoEdited.FirstName} and Last name: {organizationalLeadersInfoEdited.LastName} is not created");

            var actualLastLogin = manageUserPage.GetLastLogin(organizationalLeadersInfoEdited.FirstName, organizationalLeadersInfoEdited.LastName, organizationalLeadersInfoEdited.Email);
            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 10);
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(organizationalLeadersInfoEdited.Email), "Created User date is not matched");

            EditUserAndVerifyPopupFromGlobalSearch(organizationalLeadersInfoEdited.Email, UserType.OrganizationalLeader);

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(organizationalLeadersInfoEdited.Email);
            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(organizationalLeadersInfoEdited.Email),
                $"Failure !!  Organizational Leaders with First name: {organizationalLeadersInfoEdited.FirstName} and Last name: {organizationalLeadersInfoEdited.LastName} is not deleted");
        }


        [TestMethod]
        [Description("Verify that the Organizational Leaders can be added/edited/deleted successfully with ability to see Team Name and no ability to see subteams")]
        [TestCategory("CompanyAdmin")]
        public void ManageUsers_AddEditDeleteOL_SeeTeamNameNoSubteam()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var organizationalLeadersInfo = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_OL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                NotifyUser = true,
                CanSeeTeamName = true,
                CanViewSubteams = false,
                Team = Constants.MultiTeamForBenchmarking,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            manageUserPage.EnterOlInfo(organizationalLeadersInfo);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {organizationalLeadersInfo.Email}");

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, organizationalLeadersInfo.FirstName, organizationalLeadersInfo.LastName, organizationalLeadersInfo.Email, today),
                "Failure !! Organization leader with First name: " + organizationalLeadersInfo.FirstName + " and Last name: " + organizationalLeadersInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(organizationalLeadersInfo.Email), "Created User date is not matched");
            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", organizationalLeadersInfo.Email, "Inbox"));

            login.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            Assert.AreEqual(1, dashBoardPage.TotalTeam(), "Team count doesn't match");

            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(Constants.MultiTeamForBenchmarking),
                "Failure !! Team displays incorrectly for Organizational Leaders");

            dashBoardPage.GoToTeamAssessmentDashboard(1);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamAssessmentType);

            radarPage.Filter_OpenFilterSidebar();

            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist(SharedConstants.RadarTeam),
                "Failure !! Subteam displays incorrectly for Organizational Leaders");

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            var organizationalLeadersInfoEdited = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_OL_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                CanSeeTeamName = false,
                CanViewSubteams = false,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            manageUserPage.EditOlInfo(organizationalLeadersInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {organizationalLeadersInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(organizationalLeadersInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, organizationalLeadersInfoEdited.FirstName, organizationalLeadersInfoEdited.LastName, organizationalLeadersInfoEdited.Email, today),
                $"Failure !! organizational Leaders with First name: {organizationalLeadersInfoEdited.FirstName} and Last name: {organizationalLeadersInfoEdited.LastName} is not created");

            var actualLastLogin = manageUserPage.GetLastLogin(organizationalLeadersInfoEdited.FirstName, organizationalLeadersInfoEdited.LastName, organizationalLeadersInfoEdited.Email);
            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 10);
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(organizationalLeadersInfoEdited.Email), "Created User date is not matched");

            EditUserAndVerifyPopupFromGlobalSearch(organizationalLeadersInfoEdited.Email, UserType.OrganizationalLeader);

            topNav.LogOut();
            login.LoginToApplication(organizationalLeadersInfoEdited.Email, SharedConstants.CommonPassword);
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();

            Assert.AreEqual(1, dashBoardPage.TotalTeam(), "Team count doesn't match");

            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(Constants.MultiTeamForBenchmarking),
                "Failure !! Team displays incorrectly for Organizational Leaders after editing");

            dashBoardPage.GoToTeamAssessmentDashboard(1);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamAssessmentType);

            radarPage.Filter_OpenFilterSidebar();

            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist("Team 1"),
                "Failure !! Subteam 'Team 1' displays incorrectly for Organizational Leaders after editing");

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(organizationalLeadersInfoEdited.Email);

            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(organizationalLeadersInfoEdited.Email),
                $"Failure !! organizational Leaders with First name: {organizationalLeadersInfoEdited.FirstName} and Last name: {organizationalLeadersInfoEdited.LastName} is not deleted");

        }

        [TestMethod]
        [TestCategory("Critical"), TestCategory("CompanyAdmin")]
        public void ManageUsers_AddEditDeleteOL_SeeTeamNameAndSubteam()
        {
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            var loginPage = new LoginPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var organizationalLeadersInfo = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_OL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "French",
                NotifyUser = true,
                CanSeeTeamName = true,
                CanViewSubteams = true,
                Team = Constants.MultiTeamForBenchmarking,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            manageUserPage.EnterOlInfo(organizationalLeadersInfo);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {organizationalLeadersInfo.Email}");
            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            Assert.AreEqual(organizationalLeadersInfo.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(organizationalLeadersInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, organizationalLeadersInfo.FirstName, organizationalLeadersInfo.LastName, organizationalLeadersInfo.Email, today),
                "Failure !! organizational Leaders with First name: " + organizationalLeadersInfo.FirstName + " and Last name: " + organizationalLeadersInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(organizationalLeadersInfo.Email), "Created User date is not matched");
            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", organizationalLeadersInfo.Email, "Inbox"));

            loginPage.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();

            dashBoardPage.GridTeamView();

            Assert.AreEqual(2, dashBoardPage.TotalTeam(), "Team count doesn't match");

            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(SharedConstants.RadarTeam),
                "Failure !! Team displays incorrectly for Organizational Leaders");

            dashBoardPage.GoToTeamAssessmentDashboard(1);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamAssessmentType);

            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist(SharedConstants.RadarTeam),
                "Failure !! Subteam displays incorrectly for Organizational Leaders");

            topNav.LogOut();
            loginPage.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfo.Email);
            var organizationalLeadersInfoEdited = new OrganizationalLeadersInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_OL_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "English",
                CanSeeTeamName = false,
                CanViewSubteams = false,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            manageUserPage.EditOlInfo(organizationalLeadersInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {organizationalLeadersInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(organizationalLeadersInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            Assert.AreEqual(organizationalLeadersInfoEdited.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "Updated 'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(organizationalLeadersInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, organizationalLeadersInfoEdited.FirstName, organizationalLeadersInfoEdited.LastName, organizationalLeadersInfoEdited.Email, today),
                $"Failure !! organizational Leaders with First name: {organizationalLeadersInfoEdited.FirstName} and Last name: {organizationalLeadersInfoEdited.LastName} is not created");

            var actualLastLogin = manageUserPage.GetLastLogin(organizationalLeadersInfoEdited.FirstName, organizationalLeadersInfoEdited.LastName, organizationalLeadersInfoEdited.Email);
            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 10);
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(organizationalLeadersInfoEdited.Email), "Created User date is not matched");

            EditUserAndVerifyPopupFromGlobalSearch(organizationalLeadersInfoEdited.Email, UserType.OrganizationalLeader);

            topNav.LogOut();
            loginPage.LoginToApplication(organizationalLeadersInfoEdited.Email, SharedConstants.CommonPassword);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();

            Assert.AreEqual(1, dashBoardPage.TotalTeam(), "Team count doesn't match");

            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(Constants.MultiTeamForBenchmarking),
                "Failure !! Team displays incorrectly for Organizational Leaders after editing");

            dashBoardPage.GoToTeamAssessmentDashboard(1);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamAssessmentType);

            radarPage.Filter_OpenFilterSidebar();

            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist("Team 1"),
                "Failure !! Subteam 1 displays incorrectly for Organizational Leaders after editing");

            topNav.LogOut();
            loginPage.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(organizationalLeadersInfoEdited.Email);

            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Log.Info("Verify that Organizational Leaders is deleted");
            Assert.IsFalse(manageUserPage.IsUserDisplayed(organizationalLeadersInfoEdited.Email),
                $"Failure !! organizational Leaders with First name: {organizationalLeadersInfoEdited.FirstName} and Last name: {organizationalLeadersInfoEdited.LastName} is not deleted");

        }

        [TestMethod]
        [TestCategory("Critical"), TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageUsers_AddEditDeleteBLAdmin()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();
            var blAdminInfo = new BlAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_BL_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "German",
                NotifyUser = true,
                Tag = SharedConstants.TeamTag,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            manageUserPage.EnterBlAdminInfo(blAdminInfo, User.IsBusinessLineAdmin());
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {blAdminInfo.Email}");

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(blAdminInfo.Email);
            if (!User.IsBusinessLineAdmin())
            {
                Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
                Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            }

            Assert.AreEqual(blAdminInfo.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(blAdminInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, blAdminInfo.FirstName, blAdminInfo.LastName, blAdminInfo.Email, today),
                "Failure !! Business Line Admin with First name: " + blAdminInfo.FirstName + " and Last name: " + blAdminInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(blAdminInfo.Email), "Created User date is not matched");

            topNav.LogOut();
            login.NavigateToUrl(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", blAdminInfo.Email, "Inbox"));

            login.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            dashBoardPage.AddColumns(new List<string> { "Team Tags" });
            Assert.IsTrue(dashBoardPage.VerifyTeamTagsDisplay(blAdminInfo.Tag), "Failure !! Team tags displays incorrectly for BL Admin");

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnEditUserIcon(blAdminInfo.Email);
            var blAdminInfoEdited = new BlAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_BL_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "English",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
            };

            manageUserPage.EditBlAdminInfo(blAdminInfoEdited, User.IsBusinessLineAdmin());
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"Edited user email: {blAdminInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(blAdminInfoEdited.Email);
            if (!User.IsBusinessLineAdmin())
            {
                Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
                Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            }
            Assert.AreEqual(blAdminInfoEdited.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "Updated 'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(blAdminInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, blAdminInfoEdited.FirstName, blAdminInfoEdited.LastName, blAdminInfoEdited.Email, today),
                $"Failure !! Business Line Admin with First name: {blAdminInfoEdited.FirstName} and Last name: {blAdminInfoEdited.LastName} is not created");

            var actualLastLogin = manageUserPage.GetLastLogin(blAdminInfoEdited.FirstName, blAdminInfoEdited.LastName, blAdminInfoEdited.Email);
            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 10);
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(blAdminInfoEdited.Email), "Created User date is not matched");

            EditUserAndVerifyPopupFromGlobalSearch(blAdminInfoEdited.Email, UserType.BusinessLineAdmin);

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(blAdminInfoEdited.Email);
            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(blAdminInfoEdited.Email),
                $"Failure !! Business Line Admin with First name: {blAdminInfoEdited.FirstName} and Last name: {blAdminInfoEdited.LastName} is not deleted");

        }

        [TestMethod]
        [TestCategory("Critical"), TestCategory("Smoke"), TestCategory("CompanyAdmin")]
        public void ManageUsers_AddEditDeleteCoach()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.Coaches);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            manageUserPage.ClickOnAddNewUserButton();

            var coachInfo = new CoachInfo()
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Japanese",
                LinkedIn = "https://linkedin.com/agilityhealth",
                Bio = "This is Bio",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                ActiveAhf = true,
                AhTrainer = true
            };

            manageUserPage.EnterCoachInfo(coachInfo);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {coachInfo.Email}");

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(coachInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            Assert.AreEqual(coachInfo.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(coachInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, coachInfo.FirstName, coachInfo.LastName, coachInfo.Email, today),
            $"Failure !! Business Line Admin with First name: {coachInfo.FirstName} and Last name: {coachInfo.LastName} is not created");

            manageUserPage.ClickOnEditUserIcon(coachInfo.Email);
            var coachInfoEdited = new CoachInfo()
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "English",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            manageUserPage.EditCoachInfo(coachInfoEdited);
            manageUserPage.ClickSaveAndCloseButton();

            Log.Info($"New user email: {coachInfoEdited.Email}");

            manageUserPage.ClickOnEditUserIcon(coachInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            Assert.AreEqual(coachInfoEdited.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "Updated 'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(coachInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, coachInfoEdited.FirstName, coachInfoEdited.LastName, coachInfoEdited.Email, today),
                $"Failure !! Business Line Admin with First name: {coachInfoEdited.FirstName} and Last name: {coachInfoEdited.LastName} is not created");


            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(coachInfoEdited.Email);


            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(coachInfoEdited.Email),
                $"Failure !! Business Line Admin with First name: {coachInfoEdited.FirstName} and Last name: {coachInfoEdited.LastName} is not deleted");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48956
        [TestCategory("Critical"), TestCategory("Smoke"),]
        [TestCategory("SiteAdmin")]
        public void ManageUsers_AddEditDeletePartnerAdmin()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.PartnerAdmin);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var manageFeaturePage = new ManageFeaturesPage(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();
            manageFeaturePage.NavigateToPage(Company.Id);
            manageFeaturePage.TurnOffOrganizationalHierarchy();
            manageFeaturePage.ClickUpdateButton();

            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.ClickOnAddNewUserButton();

            var partnerAdminInfo = new PartnerAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "_PA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "Spanish",
                NotifyUser = true,
                Companies = new List<string>
                {
                    "Automation_BL (DO NOT USE)",
                    "Automation_CA (DO NOT USE)",
                    "Automation_M (DO NOT USE)",
                    "Automation_OL (DO NOT USE)"
                },
                ActiveAhf = true,
                AhTrainer = true,
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg")
            };

            manageUserPage.EnterPartnerAdminInfo(partnerAdminInfo);
            manageUserPage.AddCompanies(partnerAdminInfo.Companies);
            partnerAdminInfo.Companies.Add(User.CompanyName);
            manageUserPage.ClickSaveAndCloseButton();

            var today = DateTime.Now.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
            manageUserPage.ClickOnEditUserIcon(partnerAdminInfo.Email);
            Assert.IsTrue(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is not selected, when it should be selected");
            Assert.IsTrue(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is not selected, when it should be selected");
            Assert.AreEqual(partnerAdminInfo.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            var userImagePath = manageUserPage.GetAvatar(partnerAdminInfo.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, partnerAdminInfo.FirstName, partnerAdminInfo.LastName, partnerAdminInfo.Email, today),
                "Failure !! Partner Admin with First name: " + partnerAdminInfo.FirstName + " and Last name: " + partnerAdminInfo.LastName + " is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(partnerAdminInfo.Email), "Created User date is not matched");

            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserActivationLink("Create Your AgilityInsights Account", partnerAdminInfo.Email));

            loginPage.SetUserPassword(SharedConstants.CommonPassword);
            var lastLogin = DateTime.Now.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture);

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.WaitUntilCompanyLoaded();
            var actualCompanies = companyDashboardPage.GetColumnValues("Company Name");
            Assert.AreEqual(partnerAdminInfo.Companies.Count, actualCompanies.Count, "Number of Companies on Company Dashboard doesn't match.");

            foreach (var company in partnerAdminInfo.Companies)
            {
                Assert.IsTrue(actualCompanies.Contains(company), $"<{company}> was not found in the list <{actualCompanies}>");
            }

            v2Header.SignOut();

            loginPage.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);

            var actualLastLogin = manageUserPage.GetLastLogin(partnerAdminInfo.FirstName, partnerAdminInfo.LastName, partnerAdminInfo.Email);
            Assert.That.TimeIsClose(DateTime.Parse(lastLogin), DateTime.Parse(actualLastLogin), 5);

            var partnerAdminInfoEdited = new PartnerAdminInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = $"{SharedConstants.TeamMemberLastName}Edited",
                Email = Constants.UserEmailPrefix + "_PA_edit" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                PreferredLanguage = "English",
                Companies = new List<string>
                {
                    User.CompanyName
                },
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            manageUserPage.ClickOnEditUserIcon(partnerAdminInfo.Email);

            manageUserPage.EditPartnerAdminInfo(partnerAdminInfoEdited);
            manageUserPage.RemoveCompanies(partnerAdminInfo.Companies.Except(partnerAdminInfoEdited.Companies).ToList());
            manageUserPage.ClickSaveAndCloseButton();

            manageUserPage.ClickOnEditUserIcon(partnerAdminInfoEdited.Email);
            Assert.IsFalse(manageUserPage.IsAhTrainerCheckboxSelected(), "Ah Trainer Checkbox is selected, when it should not be selected");
            Assert.IsFalse(manageUserPage.IsAhfCheckboxSelected(), "Ahf Checkbox is selected, when it should not be selected");
            Assert.AreEqual(partnerAdminInfoEdited.PreferredLanguage, manageUserPage.GetSelectedPreferredLanguage(), "Updated 'Preferred Language' is not matched");
            manageUserPage.ClickCancelButton();

            userImagePath = manageUserPage.GetAvatar(partnerAdminInfoEdited.Email);
            Assert.IsTrue(manageUserPage.IsUserExist(userImagePath, partnerAdminInfoEdited.FirstName, partnerAdminInfoEdited.LastName, partnerAdminInfoEdited.Email, today),
                $"Failure !! Partner Admin with First name: {partnerAdminInfoEdited.FirstName} and Last name: {partnerAdminInfoEdited.LastName} is not created");
            Assert.AreEqual(today, manageUserPage.GetUserAddedDateFromGrid(partnerAdminInfoEdited.Email), "Created User date is not matched");

            EditUserAndVerifyPopupFromGlobalSearch(partnerAdminInfoEdited.Email, UserType.PartnerAdmin);

            var alertMessage = manageUserPage.ClickOnDeleteUserIcon(partnerAdminInfoEdited.Email);

            Assert.AreEqual(Constants.DeleteUserAlertMessage, alertMessage, "Alert message doesn't match");

            Assert.IsFalse(manageUserPage.IsUserDisplayed(partnerAdminInfoEdited.Email),
                $"Failure !! Partner Admin with First name: {partnerAdminInfoEdited.FirstName} and Last name: {partnerAdminInfoEdited.LastName} is not deleted");
        }

        public void EditUserAndVerifyPopupFromGlobalSearch(string email, UserType userType)
        {
            var manageUserPage = new ManageUserPage(Driver, Log, userType);

            Log.Info("Verify that edit user page is loaded successfully when user try to edit user from global search.");
            manageUserPage.Search(email);
            manageUserPage.ClickOnEditUserIconFromGlobalSearch();
            Assert.IsTrue(manageUserPage.IsEditUserPageDisplayed(), "Edit user page is not displayed.");
            manageUserPage.ClickCancelButton();
            manageUserPage.ClickOnCloseButton();
        }
    }
}