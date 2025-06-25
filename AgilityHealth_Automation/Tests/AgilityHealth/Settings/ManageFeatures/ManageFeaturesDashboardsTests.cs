using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesDashboardsTests : BaseTest
    {
        private static User CompanyAdmin1 => TestEnvironment.UserConfig.GetUserByDescription("user 3");
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");

        private static AtCommon.Dtos.Company SettingsCompany =>
            SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);

        //Team Agility Dashboard
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void ManageFeatures_TeamAgilityDashboard_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var headerFooterPage = new HeaderFooterPage(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Team Agility Dashboard' feature");
            manageFeaturesPage.TurnOnTeamAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                insightsDashboardPage.WaitUntilWidgetsLoaded();

                Assert.IsTrue(headerFooterPage.DoesInsightsButtonDisplay(), $"{userInfo.Type} - Insights dashboard link is not displayed in V2 header");
                Assert.IsTrue(insightsDashboardPage.IsTeamAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'Team Agility' tab is not displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void ManageFeatures_TeamAgilityDashboard_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Team Agility Dashboard' feature");
            manageFeaturesPage.TurnOffTeamAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                Assert.IsFalse(insightsDashboardPage.IsTeamAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'Team Agility' tab is displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //Structural Agility Dashboard
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void ManageFeatures_StructuralAgilityDashboard_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var headerFooterPage = new HeaderFooterPage(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Structural Agility Dashboard' feature");
            manageFeaturesPage.TurnOnStructuralAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                insightsDashboardPage.WaitUntilWidgetsLoaded();

                Assert.IsTrue(headerFooterPage.DoesInsightsButtonDisplay(), $"{userInfo.Type} - 'Insights' dashboard link is not displayed in V2 header");
                Assert.IsTrue(insightsDashboardPage.IsStructuralAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'Structural Agility' tab is not displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void ManageFeatures_StructuralAgilityDashboard_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Structural Agility Dashboard' feature");
            manageFeaturesPage.TurnOffStructuralAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                Assert.IsFalse(insightsDashboardPage.IsStructuralAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'Structural Agility' tab is displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //Enterprise Agility Dashboard - Feature is not implemented yet
        //[TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnterpriseAgilityDashboard_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var headerFooterPage = new HeaderFooterPage(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn On 'Enterprise Agility Dashboard' feature");
            manageFeaturesPage.TurnOnEnterpriseAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                insightsDashboardPage.WaitUntilWidgetsLoaded();

                Assert.IsTrue(headerFooterPage.DoesInsightsButtonDisplay(), $"{userInfo.Type} - 'Insights' dashboard link is not displayed in V2 header");
                Assert.IsTrue(insightsDashboardPage.IsEnterpriseAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'Enterprise Agility' tab is not displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //[TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnterpriseAgilityDashboard_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn Off 'Enterprise Agility Dashboard' feature");
            manageFeaturesPage.TurnOffEnterpriseAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                Assert.IsFalse(insightsDashboardPage.IsEnterpriseAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'Enterprise Agility' tab is displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //4 LENZ Dashboard
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_FourLenzDashboard_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var headerFooterPage = new HeaderFooterPage(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn On '4 LENZ Dashboard' feature");
            manageFeaturesPage.TurnOnFourLenzDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3"
            };

            foreach (var user in users)
            {

                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard & verify 4 LENZ Dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                insightsDashboardPage.WaitUntilWidgetsLoaded();

                Assert.IsTrue(headerFooterPage.DoesInsightsButtonDisplay(), $"{userInfo.Type} - 'Insights' dashboard link is not displayed in V2 header");
                Assert.IsTrue(insightsDashboardPage.IsFourLenzDashboardTabDisplayed(), $"{userInfo.Type} - '4 LENZ' tab is not displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_FourLenzDashboard_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn On '4 LENZ Dashboard' feature");
            manageFeaturesPage.TurnOffFourLenzDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard & verify that 4 LENZ dashboard should not be present");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                Assert.IsFalse(insightsDashboardPage.IsFourLenzDashboardTabDisplayed(), $"{userInfo.Type} - '4 LENZ' tab is displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //My Dashboard - Feature is not implemented yet
        //[TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_MyDashboard_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var headerFooterPage = new HeaderFooterPage(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn On 'My Dashboard' feature");
            manageFeaturesPage.TurnOnMyDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                insightsDashboardPage.WaitUntilWidgetsLoaded();

                Assert.IsTrue(headerFooterPage.DoesInsightsButtonDisplay(), $"{userInfo.Type} - 'Insights' dashboard link is not displayed in V2 header");
                Assert.IsTrue(insightsDashboardPage.IsMyDashboardTabDisplayed(), $"{userInfo.Type} - 'My Dashboard' tab is not displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //[TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_MyDashboard_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn Off 'My Dashboard' feature");
            manageFeaturesPage.TurnOffMyDashboard();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            var users = new List<string>
            {
                "user 3",
                "team admin 3",
                "business line admin 3",
                "org leader 3"
            };

            foreach (var user in users)
            {
                Log.Info($"Login as {user} and navigate to the 'Insight' dashboard");
                var userInfo = TestEnvironment.UserConfig.GetUserByDescription(user);
                login.LoginToApplication(userInfo.Username, userInfo.Password);
                topNav.ClickOnInsightsDashboardLink();
                Assert.IsFalse(insightsDashboardPage.IsTeamAgilityDashboardTabDisplayed(), $"{userInfo.Type} - 'My Dashboard' tab is displayed in Insights dashboard");

                if (users.IndexOf(user) == users.Count - 1) continue;
                Driver.Close();
                Driver.SwitchToFirstWindow();
                topNav.LogOut();
            }
        }

        //Business Outcomes Dashboard
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_BusinessOutCome_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Business Outcomes Dashboard' feature");
            manageFeaturesPage.TurnOnBusinessOutcomesDashboard();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            Assert.IsTrue(topNav.IsBusinessOutComeLinkDisplayed(), "On top nav, Business Outcome link isn't displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_BusinessOutCome_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Business Outcomes Dashboard' feature");
            manageFeaturesPage.TurnOffBusinessOutcomesDashboard();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            Assert.IsFalse(topNav.IsBusinessOutComeLinkDisplayed(), "On top nav, Business Outcome link is displayed");
        }

        //Enable Quick Launch For Team And Assessments
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnableQuickLaunch_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable Quick Launch For Team And Assessments' feature");
            manageFeaturesPage.TurnOnEnableQuickLaunchForTeamAndAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName} then verify quick launch button should be displayed");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            Assert.IsTrue(quickLaunchAssessmentPage.IsQuickLaunchButtonDisplayed(), "'Quick Launch' button is not displayed");

            Log.Info($"Edit {SharedConstants.RadarTeam} and verify that 'Invite team member link' should be displayed");
            var teamId = dashBoardPage.GetTeamIdFromLink(SharedConstants.RadarTeam);
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();
            Assert.IsTrue(editTeamTeamMemberPage.IsInviteTeamMemberLinkCopyIconDisplayed(), "Invite team member link is not displayed");

            Log.Info($"Go to assessment edit page of {SharedConstants.ProgramHealthRadar} and verify that 'Assessment Link' should be displayed");
            teamAssessmentDashboard.NavigateToPage(int.Parse(teamId));
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.ProgramHealthRadar, "Edit");
            Assert.IsTrue(teamAssessmentEditPage.IsAssessmentLinkCopyIconDisplayed(), "Assessment Link is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnableQuickLaunch_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable Quick Launch For Team And Assessments' feature");
            manageFeaturesPage.TurnOffEnableQuickLaunchForTeamAndAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            Assert.IsFalse(quickLaunchAssessmentPage.IsQuickLaunchButtonDisplayed(), "'Quick Launch' button is displayed");

            Log.Info($"Edit {SharedConstants.RadarTeam} and verify that 'Invite team member link' should not be displayed");
            var teamId = dashBoardPage.GetTeamIdFromLink(SharedConstants.RadarTeam);
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();
            Assert.IsFalse(editTeamTeamMemberPage.IsInviteTeamMemberLinkCopyIconDisplayed(), "Invite team member link is displayed");

            Log.Info($"Go to assessment edit page of {SharedConstants.ProgramHealthRadar} and verify that 'Assessment Link' should not be displayed");
            teamAssessmentDashboard.NavigateToPage(int.Parse(teamId));
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.ProgramHealthRadar, "Edit");
            Assert.IsFalse(teamAssessmentEditPage.IsAssessmentLinkCopyIconDisplayed(), "Assessment Link is displayed");
        }
    }
}
