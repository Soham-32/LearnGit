using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Team"), TestCategory("NewNavigation")]
    public class EditTeamStakeholderTests1 : NewNavBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _team = TeamFactory.GetNormalTeam("EditStakeholder");
                _team.Stakeholders.Add(MemberFactory.GetStakeholder());

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
        public void Team_EditStakeholder()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var stakeholdersTabPage = new StakeholdersTabPage(Driver, Log);

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

            Log.Info("Click on Stakeholder tab");
            stakeholdersTabPage.ClickOnStakeHolderTab();

            Log.Info("Open Add Stakeholder popup");
            stakeholdersTabPage.OpenAddStakeholdersPopup();
            var stakeholderInfo = TeamsFactory.GetStakeholderInfo();
            var newStakeholderEmail = stakeholderInfo.Email;
            stakeholderInfo.Email = _team.Stakeholders.Select(e => e.Email).FirstOrDefault();

            Log.Info("Enter Stakeholder info and click on 'Create & Close' button");
            stakeholdersTabPage.PopupBase.EnterTeamMemberOrLeadersInfo(stakeholderInfo);
            stakeholdersTabPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Verify that an error displays mentioning about the email already exists");
            Assert.AreEqual("The stakeholder already exists for this team.", stakeholdersTabPage.PopupBase.GetEmailExistValidationMessage(), "Validation message for email is not matched");
            
            Log.Info("Enter a new stakeholder email and click on 'Create & Close' button");
            stakeholdersTabPage.PopupBase.EnterEmail(newStakeholderEmail);
            stakeholdersTabPage.PopupBase.ClickOnFirstName();
            stakeholdersTabPage.PopupBase.ClickOnCreateAndCloseButton();

            Log.Info("Verify that the stakeholder added properly");
            var actualStakeholder = stakeholdersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(newStakeholderEmail);
            Assert.AreEqual(stakeholderInfo.FirstName, actualStakeholder.FirstName, "Firstname doesn't match");
            Assert.AreEqual(stakeholderInfo.LastName, actualStakeholder.LastName, "Lastname doesn't match");
            Assert.AreEqual(newStakeholderEmail, actualStakeholder.Email, "Email doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.Role, actualStakeholder.Role, "Role doesn't match");
            CollectionAssert.AreEquivalent(stakeholderInfo.ParticipantGroup, actualStakeholder.ParticipantGroup, "Tag doesn't match");

            Log.Info("Edit a stakeholder");
            var editStakeholderInfo = TeamsFactory.GetEditStakeholderInfo();
            stakeholdersTabPage.ClickOnStakeholderEditButton(_team.Stakeholders.Select(e => e.Email).FirstOrDefault());
            editStakeholderInfo.LastName = "EditName";
            stakeholdersTabPage.PopupBase.EnterTeamMemberOrLeadersInfo(editStakeholderInfo);
            stakeholdersTabPage.PopupBase.ClickOnFirstName();
            stakeholdersTabPage.PopupBase.ClickOnUpdateButton();

            Log.Info("Verify that the stakeholder edited properly");
            var expectedEditedStakeholder = stakeholdersTabPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(editStakeholderInfo.Email);
            Assert.AreEqual(editStakeholderInfo.FirstName, expectedEditedStakeholder.FirstName, "Firstname doesn't matched");
            Assert.AreEqual(editStakeholderInfo.LastName, expectedEditedStakeholder.LastName, "Lastname doesn't matched");
            Assert.AreEqual(editStakeholderInfo.Email, expectedEditedStakeholder.Email, "Email doesn't matched");
            Assert.That.ListsAreEqual(editStakeholderInfo.Role, expectedEditedStakeholder.Role, "Role doesn't matched");

            Log.Info("Delete a stakeholder by using stakeholder action button");
            stakeholdersTabPage.DeleteStakeholder(editStakeholderInfo.Email);

            Log.Info("Verify that the selected stakeholder deleted properly");
            Assert.IsFalse(stakeholdersTabPage.IsTeamMemberDisplayed(editStakeholderInfo.Email), $"The selected stake with email {editStakeholderInfo.Email} is not deleted properly");

        }
    }
}
