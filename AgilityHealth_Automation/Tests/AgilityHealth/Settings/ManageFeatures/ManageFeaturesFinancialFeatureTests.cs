using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesFinancialFeatureTests : BaseTest
    {
        private static User CompanyAdmin1 => TestEnvironment.UserConfig.GetUserByDescription("user 3");
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");
        private static AtCommon.Dtos.Company SettingsCompany =>
            SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);
        private static CompanyHierarchyResponse _allTeamsList;
        private static SetupTeardownApi _setup;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _setup = new SetupTeardownApi(TestEnvironment);
            _allTeamsList = _setup.GetCompanyHierarchy(SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, SiteAdminUser);
        }

        //Financial Features
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDetail = new AssessmentDetailsCommonPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Metrics Features', 'Financial Features' and turn on it's sub feature");
            manageFeaturesPage.TurnOnMetricsFeature();
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOnSubFeature("Financial Settings");
            manageFeaturesPage.TurnOnSubFeature("Team Member Costs");
            manageFeaturesPage.TurnOnSubFeature("Cost Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Go to 'Setting' page and Verify that 'Manage Financials' section should be present");
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("View Financials"), "'Manage Financials' section is not present");

            /*Log.Info("Navigate to the Team and add TeamMember then Verify that 'Employment Type' section should be present");
            teamMemberPage.NavigateToPage(team.TeamId);
            teamMemberPage.ClickAddTeamMemberButton();
            Assert.IsTrue(teamMemberPage.IsEmploymentTypeSectionPresent(), "Team > Edit > Team Members page > 'Employment Type' section is not present");*/

            Log.Info("Navigate to the radar page and Verify that 'Cost Chart' section should be present");
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsTrue(assessmentDetail.MetricsSummary_IsTabPresent("Cost Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Cost Chart' tab is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDetail = new AssessmentDetailsCommonPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Financial Features'");
            manageFeaturesPage.TurnOffFinancialFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Go to 'Setting' page and Verify that 'Manage Financials' section should not be present");
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("View Financials"), "'Manage Financials' section is present");

            /*Log.Info("Navigate to the Team and add TeamMember then Verify that 'Employment Type' section should not be present");
            teamMemberPage.NavigateToPage(team.TeamId);
            teamMemberPage.ClickAddTeamMemberButton();
            Assert.IsFalse(teamMemberPage.IsAllocationTypeSectionPresent, "Team > Edit > Team Members page > 'Employment Type' section is present");*/

            Log.Info("Navigate to the radar page and Verify that 'Cost Chart' section should not be present");
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsFalse(assessmentDetail.MetricsSummary_IsTabPresent("Cost Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Cost Chart' tab is present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_FinancialSettings_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Financial Features' and turn on 'Financial Settings' sub feature");
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOnSubFeature("Financial Settings");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("On Settings page, verify that 'Manage Financials' section should be present");
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("View Financials"), "'Manage Financials' section is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_FinancialSettings_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Financial Features' and turn off 'Financial Settings' sub feature");
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOffSubFeature("Financial Settings");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("On Settings page, verify that 'Manage Financials' section should not be present");
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("View Financials"), "'Manage Financials' section is present");
        }

        [TestCategory("KnownDefect")] //Bug id :34121
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_TeamMemberCosts_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamMemberPage = new AddTeamMemberPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.")
                .GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Financial Features' and turn on 'Team Member Costs' sub feature");
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOnSubFeature("Team Member Costs");
            manageFeaturesPage.TurnOffSubFeature("Financial Settings");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the Team and add TeamMember then Verify that 'Employment Type' section should be present");
            teamMemberPage.NavigateToPage(team.TeamId);
            teamMemberPage.ClickAddNewTeamMemberButton();
            Assert.IsTrue(teamMemberPage.IsAllocationTypeSectionPresent(), "Team > Edit > Team Members page > 'Allocation Type' section is not present");
        }

        [TestMethod]
        [TestCategory("ManageFeatures"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_TeamMemberCosts_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamMemberPage = new AddTeamMemberPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.")
                .GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Financial Features' and turn off 'Team Member Costs' sub feature");
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOffSubFeature("Team Member Costs");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the Team and add TeamMember then Verify that 'Employment Type' section should not be present");
            teamMemberPage.NavigateToPage(team.TeamId);
            teamMemberPage.ClickAddNewTeamMemberButton();
            Assert.IsFalse(teamMemberPage.IsAllocationTypeSectionPresent(), "Team > Edit > Team Members page > 'Allocation Type' section is present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_CostChart_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDetail = new AssessmentDetailsCommonPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Financial Features' and turn on 'Cost Chart' sub feature");
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOnSubFeature("Cost Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the radar page and Verify that 'Cost Chart' section should be present");
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsTrue(assessmentDetail.MetricsSummary_IsTabPresent("Cost Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Cost Chart' tab is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FinancialFeatures_CostChart_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDetail = new AssessmentDetailsCommonPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Financial Features' and turn off 'Cost Chart' sub feature");
            manageFeaturesPage.TurnOnFinancialFeature();
            manageFeaturesPage.TurnOffSubFeature("Cost Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the radar page and Verify that 'Cost Chart' section should not be present");
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsFalse(assessmentDetail.MetricsSummary_IsTabPresent("Cost Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Cost Chart' tab is present");
        }
    }
}
