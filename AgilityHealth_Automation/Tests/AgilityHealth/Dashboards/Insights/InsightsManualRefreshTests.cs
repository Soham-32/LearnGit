using System;
using System.Globalization;
using AgilityHealth_Automation.Base;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("Dashboard")]
    public class InsightsManualRefreshTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_ManualRefresh()
        {
            var insightsUser = User.IsSiteAdmin() || User.IsPartnerAdmin()
                ? User : TestEnvironment.UserConfig.GetUserByDescription("insights user");

            var login = new LoginPage(Driver, Log);
            var insightsDashboard = new InsightsDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(insightsUser.Username, insightsUser.Password);

            insightsDashboard.NavigateToStructuralAgilityPage(Company.InsightsId);
            insightsDashboard.ClickRefreshButton();

            Log.Info("Verify the Last sync date updates after clicking 'Refresh'");
            var refreshDateTimeString = insightsDashboard.GetLastSyncDate();

            var match = Regex.Match(refreshDateTimeString, @"Data as of (\d{1,2}/\d{1,2}) \((\d{1,2}:\d{2} [AP]M) (UTC|GMT([+-]\d{1,2}:\d{2}))\)");

            var date = match.Groups[1].Value;
            var time = match.Groups[2].Value;
            var zone = match.Groups[3].Value == "UTC" ? "+00:00" : match.Groups[4].Value;
            var datetime = DateTimeOffset.ParseExact($"{date}/{DateTime.Now.Year} {time} {zone}", "M/d/yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
            var diffMins = Math.Abs((DateTimeOffset.UtcNow - datetime.ToUniversalTime()).TotalMinutes);
            Assert.IsTrue(diffMins < 10, $"Dashboard not refreshed in last {diffMins:F1} minutes");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Dashboard not refreshed in ATQA Bug ID: 53581
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_EnterpriseAgilityManualRefresh()
        {
            var insightsUser = User.IsSiteAdmin() || User.IsPartnerAdmin()
                ? User : TestEnvironment.UserConfig.GetUserByDescription("insights user");

            var login = new LoginPage(Driver, Log);
            var insightsDashboard = new InsightsDashboardPage(Driver, Log);
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(insightsUser.Username, insightsUser.Password);

            Log.Info("Navigate to Enterprise Agility page and click on 'Refresh' button.");
            insightsDashboard.NavigateToEnterpriseAgilityPage(Company.InsightsId);
            insightsDashboard.ClickRefreshButtonForEnterpriseAgility();
            Assert.IsTrue(enterpriseAgilityPage.IsConfirmationPopupDisplayed(), "The confirmation popup for the Refresh activity is not displayed");

            Log.Info("Click on 'Confirm' and then 'Reload Now' button");
            enterpriseAgilityPage.ClickOnConfirmButton();
            enterpriseAgilityPage.ClickOnReloadNowButton();

            Log.Info("Verify the Last sync date updates after clicking 'Refresh'");
            var refreshDateTimeString = insightsDashboard.GetLastSyncDate();
            var match = Regex.Match(refreshDateTimeString, @"Data as of (\w+ \d{1,2}, \d{4}) \((\d{1,2}:\d{2} [AP]M) GMT([+-]\d{1,2}:\d{2})\)");
            var actualDate = DateTimeOffset.ParseExact($"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}", "MMM d, yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
            var diffMins = Math.Abs((DateTimeOffset.Now - actualDate).TotalMinutes);
            Assert.IsTrue(diffMins < 10, $"The dashboard was not refreshed for the last attempt");
        }
    }
}