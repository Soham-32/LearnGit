using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardMultiTeamLinkTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader")]
        [TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_TeamLink_MultiTeam()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            const string team = Constants.MultiTeamForGrowthJourney;
            
            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.SearchTeam(team);
            dashboardPage.GoToTeamAssessmentDashboard(1);

            Assert.AreEqual(team.ToLower(), mtDashboardPage.GetTeamName().ToLower(),
                "User not taken to Multi-Team Dashboard.");
        }
    }
}