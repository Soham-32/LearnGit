using System;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2CreateMultiTeamTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static int _multiTeamId;
        private static int _multiTeamId1;
        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _multiTeam1;
        private static IList<TeamV2Response> _team1MemberResponses;
        private static IList<TeamV2Response> _team2MemberResponses;
        private static IList<TeamV2Response> _team3MemberResponses;
        private static IList<TeamV2Response> _team4MemberResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);

                var team1 = TeamFactory.GetNormalTeam("Team", 1);
                _team1 = setupApi.CreateTeam(team1).GetAwaiter().GetResult();

                var member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_team1.Uid, member1);
                var team1Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team1.Name).TeamId;
                _team1MemberResponses = setupApi.GetTeamWithTeamMemberResponse(team1Id);

                var team2 = TeamFactory.GetNormalTeam("Team", 1);
                _team2 = setupApi.CreateTeam(team2).GetAwaiter().GetResult();

                var member2 = PulseV2Factory.GetValidTeamMemberWithRole("Scrum Master");
                setupApi.AddTeamMembers(_team2.Uid, member2);
                var team2Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team2.Name).TeamId;
                _team2MemberResponses = setupApi.GetTeamWithTeamMemberResponse(team2Id);

                var team3 = TeamFactory.GetNormalTeam("Team", 1);
                _team3 = setupApi.CreateTeam(team3).GetAwaiter().GetResult();

                var member3 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_team3.Uid, member3);
                var team3Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team3.Name).TeamId;
                _team3MemberResponses = setupApi.GetTeamWithTeamMemberResponse(team3Id);

                var team4 = TeamFactory.GetNormalTeam("Team", 1);
                _team4 = setupApi.CreateTeam(team4).GetAwaiter().GetResult();

                var member4 = PulseV2Factory.GetValidTeamMemberWithRole("Scrum Master");
                setupApi.AddTeamMembers(_team4.Uid, member4);
                var team4Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team4.Name).TeamId;
                _team4MemberResponses = setupApi.GetTeamWithTeamMemberResponse(team4Id);

                _multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _team1.Uid, _team2.Uid }).GetAwaiter().GetResult();

                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeam.Name).TeamId;

                _multiTeam1 = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeam1Response = setupApi.CreateTeam(_multiTeam1).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeam1Response.Uid, new List<Guid> { _team3.Uid, _team4.Uid }).GetAwaiter().GetResult();

                _multiTeamId1 = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeam1.Name).TeamId;
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
        public void PulseV2_MultiTeam_Create_SaveAsDraft_Edit_FilterByRole()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to multi team assessment dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Log.Info("Fill the valid data on 'Create Pulse Check' page");
            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Select Questions and verify the selected question on 'Select Questions' page");
            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();

            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            var allTeams = selectRecipientsPage.GetListOfTeams();
            foreach (var teams in allTeams)
            {
                Assert.IsTrue(selectRecipientsPage.IsTeamSelected(teams), "Teams are not selected");
            }

            Log.Info("Verify all team members");
            var listOfFirstTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team1.Name);
            Assert.That.ListsAreEqual(_team1MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfFirstTeamMembers, "List of Members does not match");

            var listOfSecondTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team2.Name);
            Assert.That.ListsAreEqual(_team2MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfSecondTeamMembers, "List of Members does not match");

            Log.Info($"Select role from filter dropdown of {_team1.Name} and verify another team and team members");
            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_team1MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name);

            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team1.Name);
            Assert.That.ListContains(_team1MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(), "Member does not match");

            var listOfTeamMembersWithRole = selectRecipientsPage.GetTeamMemberRoleByTeamName(_team1.Name);

            Assert.That.ListContains(_team1MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags.First().Name).ToList(), listOfTeamMembersWithRole.First(), "Role does not match");
            Assert.IsFalse(selectRecipientsPage.IsTeamDisplayed(_team2.Name), "Team without roles is displayed");

            Log.Info($"Select role from filter dropdown of {_team2.Name} and verify another team and team members");
            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_team2MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name);

            Assert.IsTrue(selectRecipientsPage.IsTeamDisplayed(_team2.Name), "Team without roles is not displayed");

            listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team2.Name);
            Assert.That.ListContains(_team2MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(), "Member does not match");

            listOfTeamMembersWithRole = selectRecipientsPage.GetTeamMemberRoleByTeamName(_team2.Name);
            Assert.That.ListContains(_team2MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(), listOfTeamMembersWithRole.First(), "Role does not match");

            selectRecipientsPage.Header.ClickSaveAsDraftButton();

            Log.Info("Go to edit of pulse and verify selected role and team members and publish the pulse assessment");
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var roleTeamMember = _team1MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name;
            var roleScrumMaster = _team2MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name;

            Assert.IsTrue(selectRecipientsPage.IsSelectedRoleDisplayed(roleTeamMember), "Role is not selected");
            Assert.IsTrue(selectRecipientsPage.IsSelectedRoleDisplayed(roleScrumMaster), "Role is not selected");

            listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team1.Name);
            Assert.That.ListContains(_team1MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            listOfTeamMembersWithRole = editRecipientsPage.GetTeamMemberRoleByTeamName(_team1.Name);
            Assert.That.ListContains(_team1MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");

            listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team2.Name);
            Assert.That.ListContains(_team2MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            listOfTeamMembersWithRole = editRecipientsPage.GetTeamMemberRoleByTeamName(_team2.Name);
            Assert.That.ListContains(_team2MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");

            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Go to edit of pulse and verify selected role and team members");
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(selectRecipientsPage.IsSelectedRoleDisplayed(roleTeamMember), "Role is not selected");
            Assert.IsTrue(selectRecipientsPage.IsSelectedRoleDisplayed(roleScrumMaster), "Role is not selected");

            listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team1.Name);
            Assert.That.ListContains(_team1MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            listOfTeamMembersWithRole = editRecipientsPage.GetTeamMemberRoleByTeamName(_team1.Name);
            Assert.That.ListContains(_team1MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");

            listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team2.Name);
            Assert.That.ListContains(_team2MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            listOfTeamMembersWithRole = editRecipientsPage.GetTeamMemberRoleByTeamName(_team2.Name);
            Assert.That.ListContains(_team2MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_MultiTeam_SelectRecipients_SearchTeam_And_SaveAsDraft()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to multi team assessment dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId1);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Log.Info("Fill the valid data on 'Create Pulse Check' page");
            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);

            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Select Questions and verify the selected question on 'Select Questions' page");
            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();

            Log.Info("Search team name and verify searched team is displayed and other teams are not displayed");
            selectRecipientsPage.SearchWithTeamName(_team3.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamDisplayed(_team3.Name), "Searched team is not displayed");
            Assert.IsFalse(selectRecipientsPage.IsTeamDisplayed(_team4.Name), "Team is displayed");

            Log.Info("Select Role and verify that filtered by role members are displayed along with searched team");
            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_team3MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags?.First().Name);

            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team3.Name);
            Assert.That.ListContains(_team3MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(), "Member does not match");

            var listOfTeamMembersWithRole = selectRecipientsPage.GetTeamMemberRoleByTeamName(_team3.Name);
            Assert.That.ListContains(_team3MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");
            Assert.That.ListNotContains(_team4MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.Last(), "Role does not match");

            selectRecipientsPage.ClickOnSelectTeamCheckbox(_team3.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team3.Name), "Teams are not selected");

            selectRecipientsPage.Header.ClickSaveAsDraftButton();

            Log.Info("Go to edit pulse and verify selected role and team members and publish the pulse assessment");
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team3.Name), "Teams is not selected");
            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team4.Name), "Teams is selected");

            listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team3.Name);
            Assert.That.ListContains(_team3MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            listOfTeamMembersWithRole = selectRecipientsPage.GetTeamMemberRoleByTeamName(_team3.Name);
            Assert.That.ListContains(_team3MemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");

            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(
                _team3MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name, false);

            var listOfTeamMemberOfSelectedTeam = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team3.Name);
            Assert.That.ListsAreEqual(_team3MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(),
                listOfTeamMemberOfSelectedTeam, "List of Members does not match");

            var listOfTeamMemberOfDeselectedTeam = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team3.Name);
            Assert.That.ListsAreEqual(_team3MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(),
                listOfTeamMemberOfDeselectedTeam, "List of Members does not match");

            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Go to edit of pulse and verify selected role and team members and publish the pulse assessment");
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team3.Name), "Teams is not selected");
            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team4.Name), "Teams is selected");

            listOfTeamMemberOfSelectedTeam = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team3.Name);
            Assert.That.ListsAreEqual(_team3MemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(),
                listOfTeamMemberOfSelectedTeam, "List of Members does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_MultiTeam_SelectRecipients_SearchTeam_And_RemoveSearchedTeam()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Log.Info("Fill the valid data on 'Create Pulse Check' page");
            var pulseData = PulseV2Factory.GetPulseAddData();
            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Select Questions and verify the selected question on 'Select Questions' page");
            selectQuestionPage.FillForm(pulseData);

            selectQuestionPage.Header.ClickOnNextButton();

            Log.Info("Select all teams, search team name and verify searched team is selected");
            var allTeams = selectRecipientsPage.GetListOfTeams();
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();

            foreach (var team in allTeams)
            {
                Assert.IsTrue(selectRecipientsPage.IsTeamSelected(team), "Teams are not selected");
            }

            selectRecipientsPage.SearchWithTeamName(_team1.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamDisplayed(_team1.Name), "Team is not displayed");
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team1.Name), "Team is not selected");
            Assert.IsFalse(selectRecipientsPage.IsTeamDisplayed(_team2.Name), "Team is displayed");

            Log.Info("Remove searched text and verify that all teams are selected");
            selectRecipientsPage.RemoveSearchedText();
            foreach (var team in allTeams)
            {
                Assert.IsTrue(selectRecipientsPage.IsTeamDisplayed(team), "Team is not displayed");
                Assert.IsTrue(selectRecipientsPage.IsTeamSelected(team), "Teams are not selected");
            }

            Log.Info("Deselect all teams, search team name and verify team is not selected");
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox(false);
            selectRecipientsPage.SearchWithTeamName(_team1.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamDisplayed(_team1.Name), "Team is not displayed");
            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team1.Name), "Team is selected");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_MultiTeam_SelectRecipients_SearchTeam_WithExcludedMember()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to multi team assessment dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Log.Info("Fill the valid data on 'Create Pulse Check' page");
            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Select Questions excluded question on 'Select Questions' page");
            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.Competency;
            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();

            Log.Info("Search team and verify team members");
            selectRecipientsPage.SearchWithTeamName(_team1.Name);
            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team1.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamMemberEnabled(_team1.Name, listOfTeamMembers.Last()), "TeamMember is not grayed out");
            Assert.IsFalse(selectRecipientsPage.IsTeamMemberEnabled(_team1.Name, listOfTeamMembers.First()), "TeamMember is grayed out");

            Log.Info("Select role from filter dropdown, select team and verify");
            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_team1MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name);
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team1.Name), "Team is selected");

            selectRecipientsPage.SelectDeselectRoleFromFilterDropDown(_team1MemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name, false);

            Log.Info("Go to review and publish page and verify count of team and team members");
            selectRecipientsPage.Header.ClickOnNextButton();
            reviewAndPublishPage.ClickOnTeam();
            var expectedTeamCount = reviewAndPublishPage.GetListOfTeams().Count.ToString();
            var actualTeamCount = reviewAndPublishPage.GetCountOfTeams();
            Assert.AreEqual(expectedTeamCount, actualTeamCount, "Team Count does not match");

            var actualTeamMemberCount = reviewAndPublishPage.GetCountOfTeamMembers();
            Assert.AreEqual(reviewAndPublishPage.GetListOfTeamMembersEmail(_team1.Name).Count.ToString(), actualTeamMemberCount, "Team Member count does not match");
        }
    }
}