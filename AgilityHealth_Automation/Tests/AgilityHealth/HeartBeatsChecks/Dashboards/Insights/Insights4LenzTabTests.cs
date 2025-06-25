using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards.Insights
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Insights")]
    public class Insights4LenzTabTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json")
            .DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [DataRow("citi")]
        [DataRow("amtrak")]
        [DataRow("app")]

        public void VerifyInsights4LenzDashboardPageNavigationInProd(string env)
        {
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList()
                .FirstOrDefault();
            LoginToProductionEnvironment(env);

            enterpriseAgilityPage.NavigateToInsights4LenzDashboardTabPageForProd(env, companyId);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\4LenzDashbord.png",15000);
            Assert.AreEqual("4 LENZ DASHBOARD", enterpriseAgilityPage.Get4LenzDashboardTabText(),
                $"4-Lenz dashboard tab setting button is not matched after navigating in 'Structural Agility tab' for the client - {env}");
        }
    }
}
