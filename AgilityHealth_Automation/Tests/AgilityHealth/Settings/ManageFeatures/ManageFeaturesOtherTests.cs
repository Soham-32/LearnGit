using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Reports;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesOtherTests : BaseTest
    {
        private static User CompanyAdmin1 => TestEnvironment.UserConfig.GetUserByDescription("user 3");
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");
        private static User OrganizationalLeaderUser => SiteAdminUserConfig.GetUserByDescription("org leader 3");
        private static User BusinessLineAdminUser => SiteAdminUserConfig.GetUserByDescription("business line admin 3");
        private static User TeamAdminUser => SiteAdminUserConfig.GetUserByDescription("team admin 3");
        private static User PartnerAdminUser => SiteAdminUserConfig.GetUserByDescription("partner admin");
        private static User CompanyAdmin => SiteAdminUserConfig.GetUserByDescription("company admin");
        private static User Member => SiteAdminUserConfig.GetUserByDescription("member 6");
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

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_All_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info($"Login as {CompanyAdmin1.Username} and Go to the 'Manage Feature' page then Turn off all feature");
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffAllFeatures();
            manageFeaturesPage.ClickUpdateButton();
            Driver.RefreshPage();

            Log.Info("Verify that all features should be turned off properly");
            var toggles = manageFeaturesPage.GetToggleIdsAndStatuses();
            foreach (var toggle in toggles)
            {
                Assert.AreEqual(manageFeaturesPage.OffFeatureToggleBackgroundColor, toggle.Value,
                    $"The expected background-color for toggle <{toggle.Key}> doesn't match.");
            }

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_All_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info($"Login as {CompanyAdmin1.Username} and Go to the 'Manage Feature' page then Turn on all feature");
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnAllFeatures();
            manageFeaturesPage.ClickUpdateButton();
            Driver.RefreshPage();

            Log.Info("Verify that all features should be turned on properly");
            var toggles = manageFeaturesPage.GetToggleIdsAndStatuses();
            foreach (var toggle in toggles)
            {
                Assert.AreEqual(manageFeaturesPage.OnFeatureToggleBackgroundColor, toggle.Value,
                    $"The expected background-color for toggle <{toggle.Key}> doesn't match.");
            }

        }

        //Campaigns
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        public void ManageFeatures_Campaigns_On()
        {

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);
            var growthJourney = new GrowthJourneyPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Campaigns' and turn on 'Team Agility Dashboard'");
            manageFeaturesPage.TurnOnCampaigns();
            manageFeaturesPage.TurnOnTeamAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that on Settings page, 'Manage Campaigns' section should be displays");
            topNavigation.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("View Campaigns"), "On Settings page, 'Manage Campaigns' section is not present");

            Log.Info("Verify that Campaign field should be displayed on Assessment Profile page");
            assessmentProfile.NavigateToPage(team.TeamId);
            Assert.IsTrue(assessmentProfile.DoesCampaignFieldDisplay(), "Campaign field is not displayed on Assessment Profile page");

            Log.Info("Verify that Campaign dropdown should be displayed in Team Agility Tab");
            teamAgilityPage.NavigateToPage(SettingsCompany.Id);
            teamAgilityPage.SwitchToViewByCampaigns();
            Assert.IsTrue(teamAgilityPage.DoesViewByCampaignsDisplay(), "'View By Campaigns' is not displayed");
            Assert.IsTrue(teamAgilityPage.DoesCampaignsDropdownDisplay(), "Campaigns dropdown is not displayed");

            Log.Info("Verify that Campaign option should be displayed in compare list on growth journey page");
            var setup = new SetupTeardownApi(TestEnvironment);
            var multiTeam = setup.GetCompanyHierarchy(SettingsCompany.Id, CompanyAdmin1).GetTeamByName(Constants.MultiTeamForGrowthJourney).CheckForNull($"<{Constants.MultiTeamForGrowthJourney}> was not found in the response.");
            var radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType) ?? throw new ArgumentNullException($"setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType)");
            radarPage.NavigateToGrowthJourney(multiTeam.TeamId, radar.Id, TeamType.MultiTeam);
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(growthJourney.DoesCompareTypeDisplay("Campaigns"), "Campaigns type is not displayed in Compare list on growth journey page");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        public void ManageFeatures_Campaigns_Off()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);
            var growthJourney = new GrowthJourneyPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Campaigns' and turn on 'Team Agility Dashboard'");
            manageFeaturesPage.TurnOffCampaigns();
            manageFeaturesPage.TurnOnTeamAgilityDashboard();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that on Settings page, 'Manage Campaigns' section should not be displays");
            topNav.ClickOnSettingsLink();
            Log.Info("Verify that Manage Campaigns shouldn't display on Settings page");
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("View Campaigns"), "On Settings page, 'Manage Campaigns' section is present");

            Log.Info("Verify that Campaign field should not be displayed on Assessment Profile page");
            assessmentProfile.NavigateToPage(team.TeamId);
            Log.Info("Verify that Campaign field shouldn't display in Assessment Profile screen");
            Assert.IsFalse(assessmentProfile.DoesCampaignFieldDisplay(), "Campaign field is displayed on Assessment Profile page");

            Log.Info("Verify that Campaign dropdown should not be displayed in Team Agility Tab");
            teamAgilityPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsFalse(teamAgilityPage.DoesViewByCampaignsDisplay(), "View By Campaigns is be displayed");
            Assert.IsFalse(teamAgilityPage.DoesViewByQuartersDisplay(), "View By Quarters is be displayed");

            Log.Info("Verify that Campaign option should not be displayed in compare list on growth journey page");
            var setup = new SetupTeardownApi(TestEnvironment);
            var multiTeam = setup.GetCompanyHierarchy(SettingsCompany.Id, CompanyAdmin1).GetTeamByName(Constants.MultiTeamForGrowthJourney).CheckForNull($"<{Constants.MultiTeamForGrowthJourney}> was not found in the response.");
            var radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
            radarPage.NavigateToGrowthJourney(multiTeam.TeamId, radar.Id, TeamType.MultiTeam);
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsFalse(growthJourney.DoesCompareTypeDisplay("Campaigns"), "Campaigns type is displayed in Compare list");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        public void ManageFeatures_Campaigns_On_RequireCampaignOnAssessmentCreateEdit_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            
            const string expectedValidationMessageOnAssessmentProfilePage = "The Campaign field is required.";
            const string expectedValidationMessageOnAssessmentEditPage = "Campaign is required";

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Campaigns' and turn on 'Require campaign on Assessment Create/Edit' sub-feature");
            manageFeaturesPage.TurnOnCampaigns();
            manageFeaturesPage.TurnOnSubFeature("Require campaign on Assessment Create/Edit");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the 'Assessment Profile' page and Verify that Campaign field is mandatory");
            assessmentProfile.NavigateToPage(radarTeam.TeamId);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            Assert.AreEqual(expectedValidationMessageOnAssessmentProfilePage, assessmentProfile.GetAssessmentCampaignFieldValidationMessage(), "Validation message is not matched for Campaign field on Assessment Profile page");

            const string assessmentName = SharedConstants.ProgramHealthRadar;
            Log.Info($"Navigate to the edit {assessmentName} team assessment page and Verify that Campaign field is mandatory");
            teamAssessmentDashboardPage.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(assessmentName, "Edit");
            teamAssessmentEditPage.ClickEditDetailButton();
            var editedAssessment = new TeamAssessmentInfo
            {
                Campaign = "(None)"
            };
            teamAssessmentEditPage.FillDataForAssessmentProfile(editedAssessment);
            teamAssessmentEditPage.EditPopup_ClickUpdateButton();
            Assert.AreEqual(expectedValidationMessageOnAssessmentEditPage, assessmentProfile.GetAssessmentCampaignFieldValidationMessage(), "Validation message is not matched for Campaign field on Assessment Edit page");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        public void ManageFeatures_Campaigns_On_RequireCampaignOnAssessmentCreateEdit_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Campaigns' and turn off 'Require campaign on Assessment Create/Edit' sub-feature");
            manageFeaturesPage.TurnOnCampaigns();
            manageFeaturesPage.TurnOffSubFeature("Require campaign on Assessment Create/Edit");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the 'Assessment Profile' page and Verify that Campaign field is not mandatory");
            var assessmentName = RandomDataUtil.GetAssessmentName();
            assessmentProfile.NavigateToPage(team.TeamId);
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();      
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/teammembers"), "Unable to create assessment without filled campaign field");

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            Log.Info($"Navigate to the edit {assessmentName} team assessment page and Verify that Campaign field is not mandatory");
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");
            teamAssessmentEditPage.ClickEditDetailButton();
            var editedAssessment = new TeamAssessmentInfo
            {
                AssessmentName = $"Updated_Test{RandomDataUtil.GetAssessmentName()}",
                Campaign = "(None)"
            };
            teamAssessmentEditPage.FillDataForAssessmentProfile(editedAssessment);
            teamAssessmentEditPage.EditPopup_ClickUpdateButton();
            Assert.IsFalse(teamAssessmentEditPage.IsEditAssessmentDetailsPopupDisplayed(), "'Edit Assessment Details' popup is displayed");
        }

        //Zendesk Integrations
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_ZendeskIntegration_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Zendesk Integrations' feature");
            manageFeaturesPage.TurnOnZendeskIntegrationFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Assert.IsTrue(topNav.DoesSupportCenterDashboardLinkDisplay(), "'Support Center' link is not present");
            Assert.IsTrue(dashBoardPage.IsResourceCenterButtonDisplayed(), "'Resouce Center' Button button is not displayed");
            Assert.IsTrue(dashBoardPage.IsZenDeskLinkDisplayed(), "'ZenDesk' link is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_ZendeskIntegration_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Zendesk Integrations' feature");
            manageFeaturesPage.TurnOffZendeskIntegrationFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Assert.IsFalse(dashBoardPage.IsResourceCenterButtonDisplayed(), "'Resouce Center' Button button didn't display");
            Assert.IsTrue(topNav.DoesSupportCenterDashboardLinkDisplay(), "'Support Center' link is not present");
        }

        //Reports 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Reports_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var reports = new ReportsDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var expected = $"{User.CompanyName} Reports Dashboard";
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Reports' feature and it's subfeatures");
            manageFeaturesPage.TurnOnReportsFeature();
            manageFeaturesPage.Reports_TurnOnSubFeature("Organizational Leader");
            manageFeaturesPage.Reports_TurnOnSubFeature("Business Line Admin");
            manageFeaturesPage.Reports_TurnOnSubFeature("Team Admin");
            manageFeaturesPage.ClickUpdateButton();
            topNavigation.LogOut();

            Log.Info($"Logout as {User.FullName} and Login as CA - {CompanyAdmin1.FullName} ");
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that 'Report' link is present and 'Reports' page title ");
            Assert.IsTrue(topNavigation.IsReportsLinkPresent(), "Report link is not present");

            topNavigation.ClickOnReportsLink();
            var actualTitleCa = reports.GetTitleText();
            Assert.AreEqual(expected, actualTitleCa, $"Reports Dashboard Title doesn't match for {actualTitleCa}");

            Log.Info($"Logout as a CA - {CompanyAdmin1.FullName} and Login as OL - {OrganizationalLeaderUser.FullName} ");
            topNavigation.LogOut();
            login.LoginToApplication(OrganizationalLeaderUser.Username, OrganizationalLeaderUser.Password);

            Log.Info("Verify that 'Report' link is present and 'Reports' page title ");
            Assert.IsTrue(topNavigation.IsReportsLinkPresent(), "Report link is not present");

            topNavigation.ClickOnReportsLink();
            var actualTitleOl = reports.GetTitleText();
            Assert.AreEqual(expected, actualTitleOl, $"Reports Dashboard Title doesn't match for {actualTitleOl}");

            Log.Info($"Logout as a OL - {OrganizationalLeaderUser.FullName} and Login as BL - {BusinessLineAdminUser.FullName} ");
            topNavigation.LogOut();
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);

            Log.Info("Verify that 'Report' link is present and 'Reports' page title ");
            Assert.IsTrue(topNavigation.IsReportsLinkPresent(), "Report link is not present");

            topNavigation.ClickOnReportsLink();
            var actualTitleBl = reports.GetTitleText();
            Assert.AreEqual(expected, actualTitleBl, $"Reports Dashboard Title doesn't match for {actualTitleBl}");

            Log.Info($"Logout as a BL - {BusinessLineAdminUser.FullName} and Login as TA - {TeamAdminUser.FullName} ");
            topNavigation.LogOut();
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);

            Log.Info("Verify that 'Report' link is present and 'Reports' page title ");
            Assert.IsTrue(topNavigation.IsReportsLinkPresent(), "Report link is not present");

            topNavigation.ClickOnReportsLink();
            var actualTitleTa = reports.GetTitleText();
            Assert.AreEqual(expected, actualTitleTa, $"Reports Dashboard Title doesn't match for {actualTitleTa}");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_Reports_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Reports' feature");
            manageFeaturesPage.TurnOffReportsFeature();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as CA - {CompanyAdmin1.FullName} then verify 'Reports' link is not present ");
            topNavigation.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            Assert.IsFalse(topNavigation.IsReportsLinkPresent(), "Report link is still present");

            Log.Info($"Logout as a CA - {CompanyAdmin1.FullName} and Login as OL - {OrganizationalLeaderUser.FullName} then verify 'Reports' link is not present ");
            topNavigation.LogOut();
            login.LoginToApplication(OrganizationalLeaderUser.Username, OrganizationalLeaderUser.Password);
            Assert.IsFalse(topNavigation.IsReportsLinkPresent(), "Report link is still present");

            Log.Info($"Logout as a OL - {OrganizationalLeaderUser.FullName} and Login as BL - {BusinessLineAdminUser.FullName} then verify 'Reports' link is not present ");
            topNavigation.LogOut();
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);
            Assert.IsFalse(topNavigation.IsReportsLinkPresent(), "Report link is still present");

            Log.Info($"Logout as a BL - {BusinessLineAdminUser.FullName} and Login as TA - {TeamAdminUser.FullName} then verify 'Reports' link is not present ");
            topNavigation.LogOut();
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);
            Assert.IsFalse(topNavigation.IsReportsLinkPresent(), "Report link is still present");

        }

        //Disable User Impersonation
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_DisableUserImpersonation_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Feature' page and Turn on 'Disable User Impersonation' feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnDisableUserImpersonation();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} then Verify 'Impersonation' button for OL,BL, TA and Individual tabs");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            manageUserPage.NavigateToPage(SettingsCompany.Id);
            manageUserPage.SelectTab();
            Assert.IsFalse(manageUserPage.IsImpersonateButtonPresent(OrganizationalLeaderUser.Username), "'Impersonation' button is present at 'Organizational Leader' tab");

            manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);
            manageUserPage.SelectTab();
            Assert.IsFalse(manageUserPage.IsImpersonateButtonPresent(BusinessLineAdminUser.Username), "'Impersonation' button is present at 'Business Line Admin' tab");

            manageUserPage = new ManageUserPage(Driver, Log, UserType.TeamAdmin);
            manageUserPage.SelectTab();
            Assert.IsFalse(manageUserPage.IsImpersonateButtonPresent(TeamAdminUser.Username), "'Impersonation' button is present at 'Team Admin' tab");

            manageUserPage = new ManageUserPage(Driver, Log, UserType.Member);
            manageUserPage.SelectTab();
            Assert.IsFalse(manageUserPage.IsImpersonateButtonPresent(Member.Username), "'Impersonation' button is present at 'Individual' tab");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_DisableUserImpersonation_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Feature' page and Turn off 'Disable User Impersonation' feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffDisableUserImpersonation();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} then Verify 'Impersonation' button for OL,BL, TA and Individual tabs");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            var manageUserPage = new ManageUserPage(Driver, Log, UserType.OrganizationalLeader);
            manageUserPage.NavigateToPage(SettingsCompany.Id);
            manageUserPage.SelectTab();
            Assert.IsTrue(manageUserPage.IsImpersonateButtonPresent(OrganizationalLeaderUser.Username), "'Impersonation' button is not present at 'Organizational Leader' tab");

            manageUserPage = new ManageUserPage(Driver, Log, UserType.BusinessLineAdmin);
            manageUserPage.SelectTab();
            Assert.IsTrue(manageUserPage.IsImpersonateButtonPresent(BusinessLineAdminUser.Username), "'Impersonation' button is not present at 'Business Line Admin' tab");

            manageUserPage = new ManageUserPage(Driver, Log, UserType.TeamAdmin);
            manageUserPage.SelectTab();
            Assert.IsTrue(manageUserPage.IsImpersonateButtonPresent(TeamAdminUser.Username), "'Impersonation' button is not present at 'Team Admin' tab");

            manageUserPage = new ManageUserPage(Driver, Log, UserType.Member);
            manageUserPage.SelectTab();
            Assert.IsTrue(manageUserPage.IsImpersonateButtonPresent(Member.Username), "'Impersonation' button is not present at 'Individual' tab");
        }

        //Disable File Upload
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_DisableFileUpload_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            Log.Info($"Login as {SiteAdminUser.FullName} and turn on 'MT/ET Team' feature");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnMtEtTeamFeature();
            manageFeaturesPage.ClickUpdateButton();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Log out as {SiteAdminUser.FullName} and login as {User.FullName} then navigate to the 'Manage Feature' page");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Disable File Upload' feature");
            manageFeaturesPage.TurnOnDisableFileUpload();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Logout as site admin and Login as company admin then Verify that 'Upload Photo' field should not be displayed while creating a team");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            // Team Verification
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while creating a team");

            Log.Info($"Edit the '{SharedConstants.Team}' and Verify that 'Upload photo' field should not be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.Team);
            dashBoardPage.ClickTeamEditButton(SharedConstants.Team);
            Assert.IsFalse(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while editing a team");

            //MultiTeam Verification
            Log.Info("Add multi-team Verify that 'Upload Photo' field should not be displayed while creating a multi-team");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.MultiTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while creating a multi-team");

            Log.Info($"Edit the '{SharedConstants.MultiTeam}' and Verify that 'Upload photo' field should not be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.MultiTeam);
            dashBoardPage.ClickTeamEditButton(SharedConstants.MultiTeam);
            Assert.IsFalse(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while Editing a multi-team");

            //Enterprise Team Verification
            Log.Info("Add enterprise team Verify that 'Upload Photo' field should not be displayed while creating a enterprise team");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.EnterpriseTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while creating a enterprise team");

            Log.Info($"Edit the '{SharedConstants.EnterpriseTeam}' and Verify that 'Upload photo' field should not be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.EnterpriseTeam);
            dashBoardPage.ClickTeamEditButton(SharedConstants.EnterpriseTeam);
            Assert.IsFalse(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while Editing a enterprise team");

            // My Profile
            Log.Info("Go to the 'My Profile' page and Verify that 'Upload Photo' field should not be displayed");
            topNav.HoverOnNameRoleSection();
            topNav.ClickOnMyProfile();
            Assert.IsFalse(accountSettingsPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed at 'My Profile' page");

            // User Verification
            Log.Info("Go to the 'Manage User' and Verify that 'Upload Photo' field should not be displayed while creating a user");
            manageUserPage.NavigateToPage(SettingsCompany.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnAddNewUserButton();
            Assert.IsFalse(manageUserPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while creating a user");

            Log.Info($"Edit the {CompanyAdmin1.FullName} and Verify that 'Upload Photo' field should not be displayed");
            manageUserPage.ClickCancelButton();
            manageUserPage.ClickOnEditUserIcon(CompanyAdmin1.Username);
            Assert.IsFalse(manageUserPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is displayed while editing a user");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_DisableFileUpload_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            Log.Info($"Login as {SiteAdminUser.FullName} and turn on 'MT/ET Team' feature");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnMtEtTeamFeature();
            manageFeaturesPage.ClickUpdateButton();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Log out as {SiteAdminUser.FullName} and login as {User.FullName} then navigate to the 'Manage Feature' page");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Disable File Upload' feature");
            manageFeaturesPage.TurnOffDisableFileUpload();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Logout as site admin and Login as company admin then Verify that 'Upload Photo' field should be displayed while creating a team");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            // Team Verification
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while creating a team");

            Log.Info($"Edit the '{SharedConstants.Team}' and Verify that 'Upload photo' field should be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.Team);
            dashBoardPage.ClickTeamEditButton(SharedConstants.Team);
            Assert.IsTrue(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while editing a team");

            //MultiTeam Verification
            Log.Info("Add multi-team Verify that 'Upload Photo' field should be displayed while creating a multi-team");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.MultiTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while creating a multi-team");

            Log.Info($"Edit the '{SharedConstants.MultiTeam}' and Verify that 'Upload photo' field should be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.MultiTeam);
            dashBoardPage.ClickTeamEditButton(SharedConstants.MultiTeam);
            Assert.IsTrue(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while Editing a multi-team");

            //Enterprise Team Verification
            Log.Info("Add enterprise team Verify that 'Upload Photo' field should be displayed while creating a enterprise team");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.EnterpriseTeam);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while creating a enterprise team");

            Log.Info($"Edit the '{SharedConstants.EnterpriseTeam}' and Verify that 'Upload photo' field should be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.EnterpriseTeam);
            dashBoardPage.ClickTeamEditButton(SharedConstants.EnterpriseTeam);
            Assert.IsTrue(createTeamPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while Editing a enterprise team");

            // My Profile
            Log.Info("Go to the 'My Profile' page and Verify that 'Upload Photo' field should be displayed");
            topNav.HoverOnNameRoleSection();
            topNav.ClickOnMyProfile();
            Assert.IsTrue(accountSettingsPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed at 'My Profile' page");

            // User Verification
            Log.Info("Go to the 'Manage User' and Verify that 'Upload Photo' field should be displayed while creating a user");
            manageUserPage.NavigateToPage(SettingsCompany.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnAddNewUserButton();
            Assert.IsTrue(manageUserPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while creating a user");

            Log.Info($"Edit the {CompanyAdmin1.FullName} and Verify that 'Upload Photo' field should be displayed");
            manageUserPage.ClickCancelButton();
            manageUserPage.ClickOnEditUserIcon(CompanyAdmin1.Username);
            Assert.IsTrue(manageUserPage.IsUploadPhotoFieldDisplayed(), "'Upload Photo' field is not displayed while editing a user");
        }

        //Enable External Links
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableExternalLink_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable External Links' features ");
            manageFeaturesPage.TurnOnEnableExternalLinks();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} then verify 'Manage External Link'");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("View Links"), "On Settings page, 'Manage External Link' is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableExternalLink_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable External Links' features ");
            manageFeaturesPage.TurnOffEnableExternalLinks();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} then verify 'Manage External Link'");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("View Links"), "On Settings page, 'Manage External Link' is present");
        }

        //Allow Users To Create OAuth App Registrations For API Access.
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_OauthAppRegistrations_ON_LimitToCompanyAdmins_ON()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            const string viewApps = "View Apps";

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} and go to 'Setting page' and set 'Oauth AppRegistration' is 'ON' and set 'LimitToCompanyAdmin' is 'ON' and verify {viewApps} isn't present and logout as a {SiteAdminUser.FullName} SA ");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnOauth();
            manageFeaturesPage.TurnOnSubFeature("Limit to Company Admins");
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            Log.Info($"Login as {CompanyAdmin1.FullName} CA and Verify '{viewApps}' is present and logout as a {CompanyAdmin1.FullName} CA");
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{CompanyAdmin1.FullName} CA, On Settings page, '{viewApps}' section is not present");
            topNav.LogOut();

            Log.Info($"Login as {PartnerAdminUser.FullName} PA and Verify '{viewApps}' isn't present and logout as a {PartnerAdminUser.FullName} PA");
            login.LoginToApplication(PartnerAdminUser.Username, PartnerAdminUser.Password);
            settingsPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{PartnerAdminUser.FullName} PA, On Settings page, '{viewApps}' section is present");
            topNav.LogOut();

            Log.Info($"Login as {BusinessLineAdminUser.FullName} BL and Verify '{viewApps}' isn't present and logout as a {BusinessLineAdminUser.FullName} BL");
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{BusinessLineAdminUser.FullName} BL, On Settings page, '{viewApps}' section is present");
            topNav.LogOut();

            Log.Info($"Login as {OrganizationalLeaderUser.FullName} OL and Verify '{viewApps}' isn't present and logout as a {OrganizationalLeaderUser.FullName} OL");
            login.LoginToApplication(OrganizationalLeaderUser.Username, OrganizationalLeaderUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{OrganizationalLeaderUser.FullName} OL, On Settings page, '{viewApps}' section is present");
            topNav.LogOut();

            Log.Info($"Login as {TeamAdminUser.FullName} TA and Verify '{viewApps}' isn't present and logout as a {TeamAdminUser.FullName} TA");
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{TeamAdminUser.FullName} TA, On Settings page, '{viewApps}' section is present");
            topNav.LogOut();

            Log.Info($"Login as {Member.FullName} M and Verify '{viewApps}' isn't present and logout as a {Member.FullName} M");
            login.LoginToApplication(Member.Username, Member.Password);
            settingsPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{Member.FullName} M, On Settings page, '{viewApps}' section is present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_OauthAppRegistrations_ON_LimitToCompanyAdmins_OFF()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            const string viewApps = "View Apps";

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} and go to 'Setting page' and set 'Oauth AppRegistration' is 'ON' and set 'LimitToCompanyAdmin' is 'OFF' and verify {viewApps} isn't present and logout as a {SiteAdminUser.FullName} SA ");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnOauth();
            manageFeaturesPage.TurnOffSubFeature("Limit to Company Admins");
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            Log.Info($"Login as {PartnerAdminUser.FullName} PA and Verify '{viewApps}' isn't present and logout as a {PartnerAdminUser.FullName} PA");
            login.LoginToApplication(PartnerAdminUser.Username, PartnerAdminUser.Password);
            settingsPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{PartnerAdminUser.FullName} PA, On Settings page, '{viewApps}' section is present");
            topNav.LogOut();

            Log.Info($"Login as {CompanyAdmin1.FullName} CFA and Verify '{viewApps}' is present and logout as a {CompanyAdmin1.FullName} CFA");
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{CompanyAdmin1.FullName} CFA, On Settings page, '{viewApps}' section is not present");
            topNav.LogOut();

            Log.Info($"Login as {CompanyAdmin.FullName} CA and Verify '{viewApps}' is present and logout as a {CompanyAdmin.FullName} CA");
            login.LoginToApplication(CompanyAdmin.Username, CompanyAdmin.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{CompanyAdmin.FullName} CA, On Settings page, '{viewApps}' section is not present");
            topNav.LogOut();

            Log.Info($"Login as {BusinessLineAdminUser.FullName} BL and Verify '{viewApps}' is present and logout as a {BusinessLineAdminUser.FullName} BL");
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{BusinessLineAdminUser.FullName} BL, On Settings page, '{viewApps}' section is not present");
            topNav.LogOut();

            Log.Info($"Login as {OrganizationalLeaderUser.FullName} OL and Verify '{viewApps}' is present and logout as a {OrganizationalLeaderUser.FullName} OL");
            login.LoginToApplication(OrganizationalLeaderUser.Username, OrganizationalLeaderUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{OrganizationalLeaderUser.FullName} OL, On Settings page, '{viewApps}' section is not present");
            topNav.LogOut();

            Log.Info($"Login as {TeamAdminUser.FullName} TA and Verify '{viewApps}' is present and logout as a {TeamAdminUser.FullName} TA");
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{TeamAdminUser.FullName} TA, On Settings page, '{viewApps}' section is not present");
            topNav.LogOut();

            Log.Info($"Login as {Member.FullName} M and Verify '{viewApps}' is present and logout as a {Member.FullName} M");
            login.LoginToApplication(Member.Username, Member.Password);
            settingsPage.NavigateToPage();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent(viewApps), $"{Member.FullName} M, On Settings page, '{viewApps}' section is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_OauthAppRegistrations_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            const string viewApps = "View Apps";

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} SA and go to 'Setting page' and set 'Oauth AppRegistration' is 'OFF' and verify {viewApps} isn't present and logout as a {SiteAdminUser.FullName} SA ");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffOauth();
            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            Log.Info($"Login as {CompanyAdmin1.FullName} CFA and Verify {viewApps} isn't present and logout as a {CompanyAdmin1.FullName} CFA");
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{CompanyAdmin1.FullName} CFA, On Settings page, {viewApps} section is present");
            topNav.LogOut();

            Log.Info($"Login as {CompanyAdmin.FullName} CA and Verify {viewApps} isn't present and logout as a {CompanyAdmin.FullName} CA");
            login.LoginToApplication(CompanyAdmin.Username, CompanyAdmin.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{CompanyAdmin.FullName} CA, On Settings page, {viewApps} section is not present");
            topNav.LogOut();

            Log.Info($"Login as {PartnerAdminUser.FullName} PA and Verify {viewApps}  isn't present and logout as a {PartnerAdminUser.FullName} PA");
            login.LoginToApplication(PartnerAdminUser.Username, PartnerAdminUser.Password);
            settingsPage.NavigateToPage(SettingsCompany.Id);
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{PartnerAdminUser.FullName} PA, On Settings page, {viewApps} section is present");
            topNav.LogOut();

            Log.Info($"Login as {BusinessLineAdminUser.FullName} BL and Verify {viewApps}  isn't present and logout as a {BusinessLineAdminUser.FullName} BL");
            login.LoginToApplication(BusinessLineAdminUser.Username, BusinessLineAdminUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{BusinessLineAdminUser.FullName} BL, On Settings page, {viewApps} section is present");
            topNav.LogOut();

            Log.Info($"Login as {OrganizationalLeaderUser.FullName} OL and Verify {viewApps}  isn't present and logout as a {OrganizationalLeaderUser.FullName} OL");
            login.LoginToApplication(OrganizationalLeaderUser.Username, OrganizationalLeaderUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{OrganizationalLeaderUser.FullName} OL, On Settings page, {viewApps} section is present");
            topNav.LogOut();

            Log.Info($"Login as {TeamAdminUser.FullName} TA and Verify {viewApps}  isn't present and logout as a {TeamAdminUser.FullName} TA");
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{TeamAdminUser.FullName} TA, On Settings page, {viewApps} section is present");
            topNav.LogOut();

            Log.Info($"Login as Member and Verify {viewApps}  isn't present and logout as a {Member.FullName} M");
            login.LoginToApplication(Member.Username, Member.Password);
            settingsPage.NavigateToPage();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent(viewApps), $"{Member.FullName} M, On Settings page, {viewApps} section is not present");
        }

        //Enable Language Selection
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableLanguageSelection_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and turn on 'Enable Language Selection' feature");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnEnableLanguageSelection();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and Login as company admin then Verify that 'Preferred language' drop down should be displayed while creating a team");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            // Team Verification
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsTrue(createTeamPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while creating a team");

            Log.Info($"Edit the '{SharedConstants.Team}' and Verify that 'Preferred language' drop down should be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.Team);
            dashBoardPage.ClickTeamEditButton(SharedConstants.Team);
            Assert.IsTrue(createTeamPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while editing a team");

            // My Profile Level
            Log.Info("Go to the 'My Profile' page and Verify that 'Preferred Language' drop down should be displayed");
            topNav.HoverOnNameRoleSection();
            topNav.ClickOnMyProfile();
            Assert.IsTrue(accountSettingsPage.IsPreferredLanguageDisplayed(), "'Preferred Language' drop down is not displayed at 'My Profile' page");

            // User Level
            Log.Info("Go to the 'Manage User' and verify that 'Preferred Language' drop down should be displayed while creating a user");
            manageUserPage.NavigateToPage(SettingsCompany.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnAddNewUserButton();
            Assert.IsTrue(manageUserPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while adding a user");

            Log.Info($"Edit the {CompanyAdmin1.FullName} and Verify that 'Preferred language' drop down should be displayed");
            manageUserPage.ClickCancelButton();
            manageUserPage.ClickOnEditUserIcon(CompanyAdmin1.Username);
            Assert.IsTrue(manageUserPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while editing a user");

            // Company Level
            Log.Info($"Logout as {CompanyAdmin1.FullName} and login as site admin and verify that 'Preferred language' drop down should be displayed");
            topNav.LogOut();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();
            Assert.IsTrue(addCompanyPage1.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while creating a company");

            Log.Info($"Edit the {SiteAdminUser.CompanyName} and verify that 'Preferred language' drop down should be displayed");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickEditIconByCompanyName(SiteAdminUser.CompanyName);
            Assert.IsTrue(editCompanyProfilePage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while editing a company");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableLanguageSelection_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType.CompanyAdmin);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and turn off 'Enable Language Selection' feature");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffEnableLanguageSelection();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and Login as company admin then Verify that 'Preferred language' drop down should not be displayed while creating a team");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            // Team Verification
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            Assert.IsFalse(createTeamPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is displayed while creating a team");

            Log.Info($"Edit the '{SharedConstants.Team}' and Verify that 'Preferred language' drop down should not be displayed");
            dashBoardPage.NavigateToPage(SettingsCompany.Id);
            dashBoardPage.SearchTeam(SharedConstants.Team);
            dashBoardPage.ClickTeamEditButton(SharedConstants.Team);
            Assert.IsFalse(createTeamPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is displayed while editing a team");

            // My Profile Level
            Log.Info("Go to the 'My Profile' page and Verify that 'Preferred Language' drop down should not be displayed");
            topNav.HoverOnNameRoleSection();
            topNav.ClickOnMyProfile();
            Assert.IsFalse(accountSettingsPage.IsPreferredLanguageDisplayed(), "'Preferred Language' drop down is displayed at 'My Profile' page");

            // User Level
            Log.Info("Go to the 'Manage User' and verify that 'Preferred Language' drop down should not be displayed while creating a user");
            manageUserPage.NavigateToPage(SettingsCompany.Id);
            manageUserPage.SelectTab();
            manageUserPage.ClickOnAddNewUserButton();
            Assert.IsFalse(manageUserPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is displayed while adding a user");

            Log.Info($"Edit the {CompanyAdmin1.FullName} and Verify that 'Preferred language' drop down should not be displayed");
            manageUserPage.ClickCancelButton();
            manageUserPage.ClickOnEditUserIcon(CompanyAdmin1.Username);
            Assert.IsFalse(manageUserPage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down  is displayed while editing a user");

            // Company Level
            Log.Info($"Logout as {CompanyAdmin1.FullName} and login as site admin and verify that 'Preferred language' drop down should be displayed");
            topNav.LogOut();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();
            Assert.IsTrue(addCompanyPage1.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while creating a company");

            Log.Info($"Edit the {SiteAdminUser.CompanyName} and verify that 'Preferred language' drop down should be displayed");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickEditIconByCompanyName(SiteAdminUser.CompanyName);
            Assert.IsTrue(editCompanyProfilePage.IsPreferredLanguageDisplayed(), "'Preferred language' drop down is not displayed while editing a company");
        }

        //Allow Users To Create And Manage Private App Key
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_AllowUsersToCreateAndManagePrivateAppKey_On()
        {
            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Allow Users To Create And Manage Private App Key' feature");
            manageFeaturesPage.TurnOnAllowUsersToCreateAndManagePrivateAppKey();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} ");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("On Settings page, verify that 'Manage App Keys' section present");
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("Manage App Keys"), "On Settings page, 'Manage App Keys' section is not present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_AllowUsersToCreateAndManagePrivateAppKey_Off()
        {
            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Allow Users To Create And Manage Private App Key' feature");
            manageFeaturesPage.TurnOffAllowUsersToCreateAndManagePrivateAppKey();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} ");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("On Settings page, verify that 'Manage App Keys' section is not present");
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("Manage App Keys"), "On Settings page, 'Manage App Keys' section is present");
        }
    }
}
