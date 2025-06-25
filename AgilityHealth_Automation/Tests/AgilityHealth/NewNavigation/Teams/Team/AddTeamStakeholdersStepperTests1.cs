using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamStakeholdersStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [TestCategory("KnownDefect")]//Test bug:49568
        public void Team_FullWizard_Verify_AddEditDelete_Stakeholder_Successfully()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var createStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();
            var editStakeHolderInfo = TeamsFactory.GetEditStakeholderInfo();

            Log.Info("Login to the application and switch to new navigation then switch to grid view on team dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.SwitchToGridView();

            Log.Info("Click on 'Add a New Team' button and select team type to create a team");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);

            Log.Info("Enter team info and click on 'Continue To Team Members' button");
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();

            Log.Info("Click on 'Continue to Stakeholders' button and open stakeholder popup");
            createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
            createStakeholdersStepperPage.OpenAddStakeholdersPopup();

            Log.Info("Enter stakeholder's info then click on 'Create and close' button and verify the details on the grid");
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(stakeholderInfo);
            createStakeholdersStepperPage.PopupBase.EnterEmail(stakeholderInfo.Email);
            createStakeholdersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            var actualStakeholder = createStakeholdersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.AreEqual(stakeholderInfo.FirstName, actualStakeholder.FirstName, "First name doesn't matched");
            Assert.AreEqual(stakeholderInfo.LastName, actualStakeholder.LastName, "Last name doesn't matched");
            Assert.AreEqual(stakeholderInfo.Email, actualStakeholder.Email, "Email doesn't matched");
            Assert.That.ListsAreEqual(stakeholderInfo.Role, actualStakeholder.Role, "Role doesn't matched");

            Log.Info("Click on the 'Edit stakeholder' button  then edit stakeholder's info and verify the details on the grid");
            createStakeholdersStepperPage.ClickOnStakeholderEditButton(stakeholderInfo.Email);
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(editStakeHolderInfo, true, stakeholderInfo.Role);
            createStakeholdersStepperPage.PopupBase.ClickOnFirstName();
            createStakeholdersStepperPage.PopupBase.ClickOnUpdateButton();
            var actualEditedStakeholder = createStakeholdersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editStakeHolderInfo.Email);
            Assert.AreEqual(editStakeHolderInfo.FirstName, actualEditedStakeholder.FirstName, "First name doesn't matched");
            Assert.AreEqual(editStakeHolderInfo.LastName, actualEditedStakeholder.LastName, "Last name doesn't matched");
            Assert.AreEqual(editStakeHolderInfo.Email, actualEditedStakeholder.Email, "Email doesn't matched");
            Assert.That.ListsAreEqual(editStakeHolderInfo.Role, actualEditedStakeholder.Role, "Role doesn't match");

            Log.Info("Click on the 'Delete' option and verify the deleted stakeholder");
            createStakeholdersStepperPage.ClickOnDeleteButton(actualEditedStakeholder.Email);
            Assert.IsFalse(createStakeholdersStepperPage.IsTeamMemberDisplayed(actualEditedStakeholder.Email), "Stakeholder is displayed");
        }
    }
}