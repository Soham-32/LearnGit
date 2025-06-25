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
using AgilityHealth_Automation.SetUpTearDown;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.NTierLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("NTierTeamPulse")]
    public class PulseAssessmentV2RemoveTeamMemberFromSelectedTeamNTierLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;

        private static int _team1Id;
        private static int _team2Id;
        private static int _team3Id;
        private static int _team4Id;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static IList<TeamV2Response> _team1WithMembers;
        private static IList<TeamV2Response> _team2WithMembers;
        private static IList<TeamV2Response> _team3WithMembers;
        private static IList<TeamV2Response> _team4WithMembers;

        private const int SurveyedMemberCountsForActivePulse = 1;
        private const int SurveyedMemberCountsForClosedPulse = 3;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static TeamHierarchyResponse _nTierHierarchyResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                var setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                //Create 4 teams
                var team1 = TeamFactory.GetNormalTeam("Team", 3);
                _team1 = setupApi.CreateTeam(team1).GetAwaiter().GetResult();
                _team1Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team1.Name).TeamId;
                _team1WithMembers = setupApi.GetTeamWithTeamMemberResponse(_team1Id);

                var team2 = TeamFactory.GetNormalTeam("Team", 3);
                _team2 = setupApi.CreateTeam(team2).GetAwaiter().GetResult();
                _team2Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team2.Name).TeamId;
                _team2WithMembers = setupApi.GetTeamWithTeamMemberResponse(_team2Id);

                var team3 = TeamFactory.GetNormalTeam("Team", 3);
                _team3 = setupApi.CreateTeam(team3).GetAwaiter().GetResult();
                _team3Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team3.Name).TeamId;
                _team3WithMembers = setupApi.GetTeamWithTeamMemberResponse(_team3Id);

                var team4 = TeamFactory.GetNormalTeam("Team", 3);
                _team4 = setupApi.CreateTeam(team4).GetAwaiter().GetResult();
                _team4Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team4.Name).TeamId;
                _team4WithMembers = setupApi.GetTeamWithTeamMemberResponse(_team4Id);

                //Create Multi team and add newly created three teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _team1.Uid, _team2.Uid, _team3.Uid, _team4.Uid }).GetAwaiter()
                    .GetResult();

                //Create Enterprise team and add newly created multi team as a subteams
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("Enterprise");
                var enterpriseTeamResponse = setupApi.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(enterpriseTeamResponse.Uid, new List<Guid> { multiTeamResponse.Uid }).GetAwaiter()
                    .GetResult();

                // Create n-tier team
                var nTierTeamName = "N-Tier" + RandomDataUtil.GetTeamName();
                setupUi.NTier_CreateNTierTeam(nTierTeamName, new List<string> { enterpriseTeamResponse.Name });

                #endregion

                #region Getting enterprise team response

                _nTierHierarchyResponses = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name);
                setupApi.GetTeamWithTeamMemberResponse(_nTierHierarchyResponses.TeamId);
                var questionDetailsResponse = GetQuestions(_nTierHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Getting pulse requests

                // Pulse request for draft pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team1WithMembers, _nTierHierarchyResponses.TeamId, false, period: AssessmentPeriod.TwentyFourHours);

                // Pulse request for active pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team2WithMembers, _nTierHierarchyResponses.TeamId, period: AssessmentPeriod.OneWeek);

                // Pulse request for pending pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team3WithMembers, _nTierHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                //Pulse request for Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team4WithMembers, _nTierHierarchyResponses.TeamId, period: AssessmentPeriod.SeventyTwoHours);

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
        [Description("Verifies removing a team member from the Active pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Draft_Pulse_At_NTierLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);


            AddPulseAssessment(_draftPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_draftPulseRequest, _team1, _team1Id, _team1WithMembers);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Active pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Active_Pulse_At_NTierEtLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_activePulseRequest);
            FillSurvey(_activePulseRequest, _team2);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_activePulseRequest, _team2, _team2Id, _team2WithMembers);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Active pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Pending_Pulse_At_NTierEtLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_pendingPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_pendingPulseRequest, _team3, _team3Id, _team3WithMembers);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies removing a team member from the Active pulse who has filled survey and not filled survey yet.")]
        public void PulseV2_Closed_Pulse_At_NTierEtLevel_And_RemoveTeamMemberFromSelectedTeam()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_closedPulseRequest);
            FillSurvey(_closedPulseRequest, _team4);
            CreatePulseAndVerifyPulseDetailsAfterRemovingTeamMembers(_closedPulseRequest, _team4, _team4Id, _team4WithMembers);

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

            Log.Info("Navigate to N-tier dashboard, Edit pulse and verify team member details before removing team member");
            mtEtDashboardPage.NavigateToPage(_nTierHierarchyResponses.TeamId, true);
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
                Assert.AreEqual("3/3",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers(team.Name), "Total and Surveyed members does not match");
            }
            else
            {
                Assert.AreEqual("Completed by 1 out of 3 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Team member count doesn't match");
            }

            if (pulseRequest == _activePulseRequest)
            {
                teamAssessmentDashboardPage.ClickOnPulseRadar(pulseRequest.Name);
                Assert.AreEqual(SurveyedMemberCountsForActivePulse, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
                Assert.AreEqual("1/3",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers(team.Name), "Total and Surveyed members does not match");
            }

            #endregion

            #region Remove team members

            Log.Info("Navigate to team edit page and remove two member from the team (1 who has filled survey and another who has not filled survey)");
            var removeMembersFromTeam1 = new List<string>
            {
                surveyFilledMember,
                surveyNotFilledMember
            };
            editTeamMemberPage.NavigateToPage(teamId);
            foreach (var memberEmail in removeMembersFromTeam1)
            {
                editTeamMemberPage.DeleteTeamMember(memberEmail.ToLower());
                Assert.IsFalse(editTeamMemberPage.IsTeamMemberDisplayed(memberEmail.ToLower(), 10),
                    $"The selected team member with email {memberEmail} is not deleted properly");
            }

            #endregion

            #region Verification after removing team members

            Log.Info("Navigate to N-tier dashboard, go to created pulse and verify removed member");
            mtEtDashboardPage.NavigateToPage(_nTierHierarchyResponses.TeamId, true);

            if (pulseRequest == _draftPulseRequest || pulseRequest == _pendingPulseRequest)
            {
                Assert.AreEqual("Completed by 0 out of 1 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name),
                    "Team member count doesn't match");
            }
            else if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 3 out of 3 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name),
                    "Team member count doesn't match");

            }
            else
            {
                Assert.AreEqual("Completed by 1 out of 2 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name),
                    "Team member count doesn't match");
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
