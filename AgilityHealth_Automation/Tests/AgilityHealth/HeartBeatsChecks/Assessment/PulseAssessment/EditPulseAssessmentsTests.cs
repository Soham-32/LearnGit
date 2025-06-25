using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Assessment.PulseAssessment
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Pulse")]
    public class EditPulseAssessmentsTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [DataRow("appx")]
        [DataRow("citi")]
        [DataRow("app")]
        public void VerifyTeamLevelEditPulseAssessmentForProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var pulseTeamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.PulseTeamId).ToList().FirstOrDefault();

            var pulseName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env))
                .Select(a => a.Team.PulseName).ToList().LastOrDefault();

            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, pulseTeamId);
            teamAssessmentDashboard.SwitchToPulseAssessment();
            teamAssessmentDashboard.ClickOnPulseEditLink(pulseName);
            
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\PulseAssessment.png");
            Assert.IsTrue(editRecipientsPage.Header.IsEditPulseCheckTitleDisplayed(), $"'Edit pulse check' title is not displayed for the client - {env}");
        }

        [DataTestMethod]
        [DataRow("citi")]
        [DataRow("app")]
        public void VerifyMtLevelEditPulseAssessmentForProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var mtPulseTeamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.MultiTeam.TeamId).ToList().FirstOrDefault();

            var pulseName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env))
                .Select(a => a.MultiTeam.PulseName).ToList().LastOrDefault();

            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, mtPulseTeamId);
            teamAssessmentDashboard.SwitchToPulseAssessment();
            teamAssessmentDashboard.ClickOnPulseEditLink(pulseName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\PulseAssessment.png");
            Assert.IsTrue(editRecipientsPage.Header.IsEditPulseCheckTitleDisplayed(), $"'Edit pulse check' title is not displayed for the client - {env}");
        }

        [DataTestMethod]
        [DataRow("citi")]
        [DataRow("app")]
        public void VerifyEtLevelEditPulseAssessmentForProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var etPulseTeamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.EnterpriseTeam.TeamId).ToList().FirstOrDefault();

            var pulseName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env))
                .Select(a => a.EnterpriseTeam.PulseName).ToList().LastOrDefault();

            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, etPulseTeamId);
            teamAssessmentDashboard.SwitchToPulseAssessment();
            teamAssessmentDashboard.ClickOnPulseEditLink(pulseName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\PulseAssessment.png");
            Assert.IsTrue(editRecipientsPage.Header.IsEditPulseCheckTitleDisplayed(), $"'Edit pulse check' title is not displayed for the client - {env}");
        }
    }
}
