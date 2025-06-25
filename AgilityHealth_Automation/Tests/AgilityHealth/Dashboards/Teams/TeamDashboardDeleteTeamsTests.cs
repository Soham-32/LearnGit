using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard"), TestCategory("Teams")]
    [TestCategory("CompanyAdmin")]
    public class TeamDashboardDeleteTeamsTests : BaseTest
    {

        private static AddTeamWithMemberRequest _team;
        private static AddTeamWithMemberRequest _goiTeam;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _enterpriseTeam;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _team = TeamFactory.GetNormalTeam("DeleteTeam");
            _goiTeam = TeamFactory.GetGoiTeam("DeleteGoiTeam");
            _multiTeam = TeamFactory.GetMultiTeam("DeleteMultiTeam");
            _enterpriseTeam = TeamFactory.GetEnterpriseTeam("DeleteEnterpriseTeam");

            var setup = new SetupTeardownApi(TestEnvironment);

            setup.CreateTeam(_team).GetAwaiter().GetResult();
            setup.CreateTeam(_goiTeam).GetAwaiter().GetResult();
            setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
            setup.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void TeamDashboard_DeleteTeams()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            Log.Info("Permanently Delete all created teams");
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.DeleteTeam(_team.Name, RemoveTeamReason.PermanentlyDelete);

            dashBoardPage.SearchTeam(_goiTeam.Name);
            dashBoardPage.DeleteTeam(_goiTeam.Name, RemoveTeamReason.PermanentlyDelete);

            dashBoardPage.SearchTeam(_multiTeam.Name);
            dashBoardPage.DeleteTeam(_multiTeam.Name, RemoveTeamReason.PermanentlyDelete);

            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            dashBoardPage.DeleteTeam(_enterpriseTeam.Name, RemoveTeamReason.PermanentlyDelete);

            Log.Info("Verify that all deleted teams should not be displayed at active state");
            dashBoardPage.SearchTeam(_team.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_team.Name),$"{_team.Name} team is still displayed at active state");
            dashBoardPage.SearchTeam(_goiTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is still displayed at active state");
            dashBoardPage.SearchTeam(_multiTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is still displayed at active state");
            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is still displayed at active state");
            dashBoardPage.ResetGridView();

            Log.Info("Go to archive state and verify that deleted teams should not be displayed");
            dashBoardPage.FilterTeamStatus("Archived");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is displayed at archive state");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is displayed at archive state");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is displayed at archive state");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is displayed at archive state");
        }
    }
}
