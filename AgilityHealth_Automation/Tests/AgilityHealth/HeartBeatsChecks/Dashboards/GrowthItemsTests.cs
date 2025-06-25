using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Dashboard")]
    public class GrowthItemsTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGrowthItemsDashboardPageNavigationInProd(string env)
        {
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            growthItemsDashboardPage.NavigateToGrowthItemsPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GrowthItemDashboard.png");
            Assert.AreEqual("Growth Items", growthItemsDashboardPage.GetGrowthItemsDashboardTitle(), $"Growth Items dashboard Title does not matched after navigating in 'Growth Items Dashboard' for the client - {env}");
        }
        
        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void Verify_KanbanView(string env)
        {
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            growthItemsDashboardPage.NavigateToGrowthItemsPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GrowthItemDashboard.png");

            Log.Info("Switching to Kanban View");
            growthItemsDashboardPage.ChangeViewWidget(GrowthItemWidget.Kanban);
            Assert.IsTrue(growthItemsDashboardPage.IsKanbanBoardDisplayed(), "Failure !! Unable to verify the Kanban View On Growth Items Page");
        }
    }
}
