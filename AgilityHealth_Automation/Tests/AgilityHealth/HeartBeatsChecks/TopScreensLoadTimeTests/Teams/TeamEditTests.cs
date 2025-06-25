using System;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Teams
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class TeamEditTests : BaseTest
    {
        [TestMethod]
        public void TeamEdit_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
            var team = teams.GetTeamByName(SharedConstants.Team);

            Thread.Sleep(2000);
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(team.Name);

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            dashBoardPage.ClickTeamEditButton(team.Name);
            editTeamProfilePage.WaitForTeamDetailsPageToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Team Edit", timeToLoad);
        }
    }
}
