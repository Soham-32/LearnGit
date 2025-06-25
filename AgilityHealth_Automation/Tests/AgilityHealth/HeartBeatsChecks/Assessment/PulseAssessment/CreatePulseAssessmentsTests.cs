using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Assessment.PulseAssessment
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Pulse")]
    public class CreatePulseAssessmentsTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [DataRow("appx")]
        [DataRow("citi")]
        [DataRow("app")]
        public void VerifyTeamLevelPulseAssessmentForProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var pulseTeamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().FirstOrDefault();
            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, pulseTeamId);
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();
            
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\PulseAssessment.png");
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), $"Pulse radio button is not displayed for the client - {env}");
        }

        [DataTestMethod]
        [DataRow("appx")]
        [DataRow("citi")]
        [DataRow("app")]
        public void VerifyMtLevelPulseAssessmentForProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var mtPulseTeamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.MultiTeam.TeamId).ToList().FirstOrDefault();
            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, mtPulseTeamId);
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\PulseAssessment.png");
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), $"Pulse radio button is not displayed for the client - {env}");
        }

        [DataTestMethod]
        [DataRow("appx")]
        [DataRow("citi")]
        [DataRow("app")]
        public void VerifyEtLevelPulseAssessmentForProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var etPulseTeamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.EnterpriseTeam.TeamId).ToList().FirstOrDefault();
            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, etPulseTeamId);
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\PulseAssessment.png");
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), $"Pulse radio button is not displayed for the client - {env}");
        }
    }
}
