using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
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
    public class StructuralAgilityTabTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyInsightsStructuralAgilityPageNavigationInProd(string env)
        {
            var structuralAgilityPage = new StructuralAgilityPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            structuralAgilityPage.NavigateToInsightsStructuralAgilityPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\StructuralAgilityTab.png", 25000);
            Assert.AreEqual("STRUCTURAL AGILITY", structuralAgilityPage.GetStructuralAgilityTabText(), $"Structural Agility tab setting button is not matched after navigating in 'Structural Agility tab' for the client - {env}");
        }
    }
}
