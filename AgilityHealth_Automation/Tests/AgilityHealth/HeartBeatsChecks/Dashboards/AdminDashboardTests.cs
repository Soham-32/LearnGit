using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Admin;
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
    public class AdminDashboardTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyAdminDashboardPageNavigationInProd(string env)
        {
            var adminDashboardCommonPage = new AdminDashboardCommon(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            adminDashboardCommonPage.NavigateToAdminDashboardPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\AdminDashboard.png");
            Assert.AreEqual("Admin Dashboard", adminDashboardCommonPage.GetAdminDashboardTitle(), $"Admin Dashboard Title does not matched after navigating in 'Admin Dashboard' for the client - {env}");
        }
    }
}
