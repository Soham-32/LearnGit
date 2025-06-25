using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("TeamDashboard"), TestCategory("NewNavigation")]
    public class TeamDeletionTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _enterpriseTeam;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("DeletedTeam");
                _multiTeam = TeamFactory.GetMultiTeam("DeletedMT");
                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("DeletedET");
                setup.CreateTeam(_team).GetAwaiter().GetResult();
                setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setup.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Delete")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void TeamDashboard_DeleteTeam_NormalTeam()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.SearchTeam(_team.Name);
            teamDashboardPage.ArchiveTeam("Permanently Delete");
            teamDashboardPage.SearchTeam(_team.Name);
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not deleted properly");

            teamDashboardPage.FilterArchivedTeam();
            teamDashboardPage.SearchTeam(_team.Name);
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not deleted properly");
        }

        [TestMethod]
        [TestCategory("Delete")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void TeamDashboard_DeleteTeam_MultiTeam()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.SearchTeam(_multiTeam.Name);
            teamDashboardPage.ArchiveTeam("Permanently Delete");
            teamDashboardPage.SearchTeam(_multiTeam.Name);
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Multi Team is not deleted properly");

            teamDashboardPage.FilterArchivedTeam();
            teamDashboardPage.SearchTeam(_multiTeam.Name);
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Multi Team is not deleted properly");
        }

        [TestMethod]
        [TestCategory("Delete")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void TeamDashboard_DeleteTeam_EnterpriseTeam()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.SearchTeam(_enterpriseTeam.Name);
            teamDashboardPage.ArchiveTeam("Permanently Delete");
            teamDashboardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Enterprise Team is not deleted properly");

            teamDashboardPage.FilterArchivedTeam();
            teamDashboardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Enterprise Team is not deleted properly");
        }
    }
}
