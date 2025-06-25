using System;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Dashboard
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class TeamAgilityTests : BaseTest
    {
        [TestMethod]
        public void TeamAgility_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info("Login to the application and navigate to team agility");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Thread.Sleep(10000);//Insights navigation takes time
            topNav.ClickOnInsightsDashboardLink();
            teamAgilityPage.WaitForTeamAgilityPageToLoad();

            teamAgilityPage.ClickOnEnterpriseAgilityTab();
            Log.Info("Verify time for loading team agility");

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            teamAgilityPage.ClickOnTeamAgilityTab();
            teamAgilityPage.WaitForTeamAgilityPageToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Team Agility", timeToLoad);
        }
    }
}
