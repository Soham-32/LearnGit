using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Teams
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class EnterpriseTeamEditTests : BaseTest
    {
        [TestMethod]
        public void EnterpriseTeamEdit_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(SharedConstants.EnterpriseTeam);

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            dashBoardPage.ClickTeamEditButton(SharedConstants.EnterpriseTeam);
            editTeamProfilePage.WaitForTeamDetailsPageToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Enterprise Team Edit", timeToLoad);
        }
    }
}
