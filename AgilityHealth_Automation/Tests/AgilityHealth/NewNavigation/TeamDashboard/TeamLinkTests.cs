using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("TeamDashboard"), TestCategory("NewNavigation")]
    public class TeamLinkTests : BaseTest
    {
        [TestMethod]
        [TestCategory("TeamLink")]
        [TestCategory("CompanyAdmin")]
        public void TeamDashboard_TeamLink_NormalTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Enter team name and search");
            teamDashboardPage.SwitchToGridView();
            Driver.SwitchTo().DefaultContent();
            teamDashboardPage.SearchTeam(SharedConstants.Team);

            Log.Info("Click on team avatar");
            teamDashboardPage.ClickOnTeamAvatarIcon(SharedConstants.Team);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.Team} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.Team} doesn't show after clicking on team avatar");

            Log.Info("Navigate back to team dashboard page");
            Driver.SwitchTo().DefaultContent();
            teamDashboardPage.NavigateToPage(Company.Id);

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(SharedConstants.Team);

            Log.Info("Click on team name");
            teamDashboardPage.ClickOnTeamName(SharedConstants.Team);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.Team} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.Team} doesn't show after clicking on team name");
        }

        [TestMethod]
        [TestCategory("TeamLink")]
        [TestCategory("CompanyAdmin")]
        public void TeamDashboard_TeamLink_MultiTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Enter team name and search");
            teamDashboardPage.SwitchToGridView();
            Driver.SwitchTo().DefaultContent();
            teamDashboardPage.SearchTeam(SharedConstants.MultiTeam);
            
            Log.Info("Click on team avatar");
            teamDashboardPage.ClickOnTeamAvatarIcon(SharedConstants.MultiTeam);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.MultiTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.MultiTeam} doesn't show after clicking on team avatar");

            Log.Info("Navigate back to team dashboard page");
            teamDashboardPage.NavigateToPage(Company.Id);

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(SharedConstants.MultiTeam);

            Log.Info("Click on team name");
            teamDashboardPage.ClickOnTeamName(SharedConstants.MultiTeam);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.MultiTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.MultiTeam} doesn't show after clicking on team name");
        }

        [TestMethod]
        [TestCategory("TeamLink")]
        [TestCategory("CompanyAdmin")]
        public void TeamDashboard_TeamLink_PortfolioTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Enter team name and search");
            teamDashboardPage.SwitchToGridView();
            Driver.SwitchTo().DefaultContent();
            teamDashboardPage.SearchTeam(SharedConstants.PortfolioTeam);

            Log.Info("Click on team avatar");
            teamDashboardPage.ClickOnTeamAvatarIcon(SharedConstants.PortfolioTeam);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.PortfolioTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.PortfolioTeam} doesn't show after clicking on team avatar");

            Log.Info("Navigate back to team dashboard page");
            teamDashboardPage.NavigateToPage(Company.Id);

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(SharedConstants.PortfolioTeam);

            Log.Info("Click on team name");
            teamDashboardPage.ClickOnTeamName(SharedConstants.PortfolioTeam);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.PortfolioTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.PortfolioTeam} doesn't show after clicking on team name");
        }

        [TestMethod]
        [TestCategory("TeamLink")]
        [TestCategory("CompanyAdmin")]
        public void TeamDashboard_TeamLink_EnterpriseTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Enter team name and search");
            teamDashboardPage.SwitchToGridView();
            Driver.SwitchTo().DefaultContent();
            teamDashboardPage.SearchTeam(SharedConstants.EnterpriseTeam);

            Log.Info("Click on team avatar");
            teamDashboardPage.ClickOnTeamAvatarIcon(SharedConstants.EnterpriseTeam);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.EnterpriseTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.EnterpriseTeam} doesn't show after clicking on team avatar");

            Log.Info("Navigate back to team dashboard page");
            teamDashboardPage.NavigateToPage(Company.Id);

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(SharedConstants.EnterpriseTeam);

            Log.Info("Click on team name");
            teamDashboardPage.ClickOnTeamName(SharedConstants.EnterpriseTeam);

            Log.Info("Verify that team profile page displays");
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.EnterpriseTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"{SharedConstants.EnterpriseTeam} doesn't show after clicking on team name");
        }
    }
}
