using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardGridFilteringTests : BaseTest
    {
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static bool _classInitFailed;
        private static int _teamId;
        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment);
                _teamId = teams.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.TeamForGiTab).TeamId;

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 53219
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [Description("Test 5: Verify that filtering working properly in Growth Item dashboard, Grid view")]
        public void GrowthItemDashboard_Grid_Filtering()
        {

            VerifySetup(_classInitFailed);

            Log.Info("Test 5: Verify that filtering working properly in Growth Item dashboard, Grid view");
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId);

            teamAssessmentDashboard.SelectGrowthItemsTab();

            growthItemsPage.ClickAddNewItemButton();
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Driver.NavigateToPage(ApplicationUrl);

            dashBoardPage.ClickGrowthItemDashBoard();
            growthItemGridView.SwitchToGridView();
            giDashboardGridView.ClearFilter();
            giDashboardGridView.AddSelectedColumns(new List<string> { "Category", "Radar Type", "Tags" });

            giDashboardGridView.FilterByCategory("Organizational");

            foreach (var row in giDashboardGridView.GetColumnValues("Category"))
            {
                Assert.AreEqual("Organizational", row, "Filter by Category should working properly");
            }

            giDashboardGridView.ClearFilter();
            giDashboardGridView.FilterBySurveyType(SharedConstants.TeamAssessmentType);

            foreach (var row in giDashboardGridView.GetColumnValues("Radar Type"))
            {
                Assert.AreEqual(SharedConstants.TeamAssessmentType, row, "Filter by Radar Type should working properly");
            }

            giDashboardGridView.ClearFilter();
            giDashboardGridView.FilterByTags(SharedConstants.TeamTag);

            foreach (var row in giDashboardGridView.GetColumnValues("Tags"))
            {
                StringAssert.Contains(row, SharedConstants.TeamTag, "Filter by Tags should working properly");
            }
            giDashboardGridView.ClearFilter();
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [Description("Test 6: Verify Categories dropdown list for Team view and Talent Development view on 'Main Growth Item Dashboard' page")]
        public void GrowthItemDashboard_Categories()
        {

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Log.Info("Login to the application and Navigate to 'Growth Item Dashboard' page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.ClickGrowthItemDashBoard();

            Log.Info("Clear filter and Verify 'Categories' dropdown list for 'Team' view ");
            growthItemGridView.SwitchToGridView();
            giDashboardGridView.ClearFilter();
            var actualTeamCategoryList = new List<string>(giDashboardGridView.GetTeamViewCategoryList());

            var expectedTeamCategoryTa = GrowthPlanFactory.GetTeamGrowthPlanCategories().Union(GrowthPlanFactory.GetMultiTeamGrowthPlanCategories()).ToList();
            expectedTeamCategoryTa.Remove("Individual");
            expectedTeamCategoryTa.Insert(0, "All");
            Assert.That.ListsAreEqual(expectedTeamCategoryTa, actualTeamCategoryList, "Team view 'Categories' dropdown list doesn't match");

            Log.Info("Switch to 'Talent Development' view and Verify 'Categories' dropdown list");
            giDashboardGridView.ChangeAssessmentTypeView(AssessmentWidget.Individual);
            giDashboardGridView.ClearFilter();
            var actualTalentDevelopmentCategoryList = new List<string>(giDashboardGridView.GetTalentDevelopmentViewCategoryList());
            var expectedTalentDevelopmentCategoryList = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories();
            expectedTalentDevelopmentCategoryList.Insert(0, "All");
            Assert.That.ListsAreEqual(expectedTalentDevelopmentCategoryList, actualTalentDevelopmentCategoryList, "Talent Development view 'Categories' dropdown list doesn't match");
        }

    }
}
