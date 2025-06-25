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
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AtCommon.Api.Enums;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.MultiTeamLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("MultiTeamPulse")]
    public class PulseAssessmentV2RemoveTeamMemberFromSelectedTeamMultiLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private const int SurveyedMemberCountsForActivePulse = 1;
        private const int SurveyedMemberCountsForClosedPulse = 3;

        private static int _teamIdWithDraftPulse;
        private static int _teamIdActivePulse;
        private static int _teamIdIWithPendingPulse;
        private static int _teamIdWithClosedPulse;

        private static TeamResponse _teamWithDraftPulse;
        private static TeamResponse _teamActivePulse;
        private static TeamResponse _teamWithPendingPulse;
        private static TeamResponse _teamWithClosedPulse;

        private static IList<TeamV2Response> _teamWithMembersOfDraftPulse;
        private static IList<TeamV2Response> _teamWithMembersOfActivePulse;
        private static IList<TeamV2Response> _team3WithMembers;
        private static IList<TeamV2Response> _teamWithMembersOfClosedPulse;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static TeamHierarchyResponse _multiTeamHierarchyResponses;

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
                _teamActivePulse = setupApi.CreateTeam(teamActivePulse).GetAwaiter().GetResult();
                _teamIdActivePulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamActivePulse.Name).TeamId;
                _teamWithMembersOfActivePulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdActivePulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 3);
                _teamWithPendingPulse = setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                _teamIdIWithPendingPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithPendingPulse.Name).TeamId;
                _team3WithMembers = setupApi.GetTeamWithTeamMemberResponse(_teamIdIWithPendingPulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 3);
                _teamWithClosedPulse = setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                _teamIdWithClosedPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithClosedPulse.Name).TeamId;
                _teamWithMembersOfClosedPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithClosedPulse);

                //Create Multi team and add newly created three teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeamPulse_");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _teamWithDraftPulse.Uid, _teamActivePulse.Uid, _teamWithPendingPulse.Uid, _teamWithClosedPulse.Uid }).GetAwaiter()
                    .GetResult();

                #endregion

                #region Getting Multi team response

                _multiTeamHierarchyResponses = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name);
                var questionDetailsResponse = GetQuestions(_multiTeamHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Getting pulse requests

                // Pulse request for draft pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfDraftPulse, _multiTeamHierarchyResponses.TeamId, false, period: AssessmentPeriod.TwentyFourHours);

                // Pulse request for active pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfActivePulse, _multiTeamHierarchyResponses.TeamId, period: AssessmentPeriod.OneWeek);

                // Pulse request for pending pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team3WithMembers, _multiTeamHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                //Pulse request for Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfClosedPulse, _multiTeamHierarchyResponses.TeamId, period: AssessmentPeriod.SeventyTwoHours);

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
        public void PulseV2_Draft_Pulse_At_MtLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_draftPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_draftPulseRequest, _teamWithDraftPulse, _teamIdWithDraftPulse, _teamWithMembersOfDraftPulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Active pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Active_Pulse_At_MtLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_activePulseRequest);
            FillSurvey(_activePulseRequest, _teamActivePulse);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_activePulseRequest, _teamActivePulse, _teamIdActivePulse, _teamWithMembersOfActivePulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Pending pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Pending_Pulse_At_MtLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_pendingPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_pendingPulseRequest, _teamWithPendingPulse, _teamIdIWithPendingPulse, _team3WithMembers);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Closed pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Closed_Pulse_At_MtLevel_And_RemoveTeamMemberFromSelectedTeam()
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
                        GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(team.Name),
                            participant.Email, "Inbox", "", 360),
                        $"Could not find email with subject <{SharedConstants.PulseEmailSubject(team.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestions, team.Name, participant.Email);
                }

            }
            else
            {
                //Fill survey of both members to close the pulse created within team
                var email = pulseRequest.SelectedTeams.First().SelectedParticipants.First().Email;
                Assert.IsTrue(
                    GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(team.Name),
                        email, "Inbox", "", 360),
                    $"Could not find email with subject <{SharedConstants.PulseEmailSubject(team.Name)}> sent to {email}");
                CompletePulseSurvey(TestContext, _filteredQuestions, team.Name, email);
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
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamNotSurveyedMembers = teamWithMembers.Where(a => a.IsAssessmentCompleted.Equals(false)).ToList();
            var surveyFilledMember = teamNotSurveyedMembers.First().SelectedParticipants.First().Email;
            var surveyNotFilledMember = teamNotSurveyedMembers.First().SelectedParticipants.Last().Email;

            #region Verification before removing team members

            Log.Info("Login to the Application");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Multi dashboard, Edit pulse and verify team member details before removing team member");
            mtEtDashboardPage.NavigateToPage(_multiTeamHierarchyResponses.TeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
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

            Log.Info("Navigate to Multi dashboard, go to created pulse and verify removed member");
            mtEtDashboardPage.NavigateToPage(_multiTeamHierarchyResponses.TeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
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
