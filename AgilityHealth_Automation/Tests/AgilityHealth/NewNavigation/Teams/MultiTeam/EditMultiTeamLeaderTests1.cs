using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class EditMultiTeamLeaderTests1 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private const int TotalTeamMember = 4;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetMultiTeam("EditLeader");
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
        public void MultiTeam_EditLeader()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var addTeamMembersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var leadersTabPage = new StakeholdersTabPage(Driver, Log);

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

            Log.Info("Click on Leaders tab");
            leadersTabPage.ClickOnLeadersTab();

            Log.Info("Open Add Leader popup");
            addTeamMembersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            var leaderInfo = TeamsFactory.GetTeamMemberInfo();
            var newLeaderEmail = leaderInfo.Email;
            leaderInfo.Email = _team.Members.First().Email;

            Log.Info("Enter Leader info and click on 'Create & Close' button");
            leadersTabPage.PopupBase.EnterTeamMemberOrLeadersInfo(leaderInfo);
            leadersTabPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Verify that an error displays mentioning about the email already exists");
            Assert.AreEqual("The team member already exists for this team.", leadersTabPage.PopupBase.GetEmailExistValidationMessage(), "Validation message for email is not matched");

            Log.Info("Enter a new leader email and click on 'Create & Close' button");
            leadersTabPage.PopupBase.EnterEmail(newLeaderEmail);
            leadersTabPage.PopupBase.ClickOnFirstName();
            leadersTabPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Verify that the leader added properly");
            var actualLeader = leadersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(newLeaderEmail);
            Assert.AreEqual(leaderInfo.FirstName, actualLeader.FirstName, "Firstname doesn't match");
            Assert.AreEqual(leaderInfo.LastName, actualLeader.LastName, "Lastname doesn't match");
            Assert.AreEqual(newLeaderEmail, actualLeader.Email, "Email doesn't match");

            Log.Info("Click on edit leader button");
            var editLeaderInfo = TeamsFactory.GetEditTeamMemberInfo();
            editLeaderInfo.LastName = "EditedName";
            leadersTabPage.ClickOnStakeholderEditButton(newLeaderEmail);
            leadersTabPage.PopupBase.EnterTeamMemberOrLeadersInfo(editLeaderInfo);
            leadersTabPage.PopupBase.ClickOnFirstName();
            leadersTabPage.PopupBase.ClickOnUpdateButton();

            Log.Info("Verify that the leader edited properly");
            var actualEditedTeamMember = leadersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editLeaderInfo.Email.ToLower());
            Assert.AreEqual(editLeaderInfo.FirstName, actualEditedTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(editLeaderInfo.LastName, actualEditedTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(editLeaderInfo.Email.ToLower(), actualEditedTeamMember.Email, "Email doesn't match");

            Log.Info("Delete a leader by using leader action button");
            leadersTabPage.DeleteLeader(_team.Members.First().Email);

            Log.Info("Verify that the selected leader deleted properly");
            Assert.IsFalse(leadersTabPage.IsTeamMemberDisplayed(_team.Members.First().Email), $"The selected leader with email {_team.Members.First().Email} is not deleted properly");

            Log.Info("Click on a leader checkbox");
            leadersTabPage.ClickOnLeaderCheckbox(editLeaderInfo.Email);

            Log.Info("Remove the selected leader");
            leadersTabPage.RemoveSelectedLeader();

            Log.Info("Verify that the selected leader removed properly");
            Assert.IsFalse(leadersTabPage.IsTeamMemberDisplayed(editLeaderInfo.Email), $"The selected leader with email {editLeaderInfo.Email} is not removed properly");

            Log.Info("Select many leaders");
            for (var i = 1; i < TotalTeamMember; i++)
            {
                leadersTabPage.ClickOnLeaderCheckbox(_team.Members[i].Email);
            }

            Log.Info("Remove the selected leader");
            leadersTabPage.RemoveSelectedLeader();

            Log.Info("Verify that the selected leaders removed properly");
            for (var i = 1; i < TotalTeamMember; i++)
            {
                Assert.IsFalse(leadersTabPage.IsTeamMemberDisplayed(_team.Members[i].Email), $"The selected leader with email {_team.Members[i].Email} is not removed properly");
            }
        }
    }
}
