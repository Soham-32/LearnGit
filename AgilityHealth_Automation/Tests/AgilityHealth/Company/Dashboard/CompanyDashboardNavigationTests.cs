using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompanyDashboardPage = AgilityHealth_Automation.PageObjects.AgilityHealth.Company.CompanyDashboardPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Dashboard
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardNavigationTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_GoToTeamsDashboard()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnCompanyName(User.CompanyName);

            var breadcrumbs = topNav.GetBreadcrumbText();
            Log.Info("Verify that the user was taken to the Teams Dashboard");
            Assert.IsTrue(breadcrumbs.Contains(User.CompanyName), 
                $"The company name <{User.CompanyName}> was not found in the breadcrumbs <{breadcrumbs}.>");
            Assert.IsTrue(teamDashboardPage.DoesTeamsListExist(), "The Teams Dashboard did not load properly.");
        }
    }
}
