using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Dashboard
{
    [TestClass]
    [TestCategory("SiteAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class EnterpriseAgilityTests : BaseTest
    {
        [TestMethod]
        public void EnterpriseAgility_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var enterpriseAgilityPage = new EnterpriseAgilityPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);

            Log.Info("Login to the application and navigate to team agility");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAgilityPage.NavigateToPage(445);

            Log.Info("Verify time for loading enterprise agility");
            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            teamAgilityPage.ClickOnEnterpriseAgilityTab();
            enterpriseAgilityPage.SwitchToIFrame();
            enterpriseAgilityPage.WaitTillLoadingSpinnersExists();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Enterprise Agility", timeToLoad);
        }
    }
}
