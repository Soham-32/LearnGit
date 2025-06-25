using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team")]
    public class TeamAddTeamMemberTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static TeamHierarchyResponse _teamId;
        private static User TeamAdminUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");

        //[TestMethod]
        //[TestCategory("CompanyAdmin")]
        public void Team_MemberDoesNotHaveAccessForTeamEditAndAssessmentAdd()
        {
            var setup = new SetupTeardownApi(TestEnvironment);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Create a new team with existing Team Admin user and giving access to team");
            _team = TeamFactory.GetNormalTeam("Team");
            _team.Members.Add(new AddMemberRequest
            {
                FirstName = TeamAdminUser.FirstName,
                LastName = TeamAdminUser.LastName,
                Email = TeamAdminUser.Username
            });
            var teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name);
            addTeamMemberPage.NavigateToTeamPage(_teamId.TeamId);
            addTeamMemberPage.ClickOnTeamMemberTeamAccessButton(teamResponse.Members.First().Email);

            Log.Info("Logout as company admin user");
            topNav.LogOut();
            login.LoginToApplication(TeamAdminUser.Username, TeamAdminUser.Password);

            dashBoardPage.CloseWelcomePopup();
            dashBoardPage.GridTeamView();

            Log.Info($"Verify that team admin has access to Edit & Delete team for assigned team - {SharedConstants.RadarTeam}");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(SharedConstants.RadarTeam), $"Team {SharedConstants.RadarTeam} is not displayed");
            Assert.IsTrue(dashBoardPage.IsTeamEditButtonDisplayed(SharedConstants.RadarTeam), $"Edit button is not displayed for {SharedConstants.RadarTeam} team");

            Log.Info("Verify that for other team, team admin has no access to Edit team and adding the assessment");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(teamResponse.Name), $"Team {teamResponse.Name}is not displayed");
            Assert.IsFalse(dashBoardPage.IsTeamEditButtonDisplayed(teamResponse.Name), $"Edit button is displayed for {teamResponse.Name} team");

            dashBoardPage.SearchTeam(SharedConstants.RadarTeam);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            Assert.IsTrue(teamAssessmentDashboard.IsAddAssessmentButtonDisplayed(), "Add Assessment Button is not displayed");

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            Assert.IsFalse(teamAssessmentDashboard.IsAddAssessmentButtonDisplayed(), "Add Assessment Button is displayed");
        }
    }
}