using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamAddSubTeamsStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")] //Bug Id: 53523
        [TestCategory("KnownDefectAsBL")] //Bug Id: 53523
        [TestCategory("KnownDefectAsOL")] //Bug Id: 53523
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_Add_Sub_Teams()
        {
            Verify_PortfolioMultiTeam_AddSubTeamsSteppers_AddSubTeams(TeamType.EnterpriseTeam);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")] //Bug Id: 53523
        [TestCategory("KnownDefectAsBL")] //Bug Id: 53523
        [TestCategory("KnownDefectAsOL")] //Bug Id: 53523
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_Verify_UnAssignedSubList_AfterSelecting_TeamLevel()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createEnterpriseTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);

            var enterpriseTeamInfo = EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo();

            Log.Info("Login to the application and switch to new navigation then switch to grid view on team dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.SwitchToGridView();

            Log.Info("Click on 'Add a New Team' button and select team type to create a 'Enterprise Team'");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.EnterpriseTeam);

            Log.Info("Enter team info and click on 'Continue To Add Sub Teams' button also verify unAssign sub team list");
            createEnterpriseTeamStepperPage.EnterTeamInfo(enterpriseTeamInfo);
            createEnterpriseTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            Assert.That.ListContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.EnterpriseTeam, $"List does not contain - {SharedConstants.EnterpriseTeam}");
            Assert.That.ListContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.PortfolioTeam, $"List does not contain - {SharedConstants.PortfolioTeam}");

            Log.Info("Assign enterprise sub team and verify portfolio doesn't display on unAssign section");
            addSubTeamsStepperPage.AssignSubTeam(SharedConstants.EnterpriseTeam);
            Assert.That.ListNotContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.PortfolioTeam, $"List does not contain - {SharedConstants.PortfolioTeam}");

            Log.Info("UnAssign enterprise sub team and verify unAssign sub teams list");
            addSubTeamsStepperPage.UnAssignSubTeam(SharedConstants.EnterpriseTeam);
            addSubTeamsStepperPage.RemoveSearchedText();
            Assert.That.ListContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.EnterpriseTeam, $"List does not contain - {SharedConstants.EnterpriseTeam}");
            Assert.That.ListContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.PortfolioTeam, $"List does not contain - {SharedConstants.PortfolioTeam}");

            Log.Info("Assign portfolio sub team and verify enterprise does not display on unAssign section");
            addSubTeamsStepperPage.AssignSubTeam(SharedConstants.PortfolioTeam);
            Assert.That.ListNotContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.EnterpriseTeam, $"List does not contain - {SharedConstants.EnterpriseTeam}");

            Log.Info("UnAssign portfolio sub team and verify unAssign sub teams list");
            addSubTeamsStepperPage.UnAssignSubTeam(SharedConstants.PortfolioTeam);
            addSubTeamsStepperPage.RemoveSearchedText();
            Assert.That.ListContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.EnterpriseTeam, $"List does not contain - {SharedConstants.EnterpriseTeam}");
            Assert.That.ListContains(addSubTeamsStepperPage.GetUnassignedSubTeamList(), SharedConstants.PortfolioTeam, $"List does not contain - {SharedConstants.PortfolioTeam}");
        }
    }
}
