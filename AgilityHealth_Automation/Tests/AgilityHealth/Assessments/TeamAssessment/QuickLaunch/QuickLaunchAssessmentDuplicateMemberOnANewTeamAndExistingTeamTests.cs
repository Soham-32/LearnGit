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
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchAssessmentDuplicateMemberOnANewTeamAndExistingTeamTests : BaseTest
    {
        [TestMethod]
        public void QuickLaunchAssessment_VerifyValidationMessageOnDuplicateMember_For_ExistingTeam()
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var getTeam = TeamFactory.GetNormalTeam("QuickLaunchAssessment");
            setupApi.CreateTeam(getTeam).GetAwaiter().GetResult();

            var getValidQuickLaunchAssessmentInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo(getTeam.Name, false);
            QuickLaunchAssessment_Verify_Validation_For_DuplicateMember_On_Team(getValidQuickLaunchAssessmentInfo);
        }

        [TestMethod]
        public void QuickLaunchAssessment_VerifyValidationMessageOnDuplicateMember_For_NewTeam()
        {
            var getValidQuickLaunchAssessmentInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo();
            QuickLaunchAssessment_Verify_Validation_For_DuplicateMember_On_Team(getValidQuickLaunchAssessmentInfo);
        }

        public void QuickLaunchAssessment_Verify_Validation_For_DuplicateMember_On_Team(QuickLaunchAssessment getValidQuickLaunchAssessmentInfo)
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getNewQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();
            var getExistingQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            var teamName = getValidQuickLaunchAssessmentInfo.CreateNewTeam ? getValidQuickLaunchAssessmentInfo.NewTeamName : getValidQuickLaunchAssessmentInfo.ExistingTeamName;

            Log.Info("Login as CA and create a new team via quick launch assessment");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchOptions.Assessment.ToString());
            quickLaunchAssessmentPage.EnterQuickLaunchAssessmentInfo(getValidQuickLaunchAssessmentInfo);
            var quickLaunchAssessmentLink = quickLaunchAssessmentPage.QuickLaunchAssessmentPopupCopyAssessmentAccessLink();

            Log.Info("Copy & paste assessment access link in new tab, fill member info and complete the survey");
            Driver.SwitchTo().NewWindow(WindowType.Tab);
            Driver.NavigateToPage(quickLaunchAssessmentLink);
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getExistingQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            surveyPage.CompleteRandomSurvey();

            Log.Info("Navigate to 'Team Dashboard' page and verify team");
            Driver.SwitchToFirstWindow();
            var teamId = quickLaunchAssessmentPage.GetTeamIdFromLink(teamName);
            Assert.IsTrue(quickLaunchAssessmentPage.DoesTeamDisplay(teamName), $"{teamName} team doesn't displayed");

            Log.Info("Navigate to 'Assessment Dashboard' tab then Edit assessment");
            teamAssessmentDashboardPage.NavigateToPage(int.Parse(teamId));
            teamAssessmentDashboardPage.SelectRadarLink(teamName + " " + getValidQuickLaunchAssessmentInfo.RadarName, "Edit");

            Log.Info("Click on Copy icon of assessment link filed from assessment details section and paste the link on new tab");
            lock (ClipboardLock)
            {
                teamAssessmentEditPage.ClickOnAssessmentLinkCopyIcon();
                var editAssessmentLink = CSharpHelpers.GetClipboard();
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(editAssessmentLink);
            }

            Log.Info("Fill member info for assessment access with existing member then click on 'Submit' button and verify validation message on survey page");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getExistingQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual(Constants.AlreadySurveyCompletedMessage, surveyPage.GetSurveyAlreadyCompletedText(), "Validation message is not matched on survey page");

            Log.Info("Navigate back to member access page and fill new member info for assessment access");
            Driver.Back();
            Driver.RefreshPage();
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getNewQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            var surveyUrl = Driver.GetCurrentUrl();

            Log.Info("Navigate back to member access page and fill same member info as previous for assessment access then verify survey url");
            Driver.Back();
            Driver.RefreshPage();
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getNewQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual(surveyUrl, Driver.GetCurrentUrl(), "Survey url is not matched");

            Log.Info($"Complete the survey and switch to edit 'Assessment Dashboard' page then verify added team members {getExistingQuickLaunchMemberAccessInfo.FullName()}'s and {getNewQuickLaunchMemberAccessInfo.FullName()}'s info");
            surveyPage.CompleteRandomSurvey();
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();
            Assert.That.ListContains(teamAssessmentEditPage.GetTeamMemberEmails(), getExistingQuickLaunchMemberAccessInfo.Email, "Team members is not added to the assessment");

            var actualExistingTeamMemberDetail = teamAssessmentEditPage.GetTeamMemberInfoFromGridByEmail(getExistingQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.FirstName, actualExistingTeamMemberDetail.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.LastName, actualExistingTeamMemberDetail.LastName, "Lastname doesn't match");
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.Email, actualExistingTeamMemberDetail.Email, "Email doesn't match");
            CollectionAssert.IsSubsetOf(getExistingQuickLaunchMemberAccessInfo.ParticipantGroups, actualExistingTeamMemberDetail.ParticipantGroup.StringToList());
            CollectionAssert.IsSubsetOf(getExistingQuickLaunchMemberAccessInfo.Roles, actualExistingTeamMemberDetail.ParticipantGroup.StringToList());

            var actualNewTeamMemberDetail = teamAssessmentEditPage.GetTeamMemberInfoFromGridByEmail(getNewQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.FirstName, actualNewTeamMemberDetail.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.LastName, actualNewTeamMemberDetail.LastName, "Lastname doesn't match");
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.Email, actualNewTeamMemberDetail.Email, "Email doesn't match");
            CollectionAssert.IsSubsetOf(getNewQuickLaunchMemberAccessInfo.ParticipantGroups, actualNewTeamMemberDetail.ParticipantGroup.StringToList());
            CollectionAssert.IsSubsetOf(getNewQuickLaunchMemberAccessInfo.Roles, actualNewTeamMemberDetail.ParticipantGroup.StringToList());

            Log.Info($"Navigate team member tab while team edit and verify team members {getExistingQuickLaunchMemberAccessInfo.FullName()}'s and {getNewQuickLaunchMemberAccessInfo.FullName()}'s info");
            editTeamBasePage.NavigateToPage(teamId);
            editTeamBasePage.GoToTeamMembersTab();

            var actualExistingTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGridByEmail(getExistingQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.FirstName, actualExistingTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.LastName, actualExistingTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(getExistingQuickLaunchMemberAccessInfo.Email, actualExistingTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(getExistingQuickLaunchMemberAccessInfo.Roles, actualExistingTeamMember.Role.StringToList(), "Role list doesn't match");
            Assert.That.ListsAreEqual(getExistingQuickLaunchMemberAccessInfo.ParticipantGroups, actualExistingTeamMember.ParticipantGroup.StringToList(), "Participant Group list doesn't match");

            var actualNewTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGridByEmail(getNewQuickLaunchMemberAccessInfo.Email);
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.FirstName, actualNewTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.LastName, actualNewTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(getNewQuickLaunchMemberAccessInfo.Email, actualNewTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(getNewQuickLaunchMemberAccessInfo.Roles, actualNewTeamMember.Role.StringToList(), "Role list doesn't match");
            Assert.That.ListsAreEqual(getNewQuickLaunchMemberAccessInfo.ParticipantGroups, actualNewTeamMember.ParticipantGroup.StringToList(), "Participant Group list doesn't match");
        }
    }
}
