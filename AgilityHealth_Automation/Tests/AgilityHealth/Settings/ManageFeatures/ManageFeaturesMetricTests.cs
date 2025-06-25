using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesMetricTests : BaseTest
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

        //Metrics Features
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MetricsFeatures_On()
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

            Log.Info("Turn on 'Metrics Features' and it's sub feature");
            manageFeaturesPage.TurnOnMetricsFeature();
            manageFeaturesPage.TurnOnSubFeature("Burn Up Chart");
            manageFeaturesPage.TurnOnSubFeature("Release Health Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and Login as {CompanyAdmin1.FullName} then Verify that 'Metrics Summery section' should be present");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsTrue(assessmentDetail.MetricsSummary_IsTabPresent("Burn Up Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Burn Up Chart' tab is not present");
            Assert.IsTrue(assessmentDetail.MetricsSummary_IsTabPresent("Release Health Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Release Health Chart' tab is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MetricsFeatures_Off()
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

            Log.Info("Turn off 'Metrics Features'");
            manageFeaturesPage.TurnOffMetricsFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and Login as {CompanyAdmin1.FullName} then Verify that 'Metrics Summery section' should not be present");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsFalse(assessmentDetail.MetricsSummary_IsTabPresent("Burn Up Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Burn Up Chart' tab is present");
            Assert.IsFalse(assessmentDetail.MetricsSummary_IsTabPresent("Release Health Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Release Health Chart' tabs is present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MetricsFeatures_BurnUpChart_On()
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

            Log.Info("Turn on 'Metrics Features' and turn on 'Burn Up Chart' sub feature");
            manageFeaturesPage.TurnOnMetricsFeature();
            manageFeaturesPage.TurnOnSubFeature("Burn Up Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and Login as {CompanyAdmin1.FullName} then Verify that 'Burn Up Chart' tab should be present");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsTrue(assessmentDetail.MetricsSummary_IsTabPresent("Burn Up Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Burn Up Chart' tab is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MetricsFeatures_BurnUpChart_Off()
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

            Log.Info("Turn on 'Metrics Features' and turn off 'Burn Up Chart' sub feature");
            manageFeaturesPage.TurnOnMetricsFeature();
            manageFeaturesPage.TurnOffSubFeature("Burn Up Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and Login as {CompanyAdmin1.FullName} then Verify that 'Burn Up Chart' tab should not be present");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsFalse(assessmentDetail.MetricsSummary_IsTabPresent("Burn Up Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Burn Up Chart' tab is present");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MetricsFeatures_ReleaseHealthChart_On()
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

            Log.Info("Turn on 'Metrics Features' and turn on 'Release Health Chart' sub feature");
            manageFeaturesPage.TurnOnMetricsFeature();
            manageFeaturesPage.TurnOnSubFeature("Release Health Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and Login as {CompanyAdmin1.FullName} then Verify that 'Release Health Char' tab should be present");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsTrue(assessmentDetail.MetricsSummary_IsTabPresent("Release Health Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Release Health Chart' tab is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MetricsFeatures_ReleaseHealthChart_Off()
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

            Log.Info("Turn on 'Metrics Features' and turn off 'Release Health Chart' sub feature");
            manageFeaturesPage.TurnOnMetricsFeature();
            manageFeaturesPage.TurnOffSubFeature("Release Health Chart");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and Login as {CompanyAdmin1.FullName} then Verify that 'Release Health Char' tab should not be present");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.NavigateToAssessmentRadarPageByIndex(0);
            Assert.IsFalse(assessmentDetail.MetricsSummary_IsTabPresent("Release Health Chart"), "Team > Assessment > Radar > Metrics Summery section > 'Release Health Chart' tab is present");
        }
    }
}
