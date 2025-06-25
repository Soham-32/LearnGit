using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Team"), TestCategory("NewNavigation")]
    public class EditTeamTeamMemberTests1 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private const int TotalTeamMember = 4;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetNormalTeam("EditTeamMember");
                for (var i = 0; i < TotalTeamMember; i++)
                {
                    _team.Members.Add(MemberFactory.GetTeamMember());
                }

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
        [TestCategory("KnownDefect")] // Bug Id : 45521
        public void Team_EditTeamMember()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamMembersTabPage = new TeamMembersTabPage(Driver, Log);

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

            Log.Info("Click on Team Members tab");
            teamMembersTabPage.ClickOnTeamMembersTab();

            Log.Info("Open Add Team Member popup");
            teamMembersTabPage.OpenAddTeamMembersOrLeadersPopup();
            var teamMemberInfo = TeamsFactory.GetTeamMemberInfo();
            var newTeamMemberEmail = teamMemberInfo.Email;
            teamMemberInfo.Email = _team.Members[0].Email;

            Log.Info("Enter Team Member info and click on 'Create & Close' button");
            teamMembersTabPage.PopupBase.EnterTeamMemberOrLeadersInfo(teamMemberInfo);
            teamMembersTabPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Verify that an error displays mentioning about the email already exists");
            Assert.AreEqual("The team member already exists for this team.", teamMembersTabPage.PopupBase.GetEmailExistValidationMessage(), "Validation message for email is not matched");

            Log.Info("Enter a new team member email and click on 'Create & Close' button");
            teamMembersTabPage.PopupBase.EnterEmail(newTeamMemberEmail);
            teamMembersTabPage.PopupBase.ClickOnFirstName();
            teamMembersTabPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Verify that the team member added properly");
            var actualTeamMember = teamMembersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(newTeamMemberEmail);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(teamMemberInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(newTeamMemberEmail, actualTeamMember.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.Role, actualTeamMember.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(teamMemberInfo.ParticipantGroup, actualTeamMember.ParticipantGroup, "Tag doesn't match");

            Log.Info("Click on edit team member button");
            var editTeamMemberInfo = TeamsFactory.GetEditTeamMemberInfo();
            editTeamMemberInfo.LastName = "EditedName";
            teamMembersTabPage.ClickTeamMemberEditButton(newTeamMemberEmail);
            teamMembersTabPage.PopupBase.EnterTeamMemberOrLeadersInfo(editTeamMemberInfo);
            teamMembersTabPage.PopupBase.ClickOnUpdateButton();

            Log.Info("Verify that the team member edited properly");
            var actualEditedTeamMember = teamMembersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editTeamMemberInfo.Email.ToLower());
            Assert.AreEqual(editTeamMemberInfo.FirstName, actualEditedTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(editTeamMemberInfo.LastName, actualEditedTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(editTeamMemberInfo.Email.ToLower(), actualEditedTeamMember.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(editTeamMemberInfo.Role.Concat(teamMemberInfo.Role).ToList(), actualEditedTeamMember.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(editTeamMemberInfo.ParticipantGroup.Concat(teamMemberInfo.ParticipantGroup).ToList(), actualEditedTeamMember.ParticipantGroup, "Tag doesn't match");

            Log.Info("Delete a team member by using team member action button");
            teamMembersTabPage.DeleteTeamMember(_team.Members[0].Email);

            Log.Info("Verify that the selected team member deleted properly");
            Assert.IsFalse(teamMembersTabPage.IsTeamMemberDisplayed(_team.Members[0].Email), $"The selected team member with email {_team.Members[0].Email} is not deleted properly");

            Log.Info("Click on a team member checkbox");
            teamMembersTabPage.ClickOnTeamMemberCheckbox(editTeamMemberInfo.Email);

            Log.Info("Remove the selected team member");
            teamMembersTabPage.RemoveSelectedTeamMember();

            Log.Info("Verify that the selected team member removed properly");
            Assert.IsFalse(teamMembersTabPage.IsTeamMemberDisplayed(editTeamMemberInfo.Email), $"The selected team member with email {editTeamMemberInfo.Email} is not removed properly");

            Log.Info("Select many team members");
            for (var i = 1; i < TotalTeamMember; i++)
            {
                teamMembersTabPage.ClickOnTeamMemberCheckbox(_team.Members[i].Email);
            }

            Log.Info("Remove the selected team member");
            teamMembersTabPage.RemoveSelectedTeamMember();

            Log.Info("Verify that the selected team members removed properly");
            for (var i = 1; i < TotalTeamMember; i++)
            {
                Assert.IsFalse(teamMembersTabPage.IsTeamMemberDisplayed(_team.Members[i].Email), $"The selected team member with email {_team.Members[0].Email} is not removed properly");
            }
        }
    }
}
