using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentAccountSetupEmailSettingTests : BaseTest
    {
        private static SetUpMethods _setup;
        private static TeamHierarchyResponse _team;
        private static TeamMemberInfo _teamMemberInfo1;
        private static TeamMemberInfo _teamMemberInfo2;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _setup = new SetUpMethods(testContext, TestEnvironment);
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            var firstName1 = RandomDataUtil.GetFirstName();
            var firstName2 = RandomDataUtil.GetUserName();

            _teamMemberInfo1 = new TeamMemberInfo
            {
                FirstName = firstName1,
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "member" + firstName1 + Constants.UserEmailDomain,
            };

            _teamMemberInfo2 = new TeamMemberInfo
            {
                FirstName = firstName2,
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "member" + firstName2 + Constants.UserEmailDomain,
            };
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectInQa")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_AccountSetupEmail_WhenTheParticipantsSubmitTheAssessment()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            const string team = SharedConstants.Team;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            Log.Info("Click on Assessment and go to 'Review and Finish' page");
            var assessmentName = "Test_" + RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            teamMemberCommon.ClickAddNewTeamMemberButton();
            teamMemberCommon.EnterTeamMemberInfo(_teamMemberInfo1);
            selectTeamMembers.ClickSaveAndCloseButton();

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Select 'When the participants submit the assessment' radio button and click on publish button");
            reviewAndLaunch.SelectWhenTheParticipantsSubmitTheAssessmentRadioButton();
            reviewAndLaunch.ClickOnPublish();

            Log.Info("Verify Account Setup Emails");
            var emailSearchTeamMember = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, assessmentName),
                To = _teamMemberInfo1.Email,
                Labels = new List<string> { "inbox" }
            };
            _setup.CompleteTeamMemberSurvey(emailSearchTeamMember);

            Log.Info("Verify if Team member got the 'Confirm Account' email or not");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.AccountSetupEmailSubject, _teamMemberInfo1.Email, GmailUtil.UserEmailLabel),
                $"Email is not received by {_teamMemberInfo1.Email} with subject {SharedConstants.AccountSetupEmailSubject}");

            topNav.LogOut();

            Driver.NavigateToPage(GmailUtil.GetUserCreateAccountLink(SharedConstants.AccountSetupEmailSubject, _teamMemberInfo1.Email));
            loginPage.SetUserPassword(SharedConstants.CommonPassword);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            Assert.AreEqual($"{team} Team Assessments", teamAssessmentDashboard.GetAssessmentDashboardTitle(), "Assessment dashboard title is not matched.");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_DoNotSendSetupEmail_AfterSubmitSurvey()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            Log.Info("Click on Assessment and go to 'Review and Finish' page");
            var assessmentName = "Test_" + RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            teamMemberCommon.ClickAddNewTeamMemberButton();
            teamMemberCommon.EnterTeamMemberInfo(_teamMemberInfo2);
            selectTeamMembers.ClickSaveAndCloseButton();
            selectTeamMembers.SelectTeamMemberByName(_teamMemberInfo2.FirstName + " " + _teamMemberInfo2.LastName);
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Select 'Do not send setup emails' radio button and click on 'Publish' button");
            reviewAndLaunch.SelectDoNotSendSetupEmailsRadioButton();
            reviewAndLaunch.ClickOnPublish();
            Driver.Close();

            Log.Info("Fill Assessment Survey as team members");
            var emailSearchTeamMember = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, assessmentName),
                To = _teamMemberInfo2.Email,
                Labels = new List<string> { "inbox" }
            };

            _setup.CompleteTeamMemberSurvey(emailSearchTeamMember);

            Log.Info("Verify if Team member got the 'Confirm Account' email or not");
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.AccountSetupEmailSubject, _teamMemberInfo2.Email, "", "UNREAD", 10),
                $"Email is received by {_teamMemberInfo2.Email} with subject {SharedConstants.AccountSetupEmailSubject}");
        }
    }
}