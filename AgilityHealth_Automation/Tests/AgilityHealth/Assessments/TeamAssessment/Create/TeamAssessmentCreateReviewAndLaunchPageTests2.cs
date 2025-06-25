using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateReviewAndLaunchPageTests2 : BaseTest
    {
        public static bool ClassInitFailed;
        private static int _teamId;
        private static TeamResponse _teamResponse;

        private static readonly TeamMemberInfo TeamMemberInfo1 = new TeamMemberInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "member1" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team");
                _teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_ReviewAndLaunch_AllowParticipantsToSelectTheirRolesAndParticipantGroups_On()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamMemberFullName = TeamMemberInfo1.FirstName + " " + TeamMemberInfo1.LastName;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            Log.Info("Add team member and select then go to 'Review & Finish' page");
            teamMemberCommon.ClickAddNewTeamMemberButton();
            addTeamMemberPage.EnterTeamMemberInfo(TeamMemberInfo1);
            addTeamMemberPage.ClickSaveAndCloseButton();
            selectTeamMembers.SelectTeamMemberByName(teamMemberFullName);

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Select 'Allow participant to select their roles and participant groups' checkbox");
            reviewAndLaunch.SelectAllowParticipantsToSelectTheirRolesAndParticipantGroups();
            reviewAndLaunch.ClickOnPublish();

            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_teamResponse.Name, assessmentName), TeamMemberInfo1.Email, "Inbox"));

            Driver.SwitchToLastWindow();

            Assert.IsTrue(surveyPage.IsRolesDropDownDisplayed(), "'Roles' drop down is not displayed");
            Assert.IsTrue(surveyPage.IsParticipantGroupsDisplayed(), "'Participant Groups' drop down is not displayed");

            Log.Info("On survey page select 'Roles' and 'Participant Groups' option");
            surveyPage.SelectRoleAndParticipantGroup(new List<string> { "Developer" }, (new List<string> { "Technical" }));
            Assert.AreEqual($"Hello, {teamMemberFullName}", surveyPage.GetSurveyIdentity(), "'Confirm Identity' popup title is not matched");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_ReviewAndLaunch_AllowParticipantsToSelectTheirRolesAndParticipantGroups_Off()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamMemberFullName = TeamMemberInfo1.FirstName + " " + TeamMemberInfo1.LastName;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            Log.Info("Add team member and select then go to 'Review & Finish' page");
            teamMemberCommon.ClickAddNewTeamMemberButton();
            addTeamMemberPage.EnterTeamMemberInfo(TeamMemberInfo1);
            addTeamMemberPage.ClickSaveAndCloseButton();
            selectTeamMembers.SelectTeamMemberByName(teamMemberFullName);

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Select 'Allow participant to select their roles and participant groups' checkbox");
            reviewAndLaunch.SelectAllowParticipantsToSelectTheirRolesAndParticipantGroups(false);
            reviewAndLaunch.ClickOnPublish();

            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_teamResponse.Name, assessmentName), TeamMemberInfo1.Email, "Inbox"));

            Driver.SwitchToLastWindow();

            Assert.IsFalse(surveyPage.IsRolesDropDownDisplayed(), "'Roles' drop down is displayed");
            Assert.IsFalse(surveyPage.IsParticipantGroupsDisplayed(), "'Participant Groups' drop down is displayed");
            Assert.AreEqual($"Hello, {teamMemberFullName}", surveyPage.GetSurveyIdentity(), "'Confirm Identity' popup title is not matched");
        }

    }
}