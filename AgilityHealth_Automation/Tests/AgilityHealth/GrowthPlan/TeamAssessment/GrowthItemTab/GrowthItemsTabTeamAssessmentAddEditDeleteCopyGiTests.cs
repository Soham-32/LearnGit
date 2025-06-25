using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment.GrowthItemTab
{
    [TestClass]
    [TestCategory("GrowthPlan"), TestCategory("GrowthItemTab")]
    public class GrowthItemsTabTeamAssessmentAddEditDeleteCopyGiTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo FirstTeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(), 
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };
        private static readonly TeamAssessmentInfo SecondTeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(), 
        };

        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private const string RadarTypeTooltipMessage = "By selecting a Radar Type, the Competency menu will be available.";
        private const string RadarType = "Radar Type";

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
                _team = teams.GetTeamByName(Constants.TeamForGiTab);
                _multiTeam = teams.GetTeamByName(Constants.MultiTeamForGiTab);

                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, FirstTeamAssessment);
                setup.AddTeamAssessment(_team.TeamId, SecondTeamAssessment);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_AddEditDeleteGI()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giGridViewPage = new GrowthItemGridView(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team Assessment Growth Item Tab' page and verify 'Categories' dropdown values from add growth item page");
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            growthItemsPage.SwitchToGridView();
            growthItemsPage.ClickAddNewItemButton();
            Assert.IsTrue(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Disabled on growth item page");
            Assert.IsTrue(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is not displayed");
            Assert.AreEqual(RadarTypeTooltipMessage, addGrowthItemPopup.GetRadarTypeTooltipMessage(), $"{RadarType} tooltip message doesn't match");

            addGrowthItemPopup.ClickOnCategoryDropDown();

            var actualCategoryList = new List<string>(addGrowthItemPopup.GetCategoryList());
            var expectedCategory = GrowthPlanFactory.GetTeamGrowthPlanCategories();
            Assert.That.ListsAreEqual(expectedCategory, actualCategoryList, "Team Assessment 'Categories' dropdown list doesn't match");

            Log.Info("Create a growth item on 'Team Assessment Growth Item Tab' and verify on the same page");
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem(FirstTeamAssessment.AssessmentName);
            growthItemInfo.Owner = null;

            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();


            giGridViewPage.AddSelectedColumns(new List<string> { "Radar Type", "Assessment" });
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Assessment, actualGrowthItem.Assessment, "Assessment doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.RadarType, actualGrowthItem.RadarType, "Radar Type doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            Log.Info($"Edit the {growthItemInfo.Title} growth item on 'Team Assessment Growth Item Tab' and verify on the same page");
            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(FirstTeamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            var editedGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem(SecondTeamAssessment.AssessmentName);
            editedGrowthItemInfo.Owner = null;

            addGrowthItemPopup.EnterGrowthItemInfo(editedGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on 'Growth Item Tab' page");

            var editedGrowthItem = growthItemsPage.GetGrowthItemFromGrid(editedGrowthItemInfo.Title);
            Assert.AreEqual(editedGrowthItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Assessment, editedGrowthItem.Assessment, "Assessment doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.RadarType, editedGrowthItem.RadarType, "Radar Type doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            Log.Info($"Delete {editedGrowthItemInfo.Title} growth item and verify on 'Team Assessment Growth Item Tab' page");
            growthItemsPage.DeleteGrowthItem(editedGrowthItemInfo.Title);
            Assert.IsFalse(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"Growth item {editedGrowthItemInfo.Title} exist on 'Team Assessment Growth Item Tab' page.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_CreateDelete_GI_Edit_from_GrowthItemsDashboard()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giGridViewPage = new GrowthItemGridView(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);

            Log.Info("Go to 'Team Assessment Growth Item Tab' page and create a new growth item");
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem(FirstTeamAssessment.AssessmentName);
            growthItemInfo.Owner = null;
            growthItemsPage.ClickAddNewItemButton();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            giGridViewPage.AddSelectedColumns(new List<string> { "Radar Type" });
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            Log.Info($"Go to 'Growth Item Dashboard' page to verify the {growthItemInfo.Title} growth item ");
            Driver.NavigateToPage(ApplicationUrl);
            dashBoardPage.ClickGrowthItemDashBoard();

            giGridViewPage.AddSelectedColumns(new List<string> { "Origination", "Assessment" });
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Growth Item Dashboard' page");
            Assert.AreEqual(_team.Name + "'s " + FirstTeamAssessment.AssessmentName, giGridViewPage.GetGrowthItemValue(growthItemInfo.Title, "Origination"), "Origination should display correct team name");

            var actualNewGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualNewGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Assessment, actualNewGrowthItem.Assessment, "Assessment doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualNewGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualNewGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualNewGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualNewGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualNewGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualNewGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.RadarType, actualNewGrowthItem.RadarType, "Radar Type doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualNewGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(growthItemInfo.Color, actualNewGrowthItem.Color, "Color doesn't match");

            Log.Info($"Edit the {growthItemInfo.Title} growth item on 'Growth Item' dashboard and verify on 'Team Assessment Growth Item Tab' page");
            var editedGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            editedGrowthItemInfo.Owner = null;
            giDashboardGridView.ClickGrowthItemDashboardEditButton(growthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsAssessmentFieldDisplayed(), "Assessment field is displayed");
            addGrowthItemPopup.EnterGrowthItemInfo(editedGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on 'Growth Item Dashboard' page");

            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            var editedGrowthItem = growthItemsPage.GetGrowthItemFromGrid(editedGrowthItemInfo.Title);
            Assert.AreEqual(editedGrowthItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Description, editedGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            Log.Info($"Delete the {editedGrowthItemInfo.Title} growth item from 'Team Assessment Growth Item Tab' and verify on Assessment > Growth Items Tab dashboard");
            growthItemsPage.DeleteGrowthItem(editedGrowthItemInfo.Title);
            Assert.IsFalse(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"Growth item {editedGrowthItemInfo.Title} exist on 'Team Assessment Growth Item Tab' page.");

            Driver.NavigateToPage(ApplicationUrl);
            dashBoardPage.ClickGrowthItemDashBoard();
            Assert.IsFalse(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"Growth item {editedGrowthItemInfo.Title} exist on 'Growth Item' Dashboard .");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_PullUnPullItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mTeTDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info("Go to 'Team Assessment Radar' page and create a growth item");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.Owner = null;
            growthItemsPage.ClickAddNewItemButton();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Radar' page");

            Log.Info($"Go to 'Multi Team Radar page' and pull {growthItemInfo.Title} growth item");
            mTeTDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mTeTDashboardPage.ClickOnRadar(FirstTeamAssessment.AssessmentType);
            growthItemGridView.ClickPullItemFromSubTeam();
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(growthItemInfo.Title), "Pull able item should display");

            growthItemGridView.PullItemFromSubTeam(growthItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsUnPullItemDisplayed(growthItemInfo.Title), "UnPull item should be displayed");

            growthItemGridView.ClickClosePullDialog();
            growthItemGridView.SelectAllColumn();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Pulled item title doesn't match");

            var teamName = growthItemGridView.GetCellValue(growthItemInfo.Title, "Origination");
            Assert.AreEqual(_team.Name, teamName, "Team Name in Origination should display correctly");

            Log.Info($"Unpull {growthItemInfo.Title} from 'Multi Team Radar' page");
            growthItemGridView.ClickPullItemFromSubTeam();
            growthItemGridView.UnPullItemFromSubTeam(growthItemInfo.Title);

            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(growthItemInfo.Title), "Pull able item should display");
            growthItemGridView.ClickClosePullDialog();
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Pull item count doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_AddEditDelete_From_RadarPage()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team Assessment Radar' page and create a growth item ");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(FirstTeamAssessment.AssessmentName);

            growthItemGridView.SwitchToGridView();
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.RadarType = null;
            growthItemInfo.Owner = null;

            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Radar' page");

            Log.Info($"Go to 'Team Assessment Growth Item Tab' page and verify growth item {growthItemInfo.Title}");
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(FirstTeamAssessment.AssessmentName, actualGrowthItem.Assessment, "Assessment doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            Log.Info($"Edit the {growthItemInfo.Title} growth item on 'Team Assessment Radar' page and verify on 'Team Assessment Growth Item Tab' page");
            Driver.Back();

            var editedGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            editedGrowthItemInfo.RadarType = null;
            editedGrowthItemInfo.Owner = null;
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            addGrowthItemPopup.EnterGrowthItemInfo(editedGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on 'Team Assessment Radar' page");

            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(editedGrowthItemInfo.Title);
            Assert.AreEqual(editedGrowthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(FirstTeamAssessment.AssessmentName, actualGrowthItem.Assessment, "Assessment doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            Log.Info($"Delete the {editedGrowthItemInfo.Title} growth item from 'Assessment Radar Page' and verify on 'Team Assessment Growth Item Tab' page");
            Driver.Back();
            growthItemGridView.DeleteGrowthItem(editedGrowthItemInfo.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(editedGrowthItemInfo.Title), $"Growth item {editedGrowthItemInfo.Title} is present on 'Assessment Growth Item Dashboard' page.");

            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            Assert.IsFalse(growthItemGridView.IsGiPresent(editedGrowthItemInfo.Title), $"Growth item {editedGrowthItemInfo.Title} is present on 'Team Assessment Growth Item Tab' page.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_Create_GI_From_RadarPage_EditDeleteGI_From_GrowthItemsTab()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team Assessment Radar' page and create a growth item ");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(FirstTeamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.RadarType = null;
            growthItemInfo.Owner = null;

            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Team Assessment radar page");
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Radar' page");

            Log.Info($"Go to 'Team Assessment Growth Item Tab' page and verify growth item {growthItemInfo.Title}");
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(FirstTeamAssessment.AssessmentName, actualGrowthItem.Assessment, "Assessment doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(growthItemInfo.Color, actualGrowthItem.Color, "Color doesn't match");

            Log.Info($"Edit the {growthItemInfo.Title} growth item on 'Team Assessment Growth Item Tab' page and verify on 'Team Assessment Radar' page");
            var editedGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem(SecondTeamAssessment.AssessmentName);
            editedGrowthItemInfo.RadarType = null;
            editedGrowthItemInfo.Owner = null;

            growthItemsPage.ClickGrowthItemEditButton(growthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on growth item page");
            addGrowthItemPopup.EnterGrowthItemInfo(editedGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on 'Team Assessment Growth Item Tab' page");

            Driver.Back();
            Assert.IsFalse(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does exist on Radar page of first assessment");

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SecondTeamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            Assert.IsTrue(growthItemsPage.DoesGiExist(editedGrowthItemInfo.Title), $"{editedGrowthItemInfo.Title} does not exist on Radar page of second assessment");

            growthItemGridView.ClickGrowthItemEditButton(editedGrowthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Radar page");
            addGrowthItemPopup.ClickCancelButton();
            var editedGrowthItem = growthItemGridView.GetGrowthItemFromGrid(editedGrowthItemInfo.Title);
            Assert.AreEqual(editedGrowthItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Description, editedGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(editedGrowthItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            Log.Info($"Delete the {editedGrowthItemInfo.Title} growth item on  'Team Assessment Growth Item Tab' page and verify on 'Team Assessment Radar' page");
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);
            growthItemsPage.DeleteGrowthItem(editedGrowthItemInfo.Title);
            Driver.Back();
            Assert.AreEqual(0, growthItemGridView.GetGrowthItemCount(), "Growth item isn't deleted successfully");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_CopyGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team Assessment Growth Item Tab' page and create a new growth item");
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.Category = GrowthPlanFactory.GetTeamGrowthPlanCategories().FirstOrDefault();
            growthItemInfo.CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember.FirstOrDefault() };
            growthItemInfo.RadarType = SharedConstants.TeamAssessmentType;
            growthItemInfo.Owner = null;

            growthItemsPage.ClickAddNewItemButton();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"Copy growth item {growthItemInfo.Title} and verify there two growth item ");
            growthItemsPage.ClickCopyGrowthItemButton(growthItemInfo.Title);

            Assert.AreEqual(2, growthItemsPage.TotalCopiedGi(growthItemInfo.Title), "Growth item isn't copied successfully");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_DeleteAssessmentHasGI()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);
            Log.Info("Go to 'Team Assessment Radar' page and create growth item");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(FirstTeamAssessment.AssessmentName);

            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.RadarType = null;
            growthItemInfo.Owner = null;

            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"{growthItemInfo.Title} does not exist on 'Team Assessment Radar' page");

            Driver.Back();
            Log.Info($"Delete the {FirstTeamAssessment.AssessmentName} assessment and verify growth item {growthItemInfo.Title} is exist or not on 'Team Assessment Growth Item Tab' page ");
            teamAssessmentDashboard.SelectRadarLink(FirstTeamAssessment.AssessmentName, "Edit");
            taEditPage.ClickOnDeleteAssessmentButtonAndChooseRemoveOption();

            teamAssessmentDashboard.SelectGrowthItemsTab();
            Assert.IsTrue(growthItemsPage.DoesGiExist(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} exist on 'Team Assessment Growth Item Tab' page.");
            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        public void TA_GrowthItemsTab_Kanban_AddDragCopyDeleteGI()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var growthItemTabKanbanWidgetPage = new GiDashboardKanbanWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team Assessment Growth Item Tab' page and create a growth item in 'kanban' view");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.SelectGrowthItemsTab();
            growthItemsPage.ClickKanbanAddNewGrowthItem();
            Assert.IsTrue(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Disabled on growth item page 'kanban' view");
            Assert.IsTrue(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is not displayed");
            Assert.AreEqual(RadarTypeTooltipMessage, addGrowthItemPopup.GetRadarTypeTooltipMessage(), $"{RadarType} tooltip message doesn't match");

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title),
                 "Growth Item should be added successfully");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify 'Radar Type' tooltip icon is not displayed ");
            growthItemTabKanbanWidgetPage.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is enabled on growth item page 'kanban' view");
            Assert.AreEqual(FirstTeamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            addGrowthItemPopup.ClickCancelButton();

            Log.Info($"Move the Growth Item card {growthItemInfo.Status} from {growthItemInfo.Status} section to 'Done' Section");
            growthItemKanbanView.DragDropGi(1, growthItemInfo.Status, growthItemInfo.Title, "Done");
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthItemInfo.Title),
                "Growth Item should be dragged and dropped successfully");

            Log.Info($"Copy growth item {growthItemInfo.Title} and verify there two growth item ");
            growthItemKanbanView.ClickKanbanGiCopyButton("Done", 1, growthItemInfo.Title);
            Assert.AreEqual(2, growthItemKanbanView.GetGiCount("Done", growthItemInfo.Title),
                "Growth Item should be copied successfully");

            Log.Info($" Delete both {growthItemInfo.Title} Growth item and verify it");
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthItemInfo.Title);
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthItemInfo.Title);
            Assert.IsFalse(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthItemInfo.Title),
                "Growth Item should be deleted successfully");
        }
    }
}
