using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Edit
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentEditMemberTests : BaseTest
    {
        private static bool _classInitFailed;
        private static SetUpMethods _setup;
        private static SetupTeardownApi _setupApi;
        private static TeamHierarchyResponse _team;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder2.FullName(), SharedConstants.Stakeholder3.FullName() }
        };
        private static readonly List<string> TeamMembersEmailList = new List<string> { SharedConstants.TeamMember1.Email, SharedConstants.TeamMember2.Email };
        private static readonly List<string> StakeHoldersEmailList = new List<string> { SharedConstants.Stakeholder2.Email, SharedConstants.Stakeholder3.Email };
        private static string _expectedEmailSubject;
        private static int _assessmentId;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _setup = new SetUpMethods(testContext, TestEnvironment);
                _setupApi = new SetupTeardownApi(TestEnvironment);
                _team = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
                _expectedEmailSubject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName);
                _assessmentId = _setupApi.GetAssessmentResponse(SharedConstants.Team, TeamAssessment.AssessmentName).Result.AssessmentId;

            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditAddMember()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            teamAssessmentEditPage.AddTeamMemberByEmail(SharedConstants.TeamMember3.Email);

            Log.Info("Verify team member is added properly");
            Assert.That.ListContains(teamAssessmentEditPage.GetTeamMemberEmails(), SharedConstants.TeamMember3.Email, "Emails does not match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void TeamAssessmentEditAddMemberViaLink()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var quickLaunchAssessmentAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchAssessmentAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            teamAssessmentEditPage.ClickOnAssessmentLinkCopyIcon();
            var quickLaunchAssessmentLink = CSharpHelpers.GetClipboard();
            Driver.SwitchTo().NewWindow(WindowType.Tab);
            Driver.NavigateToPage(quickLaunchAssessmentLink);
            Assert.AreEqual((_team.Name + " - " + "Assessment Access").ToLower(), quickLaunchAssessmentAccessPage.GetAssessmentAccessPageTitleText().ToLower(), "'Assessment Access' page header title doesn't match");

            Log.Info("Fill member info for assessment access and verify that member is added to the assessment");
            quickLaunchAssessmentAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchAssessmentAccessInfo);
            quickLaunchAssessmentAccessPage.ClickOnSubmitButton();
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();
            Assert.That.ListContains(teamAssessmentEditPage.GetTeamMemberEmails(), getValidQuickLaunchAssessmentAccessInfo.Email, "Newly added member is not present");
            Driver.SwitchToLastWindow();

            Log.Info("Complete the survey and verify that member has completed the survey");
            surveyPage.CompleteRandomSurvey();
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();

            Assert.That.ListContains(teamAssessmentEditPage.GetTeamMemberEmails(), getValidQuickLaunchAssessmentAccessInfo.Email, "Newly added member is not present");
            Assert.AreEqual("Completed", teamAssessmentEditPage.GetCompletedActionsText(getValidQuickLaunchAssessmentAccessInfo.Email), "Member has not completed assessment survey");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditRemoveMember()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            teamAssessmentEditPage.DeleteTeamMemberByEmail(SharedConstants.TeamMember2.Email);

            Log.Info("Verify team member is removed properly");
            Assert.That.ListNotContains(teamAssessmentEditPage.GetTeamMemberEmails(), SharedConstants.TeamMember2.Email, "Removed email does exist");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditAddStakeholder()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            teamAssessmentEditPage.AddStakeholderByEmail(SharedConstants.Stakeholder4.Email);

            Log.Info("Verify stakeholder is added properly");
            Assert.That.ListContains(teamAssessmentEditPage.GetStakeholderEmails(), SharedConstants.Stakeholder3.Email, "List are not same");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditRemoveStakeholder()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            teamAssessmentEditPage.DeleteStakeholderByEmail(SharedConstants.Stakeholder2.Email);

            Log.Info("Verify stakeholder is removed properly");
            Assert.That.ListNotContains(teamAssessmentEditPage.GetStakeholderEmails(), SharedConstants.Stakeholder2.Email, "List are not same");

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditVerifyResults()
        {
            VerifySetup(_classInitFailed);
            var loginPage = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Verify that user is navigated to radar page after clicking on the 'Results' button.");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            teamAssessmentEditPage.ClickOnResultsButton();
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_team.TeamId}/assessments/{_assessmentId}/radar", Driver.GetCurrentUrl(), "Radar page url does not match");
            var actualAssessmentName = radarPage.GetRadarTitle();
            Assert.IsTrue(actualAssessmentName.Contains(TeamAssessment.AssessmentName), "Assessment name doesn't match.");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditMembersResendInviteAndViewLink()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to {TeamAssessment.AssessmentName} edit assessment page.");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            Log.Info("Verify 'Resend Invite' & 'View Link' functionality for team members & stakeholders");
            foreach (var email in TeamMembersEmailList)
            {
                teamAssessmentEditPage.ClickMemberResendInviteLinkButton(email);
                var expectedSurveyLink = GmailUtil.GetSurveyLink(_expectedEmailSubject, email);
                teamAssessmentEditPage.ClickOnTeamMemberViewLink(email);

                Assert.AreEqual(expectedSurveyLink.Replace("/start", ""), teamAssessmentEditPage.ViewLinkPopupGetLink(), "Survey link doesn't match");
            }
            foreach (var email in StakeHoldersEmailList)
            {
                teamAssessmentEditPage.ClickStakeholderResendInviteLinkButton(email);
                var expectedSurveyLink = GmailUtil.GetSurveyLink(_expectedEmailSubject, email);
                teamAssessmentEditPage.ClickOnStakeholderViewLink(email);

                Assert.AreEqual(expectedSurveyLink.Replace("/start", ""), teamAssessmentEditPage.ViewLinkPopupGetLink(), "Survey link doesn't match");
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")]
        [TestCategory("KnownDefectAsBL")]
        [TestCategory("KnownDefectAsOL")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditVerifyReopenAssessment()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to {TeamAssessment.AssessmentName} edit assessment page.");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            Log.Info("Verify that Team Member's and Stakeholder's 'Resend Invite','View Link' & 'Remove Participant' icon is displayed.");
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMemberResendInviteIconDisplayed(SharedConstants.TeamMember1.Email), "'Resend Invite' icon not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMemberViewLinkIconDisplayed(SharedConstants.TeamMember1.Email), "'View Link' icon not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMemberRemoveParticipantIconDisplayed(SharedConstants.TeamMember1.Email), "'Remove participant' icon not displayed");

            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderResendInviteIconDisplayed(SharedConstants.Stakeholder2.Email), "'Resend Invite' icon not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderViewLinkIconDisplayed(SharedConstants.Stakeholder2.Email), "'View Link' icon not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderRemoveParticipantsIconDisplayed(SharedConstants.Stakeholder2.Email), "'Remove participant' icon not displayed");

            Log.Info("Complete the TeamMember's and Stakeholder's survey.");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                To = SharedConstants.TeamMember1.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            };
            _setup.CompleteTeamMemberSurvey(emailSearch);
            _setup.CompleteStakeholderSurvey(_team.Name, SharedConstants.Stakeholder2.Email, TeamAssessment.AssessmentName, 5);
            Driver.RefreshPage();

            Log.Info("Verify that TeamMember's and stakeholder's ReopenAssessment icon is displayed.");
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMembersReopenAssessmentIconDisplayed(SharedConstants.TeamMember1.Email), "'Reopen Assessment' icon not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderReopenAssessmentIconDisplayed(SharedConstants.Stakeholder2.Email), "'Reopen Assessment' icon not displayed");

            Assert.IsFalse(teamAssessmentEditPage.IsTeamMemberResendInviteIconDisplayed(SharedConstants.TeamMember1.Email), "'Resend Invite' icon is displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsTeamMemberViewLinkIconDisplayed(SharedConstants.TeamMember1.Email), "'View Link' icon is displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsTeamMemberRemoveParticipantIconDisplayed(SharedConstants.TeamMember1.Email), "'Remove Participant' icon is displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsStakeholderResendInviteIconDisplayed(SharedConstants.Stakeholder2.Email), "'Resend Invite' icon is displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsStakeholderViewLinkIconDisplayed(SharedConstants.Stakeholder2.Email), "'View Link' icon is displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsStakeholderRemoveParticipantsIconDisplayed(SharedConstants.Stakeholder2.Email), "'Remove Participant' icon is displayed");

            Log.Info("Click on the TeamMember's and Stakeholder's ReopenAssessment icon and Verify that 'Resend Invite', 'View Link' & 'Remove Participant' icons are displayed.");
            teamAssessmentEditPage.ClickOnTeamMembersReopenAssessmentIcon(SharedConstants.TeamMember1.Email);
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMemberResendInviteIconDisplayed(SharedConstants.TeamMember1.Email), "'Resend Invite' icon is not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMemberViewLinkIconDisplayed(SharedConstants.TeamMember1.Email), "'View Link' icon is not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsTeamMemberRemoveParticipantIconDisplayed(SharedConstants.TeamMember1.Email), "'Remove Participant' icon is not displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsTeamMembersReopenAssessmentIconDisplayed(SharedConstants.TeamMember1.Email), "'Reopen Assessment' icon is displayed");

            teamAssessmentEditPage.ClickOnStakeholdersReopenAssessmentIcon(SharedConstants.Stakeholder2.Email);
            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderResendInviteIconDisplayed(SharedConstants.Stakeholder2.Email), "'Resend Invite' icon is not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderViewLinkIconDisplayed(SharedConstants.Stakeholder2.Email), "'View Link' icon is not displayed");
            Assert.IsTrue(teamAssessmentEditPage.IsStakeholderRemoveParticipantsIconDisplayed(SharedConstants.Stakeholder2.Email), "'Remove Participant' icon is not displayed");
            Assert.IsFalse(teamAssessmentEditPage.IsStakeholderReopenAssessmentIconDisplayed(SharedConstants.Stakeholder2.Email), "'Reopen Assessment' icon is displayed");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditTeamMemberSendToAllEmail()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to {TeamAssessment.AssessmentName} edit assessment page.& click on send to all button");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            teamAssessmentEditPage.ClickOnTeamMembersSendToAllButton();

            Log.Info("Verify that survey emails are received by Team Members & Stakeholders");
            TeamMembersEmailList.ForEach(teamMemberEmail => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(_expectedEmailSubject, teamMemberEmail),
                $"Email NOT received <{_expectedEmailSubject}> sent to {teamMemberEmail}"));

            teamAssessmentEditPage.ClickOnStakeholdersSendToAllButton();
            StakeHoldersEmailList.ForEach(stakeholderEmail => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(_expectedEmailSubject, stakeholderEmail),
                $"Email NOT received <{_expectedEmailSubject}> sent to {stakeholderEmail}"));
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditPreview()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Verify that user is navigated to preview button after clicking on preview button");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            teamAssessmentEditPage.ClickOnPreviewButton();
            Driver.SwitchToLastWindow();
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/assessment/{_assessmentId}/preview", Driver.GetCurrentUrl(), "Radar page url does not match");
        }
    }
}