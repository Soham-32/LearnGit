using System;
using System.Linq;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Assessments.PulseV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AtCommon.Api.Enums;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.TeamLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("TeamPulse")]
    public class PulseAssessmentV2RemoveTeamMemberFromSelectedTeamLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private const int SurveyedMemberCountsForActivePulse = 1;
        private const int SurveyedMemberCountsForClosedPulse = 3;

        private static int _teamIdWithDraftPulse;
        private static int _teamIdWithActivePulse;
        private static int _teamIdWIthPendingPulse;
        private static int _teamIdWithClosedPulse;

        private static TeamResponse _teamWithDraftPulse;
        private static TeamResponse _teamWithActivePulse;
        private static TeamResponse _teamWIthPendingPulse;
        private static TeamResponse _teamWithClosedPulse;

        private static IList<TeamV2Response> _teamWithMembersOfDraftPulse;
        private static IList<TeamV2Response> _teamWithMembersOfActivePulse;
        private static IList<TeamV2Response> _teamWithMembersOfPendingPulse;
        private static IList<TeamV2Response> _teamWithMembersOfClosedPulse;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam1;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam2;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam3;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam4;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                //Create 4 teams
                var teamDraftPulse = TeamFactory.GetNormalTeam("Team", 3);
                _teamWithDraftPulse = setupApi.CreateTeam(teamDraftPulse).GetAwaiter().GetResult();
                _teamIdWithDraftPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithDraftPulse.Name).TeamId;
                _teamWithMembersOfDraftPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithDraftPulse);

                var teamActivePulse = TeamFactory.GetNormalTeam("Team", 3);
                _teamWithActivePulse = setupApi.CreateTeam(teamActivePulse).GetAwaiter().GetResult();
                _teamIdWithActivePulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithActivePulse.Name).TeamId;
                _teamWithMembersOfActivePulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithActivePulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 3);
                _teamWIthPendingPulse = setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                _teamIdWIthPendingPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWIthPendingPulse.Name).TeamId;
                _teamWithMembersOfPendingPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWIthPendingPulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 3);
                _teamWithClosedPulse = setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                _teamIdWithClosedPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithClosedPulse.Name).TeamId;
                _teamWithMembersOfClosedPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithClosedPulse);

                //Create Multi team and add newly created three teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _teamWithDraftPulse.Uid, _teamWithActivePulse.Uid, _teamWIthPendingPulse.Uid, _teamWithClosedPulse.Uid }).GetAwaiter()
                    .GetResult();

                #endregion

                #region Getting team response

                var questionDetailsResponseTeam1 = GetQuestions(_teamIdWithDraftPulse);
                _filteredQuestionsTeam1 = questionDetailsResponseTeam1.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam2 = GetQuestions(_teamIdWithActivePulse);
                _filteredQuestionsTeam2 = questionDetailsResponseTeam2.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam3 = GetQuestions(_teamIdWIthPendingPulse);
                _filteredQuestionsTeam3 = questionDetailsResponseTeam3.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam4 = GetQuestions(_teamIdWithClosedPulse);
                _filteredQuestionsTeam4 = questionDetailsResponseTeam4.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Getting pulse requests

                // Pulse request for draft pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam1, _teamWithMembersOfDraftPulse, _teamIdWithDraftPulse, false, period: AssessmentPeriod.TwentyFourHours);

                // Pulse request for active pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam2, _teamWithMembersOfActivePulse, _teamIdWithActivePulse, period: AssessmentPeriod.OneWeek);

                // Pulse request for pending pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam3, _teamWithMembersOfPendingPulse, _teamIdWIthPendingPulse, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                //Pulse request for Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam4, _teamWithMembersOfClosedPulse, _teamIdWithClosedPulse, period: AssessmentPeriod.SeventyTwoHours);

                #endregion
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }



        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Draft pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Draft_Pulse_At_TeamLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_draftPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_draftPulseRequest, _teamWithDraftPulse, _teamIdWithDraftPulse, _teamWithMembersOfDraftPulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Active pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Active_Pulse_At_TeamLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_activePulseRequest);
            FillSurvey(_activePulseRequest, _teamWithActivePulse);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_activePulseRequest, _teamWithActivePulse, _teamIdWithActivePulse, _teamWithMembersOfActivePulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Pending pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Pending_Pulse_At_TeamLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_pendingPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_pendingPulseRequest, _teamWIthPendingPulse, _teamIdWIthPendingPulse, _teamWithMembersOfPendingPulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Closed pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Closed_Pulse_At_TeamLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_closedPulseRequest);
            FillSurvey(_closedPulseRequest, _teamWithClosedPulse);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_closedPulseRequest, _teamWithClosedPulse, _teamIdWithClosedPulse, _teamWithMembersOfClosedPulse);

        }

        #region Fill survey for active pulse and closed pulse 

        private void FillSurvey(SavePulseAssessmentV2Request pulseRequest, TeamResponse team)
        {
            if (pulseRequest == _closedPulseRequest)
            {

                //Fill survey of both members to close the pulse created within team
                foreach (var participant in pulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(
                        GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(team.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(team.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestionsTeam4, team.Name, participant.Email);
                }

            }
            else
            {
                //Fill survey of both members to close the pulse created within team
                var email = pulseRequest.SelectedTeams.First().SelectedParticipants.First().Email;
                Assert.IsTrue(
                    GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(team.Name), email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(team.Name)}> sent to {email}");
                CompletePulseSurvey(TestContext, _filteredQuestionsTeam2, team.Name, email);
            }
        }

        #endregion


        #region Create pulse and verify pulse details after removing team members

        private void CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(SavePulseAssessmentV2Request pulseRequest, TeamResponse team, int teamId,
            IList<TeamV2Response> teamWithMembers)
        {

            var login = new LoginPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamNotSurveyedMembers = teamWithMembers.Where(a => a.IsAssessmentCompleted.Equals(false)).ToList();
            var surveyFilledMember = teamNotSurveyedMembers.First().SelectedParticipants.First().Email;
            var surveyNotFilledMember = teamNotSurveyedMembers.First().SelectedParticipants.Last().Email;

            #region Verification before removing team members

            Log.Info("Login to Application");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team dashboard, Edit pulse and verify team member details before removing team member");
            teamAssessmentDashboardPage.NavigateToPage(teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(team.Name), $"{team.Name} team is not displayed at 'Edit Recipients' tab");
            var actualTeamMemberListBeforeRemovingFromTeam = selectRecipientsPage.GetTeamMembersEmailByTeamNames(team.Name);
            var expectedTeamMemberListBeforeRemovingFromTeam = teamWithMembers.First().SelectedParticipants.Select(a => a.Email).ToList();
            Assert.That.ListsAreEqual(expectedTeamMemberListBeforeRemovingFromTeam, actualTeamMemberListBeforeRemovingFromTeam);

            editRecipientsPage.Header.ClickOnCloseIcon();
            editRecipientsPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();

            if (pulseRequest == _draftPulseRequest || pulseRequest == _pendingPulseRequest)
            {
                Assert.AreEqual("Completed by 0 out of 3 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");
            }
            else if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 3 out of 3 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");

                teamAssessmentDashboardPage.ClickOnPulseRadar(_closedPulseRequest.Name);
                Assert.AreEqual(SurveyedMemberCountsForClosedPulse, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
                Assert.AreEqual("3/3",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers(team.Name), "Total and Surveyed members does not match");
            }
            else 
            {
                Assert.AreEqual("Completed by 1 out of 3 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");
            }

            #endregion

            #region Remove team members

            Log.Info("Remove two member from the team (1 who has filled survey and another who has not filled survey)");
            
            var removeMembersFromTeam1 = new List<string>
            {
                surveyFilledMember,
                surveyNotFilledMember
            };

            Log.Info("Navigate to team edit page and remove two members from the team");
            editTeamMemberPage.NavigateToPage(teamId);
            foreach (var memberEmail in removeMembersFromTeam1)
            {
                editTeamMemberPage.DeleteTeamMember(memberEmail.ToLower());
                Assert.IsFalse(editTeamMemberPage.IsTeamMemberDisplayed(memberEmail.ToLower(), 10),
                    $"The selected team member with email {memberEmail} is not deleted properly");
            }

            #endregion

            #region Verification after removing team members

            Log.Info("Navigate to Team dashboard, go to created pulse and verify removed member");
            teamAssessmentDashboardPage.NavigateToPage(teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            if (pulseRequest == _draftPulseRequest || pulseRequest == _pendingPulseRequest)
            {
                Assert.AreEqual("Completed by 0 out of 1 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");
            }
            else if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 3 out of 3 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");
                
            }
            else
            {
                Assert.AreEqual("Completed by 1 out of 2 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");
            }

            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is not displayed");
            actualTeamMemberListBeforeRemovingFromTeam = selectRecipientsPage.GetTeamMembersEmailByTeamNames(team.Name);


            if (pulseRequest == _draftPulseRequest || pulseRequest == _pendingPulseRequest || pulseRequest == _activePulseRequest)
            {
                var membersListAfterRemovedMember = teamNotSurveyedMembers.First().SelectedParticipants.Last().Email;
                Assert.That.ListNotContains(actualTeamMemberListBeforeRemovingFromTeam, membersListAfterRemovedMember, $"Removed member is exists in the {team.Name}");
            }

            if (pulseRequest == _draftPulseRequest || pulseRequest == _pendingPulseRequest)
            {
                Assert.That.ListNotContains(actualTeamMemberListBeforeRemovingFromTeam, surveyFilledMember, $"Removed member is exists in the {team.Name}");
            }

            else if (pulseRequest == _closedPulseRequest)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamMemberEnabled(team.Name, surveyFilledMember), $"{surveyFilledMember} Team Member is enabled");
                Assert.IsTrue(editRecipientsPage.IsTeamMemberEnabled(team.Name, surveyNotFilledMember), $"{surveyNotFilledMember} Team Member is enabled");
            }
            else
            {
                Assert.IsTrue(editRecipientsPage.IsTeamMemberEnabled(team.Name, surveyFilledMember), $"{surveyFilledMember} Team Member is enabled");
            }

            editRecipientsPage.Header.ClickOnCloseIcon();
            editRecipientsPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();

            Log.Info("Go to pulse growth journey page and verify details of team members");
            if (pulseRequest == _activePulseRequest)
            {
                teamAssessmentDashboardPage.ClickOnPulseRadar(pulseRequest.Name);
                Assert.AreEqual(SurveyedMemberCountsForActivePulse, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
                Assert.AreEqual("1/2",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers(team.Name), "Total and Surveyed members does not match");
            }
            else if (pulseRequest == _closedPulseRequest)
            {
                teamAssessmentDashboardPage.ClickOnPulseRadar(pulseRequest.Name);
                Assert.AreEqual(SurveyedMemberCountsForClosedPulse, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
                Assert.AreEqual("3/3",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers(team.Name), "Total and Surveyed members does not match");
            }

            #endregion
        }

        #endregion

    }
}
