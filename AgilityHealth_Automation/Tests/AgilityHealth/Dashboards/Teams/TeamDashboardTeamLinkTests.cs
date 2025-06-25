using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardTeamLinkTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader")]
        [TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_TeamLink_NormalTeam()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            const string team = SharedConstants.Team;
            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.SearchTeam(team);
            dashboardPage.GoToTeamAssessmentDashboard(1);

            Assert.AreEqual($"{team} Team Assessments", teamAssessmentDashboard.GetAssessmentDashboardTitle(),
                "User not taken to Team Assessment Dashboard.");
        }
    }
}