using System;
using System.Collections.Generic;
using AtCommon.Utilities;
using System.Globalization;
using AgilityHealth_Automation.Base;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.EnterpriseAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("EnterpriseAgilityDashboard"), TestCategory("Dashboard")]
    public class EnterpriseAgilityDashboardTests : BaseTest
    {
        

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("EnterpriseAgility"), TestCategory("Critical"), TestCategory("Smoke"), TestCategory("HeartBeatChecks")]
        [TestCategory("Sanity")]
        public void EnterpriseAgilityDashboardVerifyLatestRefreshSuccessful()
        {
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
                leftNav.SelectCompany(User.CompanyName);
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Log.Info($"Verify that the Enterprise agility dashboard page is loaded successfully");
            teamAgility.ClickOnEnterpriseAgilityTab();
            Assert.IsTrue(enterpriseAgilityPage.IsDownloadPdfButtonEnabled(), $"Enterprise Dashboard is not loaded");

            Log.Info($"Verify Refresh date is updated for according to current date and time");
            var refreshDateTimeString = enterpriseAgilityPage.GetRefreshSyncDate();
            var match = Regex.Match(refreshDateTimeString, @"Data as of (\w+ \d{1,2}, \d{4}) \((\d{1,2}:\d{2} [AP]M) GMT([+-]\d{1,2}:\d{2})\)");
            var actualDate = DateTimeOffset.ParseExact($"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}", "MMM d, yyyy hh:mm tt zzz", CultureInfo.InvariantCulture);
            var diffMins = Math.Abs((DateTimeOffset.Now - actualDate).TotalMinutes);
            Assert.IsTrue(diffMins <= 120, $"The dashboard was not refreshed in last {diffMins} mins");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("EnterpriseAgility"), TestCategory("Critical"), TestCategory("Smoke"), TestCategory("HeartBeatChecks")]
        public void EnterpriseAgilityDashboardVerifyDownloadPdf()
        {
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
                leftNav.SelectCompany(User.CompanyName);
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Log.Info("Click on Enterprise agility tab Verify that the dashboard page is loaded");
            teamAgility.ClickOnEnterpriseAgilityTab();
            Assert.IsTrue(enterpriseAgilityPage.IsDownloadPdfButtonEnabled(), $"Enterprise Agility Dashboard is not loaded.");

            Log.Info($"Verify that pdf is downloaded successfully.");
            const string fileName = "Dashboard-details.pdf";  
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            enterpriseAgilityPage.ClickOnDownloadPdfButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"<{fileName}> file not downloaded successfully");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("EnterpriseAgility"), TestCategory("Critical"), TestCategory("Smoke"), TestCategory("HeartBeatChecks")]
        public void VerifyActiveTeamCountWithTeamDashboard()
        {
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
                leftNav.SelectCompany(User.CompanyName);
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Log.Info("Navigate to the Enterprise agility dashboard and get the 'All acive teams' count from first widget");
            teamAgility.ClickOnEnterpriseAgilityTab();
            var teamCountOnEnterpriseAgilityPage = enterpriseAgilityPage.GetAllActiveTeamsCount().ToInt();

            Log.Info($"Navigate to team dashboard and calculate the active team count from team dashboard");
            teamDashboardPage.NavigateToPage(Company.InsightsId);
            var activeTeamsCount = teamDashboardPage.GetCountOfAllActiveTeams();

            Log.Info("Verify that calculated active teams count matches with the count on Enterprise Agility Dashboard.");
            Assert.AreEqual(teamCountOnEnterpriseAgilityPage, activeTeamsCount, "Active team count does not match the expected value.");
        }

        [TestMethod]
        [TestCategory("EnterpriseAgility"), TestCategory("Critical"), TestCategory("Smoke"), TestCategory("HeartBeatChecks")]
        public void VerifyRadarOvertimeWidgets()
        {
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
                leftNav.SelectCompany(User.CompanyName);
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Log.Info("Navigate to the Enterprise agility dashboard.");
            teamAgility.ClickOnEnterpriseAgilityTab();

            var widgetTitles = new List<string> { "Agility Over Time", "Maturity Over Time", "Performance Over Time"};

            foreach (var title in widgetTitles)
            {
                Log.Info($"Validating team count for widget: {title}");

                var teamCountInWidget = enterpriseAgilityPage.GetTeamsCountAndClickOnDetailPopupLink(title);
                var teamCountInPopup = enterpriseAgilityPage.GetTeamCountFromTheTitle();
                enterpriseAgilityPage.ClickOnListOfTeamsPopupCloseButton();

                Assert.AreEqual(teamCountInWidget, teamCountInPopup, $"Mismatch in team count for widget: {title}");
            }
        }
    }
}
