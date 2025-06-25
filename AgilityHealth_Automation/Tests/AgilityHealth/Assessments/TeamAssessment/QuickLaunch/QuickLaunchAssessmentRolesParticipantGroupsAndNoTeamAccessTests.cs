using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch.QuickLaunchAssessmentPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.QuickLaunch
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchAssessmentRolesParticipantGroupsAndNoTeamAccessTests : BaseTest
    {
        [TestMethod]
        public void QuickLaunchAssessment_Verify_RolesParticipantGroups_And_NoTeamAccess()
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchAssessmentInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo();
            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            Log.Info("Login as CA and create a new team");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Click on 'Assessment' link from 'Quick Launch' button and enter information in 'Quick Launch Assessment' pop up");
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchOptions.Assessment.ToString());
            quickLaunchAssessmentPage.EnterQuickLaunchAssessmentInfo(getValidQuickLaunchAssessmentInfo);

            Log.Info("Copy assessment access link, paste in new tab");
            lock (ClipboardLock)
            {
                var quickLaunchAssessmentLink = quickLaunchAssessmentPage.QuickLaunchAssessmentPopupCopyAssessmentAccessLink();
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchAssessmentLink);
            }

            Log.Info("Fill member info for assessment with multiple Role{s} and Participant Group(s) and verify selected Role{s} and Participant Group(s)");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, quickLaunchMemberAccessPage.GetSelectedRoleList(), "Role list is not matched");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, quickLaunchMemberAccessPage.GetSelectedParticipantGroupList(), "Role list is not matched");

            Log.Info("Remove two Roles and Participant Groups and verify selected Role(s) and Participant Group(s)");
            var rolesRemoveList = new List<string> { getValidQuickLaunchMemberAccessInfo.Roles.First(), getValidQuickLaunchMemberAccessInfo.Roles.Last() };
            var participantGroupsRemoveList = new List<string> { getValidQuickLaunchMemberAccessInfo.ParticipantGroups.First(), getValidQuickLaunchMemberAccessInfo.ParticipantGroups.Last() };
            for (var i = 0; i < rolesRemoveList.Count; i++)
            {
                getValidQuickLaunchMemberAccessInfo.Roles.Remove(rolesRemoveList[i]);
                getValidQuickLaunchMemberAccessInfo.ParticipantGroups.Remove(participantGroupsRemoveList[i]);
            }

            quickLaunchMemberAccessPage.RemoveRoleAndParticipantGroup(rolesRemoveList, participantGroupsRemoveList);
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, quickLaunchMemberAccessPage.GetSelectedRoleList(), "Role list is not matched");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, quickLaunchMemberAccessPage.GetSelectedParticipantGroupList(), "Role list is not matched");

            Log.Info("Click on submit button and switch to 'Team Dashboard' window");
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Driver.Close();
            Driver.SwitchTo().Window(Driver.WindowHandles.Last());

            Log.Info("Verify that added member should not get account setup email as they didn't fill survey");
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.AccountSetupEmailSubject, getValidQuickLaunchMemberAccessInfo.Email, GmailUtil.UserEmailLabel, "", 40),
                $"Email is received by {getValidQuickLaunchMemberAccessInfo.Email} with subject {SharedConstants.AccountSetupEmailSubject}");

            Log.Info("Navigate to Edit Team, go to 'Team members' Tab and verify 'Team Member' info");
            var teamId = quickLaunchAssessmentPage.GetTeamIdFromLink(getValidQuickLaunchAssessmentInfo.NewTeamName);
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();

            var actualTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, actualTeamMember.Role.StringToList());
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, actualTeamMember.ParticipantGroup.StringToList());
            Assert.IsFalse(editTeamTeamMemberPage.IsTeamMemberHaveTeamAccess(actualTeamMember.Email), "Team member have team access");
        }
    }
}
