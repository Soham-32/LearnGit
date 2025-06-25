using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers.Permissions 
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings"),TestCategory("ManageUsersPermissions")]
    [TestCategory("CompanyAdmin")]
    public class ManagerUsersAssessmentPermissionsOnOffTests : BaseTest
    {
        private static readonly UserConfig ManageUserPermissionUserConfig = new UserConfig("MUP");
        private static User OlPermissionUser => ManageUserPermissionUserConfig.GetUserByDescription("manage permission ol user");
        private static User BlPermissionUser => ManageUserPermissionUserConfig.GetUserByDescription("manage permission bl user");
        private static User TaPermissionUser => ManageUserPermissionUserConfig.GetUserByDescription("manage permission ta user");

        private static IWebDriver _driver;
        public static bool ClassInitFailed;
        private static TeamHierarchyResponse _team;
        private static int _assessmentId;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamHealthRadarName,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var setup = new SetUpMethods(testContext, TestEnvironment);

                _team = setupApi.GetCompanyHierarchy(Company.Id, User).GetTeamByName(SharedConstants.Team);
                setup.AddTeamAssessmentAndGi(_team.TeamId, TeamAssessment, new List<GrowthItem> { GrowthItemInfo });
                var emailSearch = new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { "Auto_Member" }
                };
                setup.CompleteTeamMemberSurvey(emailSearch);
                _assessmentId =  setupApi.GetAssessmentResponse(_team.Name, TeamAssessment.AssessmentName, User, Company.Id).Result.AssessmentId;
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51205
        public void ManageUser_AssessmentPermissions_OnOff_OrgLeader()
        {
            ManageUser_AssessmentPermissions_OnOff(UserType.OrganizationalLeader, OlPermissionUser);
        }
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51205
        public void ManageUser_AssessmentPermissions_OnOff_BusinessLine()
        {
            ManageUser_AssessmentPermissions_OnOff(UserType.BusinessLineAdmin,BlPermissionUser);
        }
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51205
        public void ManageUser_AssessmentPermissions_OnOff_TeamAdmin()
        {
            ManageUser_AssessmentPermissions_OnOff(UserType.TeamAdmin,TaPermissionUser);
        }

        public void ManageUser_AssessmentPermissions_OnOff(UserType user,User permissionUser)
        {
            VerifySetup(ClassInitFailed);

            Log.Info("Launch New Driver");
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2);
            _driver = new ChromeDriver(options);
            _driver.Manage().Cookies.DeleteAllCookies();
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(180);
            _driver.Manage().Timeouts().AsynchronousJavaScript = TestEnvironment.JsTimeout;
            _driver.Manage().Window.Maximize();

            var caLoginPage = new LoginPage(Driver, Log);
            var userLoginPage = new LoginPage(_driver, Log);
            var radarPage = new RadarPage(_driver, Log);
            var taEditPage = new TeamAssessmentEditPage(_driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, user);
            var teamDashboardPage = new TeamDashboardPage(_driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(_driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(_driver, Log);
            
            const string sectionName = "Assessments";

            Log.Info($"Navigate to the login page and login as {User.FullName}");
            caLoginPage.NavigateToPage();
            caLoginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Manage Users' page and select the Tab");
            manageUserPage.NavigateToPage(Company.Id);
            manageUserPage.SelectTab();

            Log.Info("On / Off all assessment permissions and verify the permissions should be working");
            manageUserPage.EditPermission(sectionName, "View Assessment Dashboard", false);

            Log.Info($"Navigate to the login page and login as {permissionUser.FullName}");
            userLoginPage.NavigateToPage();
            userLoginPage.LoginToApplication(permissionUser.Username, permissionUser.Password);

            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.ClickOnTeamName(_team.Name);
            Assert.IsFalse(Driver.GetCurrentUrl().Contains($"/teams/{_team.TeamId}/assessments"), "User is able to navigate to 'Assessment Dashboard' tab");

            manageUserPage.EditPermission(sectionName, "View Assessment Dashboard");
            _driver.RefreshPage();
            teamDashboardPage.ClickOnTeamName(_team.Name);
            Assert.IsTrue(teamAssessmentDashboardPage.GetAssessmentDashboardTabTitleText().Contains(_team.Name), "User is not able to navigate to 'Assessment Dashboard' tab");

            manageUserPage.EditPermission(sectionName, "Add Assessment", false);
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            Assert.IsFalse(teamAssessmentDashboardPage.IsAddAssessmentButtonDisplayed(), "'Add Assessment' button is displayed");

            manageUserPage.EditPermission(sectionName, "Add Assessment");
            _driver.RefreshPage();
            Assert.IsTrue(teamAssessmentDashboardPage.IsAddAssessmentButtonDisplayed(), "'Add Assessment' button is not displayed");

            manageUserPage.EditPermission(sectionName, "Edit Assessment", false);
            _driver.RefreshPage();
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(TeamAssessment.AssessmentName), "Edit icon is displayed");

            manageUserPage.EditPermission(sectionName, "Edit Assessment");
            _driver.RefreshPage();
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(TeamAssessment.AssessmentName), "Edit icon is not displayed");
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/edit", _driver.GetCurrentUrl(), "'Radar edit' assessment page is not displayed");

            //bug
            /*manageUserPage.EditPermission(sectionName, "View Assessments", false);
            manageUserPage.EditPermission(sectionName, "View Assessments");*/

            manageUserPage.EditPermission(sectionName, "View Growth Plan", false);
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.ClickOnRadar(TeamAssessment.AssessmentName);
            Assert.IsFalse(growthItemGridView.IsGiPresent(GrowthItemInfo.Title), "Growth Item is displayed");

            manageUserPage.EditPermission(sectionName, "View Growth Plan");
            _driver.RefreshPage();
            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemInfo.Title), "Growth Item is not displayed");

            manageUserPage.EditPermission(sectionName, "Edit Growth Plan", false);
            _driver.RefreshPage();
            Assert.IsFalse(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemInfo.Title), "'Edit' button is displayed for GI");

            manageUserPage.EditPermission(sectionName, "Edit Growth Plan");
            _driver.RefreshPage();
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemInfo.Title), "'Edit' button is not displayed for GI");

            manageUserPage.EditPermission(sectionName, "Can Generate PDF", false);
            _driver.RefreshPage();
            Assert.IsFalse(radarPage.IsExportPdfButtonDisplayed(), "Pdf icon is displayed");

            manageUserPage.EditPermission(sectionName, "Can Generate PDF");
            _driver.RefreshPage();
            Assert.IsTrue(radarPage.IsExportPdfButtonDisplayed(), "Pdf icon is not displayed");

            manageUserPage.EditPermission(sectionName, "Can Delete Assessment", false);
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            Assert.IsFalse(taEditPage.IsDeleteButtonDisplayed(), "Delete button is displayed");

            manageUserPage.EditPermission(sectionName, "Can Delete Assessment");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            Assert.IsTrue(taEditPage.IsDeleteButtonDisplayed(), "Delete button is not displayed");

            //bug
            /*manageUserPage.EditPermission(sectionName, "Can Close Assessments", false);
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            taEditPage.ClickEditDetailButton();
        
            manageUserPage.EditPermission(sectionName, "Can Close Assessments");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            taEditPage.ClickEditDetailButton();*/

            //Clean up
            try
            {
                _driver.Close();
                _driver.Quit();
            }
            catch
            {
                // ignored
            }
        }
    }
}