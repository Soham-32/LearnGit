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
    public class ArchiveTeamTests : BaseTest
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
                _team = TeamFactory.GetNormalTeam("ArchivedTeam");
                _multiTeam = TeamFactory.GetMultiTeam("ArchivedMT");
                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("ArchivedET");

                var setup = new SetupTeardownApi(TestEnvironment);
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
        [TestCategory("ArchiveTeam")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void TeamDashboard_ArchiveTeam_NormalTeam()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SearchTeam(_team.Name);
            teamDashboardPage.ArchiveTeam("Archive - Project Completed");
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not archived properly");

            teamDashboardPage.FilterArchivedTeam();
            teamDashboardPage.SearchTeam(_team.Name);
            Assert.AreEqual(_team.Name, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team is not archived properly");

            teamDashboardPage.RestoreTeam();
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not restored properly");

            teamDashboardPage.FilterActiveTeam();
            Assert.AreEqual(_team.Name, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team is not restored properly");
        }

        [TestMethod]
        [TestCategory("ArchiveTeam")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void TeamDashboard_ArchiveTeam_MultiTeam()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SearchTeam(_multiTeam.Name);
            teamDashboardPage.ArchiveTeam("Archive - Project Completed");
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not archived properly");

            teamDashboardPage.FilterArchivedTeam();
            teamDashboardPage.SearchTeam(_multiTeam.Name);
            Assert.AreEqual(_multiTeam.Name, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Multi Team is not archived properly");

            teamDashboardPage.RestoreTeam();
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not restored properly");

            teamDashboardPage.FilterActiveTeam();
            Assert.AreEqual(_multiTeam.Name, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Multi Team is not restored properly");
        }

        [TestMethod]
        [TestCategory("ArchiveTeam")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void TeamDashboard_ArchiveTeam_EnterpriseTeam()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SearchTeam(_enterpriseTeam.Name);
            teamDashboardPage.ArchiveTeam("Archive - Project Completed");
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not archived properly");

            teamDashboardPage.FilterArchivedTeam();
            teamDashboardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.AreEqual(_enterpriseTeam.Name, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Enterprise Team is not archived properly");

            teamDashboardPage.RestoreTeam();
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team is not restored properly");

            teamDashboardPage.FilterActiveTeam();
            Assert.AreEqual(_enterpriseTeam.Name, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Enterprise Team is not restored properly");
        }
    }
}
