using System;
using System.Linq;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Assessments.PulseV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AtCommon.Api.Enums;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.MultiTeamLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("MultiTeamPulse")]
    public class PulseAssessmentV2ActiveAndClosedPulseAddRemoveTeamMultiTeamLevelTests : PulseV2BaseTest
    {

        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;

        private static TeamResponse _teamWithActivePulse;
        private static TeamResponse _teamWithclosedPulse;
        private static TeamResponse _team3;
        private static TeamResponse _team4;

        private static int _teamIdWithActivePulse;
        private static int _teamIdWithclosedPulse;
        private static int _multiTeamId;

        private const int SurveyedMemberCounts = 2;
        private static TeamResponse _multiTeamResponse;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static TeamHierarchyResponse _multiTeamHierarchyResponses;

        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static IList<TeamV2Response> _teamWithMembersOfActivePulse;
        private static IList<TeamV2Response> _teamWithMembersOfClosedPulse;


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation
                //Create 2 teams with 2 members in each team
                var teamActivePulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamWithActivePulse = _setupApi.CreateTeam(teamActivePulse).GetAwaiter().GetResult();
                _teamIdWithActivePulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithActivePulse.Name).TeamId;
                _teamWithMembersOfActivePulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithActivePulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamWithclosedPulse = _setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                _teamIdWithclosedPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithclosedPulse.Name).TeamId;
                _teamWithMembersOfClosedPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithclosedPulse);

                var team3 = TeamFactory.GetNormalTeam("Team", 2);
                _team3 = _setupApi.CreateTeam(team3).GetAwaiter().GetResult();
                
                var team4 = TeamFactory.GetNormalTeam("Team", 2);
                _team4 = _setupApi.CreateTeam(team4).GetAwaiter().GetResult();
                
                //Create Multi team and add 2 teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeamPulse_");
                _multiTeamResponse = _setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _teamWithActivePulse.Uid, _teamWithclosedPulse.Uid }).GetAwaiter()
                    .GetResult();
                _multiTeamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeamResponse.Name).TeamId;
               
                #endregion

                #region Getting Team Response
                _multiTeamHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeamResponse.Name);

                var questionDetailsResponse = GetQuestions(_multiTeamHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                #endregion

                #region Pulse Requests

                //Pulse request for Active Pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfActivePulse, _multiTeamHierarchyResponses.TeamId, period: AssessmentPeriod.OneWeek);

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
        [Description("Verifies add/remove team from an active pulse scenarios.")]
        public void PulseV2_ActivePulse_AddAndRemoveTeams_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_activePulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(_activePulseRequest, _teamWithActivePulse);
            
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53394
        [TestCategory("CompanyAdmin")]
        [Description("Verifies add/remove team from closed pulse scenarios.")]
        public void PulseV2_ClosedPulse_AddAndRemoveTeams_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            AddPulseAssessment(_closedPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(_closedPulseRequest, _teamWithclosedPulse);

        }

        private void CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(SavePulseAssessmentV2Request pulseRequest,
            TeamResponse team)
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

            #region Verification before adding/removing teams
            
            var login = new LoginPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboard = new MtEtDashboardPage(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            Log.Info("login to application and Navigate to Multi team dashboard");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            mtEtDashboard.NavigateToPage(_multiTeamHierarchyResponses.TeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();

            if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 1 out of 1 Teams",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            }
            else
            {
                Assert.AreEqual("Completed by 0 out of 1 Teams",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            }
            
            Log.Info("Go to pulse edit page and verify team details");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            Assert.IsTrue(editRecipientsPage.IsTeamSelected(team.Name), "Teams are not selected");

            editRecipientsPage.Header.ClickOnCloseIcon();
            editRecipientsPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();

            if (pulseRequest == _closedPulseRequest)
            {
                Log.Info("Go to pulse growth journey page and verify details of added/removed teams");
                teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
                teamAssessmentDashboardPage.ClickOnPulseRadar(pulseRequest.Name);
                Assert.AreEqual(SurveyedMemberCounts, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
                Assert.AreEqual("2/2",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers(team.Name), "Total and Surveyed members does not match");
            }

            #region Add team5 and remove 1 team form the added 2 team form the multi team

            Log.Info("Remove 1 team from the multi team");
            editEtMtSubTeamBasePage.NavigateToPage("multiteam", _multiTeamId);
            
            if (pulseRequest == _activePulseRequest)
            {
                editEtMtSubTeamBasePage.SelectSubTeamViaSearchBox(_team3.Name);
            }
            else
            {
                editEtMtSubTeamBasePage.SelectSubTeamViaSearchBox(_team4.Name);
            }

            editEtMtSubTeamBasePage.RemoveSubTeam(team.Name);
            editEtMtSubTeamBasePage.ClickUpdateSubTeamButton();
            Assert.That.ListNotContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), team.Name, $"List contains - {team.Name}");
          
            if (pulseRequest == _activePulseRequest)
            {
                Assert.That.ListContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), _team3.Name, $"List does not contain - {_team3.Name}");
            }
            else
            {
                Assert.That.ListContains(editEtMtSubTeamBasePage.GetSelectedSubTeamList(), _team4.Name, $"List does not contain - {_team4.Name}");
            }

            #endregion

            #region Verification after adding/removing teams

            mtEtDashboard.NavigateToPage(_multiTeamHierarchyResponses.TeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();

            if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 1 out of 1 Teams", teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            }
            else
            {
                Assert.AreEqual("Completed by 0 out of 0 Teams", teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            }
            
            Log.Info("Go to pulse edit page and verify team details");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            Assert.IsFalse(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is displayed");
            
            if (pulseRequest == _activePulseRequest)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(_team3.Name), "Team is not displayed");
            }
            else
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(_team4.Name), "Team is not displayed");
            }

            if (pulseRequest == _activePulseRequest)
            {
                editRecipientsPage.ClickOnSelectTeamCheckbox(_team3.Name);
                Assert.IsTrue(editRecipientsPage.IsTeamSelected(_team3.Name), "Team is not selected");
            }
            else
            {
                editRecipientsPage.ClickOnSelectTeamCheckbox(_team4.Name);
                Assert.IsTrue(editRecipientsPage.IsTeamSelected(_team4.Name), "Team is not selected");
            }
            
            editRecipientsPage.Header.ClickSaveAsDraftButton();
            
            if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 1 out of 1 Teams",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            }
            else
            {
                Assert.AreEqual("Completed by 0 out of 1 Teams",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            }
            
            #endregion

            #endregion
        }
    }
}
