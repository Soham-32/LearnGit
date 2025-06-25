using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomeNavigationTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_TopNavigationLinks()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            topNav.ClickOnBusinessOutComeLink();
            Assert.IsTrue(boDashboard.IsPageLoadedCompletely(), "Business Outcome page isn't loaded successfully.Stopping execution");

            v2Header.ClickOnGrowthPortalLink();
            Log.Info("Verify user is navigated to Growth Portal page");
            Assert.IsTrue(Driver.GetCurrentUrl().EndsWith("/growthportal"), $"User isn't navigated to Growth Portal page, instead navigated to {Driver.GetCurrentUrl()}");

            boDashboard.NavigateToPage(Company.Id);
            v2Header.ClickOnSettingsLink();
            Log.Info("Verify user is navigated to Settings page");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/V2/settings/company/"), $"User isn't navigated to Settings page, instead navigated to {Driver.GetCurrentUrl()}");

            boDashboard.NavigateToPage(Company.Id);
            v2Header.ClickOnSupportCenterLink();
            Log.Info("Verify user is navigated to Support Center page");
            Driver.SwitchToLastWindow();
            Assert.IsTrue(Driver.GetCurrentUrl().Equals("https://support.agilityinsights.ai/hc/en-us"), $"User isn't navigated to Support Center page, instead navigated to {Driver.GetCurrentUrl()}");
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_SmokeTest()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            topNav.ClickOnBusinessOutComeLink();
            Assert.IsTrue(boDashboard.IsPageLoadedCompletely(), "Business Outcome page isn't loaded successfully.");
        }
    }
}
