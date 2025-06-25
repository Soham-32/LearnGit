using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards.Insights
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Insights")]
    public class EnterpriseAgilityTabTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyInsightsEnterpriseAgilityPageNavigationInProd(string env)
        {
            if (env == "eu") { return; }
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            enterpriseAgilityPage.NavigateToInsightsEnterpriseAgilityTabPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\EnterpriseAgilityTab.png", 30000);
            Assert.AreEqual("ENTERPRISE AGILITY", enterpriseAgilityPage.GetEnterpriseAgilityTabText(), $"Enterprise Agility tab setting button is not matched after navigating in 'Structural Agility tab' for the client - {env}");
        }
    }
}
