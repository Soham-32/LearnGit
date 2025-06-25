using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("TeamDashboard"), TestCategory("NewNavigation")]
    public class FilterTeamTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [TestCategory("FilterTeamType")]
        public void TeamDashboard_GridView_FilterByTeamType()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SwitchToGridView();

            teamDashboardPage.FilterTeamType("Team");

            Assert.IsTrue(teamDashboardPage.CheckTeamFilterCorrectly("Team"),
                "Failure !! Filtered data displays incorrectly for Team");

            teamDashboardPage.FilterTeamType("Multi-Team");

            Assert.IsTrue(teamDashboardPage.CheckTeamFilterCorrectly("Multi-Team"),
                "Failure !! Filtered data displays incorrectly for Multi-Team");

            teamDashboardPage.FilterTeamType("Portfolio Team");

            Assert.IsTrue(teamDashboardPage.CheckTeamFilterCorrectly("Portfolio Team"),
                "Failure !! Filtered data displays incorrectly for Portfolio Team");

            teamDashboardPage.FilterTeamType("Enterprise Team");

            Assert.IsTrue(teamDashboardPage.CheckTeamFilterCorrectly("Enterprise Team"),
                "Failure !! Filtered data displays incorrectly for Enterprise Team");
        }
    }
}
