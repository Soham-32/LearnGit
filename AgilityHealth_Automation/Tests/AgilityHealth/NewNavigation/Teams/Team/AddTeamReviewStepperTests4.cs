using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamReviewStepperTests4 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_Edit_Stakeholders()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var createStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();
            var editStakeholderInfo = TeamsFactory.GetEditStakeholderInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Create a new team and navigate till 'Stakeholder' stepper");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();

            Log.Info("Create a stakeholder and verify the details on the grid");
            createStakeholdersStepperPage.OpenAddStakeholdersPopup();
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(stakeholderInfo);
            createStakeholdersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            var actualStakeholder = createStakeholdersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.AreEqual(stakeholderInfo.FirstName, actualStakeholder.FirstName, "First name doesn't matched");
            Assert.AreEqual(stakeholderInfo.LastName, actualStakeholder.LastName, "Last name doesn't matched");
            Assert.AreEqual(stakeholderInfo.Email, actualStakeholder.Email, "Email doesn't matched");
            Assert.That.ListsAreEqual(stakeholderInfo.Role, actualStakeholder.Role, "Role doesn't matched");

            Log.Info("Click on the 'Continue To Review' button and validate the stakeholder info on 'Review' stepper");
            createStakeholdersStepperPage.ClickOnContinueToReviewButton();
            var stake1 = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.AreEqual(stakeholderInfo.FirstName, stake1.FirstName, "First name doesn't match");
            Assert.AreEqual(stakeholderInfo.LastName, stake1.LastName, "Last name doesn't match");
            Assert.AreEqual(stakeholderInfo.Email, stake1.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.Role, stake1.Role, "Role doesn't match");

            Log.Info("click on the 'Edit' button of stakeholder section edit stakeholder's info also verify the details on the grid");
            reviewStepperPage.ClickOnEditStakeholdersButton();
            createStakeholdersStepperPage.ClickOnStakeholderEditButton(stakeholderInfo.Email);
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(editStakeholderInfo, true, stakeholderInfo.Role);
            createStakeholdersStepperPage.PopupBase.ClickOnFirstName();
            createStakeholdersStepperPage.PopupBase.ClickOnUpdateButton();
            var expectedEditedStakeholder = createStakeholdersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editStakeholderInfo.Email);
            Assert.AreEqual(editStakeholderInfo.FirstName, expectedEditedStakeholder.FirstName, "First name doesn't matched");
            Assert.AreEqual(editStakeholderInfo.LastName, expectedEditedStakeholder.LastName, "Last name doesn't matched");
            Assert.AreEqual(editStakeholderInfo.Email, expectedEditedStakeholder.Email, "Email doesn't matched");
            Assert.That.ListsAreEqual(editStakeholderInfo.Role, expectedEditedStakeholder.Role, "Role doesn't matched");

            Log.Info("Click on the 'Continue to review' button then navigate to 'Review' stepper and verify the edited 'Stakeholder'");
            createStakeholdersStepperPage.ClickOnContinueToReviewButton();
            var actualEditedStakeholder = createStakeholdersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editStakeholderInfo.Email);
            Assert.AreEqual(expectedEditedStakeholder.FirstName, actualEditedStakeholder.FirstName, "First name doesn't matched");
            Assert.AreEqual(expectedEditedStakeholder.LastName, actualEditedStakeholder.LastName, "Last name doesn't matched");
            Assert.AreEqual(expectedEditedStakeholder.Email, actualEditedStakeholder.Email, "Email doesn't matched");
            Assert.That.ListsAreEqual(expectedEditedStakeholder.Role, actualEditedStakeholder.Role, "Role doesn't matched");
        }
    }
}
