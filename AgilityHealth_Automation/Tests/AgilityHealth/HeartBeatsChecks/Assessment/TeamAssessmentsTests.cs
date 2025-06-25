using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Assessment
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_TeamAssessment")]
    public class TeamAssessmentsTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyTeamAssessmentPageNavigationInProd(string env)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList().First();
            teamAssessmentDashboard.NavigateToTeamAssessmentPage(env, teamId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamAssessment.png");
            Assert.IsTrue(teamAssessmentDashboard.IsAssessmentActive(), $"Team Assessment Tab is not active for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyEditTeamAssessmentPageNavigationInProd(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var commonGridPage = new GridPage(Driver, Log);

            Log.Info($"Login to {env}, Navigate to the Team Dashboard page, select the first team from the list.");
            LoginToProductionEnvironment(env);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);
            teamDashboardPage.GridTeamView();
            teamDashboardPage.FilterTeamType("Team");
            commonGridPage.SortGridColumn("Number of Team Assessments",true);
            teamDashboardPage.SelectFirstTeamFromDashboard();

            Log.Info($"Click on the first radar in the Assessment Dashboard & verify edit assessment page is displayed for the '{env}' environment.");
            teamAssessmentDashboard.EditFirstRadarFromAssessmentDashboard();
            Assert.IsTrue(taEditPage.IsReturnToDashboardButtonDisplayed(), $"'Return To Dashboard' button is not displayed for the '{env}' environment.");
        }
    }
}
