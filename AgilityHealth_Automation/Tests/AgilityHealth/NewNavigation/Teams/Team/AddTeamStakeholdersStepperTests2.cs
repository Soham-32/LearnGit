using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamStakeholdersStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [TestCategory("KnownDefect")]//Bug:50355
        public void Team_FullWizard_Verify_Create_Stakeholders_ValidationMessage()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var createStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var editStakeholderInfo = TeamsFactory.GetEditStakeholderInfo();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Click on 'Add a New Team' button and select team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);

            Log.Info("Enter team info, Click on 'Continue To Team Member' button and click on 'Continue To Stakeholder' button");
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();

            Log.Info("Open 'Add New Stakeholder' popup and verify 'Create And Add New' and 'Create And Close' button are disabled");
            createStakeholdersStepperPage.OpenAddStakeholdersPopup();
            Assert.IsFalse(createTeamMembersStepperPage.PopupBase.IsCreateAndAddNewButtonEnable(), "'Create And Add New' button is enabled");
            Assert.IsFalse(createTeamMembersStepperPage.PopupBase.IsCreateAndCloseButtonEnable(), "'Create And Close' button is enabled");

            Log.Info("Enter stakeholder info and verify 'Create And Add New' and 'Create And Close' button are enabled");
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(editStakeholderInfo);
            createStakeholdersStepperPage.PopupBase.EnterEmail(editStakeholderInfo.Email);
            Assert.IsTrue(createTeamMembersStepperPage.PopupBase.IsCreateAndAddNewButtonEnable(), "'Create And Add New' button is disabled");
            Assert.IsTrue(createTeamMembersStepperPage.PopupBase.IsCreateAndCloseButtonEnable(), "'Create And Close' button is disabled");

            Log.Info("Remove roles and verify validation message after click on 'Create And Add New' button");
            createStakeholdersStepperPage.PopupBase.RemoveRoles(editStakeholderInfo.Role);
            createStakeholdersStepperPage.PopupBase.ClickOnCreateAndAddNewButton();
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, createStakeholdersStepperPage.PopupBase.GetFieldValidationMessage("Role"), "Validation message for role is not matched");

            Log.Info("Remove firstname, email, lastname and verify the validation messages");
            createStakeholdersStepperPage.PopupBase.RemoveFirstName();
            createStakeholdersStepperPage.PopupBase.RemoveEmail();
            createStakeholdersStepperPage.PopupBase.RemoveLastName();
            createStakeholdersStepperPage.PopupBase.ClickOnFirstName();
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, createStakeholdersStepperPage.PopupBase.GetFieldValidationMessage("FirstName"), "Validation message for first name is not matched");
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, createStakeholdersStepperPage.PopupBase.GetFieldValidationMessage("LastName"), "Validation message for last name is not matched");
            Assert.AreEqual(ValidationMessageForTeamMemberLeaderStakeholderPopup, createStakeholdersStepperPage.PopupBase.GetFieldValidationMessage("Email"), "Validation message for email is not matched");

            Log.Info("Create stakeholder and open 'Edit Stakeholder' popup via clicking on 'Edit' button for newly created stakeholder");
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(stakeholderInfo);
            createStakeholdersStepperPage.PopupBase.EnterEmail(stakeholderInfo.Email);
            createStakeholdersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            createStakeholdersStepperPage.ClickOnStakeholderEditButton(stakeholderInfo.Email);

            Log.Info("Remove email and verify the 'Update' is disabled");
            createStakeholdersStepperPage.PopupBase.RemoveEmail();
            Assert.IsFalse(createTeamMembersStepperPage.PopupBase.IsUpdateButtonEnable(), "'Update' button is enabled");
        }
    }
}
