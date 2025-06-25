using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Assessment
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_TalentDevelopment")]
    public class IndividualParticipantRadarTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void ParticipantRadarForIndividualAssessmentsPageNavigationTestInProd(string env)
        {
            if (env == "prudential" || env == "nscorp" || env == "7eleven" || env == "nasa" || env == "usaf") { return; }
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.GoiTeam.TeamId).ToList().FirstOrDefault();
            var participantRadarId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env))
                .Select(a => a.GoiTeam.ParticipantRadarId).ToList().FirstOrDefault();

            teamAssessmentDashboard.NavigateToParticipantRadarPage(env, teamId, participantRadarId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\IAParticipantRadar.png");
            Assert.IsTrue(teamAssessmentDashboard.IsParticipantRadarTabDisplayed(), "Participant radar is not displayed");
        }
    }
}
