using System;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Dashboard
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class IdeaBoardTests : BaseTest
    {
        [TestMethod]
        public void IdeaBoard_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            var radarTeam = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam);
            Thread.Sleep(10000);//Navigation takes time

            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.ProgramHealthRadar);

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            radarPage.ClickOnIdeaboardLink();
            Driver.SwitchToLastWindow();
            ideaboardPage.WaitForIdeaboardToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Ideaboard", timeToLoad);
        }
    }
}
