using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Dashboard
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class TeamDashboardTests : BaseTest
    {
        [TestMethod]
        public void TeamDashboard_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            Driver.RefreshPage();
            dashBoardPage.WaitForTeamDashboardToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Team Dashboard", timeToLoad);
        }
    }
}
