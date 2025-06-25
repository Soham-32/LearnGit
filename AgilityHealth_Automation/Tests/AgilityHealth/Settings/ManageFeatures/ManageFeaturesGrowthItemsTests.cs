using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesGrowthItemsTests : BaseTest
    {
        private static User CompanyAdmin1 => TestEnvironment.UserConfig.GetUserByDescription("user 3");
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        private static AtCommon.Dtos.Company SettingsCompany =>
            SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);

        private static CompanyHierarchyResponse _allTeamsList;
        private static AddTeamWithMemberRequest _teamRequest;

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(), 
            Campaign = "AT Campaign",
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };

        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static RadarResponse _radar;
        private static SetupTeardownApi _setup;
        private static SetUpMethods _setupUi;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _teamRequest = TeamFactory.GetNormalTeam("TeamForMemberLogin", 2);

            _setup = new SetupTeardownApi(TestEnvironment);
            _setupUi = new SetUpMethods(_, TestEnvironment);

            _setup.CreateTeam(_teamRequest, CompanyAdmin1).GetAwaiter().GetResult();
            _allTeamsList = _setup.GetCompanyHierarchy(
                SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, SiteAdminUser);
            _team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.")
                .GetTeamByName(SharedConstants.Team);
            _multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.")
                .GetTeamByName(SharedConstants.MultiTeam);
            _enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.")
                .GetTeamByName(SharedConstants.EnterpriseTeam);
            _radar = _setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);

            _setupUi.AddTeamAssessment(_team.TeamId, TeamAssessment, SiteAdminUser);
        }

        //Growth Items Display
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthItemDisplay_New()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);
            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn 'New' 'Growth Items Display' feature");
            manageFeaturesPage.TurnGiDisplayToNew();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info($"Navigate to radar page of {multiTeam.TeamId}");
            teamsDashboard.NavigateToPage(Company.Id);
            mtDashboardPage.NavigateToPage(multiTeam.TeamId);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.SwitchToGridView();

            Log.Info("Verify that Pull Items From Subteam button displays in new view for MT");
            Assert.IsTrue(growthItemGridView.IsShowPullableItemDisplayed(), "Show Pullable Item button should be displayed");
            Assert.IsFalse(growthItemGridView.IsPullItemFromSubteamDisplayed(), "Pull Item From Subteam button should not be displayed");

            mtDashboardPage.NavigateToPage(enterpriseTeam.TeamId, true);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.SwitchToGridView();

            Log.Info("Verify that Pull Items From Subteam button displays in new view for ET");
            Assert.IsTrue(growthItemGridView.IsShowPullableItemDisplayed(), "Show Pullable Item button should be displayed");
            Assert.IsFalse(growthItemGridView.IsPullItemFromSubteamDisplayed(), "Pull Item From Subteam button should not be displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthItemDisplay_Classic()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);
            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Turn 'Classic' 'Growth Items Display' feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnGiDisplayToClassic();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info($"Navigate to radar page of {multiTeam.TeamId}");
            teamsDashboard.NavigateToPage(Company.Id);
            mtDashboardPage.NavigateToPage(multiTeam.TeamId);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.SwitchToGridView();

            Log.Info("Verify that Pull Items From Subteam button displays in classic view for MT");
            Assert.IsTrue(growthItemGridView.IsPullItemFromSubteamDisplayed(), "Pull Item From Subteam button should be displayed");
            Assert.IsFalse(growthItemGridView.IsShowPullableItemDisplayed(), "Show Pullable Item button should not be displayed");

            mtDashboardPage.NavigateToPage(enterpriseTeam.TeamId, true);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.SwitchToGridView();

            Log.Info("Verify that Pull Items From Subteam button displays in classic view for ET");
            Assert.IsTrue(growthItemGridView.IsPullItemFromSubteamDisplayed(), "Pull Item From Subteam button should be displayed");
            Assert.IsFalse(growthItemGridView.IsShowPullableItemDisplayed(), "Show Pullable Item button should not be displayed");

        }
        //Growth Item Types
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_GrowthItemTypes_New()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffCustomGrowthItemsTypes();

            Log.Info("Turn New 'Growth Item Types' feature");
            manageFeaturesPage.TurnOnNewGrowthItemTypes();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            mtetDashboardPage.NavigateToPage(multiTeam.TeamId);
            mtetDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            var expectedItems = new List<string>
            {
                "Agile Enablement",
                "Tech Agility & Tools",
                "Culture & Leadership",
                "Product & Program",
                "Other"
            };

            var actualItems = addGrowthItemPopup.GetTypeListItems();

            Assert.AreEqual(expectedItems.Count, actualItems.Count, "Count of list items does not match.");
            foreach (var item in expectedItems)
            {
                Assert.IsTrue(actualItems.Contains(item), $"{item} was not found in the list.");
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_GrowthItemTypes_Classic()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.MultiTeamForGrowthJourney);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboard.WaitUntilLoaded();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffCustomGrowthItemsTypes();

            Log.Info("Turn Classic 'Growth Item Types' feature");
            manageFeaturesPage.TurnOffNewGrowthItemTypes();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            mtetDashboardPage.NavigateToPage(multiTeam.TeamId);
            mtetDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            var expectedItems = new List<string>
            {
                "Agile Ceremonies",
                "Backlog Management",
                "Coaching",
                "Estimating",
                "Knowledge Transfer",
                "Leadership/Management",
                "Planning",
                "Process Improvement",
                "Self-Study",
                "Technical Excellence",
                "Tools | Technology",
                "Training",
                "Other"
            };

            var actualItems = addGrowthItemPopup.GetTypeListItems();

            Assert.AreEqual(expectedItems.Count, actualItems.Count, "Count of list items does not match.");
            foreach (var item in expectedItems)
            {
                Assert.IsTrue(actualItems.Contains(item), $"{item} was not found in the list.");
            }
        }
        //Growth Item Type Field Required
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthItemTypeFieldRequired_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var radarPage = new RadarPage(Driver, Log);

            login.NavigateToPage();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            manageFeaturesPage.TurnOnGrowthItemTypeFieldRequired();
            manageFeaturesPage.ClickUpdateButton();

            teamsDashboard.NavigateToPage(Company.Id);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Type = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed("Type"), "Validation message for 'Type' field is not present");
            addGrowthItemPopup.ClickCancelButton();

            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemInfo.Category = "Enterprise";
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed("Type"), "Validation message for 'Type' field is not present");
            addGrowthItemPopup.ClickCancelButton();

            radarPage.NavigateToPage(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed("Type"), "Validation message for 'Type' field is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthItemTypeFieldRequired_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var radarPage = new RadarPage(Driver, Log);

            login.NavigateToPage();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            manageFeaturesPage.TurnOffGrowthItemTypeFieldRequired();
            manageFeaturesPage.ClickUpdateButton();

            teamsDashboard.NavigateToPage(Company.Id);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Type = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed("Type"), "Validation message for 'Type' field is present");
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");

            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemInfo.Category = "Enterprise";
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed("Type"), "Validation message for 'Type' field is present");
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");

            radarPage.NavigateToPage(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed("Type"), "Validation message for 'Type' field is present");
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");
        }
        //Custom Growth Item Types
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug: 35813
        [TestCategory("ManageFeatures")]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_CustomGrowthItemTypes_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            //var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            //var addGrowthItems = new GrowthPlanAddItemPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            //var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            //var etDashboard = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Creating a one custom growth item type");
            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(SettingsCompany.Id);
            var customGrowthItemTypeText = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();
            _setup.CreateGrowthItemCustomType(newCustomGrowthItemTypeRequest);
            var expectedCustomGrowthItemTypeList = _setup.GetGrowthItemCustomType(SettingsCompany.Id).Result.Select(a => a.CustomText).ToList();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnNewGrowthItemTypes();
            manageFeaturesPage.TurnOnCustomGrowthItemsTypes();
            manageFeaturesPage.ClickUpdateButton();

            //v2
            //Team
            //Log.Info($"On v2 at team level, verify that edited custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Add Growth Item' tab");

            //growthPlanDashboard.NavigateToPage(Company.Id, _team.TeamId);
            //growthPlanDashboard.ClickOnAddGrowthItemButton();
            //addGrowthItems.ClickOnTypesDropDown();
            //var actualCustomGrowthItemTypeListV2ForTeam = new List<string>(addGrowthItems.GetTypesList());
            //Assert.That.ListContains(actualCustomGrowthItemTypeListV2ForTeam, customGrowthItemTypeText, $"On v2 at team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            //Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, actualCustomGrowthItemTypeListV2ForTeam, "On v2 at team level, custom Growth Item Types list doesn't match");


            //addGrowthItems.DoubleClickOnOwnerDropDown();
            //growthPlanAddItemPage.FillForm(growthItemInfo);
            //addGrowthItems.ClickOnSaveButton();

            //Log.Info($"On v2 at team level, verify that custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Edit Growth Item' tab");
            //growthPlanDashboard.ClickOnColumnValueByGiTitleName(growthItemInfo.Title, "Title");
            //addGrowthItems.ClickOnTypesDropDown();
            //var editedGrowthItemTypeListV2ForTeam = new List<string>(addGrowthItems.GetTypesList());
            //Assert.That.ListContains(editedGrowthItemTypeListV2ForTeam, customGrowthItemTypeText, $"On v2 at team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            //Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editedGrowthItemTypeListV2ForTeam, "On v2 at team level, custom Growth Item Types list doesn't match");

            ////Multi-team
            //Log.Info($"On v2 multi team level, verify that edited custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Add Growth Item' tab");
            //growthPlanDashboard.NavigateToPage(Company.Id, _multiTeam.TeamId);
            //growthPlanDashboard.ClickOnAddGrowthItemButton();
            //addGrowthItems.ClickOnTypesDropDown();
            //var actualGrowthItemTypeListV2ForMultiTeamList = new List<string>(addGrowthItems.GetTypesList());
            //Assert.That.ListContains(actualGrowthItemTypeListV2ForMultiTeamList, customGrowthItemTypeText, $"On v2 multi team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            //Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, actualGrowthItemTypeListV2ForMultiTeamList, "On v2 multi team level, custom Growth Item Types list doesn't match");

            //addGrowthItems.DoubleClickOnOwnerDropDown();
            //growthPlanAddItemPage.FillForm(growthItemInfo);
            //addGrowthItems.ClickOnSaveButton();

            //Log.Info($"On v2 multi team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list from 'Edit Growth Item' tab");
            //growthPlanDashboard.ClickOnColumnValueByGiTitleName(growthItemInfo.Title, "Title");
            //addGrowthItems.ClickOnTypesDropDown();
            //var editedGrowthItemTypeListV2ForMultiTeam = new List<string>(addGrowthItems.GetTypesList());
            //Assert.That.ListContains(editedGrowthItemTypeListV2ForMultiTeam, customGrowthItemTypeText, $"On v2 multi team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            //Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editedGrowthItemTypeListV2ForMultiTeam, "On v2 multi team level, custom Growth Item Types list doesn't match");

            ////Enterprise-team
            //Log.Info($"On v2 enterprise team level, verify that edited custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Add Growth Item' tab");
            //growthPlanDashboard.NavigateToPage(Company.Id, _enterpriseTeam.TeamId);
            //growthPlanDashboard.ClickOnAddGrowthItemButton();
            //addGrowthItems.ClickOnTypesDropDown();
            //var actualGrowthItemTypeListV2ForEnterpriseTeamList = new List<string>(addGrowthItems.GetTypesList());
            //Assert.That.ListContains(actualGrowthItemTypeListV2ForEnterpriseTeamList, customGrowthItemTypeText, $"On v2 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            //Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, actualGrowthItemTypeListV2ForEnterpriseTeamList, "On v2 enterprise team level, custom growth item types list doesn't match");

            //addGrowthItems.DoubleClickOnOwnerDropDown();
            //growthItemInfo.Category = "Enterprise";
            //growthPlanAddItemPage.FillForm(growthItemInfo);
            //addGrowthItems.ClickOnSaveButton();

            //Log.Info($"On v2 enterprise team level, verify that custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Edit Growth Item' tab");
            //growthPlanDashboard.ClickOnColumnValueByGiTitleName(growthItemInfo.Title, "Title");
            //addGrowthItems.ClickOnTypesDropDown();
            //var editedGrowthItemTypeListV2ForEnterpriseTeam = new List<string>(addGrowthItems.GetTypesList());
            //Assert.That.ListContains(editedGrowthItemTypeListV2ForEnterpriseTeam, customGrowthItemTypeText, $"On v2 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            //Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editedGrowthItemTypeListV2ForEnterpriseTeam, "On v2 enterprise team level, custom Growth Item Types list doesn't match");

            //Verify at V1
            //Team
            Log.Info($"On v1 team level, verify that custom growth item type {customGrowthItemTypeText} is present in the growth item type list from 'Add Growth Item' tab");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            var actualGrowthItemTypeListV1ForTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListContains(actualGrowthItemTypeListV1ForTeam, customGrowthItemTypeText, $"On v1 team level, custom growth item type <{customGrowthItemTypeText}> is present.");
            Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, actualGrowthItemTypeListV1ForTeam, "On v1 team level, custom Growth Item Types list doesn't match");

            growthItemInfo.Category = "Organizational";
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"On v1 team level, verify that custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Edit Growth Item' tab");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            var editGrowthItemTypeListV1ForTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListContains(editGrowthItemTypeListV1ForTeam, customGrowthItemTypeText, $"On v1 team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editGrowthItemTypeListV1ForTeam, "On v1 team level, custom Growth Item Types list doesn't match");

            //Multi-Team
            Log.Info($"On v1 multi team level, verify that custom growth item type {customGrowthItemTypeText} is present in the growth item type list from 'Add Growth Item' tab");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.ClickAddNewGrowthItem();
            var actualGrowthItemTypeListV1ForMultiTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListContains(actualGrowthItemTypeListV1ForMultiTeam, customGrowthItemTypeText, $"On v1 multi team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editGrowthItemTypeListV1ForTeam, "On v1 multi team level, custom Growth Item Types list doesn't match");

            growthItemInfo.Category = "Enterprise";
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"On v1 multi team level, verify that custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Edit Growth Item' tab");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            var editGrowthItemTypeListV1ForMultiTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListContains(editGrowthItemTypeListV1ForMultiTeam, customGrowthItemTypeText, $"On v1 multi team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editGrowthItemTypeListV1ForTeam, "On v1 multi team level, custom Growth Item Types list doesn't match");

            //Enterprise-Team
            Log.Info($"On v1 enterprise team level, verify that custom growth item type {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            radarPage.NavigateToPage(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.ClickAddNewGrowthItem();
            var actualGrowthItemTypeListV1ForEnterpriseTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListContains(actualGrowthItemTypeListV1ForEnterpriseTeam, customGrowthItemTypeText, $"On v1 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editGrowthItemTypeListV1ForTeam, "On v1 enterprise team level, custom Growth Item Types list doesn't match");

            growthItemInfo.Owner = null;
            growthItemInfo.Category = "";
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo, false);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"On v1 enterprise team level, verify that custom growth item {customGrowthItemTypeText} is present in the growth item type list from 'Edit Growth Item' tab");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            var editGrowthItemTypeListV1ForEnterpriseTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListContains(editGrowthItemTypeListV1ForEnterpriseTeam, customGrowthItemTypeText, $"On v1 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedCustomGrowthItemTypeList, editGrowthItemTypeListV1ForTeam, "On v1 enterprise team level, custom Growth Item Types list doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_CustomGrowthItemTypes_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            //var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            //var addGrowthItems = new GrowthPlanAddItemPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            // var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            //var etDashboard = new MtEtDashboardPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var topNav = new TopNavigation(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Log.Info("Creating a one custom growth item type");
            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(SettingsCompany.Id);
            var customGrowthItemTypeText = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();
            var expectedGrowthItemTypeList = GrowthPlanFactory.GetNewGrowthPlanTypes();
            _setup.CreateGrowthItemCustomType(newCustomGrowthItemTypeRequest, SiteAdminUser);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Custom Growth Item Types' feature");
            manageFeaturesPage.TurnOnNewGrowthItemTypes();
            manageFeaturesPage.TurnOffCustomGrowthItemsTypes();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            /*
             V2
            //Team
            Log.Info($"On v2 team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            growthPlanDashboard.NavigateToPage(Company.Id, _team.TeamId);
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            addGrowthItems.ClickOnTypesDropDown();
            var expectedGrowthItemTypeList = GrowthPlanFactory.GetNewGrowthPlanTypes();
            var actualGrowthItemTypeListV2ForTeam = new List<string>(addGrowthItems.GetTypesList());
            Assert.That.ListNotContains(actualGrowthItemTypeListV2ForTeam, customGrowthItemTypeText, $"On v2 team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, actualGrowthItemTypeListV2ForTeam, "On v2 team level, custom growth item types list doesn't match");

            addGrowthItems.DoubleClickOnOwnerDropDown();
            growthPlanAddItemPage.FillForm(growthItemInfo);
            addGrowthItems.ClickOnSaveButton();

            Log.Info($"On v2 team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list from 'Edit Growth Item' tab");
            growthPlanDashboard.ClickOnColumnValueByGiTitleName(growthItemInfo.Title, "Title");
            addGrowthItems.ClickOnTypesDropDown();
            var editGrowthItemTypeListV2ForTeam = new List<string>(addGrowthItems.GetTypesList());
            Assert.That.ListNotContains(editGrowthItemTypeListV2ForTeam, customGrowthItemTypeText, $"On v2 team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, editGrowthItemTypeListV2ForTeam, "On v2 team level, custom Growth Item Types list doesn't match");

            //Multi-team
            Log.Info($"On v2 multi team level, verify that custom growth item type {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            growthPlanDashboard.NavigateToPage(Company.Id, _multiTeam.TeamId);
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            addGrowthItems.ClickOnTypesDropDown();
            var actualGrowthItemTypeListV2ForMultiTeamList = new List<string>(addGrowthItems.GetTypesList());
            Assert.That.ListNotContains(actualGrowthItemTypeListV2ForMultiTeamList, customGrowthItemTypeText, $"On v2 multi team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, actualGrowthItemTypeListV2ForMultiTeamList, "On v2 multi team level, custom growth item types list doesn't match");

            addGrowthItems.DoubleClickOnOwnerDropDown();
            growthPlanAddItemPage.FillForm(growthItemInfo);
            addGrowthItems.ClickOnSaveButton();

            Log.Info($"On v2 multi team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list 'Edit Growth Item' tab");
            growthPlanDashboard.ClickOnColumnValueByGiTitleName(growthItemInfo.Title, "Title");
            addGrowthItems.ClickOnTypesDropDown();
            var editGrowthItemTypeListV2ForMultiTeam = new List<string>(addGrowthItems.GetTypesList());
            Assert.That.ListNotContains(editGrowthItemTypeListV2ForMultiTeam, customGrowthItemTypeText, $"On v2 multi team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, editGrowthItemTypeListV2ForMultiTeam, "On v2 multi team level, custom growth item types list doesn't match");

            //Enterprise-team
            Log.Info($"On v2 enterprise team level, verify that custom growth item type {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            growthPlanDashboard.NavigateToPage(Company.Id, _enterpriseTeam.TeamId);
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            addGrowthItems.ClickOnTypesDropDown();
            var actualGrowthItemTypeListV2ForEnterpriseTeamList = new List<string>(addGrowthItems.GetTypesList());

            Assert.That.ListNotContains(actualGrowthItemTypeListV2ForEnterpriseTeamList, customGrowthItemTypeText, $"On v2 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, actualGrowthItemTypeListV2ForEnterpriseTeamList, "On v2 enterprise team level, custom growth item types list doesn't match");

            addGrowthItems.DoubleClickOnOwnerDropDown();
            growthItemInfo.Category = "Enterprise";
            growthPlanAddItemPage.FillForm(growthItemInfo);
            addGrowthItems.ClickOnSaveButton();

            Log.Info($"On v2 enterprise team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list 'Edit Growth Item' tab");
            growthPlanDashboard.ClickOnColumnValueByGiTitleName(growthItemInfo.Title, "Title");
            addGrowthItems.ClickOnTypesDropDown();
            var editGrowthItemTypeListV2ForEnterpriseTeam = new List<string>(addGrowthItems.GetTypesList());
            Assert.That.ListNotContains(editGrowthItemTypeListV2ForEnterpriseTeam, customGrowthItemTypeText, $"On v2 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, editGrowthItemTypeListV2ForEnterpriseTeam, "On v2 enterprise team level, custom growth item types list doesn't match");
            */

            //Verify at V1
            //Team
            Log.Info($"On v1 team level, verify that custom growth item type {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            var actualGrowthItemTypeListV1ForTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListNotContains(actualGrowthItemTypeListV1ForTeam, customGrowthItemTypeText, $"On v1 team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, actualGrowthItemTypeListV1ForTeam, "On v1 team level, custom Growth Item Types list doesn't match on v1 at team level");

            growthItemInfo.Category = "Organizational";
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"On v1 team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list from 'Edit Growth Item' tab");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            var editGrowthItemTypeListV1ForTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListNotContains(editGrowthItemTypeListV1ForTeam, customGrowthItemTypeText, $"On v1 team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, editGrowthItemTypeListV1ForTeam, "On v1 team level, custom Growth Item Types list doesn't match");

            //Multi-Team
            Log.Info($"On v1 multi team level, verify that custom growth item type {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.ClickAddNewGrowthItem();
            var actualGrowthItemTypeListV1ForMultiTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListNotContains(actualGrowthItemTypeListV1ForMultiTeam, customGrowthItemTypeText, $"On v1 multi team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, actualGrowthItemTypeListV1ForMultiTeam, "On v1 multi team level, custom Growth Item Types list doesn't match");

            growthItemInfo.Category = "Enterprise";
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"On v1 multi team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list from 'Edit Growth Item' tab");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            var editGrowthItemTypeListV1ForMultiTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListNotContains(editGrowthItemTypeListV1ForMultiTeam, customGrowthItemTypeText, $"On v1 multi team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, editGrowthItemTypeListV1ForMultiTeam, "On v1 multi team level, custom Growth Item Types list doesn't match");

            //Enterprise-Team
            Log.Info($"On v1 enterprise team level, verify that custom growth item type {customGrowthItemTypeText} is not present in the growth item type list from 'Add Growth Item' tab");
            radarPage.NavigateToPage(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.ClickAddNewGrowthItem();
            var actualGrowthItemTypeListV1ForEnterpriseTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListNotContains(actualGrowthItemTypeListV1ForEnterpriseTeam, customGrowthItemTypeText, $"On v1 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, actualGrowthItemTypeListV1ForEnterpriseTeam, "On v1 enterprise team level, custom Growth Item Types list doesn't match");

            growthItemInfo.Owner = null;
            growthItemInfo.Category = "";
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo, false);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"On v1 enterprise team level, verify that custom growth item {customGrowthItemTypeText} is not present in the growth item type list from 'Edit Growth Item' tab");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            var editGrowthItemTypeListV1ForEnterpriseTeam = addGrowthItemPopup.GetTypeListItems().ToList();
            Assert.That.ListNotContains(editGrowthItemTypeListV1ForEnterpriseTeam, customGrowthItemTypeText, $"On v1 enterprise team level, custom growth item type <{customGrowthItemTypeText}> is not present");
            Assert.That.ListsAreEqual(expectedGrowthItemTypeList, editGrowthItemTypeListV1ForEnterpriseTeam, "On v1 enterprise team level, custom Growth Item Types list doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnableAiInsights_On()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable AI Insights' feature'");
            manageFeaturesPage.TurnOnEnableAiInsights();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to  Click on the 'Growth Items' dashboard");
            growthItemsDashboardPage.NavigateToPage(Company.Id);
            Assert.IsTrue(growthItemsDashboardPage.IsAiSummarizationButtonDisplayed(), "The 'AI Summarization' button is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnableAiInsights_Off()
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable AI Insights' feature'");
            manageFeaturesPage.TurnOffEnableAiInsights();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to  Click on the 'Growth Items' dashboard");
            growthItemsDashboardPage.NavigateToPage(Company.Id);
            Assert.IsFalse(growthItemsDashboardPage.IsAiSummarizationButtonDisplayed(), "The 'AI Summarization' button is displayed");
        }
    }
}
