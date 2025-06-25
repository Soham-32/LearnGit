using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.LeftNav
{
    [TestClass]
    [TestCategory("LeftNav"), TestCategory("NewNavigation")]
    public class LeftNavTeamStructureTests : BaseTest
    {
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA", true);

        [TestMethod]
        [TestCategory("Search")]
        [TestCategory("CompanyAdmin")]
        public void LeftNav_SearchTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            const string partialSearchText = "Automation";
            leftNavPage.SearchTeam(partialSearchText);
            var searchResult = leftNavPage.GetHighlightedResult();

            Assert.IsTrue(searchResult.Count > 0, "There is no result in the search");

            foreach (var result in searchResult)
            {
                Assert.AreEqual(partialSearchText, result, "Search result doesn't match");
            }

            const string fullSearchText = SharedConstants.Team;
            leftNavPage.SearchTeam(fullSearchText);
            searchResult = leftNavPage.GetHighlightedResult();

            Assert.IsTrue(searchResult.Count > 0, "There is no result in the search");

            foreach (var result in searchResult)
            {
                Assert.AreEqual(fullSearchText, result, "Search result doesn't match");
            }
        }

        [TestMethod]
        [TestCategory("ExpandCollapse")]
        [TestCategory("CompanyAdmin")]
        public void LeftNav_ExpandAndCollapse()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            leftNavPage.ExpandTeam(SharedConstants.EnterpriseTeam);

            Assert.IsTrue(leftNavPage.DoesTeamDisplay(SharedConstants.PortfolioTeam), $"{SharedConstants.PortfolioTeam} doesn't show after expanding");

            leftNavPage.ExpandTeam(SharedConstants.PortfolioTeam);

            Assert.IsTrue(leftNavPage.DoesTeamDisplay(SharedConstants.MultiTeam), $"{SharedConstants.MultiTeam} doesn't show after expanding");

            leftNavPage.CollapseTeam(SharedConstants.PortfolioTeam);

            Assert.IsFalse(leftNavPage.DoesTeamDisplay(SharedConstants.MultiTeam), $"{SharedConstants.MultiTeam} still shows after collapsing");

            leftNavPage.CollapseTeam(SharedConstants.EnterpriseTeam);

            Assert.IsFalse(leftNavPage.DoesTeamDisplay(SharedConstants.PortfolioTeam), $"{SharedConstants.PortfolioTeam} still shows after expanding");

            leftNavPage.CollapseTeam(User.CompanyName);
            Assert.IsFalse(leftNavPage.DoesTeamDisplay(SharedConstants.EnterpriseTeam), $"{SharedConstants.EnterpriseTeam} still shows after expanding");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("SelectTeam")]
        [TestCategory("CompanyAdmin")]
        public void LeftNav_SelectTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamProfileTabPage = new TeamProfileTabPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            leftNavPage.ClickOnATeam(SharedConstants.EnterpriseTeam);
            teamProfileTabPage.ClickOnTeamProfileTab();
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.EnterpriseTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"Page title doesn't show properly for the selected team {SharedConstants.EnterpriseTeam}");

            Driver.SwitchTo().DefaultContent();
            leftNavPage.ClickOnATeam(SharedConstants.PortfolioTeam);
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.PortfolioTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"Page title doesn't show properly for the selected team {SharedConstants.PortfolioTeam}");

            Driver.SwitchTo().DefaultContent();
            leftNavPage.ClickOnATeam(SharedConstants.MultiTeam);
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.MultiTeam} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"Page title doesn't show properly for the selected team {SharedConstants.MultiTeam}");

            Driver.SwitchTo().DefaultContent();
            leftNavPage.ClickOnATeam(SharedConstants.Team);
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual($"{SharedConstants.Team} (Team Profile)", teamDashboardPage.TeamBase.GetActiveTeamProfilePageTitle(), $"Page title doesn't show properly for the selected team {SharedConstants.Team}");

            Driver.SwitchTo().DefaultContent();
             teamDashboardPage.ClickOnGainInsightsTab();
            teamDashboardPage.SwitchToIframeForNewNav();
            Assert.AreEqual("Gain Insights", teamDashboardPage.GetActiveMainTab(), "Gain Insights tab should be active");

            leftNavPage.ClickOnATeam(SharedConstants.MultiTeam);
            Assert.AreEqual("Gain Insights", teamDashboardPage.GetActiveMainTab(), "Gain Insights tab should be active");

            leftNavPage.ClickOnATeam(SharedConstants.PortfolioTeam);
            Assert.AreEqual("Gain Insights", teamDashboardPage.GetActiveMainTab(), "Gain Insights tab should be active");

            leftNavPage.ClickOnATeam(SharedConstants.EnterpriseTeam);
            Assert.AreEqual("Gain Insights", teamDashboardPage.GetActiveMainTab(), "Gain Insights tab should be active");
        }

        [TestMethod]
        [TestCategory("SelectCompany")]
        [TestCategory("SiteAdmin")]
        public void LeftNav_SelectCompany()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            var caUser = SiteAdminUserConfig.GetUserByDescription("user 2");
            teamDashboardPage.NavigateToPage(Company.Id);

            teamDashboardPage.ClickOnGainInsightsTab();
            Assert.AreEqual("Gain Insights", teamDashboardPage.GetActiveMainTab(), "Gain Insights tab should be active");

            leftNavPage.SelectCompany(caUser.CompanyName);
            Assert.AreEqual("Gain Insights", teamDashboardPage.GetActiveMainTab(), "Gain Insights tab should be active");
        }
    }
}
