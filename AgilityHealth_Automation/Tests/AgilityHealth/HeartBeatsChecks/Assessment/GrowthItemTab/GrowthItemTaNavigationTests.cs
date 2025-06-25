using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.Utilities;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Assessment.GrowthItemTab
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_TeamAssessment")]
    public class GrowthItemTaNavigationTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void GrowthItemTaNavigation(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList()
                .FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();
            
            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamAssessmentGiTab.png");
            Assert.IsTrue(growthItemsPage.IsAddNewItemButtonDisplayed(), $"'Add New Item' button is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGrowthItemExportToExcel(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList()
                .FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();
            
            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.ClickExportToExcelButton();
           
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabExportToExcel.png");
            Assert.IsTrue(FileUtil.IsFileDownloaded("Growth Items.xlsx"), $"Excel file is not downloaded successfully for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGrowthItemAddNewItem(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList()
                .FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();
            
            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToGridView();
            growthItemsPage.ClickAddNewItemButton();
            
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabAddNewItemPopup.png");
            Assert.IsTrue(growthItemsPage.IsGrowthPlanItemPopupDisplayed(), $"'Add Growth plan Item' popup is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGrowthItemNotification(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList()
                .FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToGridView();
            growthItemsPage.ClickOnNotificationsButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabNotification.png");
            Assert.IsTrue(growthItemsPage.IsNotificationsPopupDisplayed(), $"'Notifications' popup is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyEditGrowthItem(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToGridView();
            growthItemsPage.ClickGrowthItemEditButton(growthItemsPage.GetGrowthItemFromGrid(1).Title);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\EditGrowthItem.png");
            Assert.IsTrue(growthItemsPage.IsEditGrowthPlanItemPopupDisplayed(), $"'Edit Growth Plan Items' popup is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGrowthItemSortingColumns(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var commonGridPage = new GridPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToGridView();
            growthItemsPage.SortGridColumn("Title");
            var actualColumnText = commonGridPage.GetGrowthItemsColumnValues("Title");
            var expectedColumnText = new CSharpHelpers().SortListAscending(actualColumnText);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiSortingColumn.png");
            for (var i = 0; i < expectedColumnText.Count; i++)
            {
                Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]} for env - {env}'");
                Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th {actualColumnText} text doesn't match during ascending");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void CreateGrowthItemKanbanView(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToKanbanView();
            growthItemsPage.ClickKanbanAddNewGrowthItem();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabAddNewItemPopup.png");
            Assert.IsTrue(addGrowthItemPopup.IsRadarTypeEnabled(), $"'Radar Type' dropdown is Disabled in 'Kanban' view for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void EditGrowthItemKanbanView(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);
            var growthItemTabKanbanWidgetPage = new GiDashboardKanbanWidgetPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToKanbanView();
            var growthItemDetails = growthItemsPage.GetGrowthItemFromGrid(1);
            growthItemTabKanbanWidgetPage.ClickEditKanbanGrowthItem(growthItemDetails.Title, growthItemDetails.Status);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabAddNewItemPopup.png");
            Assert.IsTrue(giDashboardKanbanView.IsEditGiPopupTitleDisplayed(), $"'Edit Growth plan Item' popup is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void CustomizeGrowthItemKanbanView(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);
            var growthItemKanbanViewWidget = new GrowthItemKanbanViewWidget(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToKanbanView();
            growthItemKanbanViewWidget.ShowAllStatusPanels();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabAddNewItemPopup.png");
            Assert.IsTrue(giDashboardKanbanView.IsColumnTitleListDisplayed(), $"Customized titles are not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void GrowthItemKanbanCategoryFilter(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToKanbanView();
            var categories = new List<string> { "Team", "Organizational", "Enterprise", "All" };

            foreach (var category in categories)
            {
                giDashboardKanbanView.FilterByCategory(category);
                TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabCategoriesFilter.png");
                Assert.AreEqual(category, giDashboardKanbanView.GetCategoryFilterText(),
                    $"Selected 'Radar Type' is not displayed for the client - {env}");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void GrowthItemKanbanRadarTypeFilter(string env)
        {
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();

            growthItemsPage.NavigateToTeamGrowthItemTabForProd(env, companyId, teamId);
            growthItemsPage.SwitchToKanbanView();
            giDashboardKanbanView.FilterBySurveyType("All");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GiTabAddNewItemPopup.png");
            Assert.AreEqual("All", giDashboardKanbanView.GetRadarTypeFilterText(), $"Selected 'Radar Type' is not displayed for the client - {env}");
        }
    }
}
