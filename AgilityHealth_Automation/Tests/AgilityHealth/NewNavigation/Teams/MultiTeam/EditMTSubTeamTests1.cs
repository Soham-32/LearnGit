using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class EditMtSubTeamTests1 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetMultiTeam("EditSubTeam");
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
        public void MultiTeam_EditSubTeam()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var subTeamTabPage = new SubTeamTabPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var createStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);
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
            subTeamTabPage.AssignSubTeam(SharedConstants.Team);

            Log.Info("Click on Update Sub-Teams");
            subTeamTabPage.ClickOnUpdateSubTeam();

            Log.Info("Verify that assigned sub-team displays in sub-team grid");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.Team), "Assigned sub-team should be displayed in the grid");

            Log.Info("Add a new team from Sub-Team screen");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);
            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
            createStakeholdersStepperPage.ClickOnContinueToReviewButton();
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
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(teamInfo.TeamName), "Newly created sub-team should be displayed in the grid");

            Log.Info("Filter a sub-team with team type = Multi-Team");
            teamDashboardPage.FilterTeamType("Multi-Team");

            Log.Info("Verify that there is no team displays with team type = Multi-Team");
            Assert.IsTrue(teamDashboardPage.IsNoTeamMessageDisplayed(), "Team filter doesn't work properly with team type = Multi-Team");

            Log.Info("Filter a sub-team with team type = Team");
            teamDashboardPage.FilterTeamType("Team");

            Log.Info("Verify that team with team type = Team displays");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.Team), "Team filter doesn't work properly with team type = Team");

            Log.Info("Search with team name");
            teamDashboardPage.SearchTeam(SharedConstants.Team);

            Log.Info("Verify that search with team name works properly");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.Team), "Searched team doesn't show in the grid");

            Log.Info("Click on Edit Sub-Teams");
            subTeamTabPage.ClickOnEditSubTeams();

            Log.Info("Assign sub-team from the team");
            subTeamTabPage.UnAssignSubTeam(SharedConstants.Team);

            Log.Info("Click on Update Sub-Teams");
            subTeamTabPage.ClickOnUpdateSubTeam();

            Log.Info("Verify that sub-team is unassigned properly");
            Assert.IsTrue(subTeamTabPage.IsSubTeamDisplayed(SharedConstants.Team), "Sub-team is not unassigned properly");
        }
    }
}
