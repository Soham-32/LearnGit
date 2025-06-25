using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Radar
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Radars")]
    public class EnterpriseTeamRadarTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyEnterpriseTeamRadarPageNavigationInProd(string env)
        {
            if (env == "nscorp" || env == "raytheon" || env == "usaf") { return; }
            var mtEtRadarCommonPage = new MtEtRadarCommonPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var enterpriseTeamId =
                ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.EnterpriseTeam.TeamId).ToList().FirstOrDefault();
            var radarId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.EnterpriseTeam.RadarId)
                .ToList().FirstOrDefault();

            mtEtRadarCommonPage.NavigateToMultiTeamRadarPageForProd(env, enterpriseTeamId, radarId, "enterprise");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\EnterpriseTeamRadar.png");
            Assert.IsTrue(radarPage.IsRadarIconOnHeaderPresent(), "Enterprise Team radar is not displayed");
        }
    }
}
