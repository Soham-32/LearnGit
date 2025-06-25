using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Edit
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2PulseResultsTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;
        private static int _multiTeamId;
        private static IList<TeamV2Response> _multiTeamWithMember;
        private static CreatePulseAssessmentResponse _pulseAssessmentResponse;
        private static SelectedParticipantsResponse _team1ParticipantsResponse;
        private static SelectedParticipantsResponse _team2ParticipantsResponse;
        private static AddTeamWithMemberRequest _multiTeam;
        private static SavePulseAssessmentV2Request _pulseRequest;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private const string PulseLogoTeamCount = "Completed by 0 out of 2 Teams";
        private const string TeamCompletionCount = "0/2";
        private const string AssessmentCompletionStatusOfTeam = "Incomplete";
        private const int SurveyedMemberCounts = 4;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setupApi = new SetupTeardownApi(TestEnvironment);

                var team = TeamFactory.GetNormalTeam("Team", 5);
                var team1 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                team = TeamFactory.GetNormalTeam("Team", 4);
                var team2 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                _multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = _setupApi.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { team1.Uid, team2.Uid }).GetAwaiter()
                    .GetResult();

                _multiTeamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeam.Name).TeamId;

                _multiTeamWithMember = _setupApi.GetTeamWithTeamMemberResponse(_multiTeamId);
                var questionDetailsResponse = GetQuestions(_multiTeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _multiTeamWithMember, _multiTeamId);
                _pulseAssessmentResponse = AddPulseAssessment(_pulseRequest);

                foreach (var participant in _pulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(team1.Name),
                            participant.Email, "Inbox", "", 360),
                        $"Could not find email with subject <{SharedConstants.PulseEmailSubject(team1.Name)}> sent to {participant.Email}");
                }

                CompletePulseSurvey(_, _filteredQuestions, team1.Name,
                    _pulseRequest.SelectedTeams.First().SelectedParticipants.First().Email);

                CompletePulseSurvey(_, _filteredQuestions, team1.Name,
                    _pulseRequest.SelectedTeams.First().SelectedParticipants.Last().Email);

                CompletePulseSurvey(_, _filteredQuestions, team2.Name,
                    _pulseRequest.SelectedTeams.Last().SelectedParticipants.First().Email);

                CompletePulseSurvey(_, _filteredQuestions, team2.Name,
                    _pulseRequest.SelectedTeams.Last().SelectedParticipants.Last().Email);

                _team1ParticipantsResponse = _setupApi.GetPulseParticipantsResponse(_pulseAssessmentResponse.PulseAssessmentId, _multiTeamWithMember.First().TeamId);

                _team2ParticipantsResponse = _setupApi.GetPulseParticipantsResponse(_pulseAssessmentResponse.PulseAssessmentId, _multiTeamWithMember.Last().TeamId);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [Description("Create a pulse with 2 teams (5 and 4 members each), then fill out surveys for 2 members in each team. Verify the team counts and percentages under the Pulse Logo, Results page, and Edit Recipients page. Next, remove 2 non-surveyed and 1 surveyed member from team 1, edit and save the pulse, and verify the team and team member's counts again. After that, remove 1 non-surveyed and 1 surveyed member from team 1, edit and save the pulse, and verify the team and team member's counts. Finally, fill out the survey for the remaining member in team 1, save the pulse, and verify the team counts once more.")]
        public void PulseV2_Verify_PulseLogoAndResultsInformation()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard, edit Pulse and send reminder email to new member");
            mtEtDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();

            Assert.AreEqual("Completed by 4 out of 9 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(_pulseRequest.Name), "Participant info doesn't match");
            Assert.AreEqual(PulseLogoTeamCount,
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(_pulseRequest.Name), "Team count info doesn't match");

            teamAssessmentDashboardPage.ClickOnPulseRadar(_pulseRequest.Name);
            var listOfPulseAssessments = pulseGrowthJourneyPage.GetPulseAssessmentNames();
            Assert.AreEqual(_pulseRequest.Name, listOfPulseAssessments.First(), "Pulse assessment name is not showing");
            Assert.AreEqual(SurveyedMemberCounts, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Surveyed member count does not match");
            Assert.AreEqual("4/9", pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers("Summary Row"), "Summary Row member count does not match");

            mtEtDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            foreach (var team in _multiTeamWithMember)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is not displayed");
                Assert.AreEqual(AssessmentCompletionStatusOfTeam, editRecipientsPage.GetAssessmentStatusOfTeam(team.Name),
                    "Assessment status for team does not show");
            }
            Assert.AreEqual(TeamCompletionCount, editRecipientsPage.GetTeamCompletionCount(), "Team completion count does not match");
            Assert.AreEqual("0.00% Teams", editRecipientsPage.GetTeamCompletionPercentage(), "Team completion Percentage does not match");


            var team1NotSurveyedMembers = _team1ParticipantsResponse.SelectedParticipants.Where(a => a.IsPulseAssessmentCompleted.Equals(false)).ToList();
            var removeMembersFromTeam1 = new List<string>
            {
                _team1ParticipantsResponse.SelectedParticipants.First().Email,
                team1NotSurveyedMembers.First().Email,
                team1NotSurveyedMembers.Last().Email
            };

            DeleteMemberFromTeamAndPulse(_multiTeamWithMember.First().TeamId, removeMembersFromTeam1, _pulseRequest.Name, 2);

            var team2NotSurveyedMembers = _team2ParticipantsResponse.SelectedParticipants.Where(a => a.IsPulseAssessmentCompleted.Equals(false)).ToList();
            var removeMembersFromTeam2 = new List<string>
            {
                _team2ParticipantsResponse.SelectedParticipants.First().Email,
                team2NotSurveyedMembers.First().Email
            };

            DeleteMemberFromTeamAndPulse(_multiTeamWithMember.Last().TeamId, removeMembersFromTeam2, _pulseRequest.Name, 3);

            var team1 = _setupApi.GetPulseParticipantsResponse(_pulseAssessmentResponse.PulseAssessmentId, _multiTeamWithMember.First().TeamId);

            CompletePulseSurvey(TestContext, _filteredQuestions, _multiTeamWithMember.First().Name, team1.SelectedParticipants.First(a => a.IsPulseAssessmentCompleted.Equals(false)).Email);

            Driver.RefreshPage();

            Assert.AreEqual(5, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Surveyed member count does not match");
            Assert.AreEqual("5/6", pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers("Summary Row"), "Summary row member count does not match");

            mtEtDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();

            Assert.AreEqual("Completed by 1 out of 2 Teams",
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(_pulseRequest.Name), "Team info doesn't match");

            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            Assert.AreEqual("Completed", editRecipientsPage.GetAssessmentStatusOfTeam(_multiTeamWithMember.First().Name),
                "Assessment status for team does not show");
            Assert.AreEqual(AssessmentCompletionStatusOfTeam, editRecipientsPage.GetAssessmentStatusOfTeam(_multiTeamWithMember.Last().Name), "Assessment status for team does not show");
            Assert.AreEqual("1/2", editRecipientsPage.GetTeamCompletionCount(), "Team completion count does not match");
            Assert.AreEqual("50.00% Teams", editRecipientsPage.GetTeamCompletionPercentage(), "Team completion Percentage does not match");
        }

        private void DeleteMemberFromTeamAndPulse(int teamId, List<string> teamMemberEmail, string pulseName, int noOfNoneSurveyedMember)
        {
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var editTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);

            editTeamMemberPage.NavigateToPage(teamId);
            foreach (var memberEmail in teamMemberEmail)
            {
                editTeamMemberPage.DeleteTeamMember(memberEmail.ToLower());
                Assert.IsFalse(editTeamMemberPage.IsTeamMemberDisplayed(memberEmail.ToLower(), 10),
                    $"The selected team member with email {memberEmail} is not deleted properly");
            }

            mtEtDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseName);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            editRecipientsPage.Header.ClickSaveAsDraftButton();

            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            Assert.AreEqual($"Completed by 4 out of {9 - noOfNoneSurveyedMember} Team Members",
                teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(
                    _pulseRequest.Name),
                "Participant info doesn't match");

            Assert.AreEqual(PulseLogoTeamCount,
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(
                    _pulseRequest.Name), "Participant info doesn't match");

            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            Assert.AreEqual(AssessmentCompletionStatusOfTeam, editRecipientsPage.GetAssessmentStatusOfTeam(_multiTeamWithMember.First().Name), "Assessment status for team does not show");
            Assert.AreEqual(TeamCompletionCount, editRecipientsPage.GetTeamCompletionCount(), "Team complete counts are not matching");

            editRecipientsPage.Header.ClickSaveAsDraftButton();
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseRadar(_pulseRequest.Name);
            Assert.AreEqual(SurveyedMemberCounts, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
            Assert.AreEqual($"4/{9 - noOfNoneSurveyedMember}",
                pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers("Summary Row"), "Total and Surveyed members does not match");
        }

    }
}