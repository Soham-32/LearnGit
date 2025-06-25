using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.QuickLaunch
{
    [TestClass]
    [TestCategory("Team"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchTeamMultipleRolesParticipantGroupsTests : BaseTest
    {
        [TestMethod]
        public void QuickLaunch_Team_Verify_Roles_And_ParticipantGroups_For_TeamMember()
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchTeamPage = new QuickLaunchTeamPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchTeamInfo = QuickLaunchTeamFactory.GetValidQuickLaunchTeamInfo();
            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            Log.Info("Login as CA and create a new team via quick launch team");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Team.ToString());
            quickLaunchTeamPage.EnterQuickLaunchTeamInfo(getValidQuickLaunchTeamInfo);

            lock (ClipboardLock)
            {
                var quickLaunchTeamLink = quickLaunchTeamPage.CopyTeamAccessLink();

                Log.Info("Copy team access link, paste in new tab then fill member info for team member details and verify selected Role(s) & Participant Group(s)");
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchTeamLink);
            }

            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, quickLaunchMemberAccessPage.GetSelectedRoleList(), "Roles list is not matched");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, quickLaunchMemberAccessPage.GetSelectedParticipantGroupList(), "Participant Groups list is not matched");

            Log.Info("Remove two roles and participant groups and verify selected Role(s) and Participant Group(s)");
            var rolesRemoveList = new List<string> { getValidQuickLaunchMemberAccessInfo.Roles.First(), getValidQuickLaunchMemberAccessInfo.Roles.Last() };
            var participantGroupsRemoveList = new List<string> { getValidQuickLaunchMemberAccessInfo.ParticipantGroups.First(), getValidQuickLaunchMemberAccessInfo.ParticipantGroups.Last() };
            for (var i = 0; i < rolesRemoveList.Count; i++)
            {
                getValidQuickLaunchMemberAccessInfo.Roles.Remove(rolesRemoveList[i]);
                getValidQuickLaunchMemberAccessInfo.ParticipantGroups.Remove(participantGroupsRemoveList[i]);
            }
            quickLaunchMemberAccessPage.RemoveRoleAndParticipantGroup(rolesRemoveList, participantGroupsRemoveList);
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, quickLaunchMemberAccessPage.GetSelectedRoleList(), "Roles list is not matched");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, quickLaunchMemberAccessPage.GetSelectedParticipantGroupList(), "Participant Groups list is not matched");

            Log.Info("Click on 'Submit' button then switch to 'Team Dashboard' window");
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Driver.Close();
            Driver.SwitchTo().Window(Driver.WindowHandles.Last());

            Log.Info("Navigate to 'Edit Team' then go to 'Team members' tab and verify team member info");
            var teamId = quickLaunchAssessmentPage.GetTeamIdFromLink(getValidQuickLaunchTeamInfo.TeamName);
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();
            var actualTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGridByEmail(getValidQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.FirstName, actualTeamMember.FirstName, "Firstname is not matched");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.LastName, actualTeamMember.LastName, "Lastname is not matched");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.Email, actualTeamMember.Email, "Email is not matched");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, actualTeamMember.Role.StringToList(), "Roles list is not matched");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, actualTeamMember.ParticipantGroup.StringToList(), "Participant Groups list is not matched");
            Assert.IsFalse(editTeamTeamMemberPage.IsTeamMemberHaveTeamAccess(actualTeamMember.Email), "Team member have team access");
        }
    }
}
