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
    public class MultiTeamRadarTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyMultiRadarPageNavigationInProd(string env)
        {
            var mtEtRadarCommonPage = new MtEtRadarCommonPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var multiTeamId =
                ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.MultiTeam.TeamId).ToList().FirstOrDefault();
            var radarId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.MultiTeam.RadarId)
                .ToList().FirstOrDefault();

            mtEtRadarCommonPage.NavigateToMultiTeamRadarPageForProd(env, multiTeamId, radarId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\MultiTeamRadar.png");
            Assert.IsTrue(radarPage.IsRadarIconOnHeaderPresent(), "Multi Team radar is not displayed");
        }
    }
}
