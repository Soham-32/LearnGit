using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddMultiTeamEndToEndTests : NewNavBaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug Id : 45352 , 45959
        [TestCategory("CompanyAdmin")]
        public void Team_FullWizard_Add_EndToEnd()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var createTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var createStakeholdersStepperPage = new AddStakeholdersStepperPage(Driver, Log);
            var reviewStepperPage = new ReviewStepperPage(Driver, Log);
            var teamMembersTabPage = new TeamMembersTabPage(Driver, Log);
            var stakeholdersTabPage = new StakeholdersTabPage(Driver, Log);
            var teamProfileTabPage = new TeamProfileTabPage(Driver, Log);

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
            var actualTeamMember = createTeamMembersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(teamMemberInfo.Email);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "First name doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Last name doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, actualTeamMember.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.Role, actualTeamMember.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.ParticipantGroup, actualTeamMember.ParticipantGroup, "Tag doesn't match");

            Log.Info("Click on 'Continue To Stakeholder' button and select the 'Add New Stakeholder' option from 'Add Stakeholders' dropdown");
            createTeamMembersStepperPage.ClickOnContinueToStakeholderButton();
            createStakeholdersStepperPage.OpenAddStakeholdersPopup();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();

            Log.Info("Enter Stakeholder info and click on 'Create & Close' button as well as validate the newly created stakeholder info from grid");
            createStakeholdersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(stakeholderInfo);
            createStakeholdersStepperPage.PopupBase.EnterEmail(stakeholderInfo.Email);
            createStakeholdersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            var actualStakeHolder = createStakeholdersStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.AreEqual(stakeholderInfo.FirstName, actualStakeHolder.FirstName, "First name doesn't match");
            Assert.AreEqual(stakeholderInfo.LastName, actualStakeHolder.LastName, "Last name doesn't match");
            Assert.AreEqual(stakeholderInfo.Email, actualStakeHolder.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.Role, actualStakeHolder.Role, "Role doesn't match");

            Log.Info("Click on 'Continue To Review' button");
            createStakeholdersStepperPage.ClickOnContinueToReviewButton();

            Log.Info("Get expected and actual 'Team Profile' text and validate team profile info on 'Review' stepper");
            var expectedTeamProfileText = reviewStepperPage.GetExpectedTeamProfileText(teamInfo).ReplaceStringData();
            var actualTeamProfileText = reviewStepperPage.GetTeamProfileText().ReplaceStringData();
            Assert.AreEqual(expectedTeamProfileText, actualTeamProfileText, "Team profile text doesn't match");

            Log.Info("Validate the Team tag info on 'Review' stepper");
            var actualTeamTagsText = reviewStepperPage.GetTeamTagsText();
            CollectionAssert.AreEquivalent(teamInfo.Tags, actualTeamTagsText, "Tags are not matching");

            Log.Info("Validate the team member info on 'Review' stepper");
            var memberInfo = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(teamMemberInfo.Email);
            Assert.AreEqual(teamMemberInfo.FirstName, memberInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, memberInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, memberInfo.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.Role, memberInfo.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.ParticipantGroup, memberInfo.ParticipantGroup, "Tag doesn't match");

            Log.Info("Validate the stakeholder info on 'Review' stepper");
            var stake1 = reviewStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.AreEqual(stakeholderInfo.FirstName, stake1.FirstName, "First name doesn't match");
            Assert.AreEqual(stakeholderInfo.LastName, stake1.LastName, "Last name doesn't match");
            Assert.AreEqual(stakeholderInfo.Email, stake1.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.Role, stake1.Role, "Role doesn't match");

            Log.Info("Click on 'Finish' button and validate the team profile info on 'Team Profile' tab");
            reviewStepperPage.ClickOnFinishButton();
            var actualTeamInfo = teamProfileTabPage.GetTeamInfo();
            Assert.AreEqual(teamInfo.TeamName, actualTeamInfo.TeamName, "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, actualTeamInfo.WorkType, "Work Type doesn't match");
            Assert.AreEqual(teamInfo.PreferredLanguage, actualTeamInfo.PreferredLanguage, "Preferred Language doesn't match");
            Assert.AreEqual(teamInfo.Methodology, actualTeamInfo.Methodology, "Methodology doesn't match");
            Assert.AreEqual(teamInfo.ExternalIdentifier, actualTeamInfo.ExternalIdentifier, "External Identifier doesn't match");
            Assert.AreEqual(teamInfo.DepartmentAndGroup, actualTeamInfo.DepartmentAndGroup, "Department doesn't match");
            Assert.AreEqual(teamInfo.DateEstablished, actualTeamInfo.DateEstablished, "Date Established doesn't match");
            Assert.AreEqual(teamInfo.TeamBioOrBackground, actualTeamInfo.TeamBioOrBackground, "Team BIO doesn't match");

            Log.Info("Click on 'Team Members' tab and validate the team member info on 'Team Members' tab");
            teamMembersTabPage.ClickOnTeamMembersTab();
            var teamMember = teamMembersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(teamMemberInfo.Email);
            Assert.AreEqual(teamMemberInfo.FirstName, teamMember.FirstName, "First name doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, teamMember.LastName, "Last name doesn't match");
            Assert.AreEqual(teamMemberInfo.Email, teamMember.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.Role, teamMember.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.ParticipantGroup, teamMember.ParticipantGroup, "Tag doesn't match");

            Log.Info("Validate the stakeholder info on 'Stakeholders' tab");
            stakeholdersTabPage.ClickOnStakeHolderTab();
            var stakeholder = stakeholdersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(stakeholderInfo.Email);
            Assert.AreEqual(stakeholderInfo.FirstName, stakeholder.FirstName, "First name doesn't match");
            Assert.AreEqual(stakeholderInfo.LastName, stakeholder.LastName, "Last name doesn't match");
            Assert.AreEqual(stakeholderInfo.Email, stakeholder.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.Role, stakeholder.Role, "Role doesn't match");

            Log.Info("Navigate to 'Team Dashboard' and search newly created team then validate team info");
            teamDashboardPage.NavigateToPage(Company.Id);
            teamDashboardPage.SearchTeam(teamInfo.TeamName);
            var expectedTeamTagsValues = teamInfo.Tags.Select(item => item.Value).ToList();
            expectedTeamTagsValues.AddRange(new List<string> { teamInfo.Methodology, teamInfo.WorkType });
            Assert.AreEqual(teamInfo.TeamName, teamDashboardPage.GetTeamGridCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(teamInfo.WorkType, teamDashboardPage.GetTeamGridCellValue(1, "Work Type"), "Work Type doesn't match");
            CollectionAssert.AreEquivalent(expectedTeamTagsValues, teamDashboardPage.GetTeamTagsValue(), "Team Tags doesn't match");
        }

    }

}