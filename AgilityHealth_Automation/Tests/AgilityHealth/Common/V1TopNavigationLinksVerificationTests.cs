using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPortal;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Reports;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Common
{
    [TestClass]
    [TestCategory("Common")]
    public class V1TopNavigationLinksVerificationTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Sanity")]
        [TestCategory("KnownDefect")] // Bug : 47508 
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"),TestCategory("OrgLeader"),TestCategory("Member")]
        public void V1_TopNavigation_Links_Verification()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var reportsDashboardPage = new ReportsDashboardPage(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var selectCompanyPage = new SelectCompanyPage(Driver);


            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            if (User.IsMember() && TestEnvironment.EnvironmentName.Equals("atqa"))
            {
                selectCompanyPage.SelectCompany(User.CompanyName);
            }

            if (User.IsCompanyAdmin())
            {
                //Business Outcomes
                Log.Info("From V1, navigate to 'Business Outcomes' link and verify current url & column text.");
                dashboardPage.NavigateToPage(Company.Id);
                topNav.ClickOnBusinessOutComeLink();
                businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
                Assert.AreEqual(businessOutcomesDashboard.GetPageUrl(Company.Id), Driver.GetCurrentUrl(), "Url doesn't matched.");
                Assert.IsTrue(businessOutcomesDashboard.IsPageLoadedCompletely(), "Business Outcome page isn't loaded successfully.");

                //Reports
                Log.Info("From V1, navigate to 'Reports' link and verify current url & page title ");
                dashboardPage.NavigateToPage(Company.Id);
                topNav.ClickOnReportsLink();
                Assert.AreEqual(reportsDashboardPage.GetPageUrl(Company.Id), Driver.GetCurrentUrl(), "Url doesn't matched.");
                Assert.IsTrue(reportsDashboardPage.IsPageTitleDisplayed(), "Page title does not displayed.");
            }

            //Insights
            Log.Info("From V1, navigate to 'Insights' link and verify current url & widget name.");
            dashboardPage.NavigateToPage(Company.Id);
            topNav.ClickOnInsightsDashboardLink();
            teamAgilityPage.WaitUntilWidgetsLoaded();
            if (User.IsCompanyAdmin())
            {
                Assert.AreEqual(teamAgilityPage.GetPageUrl(Company.Id), Driver.GetCurrentUrl(), "Url doesn't matched.");
            }
            Assert.IsTrue(teamAgilityPage.IsWidgetDisplayed(TeamAgilityWidgets.AgilityIndex), "Widget does not display.");
            
            //Growth Portal
            Log.Info("From V1, navigate to 'Growth Portal' link and verify current url & page title ");
            dashboardPage.NavigateToPage(Company.Id);
            topNav.ClickOnGrowthPortalLink();
            Driver.SwitchToLastWindow();
            Assert.AreEqual(growthPortalPage.GetPageUrl(Company.Id, false), Driver.GetCurrentUrl(), "Url doesn't matched.");
            Assert.IsTrue(growthPortalPage.IsPageTitleDisplayed(), "Page title does not displayed.");
            
            //Settings
            Log.Info("From V1, navigate to 'Settings' link and verify current url & setting option ");
            dashboardPage.NavigateToPage(Company.Id);
            topNav.ClickOnSettingsLink();
            const string buttonName = "View Feature Settings";
            Assert.AreEqual(settingsPage.GetPageUrl(Company.Id), Driver.GetCurrentUrl(), "Url doesn't matched.");
            if (User.IsCompanyAdmin())
            {
                Assert.IsTrue(settingsPage.IsSettingOptionPresent(buttonName), $"'{buttonName}' button does not present.");
            }

            //Support Center
            Log.Info("From V1, navigate to 'Support Center' link and verify current url.");
            dashboardPage.NavigateToPage(Company.Id);
            topNav.ClickOnSupportCenterLink();
            Driver.SwitchToLastWindow();
            Assert.AreEqual("https://support.agilityinsights.ai/hc/en-us", Driver.GetCurrentUrl(), "Url doesn't matched.");
        }
    }
}
