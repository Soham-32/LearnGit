using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("PortfolioTeam"), TestCategory("NewNavigation")]
    public class EditPortfolioTeamSubTeamTests1 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetEnterpriseTeam("EditSubTeam");
                var setup = new SetupTeardownApi(TestEnvironment);
                setup.CreateTeam(_team).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 45521, 45959
        public void Portfolio_EditSubTeam()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var subTeamTabPage = new SubTeamTabPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamSubTeamStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var createLeadersStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Switch to grid view");
            teamDashboardPage.SwitchToGridView();

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(_team.Name);

            Log.Info("Click on Edit button");
            teamDashboardPage.ClickOnEditTeamButton(_team.Name);

            Log.Info("Click on Sub Team tab");
            subTeamTabPage.ClickOnSubTeamTab();

            Log.Info("Click on Edit Sub-Teams");
            subTeamTabPage.ClickOnEditSubTeams();

            Log.Info("Assign an existing sub-team to the team");
            subTeamTabPage.AssignSubTeam(SharedConstants.MultiTeam);

            Log.Info("Click on Update Sub-Teams");
            subTeamTabPage.ClickOnUpdateSubTeam();

            Log.Info("Verify that assigned sub-team displays in sub-team grid");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.MultiTeam), "Assigned sub-team should be displayed in the grid");

            Log.Info("Add a new multi team from Sub-Team screen");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.MultiTeam);
            var mtInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
            createTeamStepperPage.EnterTeamInfo(mtInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            addTeamSubTeamStepperPage.ClickOnContinueToAddLeadersButton();
            createLeadersStepperPage.ClickOnContinueToReviewButton();
            reviewStepperPage.ClickOnFinishButton();

            Log.Info("Go back to team dashboard");
            teamDashboardPage.NavigateToPage(Company.Id);

            Log.Info("Switch to grid view");
            teamDashboardPage.SwitchToGridView();

            Log.Info("Enter team name and search");
            teamDashboardPage.SearchTeam(_team.Name);

            Log.Info("Click on Edit button");
            teamDashboardPage.ClickOnEditTeamButton(_team.Name);

            Log.Info("Click on Sub Team tab");
            subTeamTabPage.ClickOnSubTeamTab();

            Log.Info("Verify that newly created sub-team displays in sub-team grid");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(mtInfo.TeamName), "Newly created sub-team should be displayed in the grid");

            Log.Info("Filter a sub-team with team type = Team");
            teamDashboardPage.FilterTeamType("Team");

            Log.Info("Verify that there is no team displays with team type = Team");
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team filter doesn't work properly with team type = Team");

            Log.Info("Filter a sub-team with team type = Multi-Team");
            teamDashboardPage.FilterTeamType("Multi-Team");

            Log.Info("Verify that team with team type = Multi-Team displays");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.MultiTeam), "Team filter doesn't work properly with team type = Multi-Team");

            Log.Info("Search with team name");
            teamDashboardPage.SearchTeam(SharedConstants.MultiTeam);

            Log.Info("Verify that search with team name works properly");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.MultiTeam), "Searched team doesn't show in the grid");

            Log.Info("Click on Edit Sub-Teams");
            subTeamTabPage.ClickOnEditSubTeams();

            Log.Info("Assign sub-team from the team");
            subTeamTabPage.UnAssignSubTeam(SharedConstants.MultiTeam);

            Log.Info("Click on Update Sub-Teams");
            subTeamTabPage.ClickOnUpdateSubTeam();

            Log.Info("Verify that sub-team is unassigned properly");
            Assert.IsFalse(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.MultiTeam), "Sub-team is not unassigned properly");
        }
    }
}
