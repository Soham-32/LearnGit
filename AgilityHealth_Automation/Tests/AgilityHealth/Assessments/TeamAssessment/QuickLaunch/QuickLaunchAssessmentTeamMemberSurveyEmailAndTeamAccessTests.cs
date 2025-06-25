using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
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
    public class QuickLaunchAssessmentTeamMemberSurveyEmailAndTeamAccessTests : BaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("Critical")]
        public void QuickLaunchAssessment_Verify_TeamMember_SurveyEmail_And_TeamAccess()
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editTeamTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);

            var getValidQuickLaunchAssessmentInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo();
            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

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

            Log.Info("Fill member info for assessment access page and submit");
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();

            Log.Info("Complete the survey and verify that added member should get account setup email");
            surveyPage.CompleteRandomSurvey();
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.AccountSetupEmailSubject, getValidQuickLaunchMemberAccessInfo.Email, GmailUtil.UserEmailLabel),
                $"Email is not received by {getValidQuickLaunchMemberAccessInfo.Email} with subject {SharedConstants.AccountSetupEmailSubject}");

            Log.Info("Setup member's account");
            Driver.NavigateToPage(GmailUtil.GetUserCreateAccountLink(SharedConstants.AccountSetupEmailSubject, getValidQuickLaunchMemberAccessInfo.Email));
            login.SetUserPassword(SharedConstants.CommonPassword);

            Log.Info("Verify that user should be navigate to 'Radar' page");
            var radarTitle = (getValidQuickLaunchAssessmentInfo.NewTeamName + " " + getValidQuickLaunchAssessmentInfo.RadarName + " - " + getValidQuickLaunchAssessmentInfo.RadarName + " " + "Radar");
            Assert.AreEqual(radarTitle.ToLower(), radarPage.GetRadarTitle().ToLower(), "RadarTitle doesn't match");

            Log.Info("Navigate to 'Team Dashboard' page and verify 'Team' should be displayed");
            teamDashboardPage.NavigateToPage(Company.Id);
            Assert.IsTrue(teamDashboardPage.DoesTeamDisplay(getValidQuickLaunchAssessmentInfo.NewTeamName), "Team is not displayed");

            Log.Info("Navigate to 'Team Assessment' dashboard page and verify the 'Assessment' name");
            var teamId = int.Parse(quickLaunchAssessmentPage.GetTeamIdFromLink(getValidQuickLaunchAssessmentInfo.NewTeamName));
            teamAssessmentDashboardPage.NavigateToPage(teamId);
            Assert.AreEqual(($"{getValidQuickLaunchAssessmentInfo.NewTeamName} Team Assessments").ToLower(), teamAssessmentDashboardPage.GetAssessmentDashboardTitle().ToLower(), "Assessment dashboard title is not matched.");

            Log.Info("Verify that Assessment status should be 'Closed'");
            var assessmentName = getValidQuickLaunchAssessmentInfo.NewTeamName + " " + getValidQuickLaunchAssessmentInfo.RadarName;
            var data = teamAssessmentDashboardPage.GetAssessmentStatus(assessmentName);
            Assert.AreEqual("Closed", data[0], "Assessment status is incorrect");

            Log.Info("Verify the number of Team Members should display correctly");
            var participant = teamAssessmentDashboardPage.GetAssessmentParticipantTeamMembers(assessmentName);
            Assert.AreEqual("Completed by 1 out of 1  Team Members", participant, "Participant info doesn't match");

            Log.Info("Log out as a member and log in as CA ");
            Driver.Close();
            Driver.SwitchToFirstWindow();
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Edit Team, go to 'Team members' Tab and verify 'Team Member' info");
            editTeamBasePage.NavigateToPage(teamId.ToString());
            editTeamBasePage.GoToTeamMembersTab();

            var actualTeamMember = editTeamTeamMemberPage.GetTeamMemberInfoFromGrid(1);
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.LastName, actualTeamMember.LastName, "Lastname doesn't match");
            Assert.AreEqual(getValidQuickLaunchMemberAccessInfo.Email, actualTeamMember.Email, "Email doesn't match");
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.Roles, actualTeamMember.Role.StringToList());
            Assert.That.ListsAreEqual(getValidQuickLaunchMemberAccessInfo.ParticipantGroups, actualTeamMember.ParticipantGroup.StringToList());
            Assert.IsTrue(editTeamTeamMemberPage.IsTeamMemberHaveTeamAccess(actualTeamMember.Email), "Team member does not have team access");
        }
    }
}
