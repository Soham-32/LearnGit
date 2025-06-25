using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Assessment
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_TalentDevelopment")]
    public class IndividualRollUpRadarTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void RollUpRadarForIndividualAssessmentsPageNavigationTestInProd(string env)
        {
            if (env == "prudential" || env == "nscorp" || env == "7eleven" || env == "nasa" || env == "usaf") { return; }

            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var rollUpRadarId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env))
                .Select(a => a.GoiTeam.RollupRadarId).ToList().FirstOrDefault();
            teamAssessmentDashboard.NavigateToRollUpRadarPage(env, rollUpRadarId);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\IARollupRadar.png");
            Assert.IsTrue(radarPage.IsRadarIconOnHeaderPresent());
        }
    }
}
