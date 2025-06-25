using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch;
using AtCommon.ObjectFactories;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch.QuickLaunchAssessmentPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.QuickLaunch
{
    [TestClass]
    [TestCategory("Assessments"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]

    public class QuickLaunchAssessmentForNewAndExistingTeamTests : BaseTest
    {

        [TestMethod]
        public void QuickLaunchAssessment_For_Existing_Team()
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var getTeam = TeamFactory.GetNormalTeam("TestQuickLaunchAssessment");
            setupApi.CreateTeam(getTeam).GetAwaiter().GetResult();

            var getValidQuickLaunchAssessmentInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo(getTeam.Name, false);

            TeamAssessment_QuickLaunch_Assessment(getValidQuickLaunchAssessmentInfo);

        }

        [TestMethod]
        [TestCategory("Critical")]
        public void QuickLaunchAssessment_For_New_Team()
        {
            var getValidQuickLaunchAssessmentInfo =
                QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo();

            TeamAssessment_QuickLaunch_Assessment(getValidQuickLaunchAssessmentInfo);
        }

        public void TeamAssessment_QuickLaunch_Assessment(QuickLaunchAssessment getValidQuickLaunchAssessmentInfo)
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);

            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            var teamName = getValidQuickLaunchAssessmentInfo.CreateNewTeam ? getValidQuickLaunchAssessmentInfo.NewTeamName : getValidQuickLaunchAssessmentInfo.ExistingTeamName;

            Log.Info("Login as CA and create a new team");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Click on 'Assessment' link from 'Quick Launch' button and enter information in 'Quick Launch Assessment' pop up");
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchOptions.Assessment.ToString());
            quickLaunchAssessmentPage.EnterQuickLaunchAssessmentInfo(getValidQuickLaunchAssessmentInfo);

            Log.Info("Copy assessment access link, paste in new tab and verify page");
            lock (ClipboardLock)
            {
                var quickLaunchAssessmentLink = quickLaunchAssessmentPage.QuickLaunchAssessmentPopupCopyAssessmentAccessLink();
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchAssessmentLink);
            }
            Assert.AreEqual((teamName + " - " + "Assessment Access").ToLower(), quickLaunchMemberAccessPage.GetAssessmentAccessPageTitleText().ToLower(), "'Assessment Access' page header title doesn't match");

            Log.Info("Fill member info for assessment access and verify that it is navigate to 'Survey' page");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual($"Hello, {getValidQuickLaunchMemberAccessInfo.FullName()}", surveyPage.GetSurveyIdentity(), "Identity popup title doesn't match on survey page or user is not navigated to survey page");

            Log.Info("Complete the survey and switch to 'Team Dashboard' window");
            surveyPage.CompleteRandomSurvey();
            Driver.SwitchToFirstWindow();

            Log.Info($"Navigate team member tab while team edit and verify team member {getValidQuickLaunchMemberAccessInfo.FullName()}'s info");
            var teamId = quickLaunchAssessmentPage.GetTeamIdFromLink(teamName);
            Assert.IsTrue(quickLaunchAssessmentPage.DoesTeamDisplay(teamName), $"{teamName} team doesn't displayed");

            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();
            var actualTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, actualTeamMember.Role.StringToList());
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, actualTeamMember.ParticipantGroup.StringToList());

            Log.Info("Navigate to 'Assessment Dashboard Tab' verify that assessment status should be 'Closed'");
            teamAssessmentDashboardPage.NavigateToPage(int.Parse(teamId));
            var assessmentName = teamName + " " + getValidQuickLaunchAssessmentInfo.RadarName;
            var data = teamAssessmentDashboardPage.GetAssessmentStatus(assessmentName);
            Assert.AreEqual("Closed", data[0], "Assessment status is not closed");

            Log.Info("Edit assessment, Verify added team member and Assessment is completed or not");
            teamAssessmentDashboardPage.SelectRadarLink(assessmentName, "Edit");
            Assert.That.ListContains(teamAssessmentEditPage.GetTeamMemberEmails(), getValidQuickLaunchMemberAccessInfo.Email, "Emails does not match");
            Assert.AreEqual("Completed", teamAssessmentEditPage.GetCompletedActionsText(getValidQuickLaunchMemberAccessInfo.Email), "Assessment is not completed");
        }


    }
}
