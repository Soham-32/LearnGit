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
    public class AddTeamReviewStepperTests5 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_Expand_Collapse_All_Sections()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var createStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Click on 'Add a New Team' button and select team type to create a team");
            teamDashboardPage.SwitchToGridView();
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.FullWizardTeam);

            Log.Info("Enter team info and click on 'Continue To Team Members' button");
            var teamInfo = TeamsFactory.GetValidFullWizardTeamInfo();
            createTeamStepperPage.EnterTeamInfo(teamInfo);
            createTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();

            Log.Info("Click on 'Add New Team Members' option from 'Add Team Members' dropdown");
            createTeamMembersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            var teamMemberInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Enter Team Member info and click on 'Create & Close' button as well as validate the newly created team member info from grid");
            createTeamMembersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(teamMemberInfo);
            createTeamMembersStepperPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Click on 'Continue To Stakeholder' button and select the 'Add New Stakeholder' option from 'Add Stakeholders' dropdown");
            createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
            createStakeholdersStepperPage.OpenAddStakeholdersPopup();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();

            Log.Info("Enter Stakeholder info and click on 'Create & Close' button as well as validate the newly created stakeholder info from grid");
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(stakeholderInfo);
            createStakeholdersStepperPage.PopupBase.EnterEmail(stakeholderInfo.Email);
            createStakeholdersStepperPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Click on 'Continue To Review' button");
            createStakeholdersStepperPage.ClickOnContinueToReviewButton();

            //Click on 'v' collapse of Team Tag Section
            Log.Info("Click on team tags section and validate the team tags section is collapsed");
            var actualTeamTagsText = reviewStepperPage.GetTeamTagsText();
            reviewStepperPage.ClickOnTeamTagsSectionHeader();
            Assert.IsFalse(reviewStepperPage.IsTeamTagsSectionExpanded(), "Team Tag section is not collapsed");

            //click on again 'v' expand of Team Tag Section 
            Log.Info("Click on team tags section and validate the team tags info on 'Review' stepper");
            reviewStepperPage.ClickOnTeamTagsSectionHeader();
            Assert.IsTrue(reviewStepperPage.IsTeamTagsSectionExpanded(), "Team Tag section is collapsed");
            CollectionAssert.AreEquivalent(teamInfo.Tags, actualTeamTagsText, "Tags are not matching");

            //Click on 'v' collapse of Team Members Section
            Log.Info("Click on team members section and validate the team members section is collapsed");
            reviewStepperPage.ClickOnTeamMembersSection();
            Assert.IsFalse(reviewStepperPage.IsTeamMembersSectionExpanded(), "Team member section is not collapsed");

            //Click on again 'v' collapse of Team Members Section
            Log.Info("Click on team members section and validate the team member info on 'Review' stepper");
            reviewStepperPage.ClickOnTeamMembersSection();
            var memberInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(teamMemberInfo.Email);
            Assert.IsTrue(reviewStepperPage.IsTeamMembersSectionExpanded(), "Team member section is collapsed");
            Assert.AreEqual(teamMemberInfo.FirstName, memberInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, memberInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, memberInfo.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.Role, memberInfo.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.ParticipantGroup, memberInfo.ParticipantGroup, "Tag doesn't match");

            //Click on 'v' collapse of Stakeholders Section
            Log.Info("Click on stakeholders section and validate the stakeholder section is collapsed");
            reviewStepperPage.ClickOnStakeholdersSection();
            Assert.IsFalse(reviewStepperPage.IsStakeholdersSectionExpanded(), "Stakeholders section is not collapsed");

            //Click on again 'v' collapse of Stakeholders Section
            Log.Info("Click on stakeholders section and validate the stakeholder info on 'Review' stepper");
            reviewStepperPage.ClickOnStakeholdersSection();
            var stake1 = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.IsTrue(reviewStepperPage.IsStakeholdersSectionExpanded(), "Stakeholders section is collapsed");
            Assert.AreEqual(stakeholderInfo.FirstName, stake1.FirstName, "First name doesn't match");
            Assert.AreEqual(stakeholderInfo.LastName, stake1.LastName, "Last name doesn't match");
            Assert.AreEqual(stakeholderInfo.Email, stake1.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.Role, stake1.Role, "Role doesn't match");
        }

    }

}