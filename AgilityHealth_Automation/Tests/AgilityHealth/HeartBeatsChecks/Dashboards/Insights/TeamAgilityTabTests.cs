using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards.Insights
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Insights")]
    public class TeamAgilityTabTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [DataRow("eu")]
        [DataRow("federalreserve")]
        [DataRow("citi")]
        public void VerifyInsightsTeamAgilityPageNavigationInProd(string env)
        {
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            teamAgilityPage.NavigateToInsightsTeamAgilityPageForProd(env, companyId);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamAgilityTab.png", 25000);
            Assert.AreEqual("TEAM AGILITY", teamAgilityPage.GetTeamAgilityTabText(), $"Team Agility tab radar title does not matched after navigating in 'Team Agility tab' for the client - {env}");
        }
    }
}
