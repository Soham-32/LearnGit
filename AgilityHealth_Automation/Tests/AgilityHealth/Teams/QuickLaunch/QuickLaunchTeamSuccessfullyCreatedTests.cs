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
    [TestCategory("Smoke")]
    [TestCategory("Team"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchTeamSuccessfullyCreatedTests : BaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 48340
        public void QuickLaunch_Team_Successfully_Created()
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
            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            Log.Info("Login as CA then click on 'Team' link from 'Quick Launch' button and enter information in 'Quick Team Creation' pop up");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Team.ToString());
            quickLaunchTeamPage.EnterQuickLaunchTeamInfo(getValidQuickLaunchTeamInfo);

            Log.Info("Copy team access link, paste in new tab");
            lock (ClipboardLock)
            {
                var quickLaunchTeamLink = quickLaunchTeamPage.CopyTeamAccessLink();
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchTeamLink);
            }

            Log.Info("Fill member info for team access and verify that team member should be successfully added");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual("You've been added to the team!", quickLaunchMemberAccessPage.GetUserSuccessfullyAddedToTheTeamMessage(), "User is not added to the Team");

            Log.Info("Navigate to team dashboard and verify team");
            Driver.SwitchToFirstWindow();
            dashBoardPage.SearchTeam(getValidQuickLaunchTeamInfo.TeamName);
            Assert.AreEqual(getValidQuickLaunchTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(getValidQuickLaunchTeamInfo.WorkType, dashBoardPage.GetCellValue(1, "Work Type"), "Work Type doesn't match");
            Assert.AreEqual("1", dashBoardPage.GetCellValue(1, "Number of Team Members"), "Number of Team members count doesn't match");

            Log.Info($"Edit team and go to team member tab then verify team member {getValidQuickLaunchMemberAccessInfo.FullName()}'s info");
            var teamId = quickLaunchAssessmentPage.GetTeamIdFromLink(getValidQuickLaunchTeamInfo.TeamName);
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();
            var actualTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, actualTeamMember.Role.StringToList(), "Roles list doesn't match");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, actualTeamMember.ParticipantGroup.StringToList(), "Participant Groups list doesn't match");
            Assert.IsFalse(editTeamTeamMemberPage.IsTeamMemberHaveTeamAccess(actualTeamMember.Email), "Team member have team access");
        }
    }
}
