using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2SelectRecipientsTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IList<TeamV2Response> _teamMemberResponses;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("PulseV2Team", 1);

                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

                //Add teamMember
                var member = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_teamResponse.Uid, member);
                _teamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_SelectRecipients_VerifyTeamMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, verify the team and teamMembers under 'Select Recipients' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();

            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_teamResponse.Name), "Team is not selected");

            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_teamResponse.Name);
            Assert.That.ListsAreEqual(_teamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers, "List of Members does not match");

            Log.Info("Verify the teamMembers after Select/Remove the role from filter");
            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_teamMemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name);

            listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_teamResponse.Name);
            Assert.That.ListContains(_teamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(), "Member does not match");
            var listOfTeamMembersWithRole = selectRecipientsPage.GetTeamMemberRoleByTeamName(_teamResponse.Name);
            Assert.That.ListContains(_teamMemberResponses.First().SelectedParticipants.Where(a=>a.Tags.Count!=0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(), listOfTeamMembersWithRole.First(), "Role does not match");

            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_teamMemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name, false);

            listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_teamResponse.Name);
            Assert.That.ListsAreEqual(_teamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers, "List of Members does not match");

            selectRecipientsPage.Header.ClickSaveAsDraftButton();
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            editRecipientsPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(editRecipientsPage.IsTeamSelected(_teamResponse.Name), "Team is not selected");
            Assert.That.ListsAreEqual(_teamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers, "List of Members does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_SelectRecipients_NextDisable()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, verify the team and teamMembers under 'Select Recipients' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);
            var dimensionName = pulseData.Questions.First().DimensionName;
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox isn't selected");

            selectQuestionPage.Header.ClickOnNextButton();

            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_teamResponse.Name), "Team is not selected");
            Assert.IsTrue(selectRecipientsPage.Header.IsNextButtonEnabled(), "Next button is disabled");

            selectRecipientsPage.ClickOnSelectTeamCheckbox(_teamResponse.Name, false);

            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_teamResponse.Name), "Team is selected");
            Assert.IsFalse(selectRecipientsPage.Header.IsNextButtonEnabled(), "Next button is enabled");
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_SelectRecipients_DisabledMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, verify the team and teamMembers under 'Select Recipients' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.Competency;
            selectQuestionPage.FillForm(pulseData);

            var dimensionName = pulseData.Questions.First().DimensionName;
            var subDimensionName = pulseData.Questions.First().SubDimensionName;
            var competencyName = pulseData.Questions.First().CompetencyName;

            //Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is selected");

            //Sub-Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(subDimensionName), $"{dimensionName} - sub dimension checkbox is selected");

            //Competency Verification
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(competencyName), $"{competencyName} - competency checkbox isn't selected");

            selectQuestionPage.Header.ClickOnNextButton();

            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_teamResponse.Name), "Team is not selected");

            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_teamResponse.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamMemberEnabled(_teamResponse.Name, listOfTeamMembers.Last()), "TeamMember is grayed out");
            Assert.IsFalse(selectRecipientsPage.IsTeamMemberEnabled(_teamResponse.Name, listOfTeamMembers.First()), "TeamMember is not grayed out");
        }
    }
}
