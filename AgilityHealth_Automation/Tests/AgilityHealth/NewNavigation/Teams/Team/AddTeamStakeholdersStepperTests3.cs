using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamStakeholdersStepperTests3 : NewNavBaseTest
    {
        public static bool ClassInitFailed;
        private static TeamResponse _expectedTeamResponse;
        private const string ValidationMessage = "(Stakeholder role is required.)";

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                // Getting team details from existing team
                _expectedTeamResponse = setup.GetTeamResponse(SharedConstants.Team);
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_AddFromDirectory_AddStakeholdersFromMembersTab_Successfully()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var addStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Click on 'Add a New Team' button and select team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);

            Log.Info("Enter team info and click on 'Continue' button and navigate to till 'Stakeholder' stepper");
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            addTeamMembersStepperPage.ClickOnContinueToStakeholderButton();

            Log.Info("Select 'Stakeholder From Directory Option' from 'Add Stakeholders' dropdown and verify added members from 'Member' tab");
            addStakeholdersStepperPage.OpenSelectStakeholdersFromDirectoryPopup();
            var expectedMemberList = _expectedTeamResponse.Stakeholders.Select(a => a.Email).ToList();
            addTeamMembersStepperPage.SelectTeamMemberFromDictionaryPopupBase.AddMembersFromDirectory(expectedMemberList);
            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(addTeamMembersStepperPage.IsTeamMemberDisplayed(expectedMember), $"Stakeholder : {expectedMember} is not added");
            }

            Log.Info("Verify the validation message for stakeholder's role");
            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(addStakeholdersStepperPage.IsValidationMessageDisplayed(), "Validation message is not displayed");
                Assert.AreEqual(ValidationMessage, addStakeholdersStepperPage.GetValidationMessageForRole(), "Validation message is not matched");
                addStakeholdersStepperPage.ClickOnStakeholderEditButton(expectedMember);
                foreach (var role in stakeholderInfo.Role)
                {
                    addStakeholdersStepperPage.PopupBase.SelectRole(role);
                }
                addStakeholdersStepperPage.PopupBase.ClickOnUpdateButton();
            }
            Assert.IsFalse(addStakeholdersStepperPage.IsValidationMessageDisplayed(), "Validation message is displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_AddFromDirectory_AddStakeholdersFromTeamsTab_Successfully()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var addStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);

            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Click on 'Add a New Team' button and select team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);

            Log.Info("Enter team info then click on 'Continue' button and navigate to till 'Stakeholder' stepper");
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            addTeamMembersStepperPage.ClickOnContinueToStakeholderButton();

            Log.Info("Select 'Stakeholder From Directory Option' from 'Add Stakeholders' dropdown and verify added members from 'Team' tab");
            addStakeholdersStepperPage.OpenSelectStakeholdersFromDirectoryPopup();
            addTeamMembersStepperPage.SelectTeamMemberFromDictionaryPopupBase.AddTeamsFromDirectory(new List<string> { _expectedTeamResponse.Name });
            var expectedMemberList = _expectedTeamResponse.Stakeholders.Select(a => a.Email).ToList();
            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(addTeamMembersStepperPage.IsTeamMemberDisplayed(expectedMember), $"Stakeholder : {expectedMember} is not added");
            }

            Log.Info("Verify the validation message for stakeholder's role");
            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(addStakeholdersStepperPage.IsValidationMessageDisplayed(), "Validation message is not displayed");
                Assert.AreEqual(ValidationMessage, addStakeholdersStepperPage.GetValidationMessageForRole(), "Validation message is not matched");
                addStakeholdersStepperPage.ClickOnStakeholderEditButton(expectedMember);
                foreach (var role in stakeholderInfo.Role)
                {
                    addStakeholdersStepperPage.PopupBase.SelectRole(role);
                }
                addStakeholdersStepperPage.PopupBase.ClickOnUpdateButton();
            }
            Assert.IsFalse(addStakeholdersStepperPage.IsValidationMessageDisplayed(), "Validation message is displayed");
        }
    }
}