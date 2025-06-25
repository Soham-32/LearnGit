using System;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
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
    public class EditTeamMemberTests : BaseTest
    {
        [TestMethod]
        public void EditTeamMemberPage_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            var team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            Thread.Sleep(2000);//Team Dashboard taking time to load
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(team.Name);
            dashBoardPage.ClickTeamEditButton(team.Name);

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            editTeamBasePage.GoToTeamMembersTab();
            editTeamTeamMemberPage.WaitForTeamMembersPageToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Edit Member Page", timeToLoad);
        }
    }
}
