using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesTeamsTests : BaseTest
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

        //Integrations
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_Integrations_On()
        {
            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Integrations' feature");
            manageFeaturesPage.TurnOnIntegrations();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that on Settings page, 'View Integration Settings' section should be displays");
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("View Integration Settings"), "On Settings page, 'Manage Integrations' section is not present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_Integrations_Off()
        {
            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Integrations' feature");
            manageFeaturesPage.TurnOffIntegrations();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that on Settings page, 'View Integration Settings' section should not be displayed");
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("View Integration Settings"), "On Settings page, 'Manage Integrations' section is present");
        }

        //MT/Enterprise Team
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MtEtTeam_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'MT/Enterprise Team' feature");
            manageFeaturesPage.TurnOnMtEtTeamFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.ClickAddATeamButton();

            Log.Info("Verify that 'Multi Team' & 'Enterprise Team' radio buttons should be displayed");
            Assert.IsTrue(dashBoardPage.AddTeam_DoesRadioButtonExist(TeamType.MultiTeam), "Add Team pop up doesn't have multi team radio button");
            Assert.IsTrue(dashBoardPage.AddTeam_DoesRadioButtonExist(TeamType.EnterpriseTeam), "Add Team pop up doesn't have enterprise team radio button");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_MtEtTeam_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'MT/Enterprise Team' feature");
            manageFeaturesPage.TurnOffMtEtTeamFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as  {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.ClickAddATeamButton();

            Assert.IsFalse(dashBoardPage.AddTeam_DoesRadioButtonExist(TeamType.MultiTeam), "Add Team pop up has multi team radio button");
            Assert.IsFalse(dashBoardPage.AddTeam_DoesRadioButtonExist(TeamType.EnterpriseTeam), "Add Team pop up has enterprise team radio button");

        }

        //Enable N-Tier Architecture
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_NTier_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable N-Tier Architecture' feature");
            manageFeaturesPage.TurnOnNTier();
            manageFeaturesPage.TurnOnMtEtTeamFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.ClickAddATeamButton();
            Assert.IsTrue(dashBoardPage.AddTeam_DoesRadioButtonExist(TeamType.NTier), "N-Tier feature is not turned on");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_NTier_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable N-Tier Architecture' feature");
            manageFeaturesPage.TurnOffNTier();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("verify that 'N-Tier' feature is off");
            Assert.IsFalse(dashBoardPage.AddTeam_DoesRadioButtonExist(TeamType.NTier), "N-Tier feature is not turned off");
        }

        //Benchmarking
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_Team_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Benchmarking' feature and turn on 'Team' sub-feature");
            manageFeaturesPage.TurnOnBenchmarking();
            manageFeaturesPage.TurnOnBenchmarkingSubFeature("Team");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);
            radarPage.RadarSwitchView(ViewType.Benchmarking);

            Log.Info("Verify that Benchmarking popup is displayed for team radar");
            Assert.IsTrue(radarPage.DoesBenchMarkingPopupDisplay(), "Benchmarking popup is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_Team_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Benchmarking' feature and turn off 'Team' sub-feature");
            manageFeaturesPage.TurnOnBenchmarking();
            manageFeaturesPage.TurnOffBenchmarkingSubFeature("Team");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);

            Log.Info("Verify that Benchmarking popup should not be displayed for team");
            Assert.IsFalse(radarPage.DoesBenchmarkingViewExist(), "Benchmarking option is exist for Team");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_MultiTeam_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var mtRadarPage = new MultiTeamRadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Benchmarking' feature and turn on 'Multi-Team' sub-feature");
            manageFeaturesPage.TurnOnBenchmarking();
            manageFeaturesPage.TurnOnBenchmarkingSubFeature("Multi-Team");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            mtRadarPage.NavigateToPage(multiTeam.TeamId, SharedConstants.TeamSurveyId);
            mtRadarPage.RadarSwitchView("Benchmarking");

            Log.Info("Verify that Benchmarking popup should be displayed for multi-team radar");
            Assert.IsTrue(radarPage.DoesBenchMarkingPopupDisplay(), "Benchmarking popup is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_MultiTeam_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var mtRadarPage = new MultiTeamRadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Benchmarking' feature and turn off 'Multi-Team' sub-feature");
            manageFeaturesPage.TurnOnBenchmarking();
            manageFeaturesPage.TurnOffBenchmarkingSubFeature("Multi-Team");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that Benchmarking popup should not be displayed for Multi-team");
            mtRadarPage.NavigateToPage(multiTeam.TeamId, SharedConstants.TeamSurveyId);
            Assert.IsFalse(radarPage.DoesBenchmarkingViewExist(), "Benchmarking option is exist for Multi-Team");

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_EnterpriseTeam_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var mtRadarPage = new MultiTeamRadarPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Benchmarking' feature and turn on 'Enterprise-Team' sub-feature");
            manageFeaturesPage.TurnOnBenchmarking();
            manageFeaturesPage.TurnOnBenchmarkingSubFeature("Enterprise Team");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            mtRadarPage.NavigateToPage(enterpriseTeam.TeamId, SharedConstants.TeamSurveyId, true);
            mtRadarPage.EtRadarSwitchView("Benchmarking");

            Log.Info("Verify that Benchmarking popup should be displayed for enterprise team radar");
            Assert.IsTrue(radarPage.DoesBenchMarkingPopupDisplay(), "Benchmarking popup is not displayed");

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_EnterpriseTeam_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var mtRadarPage = new MultiTeamRadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Benchmarking' feature and turn off 'Enterprise-Team' sub-feature");
            manageFeaturesPage.TurnOnBenchmarking();
            manageFeaturesPage.TurnOffBenchmarkingSubFeature("Enterprise Team");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that Benchmarking popup should not be displayed");
            mtRadarPage.NavigateToPage(enterpriseTeam.TeamId, SharedConstants.TeamSurveyId, true);
            Assert.IsFalse(radarPage.DoesBenchmarkingViewExist(), "Benchmarking option is exist for Enterprise Team");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Benchmarking_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var mtRadarPage = new MultiTeamRadarPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);
            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Benchmarking' feature");
            manageFeaturesPage.TurnOffBenchmarking();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);

            Log.Info("Verify that Benchmarking popup should not be displayed for team");
            Assert.IsFalse(radarPage.DoesBenchmarkingViewExist(), "Benchmarking option is exist for Team");

            Log.Info("Verify that Benchmarking popup should not be displayed for Multi-team");
            mtRadarPage.NavigateToPage(multiTeam.TeamId, SharedConstants.TeamSurveyId);
            Assert.IsFalse(radarPage.DoesBenchmarkingViewExist(), "Benchmarking option is exist for Multi-Team");

            Log.Info("Verify that Benchmarking popup should not be displayed for enterprise team");
            mtRadarPage.NavigateToPage(enterpriseTeam.TeamId, SharedConstants.TeamSurveyId, true);
            Assert.IsFalse(radarPage.DoesBenchmarkingViewExist(), "Benchmarking option is exist for Enterprise Team");
        }

        //Organizational Hierarchy
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_OrganizationalHierarchy_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Organizational Hierarchy' feature");
            manageFeaturesPage.TurnOnOrganizationalHierarchy();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            dashBoardPage.NavigateToPage(SettingsCompany.Id);

            const string orgStructureOption = "Org Structure";
            Log.Info($"On Dashboard, verify that View dropdown has {orgStructureOption} option");
            dashBoardPage.MoveToViewButton();
            Assert.IsTrue(dashBoardPage.DoesViewItemExist(orgStructureOption), $"View dropdown doesn't have {orgStructureOption} option");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_OrganizationalHierarchy_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Organizational Hierarchy' feature");
            manageFeaturesPage.TurnOffOrganizationalHierarchy();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            dashBoardPage.NavigateToPage(SettingsCompany.Id);

            const string orgStructureOption = "Org Structure";
            Log.Info($"On Dashboard, verify that View dropdown doesn't have {orgStructureOption} option");
            dashBoardPage.MoveToViewButton();
            Assert.IsFalse(dashBoardPage.DoesViewItemExist(orgStructureOption), $"View dropdown has {orgStructureOption} option");
        }

        //Strict External Identifier Configuration
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_StrictExternalIdentifier_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Strict External Identifier Configuration' feature");
            manageFeaturesPage.TurnOnStrictExternalIdentifier();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.CloseDeploymentPopup();

            Log.Info("Validate the ExternalIdentifier textbox has been replaced by a dropdown");
            createTeamPage.NavigateToPage(Company.Id.ToString());
            Assert.IsTrue(createTeamPage.IsExternalIdentifierListBoxVisible(), "The 'ExternalIdentifier' list box should be visible.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_StrictExternalIdentifier_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Strict External Identifier Configuration' feature");
            manageFeaturesPage.TurnOffStrictExternalIdentifier();
            manageFeaturesPage.ClickUpdateButton();
            topNavigation.LogOut();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.CloseDeploymentPopup();

            Log.Info("Validate the ExternalIdentifier textbox has been replaced by a dropdown");
            createTeamPage.NavigateToPage(Company.Id.ToString());
            Assert.IsTrue(createTeamPage.IsExternalIdentifierTextboxVisible(), "The 'ExternalIdentifier' textbox should NOT be displayed.");
            Assert.IsFalse(createTeamPage.IsExternalIdentifierListBoxVisible(), "The 'ExternalIdentifier' list box should be visible.");
        }

        //Disable Add From Directory
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_DisableAddFromDirectory_On()
        {
            var login = new LoginPage(Driver, Log);
            var member = new TeamMemberCommon(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);
            var user = User.IsCompanyAdmin() ? CompanyAdmin1 : User;

            Log.Info($"Login as {user.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(user.Username, user.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Disable Add From Directory' feature");
            manageFeaturesPage.TurnOnDisableAddFromDirectory();
            manageFeaturesPage.ClickUpdateButton();
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            member.NavigateToPage(team.TeamId);

            Log.Info("verify that 'Add From Directory' button isn't visible");
            Assert.IsFalse(member.IsAddFromDirectoryButtonPresent(), "'Add From Directory' button is visible");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_DisableAddFromDirectory_Off()
        {
            var login = new LoginPage(Driver, Log);
            var member = new TeamMemberCommon(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);
            var user = User.IsCompanyAdmin() ? CompanyAdmin1 : User;

            Log.Info($"Login as {user.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(user.Username, user.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Disable Add From Directory' feature");
            manageFeaturesPage.TurnOffDisableAddFromDirectory();
            manageFeaturesPage.ClickUpdateButton();
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            member.NavigateToPage(team.TeamId);

            Log.Info("verify that 'Add From Directory' button is visible");
            Assert.IsTrue(member.IsAddFromDirectoryButtonPresent(),
                "'Add From Directory' button isn't visible");
        }

        //Enable Bulk Data Management
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_BulkDataManagement_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable Bulk Data Management' features ");
            manageFeaturesPage.TurnOnEnableBulkDataManagement();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsTrue(dashBoardPage.IsBulkManagementButtonDisplayed(), "Bulk Management options is not displayed.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_BulkDataManagement_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable Bulk Data Management' features ");
            manageFeaturesPage.TurnOffEnableBulkDataManagement();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsFalse(dashBoardPage.IsBulkManagementButtonDisplayed(), "Bulk Management options is displayed.");
        }

        //Enable External Identifier Duplicate Checking
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableExternalIdentifierDuplicateChecking_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            const string expectedExternalIdentifierValidationText = "Duplicated External Identifier.";

            Log.Info($"Login as {SiteAdminUser.FullName} and turn off 'Strict External Identifier Configuration'");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffStrictExternalIdentifier();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Feature' page and Turn on 'Enable External Identifier Duplicate Checking' feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnEnableExternalIdentifierDuplicateChecking();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Create two teams with same external identifier then verify validation message");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();

            var teamInfo = new TeamInfo()
            {
                TeamName = RandomDataUtil.GetTeamName(),
                WorkType = SharedConstants.NewTeamWorkType,
                ExternalIdentifier = "ExternalIdentifier" + RandomDataUtil.GetCompanyZipCode()

            };
            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();

            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();

            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();

            Assert.AreEqual(expectedExternalIdentifierValidationText, createTeamPage.GetExternalIdentifierValidationMessage(), "'External Identifier' validation message is not matched");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableExternalIdentifierDuplicateChecking_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            Log.Info($"Login as {SiteAdminUser.FullName} and turn off 'Strict External Identifier Configuration'");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffStrictExternalIdentifier();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Feature' page and Turn off 'Enable External Identifier Duplicate Checking' feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffEnableExternalIdentifierDuplicateChecking();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Create a two team with same external identifier");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();

            var teamInfo = new TeamInfo()
            {
                TeamName = RandomDataUtil.GetTeamName(),
                WorkType = SharedConstants.NewTeamWorkType,
                ExternalIdentifier = "ExternalIdentifier" + RandomDataUtil.GetCompanyZipCode()

            };
            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();

            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();

            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();

            Assert.IsTrue(addTeamMemberPage.IsAddTeamMemberPageDisplayed(), "'Add Team Member' page is not displayed");
        }

        //Display Team AgilityHealth Index 
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_DisplayTeamAgilityHealthIndex_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Display Team AgilityHealth Index' feature");
            manageFeaturesPage.TurnOnDisplayTeamAgilityHealthIndex();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to Team Dashboard and Verify 'Team Agility Index' should be present");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.GridTeamView();

            var allColumnNames = dashBoardPage.GetAllColumnNames();
            Assert.That.ListContains(allColumnNames, "AH Index", "'AH Index' column is not present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_DisplayTeamAgilityHealthIndex_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Display Team AgilityHealth Index' feature");
            manageFeaturesPage.TurnOffDisplayTeamAgilityHealthIndex();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to Team Dashboard and Verify 'Team Agility Index' should not be present");
            dashBoardPage.NavigateToPage(Company.Id);
            dashBoardPage.GridTeamView();

            var allColumnNames = dashBoardPage.GetAllColumnNames();
            Assert.That.ListNotContains(allColumnNames, "AH Index", "'AH Index' column is present");
        }

    }
}
