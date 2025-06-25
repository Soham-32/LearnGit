using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPortal;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Reports;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.PageObjects.Learn;
using AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Common
{
    [TestClass]
    [TestCategory("Common")]
    public class V2CompanyDashboardTopNavigationTests : NewNavBaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Sanity")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void V2_CompanyDashboard_TopNavigationLink()
        {
            var login = new LoginPage(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var reports = new ReportsDashboardPage(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);
            var v2Settings = new SettingsV2Page(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var insightsDashboardPage = new InsightsDashboardPage(Driver, Log);
            const string companyName = "Automation_UserPermission (DO NOT USE)";
            var companyId = User.IsPartnerAdmin() ? 107 : 2;

            Log.Info("Navigate to company dashboard and verify all links from top");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            //Insights
            Log.Info("From V2, navigate to 'Insights' link and verify current url & widget name.");
            companyDashboardPage.WaitUntilLoaded();
            v2Header.ClickInsightsButton();
            insightsDashboardPage.ClickOnTeamAgilityTab();
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("V2/insights"), "Url does not contains 'V2/insights'.");
            teamAgilityPage.WaitUntilWidgetsLoaded();
            Assert.IsTrue(teamAgilityPage.IsWidgetDisplayed(TeamAgilityWidgets.AgilityIndex), "Widget does not displayed.");

            //Business Outcomes
            Log.Info("From V2, navigate to 'Business Outcomes' link and verify current url & column text.");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.WaitUntilLoaded();
            v2Header.ClickOnBusinessOutComeLink();
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("V2/outcomes"), "Url does not contains 'V2/outcomes'.");
            Assert.IsTrue(businessOutcomesDashboard.IsPageLoadedCompletely(), "Business Outcome page does not loaded successfully.");

            if (User.IsSiteAdmin())
            {
                //Growth Plan
                Log.Info("From V2, navigate to 'Growth Plan' link and verify current url.");
                companyDashboardPage.NavigateToPage();
                companyDashboardPage.WaitUntilLoaded();
                v2Header.ClickOnGrowthPlanLink();
                Assert.IsTrue(Driver.GetCurrentUrl().Contains("v2/growthplan"), "Url does not contains 'v2/growthplan'.");


                //Growth Portal
                Log.Info("From V2, navigate to 'Growth Portal' link and verify current url & page title.");
                companyDashboardPage.NavigateToPage();
                companyDashboardPage.WaitUntilLoaded();
                v2Header.ClickOnGrowthPortalLink();
                Assert.IsTrue(Driver.GetCurrentUrl().Contains("growthportal"), "Url does not contains 'growthportal'.");
                Assert.IsTrue(growthPortalPage.IsPageTitleDisplayed(), "Page title does not displayed.");
            }

            //Reports
            Log.Info("From V2, navigate to 'Reports' link and verify current url & page title.");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.WaitUntilLoaded();
            v2Header.ClickOnReportsLink();
            reports.SelectCompany(companyName);
            Assert.AreEqual(reports.GetPageUrl(107, false), Driver.GetCurrentUrl(), "Url doesn't matched.");
            Assert.IsTrue(reports.IsPageTitleDisplayed(), "Page title does not displayed.");

            //Settings
            Log.Info("From V2, navigate to 'Settings' link and verify current url & setting option.");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.WaitUntilLoaded();
            v2Header.ClickOnSettingsLink();
            const string buttonName = "Manage Features";
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("V2/settings"), "Url does not contains 'V2/settings'.");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button does not present.");

            //Support Center
            if (!TestEnvironment.EnvironmentName.Equals("atqa")) return;
            Log.Info("From V2, navigate to 'Support Center' link and verify current url.");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.WaitUntilLoaded();
            v2Header.ClickOnSupportCenterLink();
            Driver.SwitchToLastWindow();
            Assert.AreEqual("https://support.agilityinsights.ai/hc/en-us", Driver.GetCurrentUrl(), "Url doesn't matched.");

        }

        [DataTestMethod]
        [DataRow("srca")]
        public void SsoTestingForLearnMenuNavigationForSrca(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var homePage = new HomePage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();

            Log.Info("Navigate to the 'Learn' menu and verify details");
            SwitchToNewNav();
            teamDashboardPage.NavigateToNewNavTeamDashboard(env, companyId);
            teamDashboardPage.ClickOnLearnTab();
            Driver.SwitchToLastWindow();

            Assert.IsTrue(homePage.IsAllCoursesTabDisplayed(), $"'All Courses' on learn tab is not displayed for the client - {env}");
            Assert.IsTrue(homePage.IsMyDashboardTabDisplayed(), $"'My Dashboard' on learn tab is not displayed for the client - {env}");
            Assert.IsTrue(homePage.IsWelcomeTitleDisplayed(), $"'Welcome to Agility University' title on learn tab is not displayed for the client - {env}");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SsoTestingForLearnMenuNavigationForSrca.png", 10000);
        }
    }
}
