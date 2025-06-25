using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentRadarJumpLinksTests : BaseTest
    {
        private static SetupTeardownApi _setupApi;
        private static TeamHierarchyResponse _team;
        private static int _assessmentId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _setupApi = new SetupTeardownApi(TestEnvironment);
            _team = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam);
            _assessmentId = _setupApi.GetAssessmentResponse(SharedConstants.RadarTeam, SharedConstants.TeamHealth2Radar).Result.AssessmentId;
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug : 48955
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamRadarThumbnailsJumpLinks()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to assessment dashboard page for - {SharedConstants.RadarTeam} team and Verify assessment 'Radar Edit' functionality for - {SharedConstants.TeamHealth2Radar} assessment");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Edit");

            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/edit", Driver.GetCurrentUrl(), "'Radar edit' assessment page is not displayed");
            var assessmentHeaderName = taEditPage.GetAssessmentNameFromHeader();
            Assert.IsTrue(assessmentHeaderName.Contains(SharedConstants.TeamHealth2Radar), $" {SharedConstants.TeamHealth2Radar} - Assessment name is not present");

            Log.Info($"Click on 'Return To Dashboard' button and Verify assessment 'View Radar' functionality for - {SharedConstants.TeamHealth2Radar} assessment");
            taEditPage.ClickReturnToDashboardButton();
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "View Radar");
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/radar#radar", Driver.GetCurrentUrl(), "'Radar' is not displayed");
            var assessmentTitle = radarPage.GetRadarTitle();
            Assert.IsTrue(assessmentTitle.Contains(SharedConstants.TeamHealth2Radar), $" {SharedConstants.TeamHealth2Radar} - Assessment name is not present");

            Log.Info($"Navigate to assessment dashboard page for - {SharedConstants.Team} team and Verify assessment 'Radar Strengths' functionality for - {SharedConstants.TeamHealth2Radar} assessment");
            Driver.Navigate().Back();
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Strengths");
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/radar#strengths", Driver.GetCurrentUrl(), "'Strengths' is not displayed");
            assessmentTitle = radarPage.GetRadarTitle();
            Assert.IsTrue(assessmentTitle.Contains(SharedConstants.TeamHealth2Radar), $" {SharedConstants.TeamHealth2Radar} - Assessment name is not present");

            Log.Info($"Navigate to assessment dashboard page for - {SharedConstants.Team} team and Verify assessment 'Radar Impediments' functionality for - {SharedConstants.TeamHealth2Radar} assessment");
            Driver.Navigate().Back();
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Impediments");
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/radar#impediments", Driver.GetCurrentUrl(), "'Impediments' section is not displayed");
            assessmentTitle = radarPage.GetRadarTitle();
            Assert.IsTrue(assessmentTitle.Contains(SharedConstants.TeamHealth2Radar), $" {SharedConstants.TeamHealth2Radar} - Assessment name is not present");

            Log.Info($"Navigate to assessment dashboard page for - {SharedConstants.Team} team and Verify assessment 'Radar Growth Plan' functionality for - {SharedConstants.TeamHealth2Radar} assessment");
            Driver.Navigate().Back();
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Growth Plan");
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/radar#growth_plan", Driver.GetCurrentUrl(), "'Growth Item' section is not displayed");
            assessmentTitle = radarPage.GetRadarTitle();
            Assert.IsTrue(assessmentTitle.Contains(SharedConstants.TeamHealth2Radar), $" {SharedConstants.TeamHealth2Radar} - Assessment name is not present");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamRadarHeaderJumpLinks()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Verify all the header jump links");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);

            radarPage.ClickOnJumpLink("Radar");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("radar"), "'Radar' is not displayed");

            radarPage.ClickOnJumpLink("Analytics");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("analytics"), "'Analytics' is not displayed");

            radarPage.ClickOnJumpLink("Dimension Notes");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("notes"), "'Dimension Notes' is not displayed");

            radarPage.ClickOnJumpLink("Strengths");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("item0"), "'Strengths' is not displayed");

            radarPage.ClickOnJumpLink("Growth Opportunities");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("growth_opportunities"), "'Growth Opportunities' is not displayed");

            radarPage.ClickOnJumpLink("Impediments");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("impediments"), "'Impediments' is not displayed");

            radarPage.ClickOnJumpLink("Growth Plan");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("growth_plan"), "'Growth Plan' is not displayed");

            radarPage.ClickOnJumpLink("Metrics Summary");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("metrics_summary"), "'Metrics Summary' is not displayed");
        }
    }
}
