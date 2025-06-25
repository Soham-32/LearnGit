using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.QuickLaunch
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("Team"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchTeamDuplicateMemberTests : BaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 48340
        public void QuickLaunchTeam_VerifyValidationMessageOnDuplicateMember()
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var quickLaunchTeamPage = new QuickLaunchTeamPage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchTeamInfo = QuickLaunchTeamFactory.GetValidQuickLaunchTeamInfo();
            var getNewQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();
            var getExistingQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            Log.Info("Login as CA and create a new team via quick launch team");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Team.ToString());
            quickLaunchTeamPage.EnterQuickLaunchTeamInfo(getValidQuickLaunchTeamInfo);

            lock (ClipboardLock)
            {
                var quickLaunchTeamLink = quickLaunchTeamPage.CopyTeamAccessLink();

                Log.Info("Copy team access link, paste in new tab and fill member info for team member details");
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchTeamLink);
                quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getExistingQuickLaunchMemberAccessInfo);
                quickLaunchMemberAccessPage.ClickOnSubmitButton();

                Log.Info("Again go to new tab and navigate to team access link");
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchTeamLink);
            }

            Log.Info("Fill member info same as previous info to team detail page and click on Submit button");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getExistingQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual("The team member already exists for this team.", quickLaunchMemberAccessPage.GetValidationMessageText("EmailAddress"), "Validation message is not matched for 'Email' field");

            Log.Info("Go to team dashboard page and verify team name and work type");
            Driver.SwitchToFirstWindow();
            dashBoardPage.SearchTeam(getValidQuickLaunchTeamInfo.TeamName);
            Assert.AreEqual(getValidQuickLaunchTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name is not matched");
            Assert.AreEqual(getValidQuickLaunchTeamInfo.WorkType, dashBoardPage.GetCellValue(1, "Work Type"), "Work Type is not matched");
            Assert.AreEqual("1", dashBoardPage.GetCellValue(1, "Number of Team Members"), "Number of Team members count is not matched");

            Log.Info($"Navigate team member tab while team edit and verify team member {getExistingQuickLaunchMemberAccessInfo.FullName()}'s info");
            var teamId = quickLaunchAssessmentPage.GetTeamIdFromLink(getValidQuickLaunchTeamInfo.TeamName);
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();

            var actualExistingTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGridByEmail(getExistingQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.FirstName, actualExistingTeamMember.FirstName, "Firstname is not matched");
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.LastName, actualExistingTeamMember.LastName, "Lastname is not matched");
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.Email, actualExistingTeamMember.Email, "Email is not matched");
            Assert.That.ListsAreEqual(getExistingQuickLaunchMemberAccessInfo.Roles, actualExistingTeamMember.Role.StringToList(), "Roles list is not matched");
            Assert.That.ListsAreEqual(getExistingQuickLaunchMemberAccessInfo.ParticipantGroups, actualExistingTeamMember.ParticipantGroup.StringToList(), "Participant Groups is not matched");
            Assert.IsFalse(editTeamTeamMemberPage.IsTeamMemberHaveTeamAccess(actualExistingTeamMember.Email), "Team member have team access");

            Log.Info("Copy invite team member link and navigate to new tab then paste copied link");
            lock (ClipboardLock)
            {
                editTeamTeamMemberPage.ClickOnInviteTeamMemberLinkCopyIcon();
                var editTeamMemberTeamLink = CSharpHelpers.GetClipboard();
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(editTeamMemberTeamLink);
                Log.Info("Fill existing member info to the team detail page and verify validation message");
                quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getExistingQuickLaunchMemberAccessInfo);
                quickLaunchMemberAccessPage.ClickOnSubmitButton();
                Assert.AreEqual("The team member already exists for this team.", quickLaunchMemberAccessPage.GetValidationMessageText("EmailAddress"), "Validation message is not matched for FirstName field");

                Log.Info("Again go to new tab and paste the copied link then fill the new member info");
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(editTeamMemberTeamLink);
                quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getNewQuickLaunchMemberAccessInfo);
                quickLaunchMemberAccessPage.ClickOnSubmitButton();
            }

            Log.Info($"Go to team dashboard and navigate team member tab while team edit then verify team member {getNewQuickLaunchMemberAccessInfo.FullName()}'s info");
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();

            var actualNewTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGridByEmail(getNewQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.FirstName, actualNewTeamMember.FirstName, "Firstname is not matched");
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.LastName, actualNewTeamMember.LastName, "Lastnam is not matched");
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.Email, actualNewTeamMember.Email, "Email is not matched");
            Assert.That.ListsAreEqual(getNewQuickLaunchMemberAccessInfo.Roles, actualNewTeamMember.Role.StringToList(), "Roles list is not matched");
            Assert.That.ListsAreEqual(getNewQuickLaunchMemberAccessInfo.ParticipantGroups, actualNewTeamMember.ParticipantGroup.StringToList(), "Participant Groups is not matched");
            Assert.IsFalse(editTeamTeamMemberPage.IsTeamMemberHaveTeamAccess(actualNewTeamMember.Email), "Team member have team access");
        }
    }
}
