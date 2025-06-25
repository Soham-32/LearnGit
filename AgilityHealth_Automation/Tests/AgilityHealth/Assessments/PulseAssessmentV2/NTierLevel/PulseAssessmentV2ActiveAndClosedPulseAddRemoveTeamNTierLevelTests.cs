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
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.NTierLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("NTierTeamPulse")]
    public class PulseAssessmentV2ActiveAndClosedPulseAddRemoveTeamNTierLevelTests : PulseV2BaseTest
    {

        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;
        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;

        private static int _team1Id;
        private static int _team2Id;
        private static int _multiTeamId;

        private const int SurveyedMemberCounts = 2;
        private static TeamResponse _enterpriseTeamResponse;
        private static TeamResponse _multiTeamResponse;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static TeamHierarchyResponse _nTierHierarchyResponses;

        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static IList<TeamV2Response> _team1WithMembers;
        private static IList<TeamV2Response> _team2WithMembers;


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation
                //Create 2 teams with 2 members in each team
                var team1 = TeamFactory.GetNormalTeam("Team", 2);
                _team1 = _setupApi.CreateTeam(team1).GetAwaiter().GetResult();
                _team1Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team1.Name).TeamId;
                _team1WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team1Id);

                var team2 = TeamFactory.GetNormalTeam("Team", 2);
                _team2 = _setupApi.CreateTeam(team2).GetAwaiter().GetResult();
                _team2Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team2.Name).TeamId;
                _team2WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team2Id);

                var team3 = TeamFactory.GetNormalTeam("Team", 2);
                _team3 = _setupApi.CreateTeam(team3).GetAwaiter().GetResult();

                var team4 = TeamFactory.GetNormalTeam("Team", 2);
                _team4 = _setupApi.CreateTeam(team4).GetAwaiter().GetResult();

                //Create Multi team and add 2 teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                _multiTeamResponse = _setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _team1.Uid, _team2.Uid }).GetAwaiter()
                    .GetResult();
                _multiTeamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeamResponse.Name).TeamId;

                //Create Enterprise team and add multi team as a subteams
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("Enterprise");
                _enterpriseTeamResponse = _setupApi.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(_enterpriseTeamResponse.Uid, new List<Guid> { _multiTeamResponse.Uid }).GetAwaiter()
                    .GetResult();

                // Create n-tier team
                var nTierTeamName = "N-Tier" + RandomDataUtil.GetTeamName();
                setupUi.NTier_CreateNTierTeam(nTierTeamName, new List<string> { _enterpriseTeamResponse.Name });

                #endregion

                #region Getting Team Response

                _nTierHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(nTierTeamName);
                var questionDetailsResponse = GetQuestions(_nTierHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Pulse Requests

                //Pulse request for Active Pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team1WithMembers, _nTierHierarchyResponses.TeamId, period: AssessmentPeriod.OneWeek);

                //Pulse request for Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team2WithMembers, _nTierHierarchyResponses.TeamId, period: AssessmentPeriod.SeventyTwoHours);

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
        [Description("Verifies add/remove team form active pulse scenarios.")]
        public void PulseV2_ActivePulse_AddAndRemoveTeams_Verification_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_activePulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(_activePulseRequest, _team1);

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53394
        [TestCategory("CompanyAdmin")]
        [Description("Verifies add/remove team form closed pulse scenarios.")]
        public void PulseV2_ClosedPulse_AddAndRemoveTeams_Verification_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_closedPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(_closedPulseRequest, _team2);

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

            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboard = new MtEtDashboardPage(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            Log.Info("Navigate to enterprise team dashboard");
            mtEtDashboard.NavigateToPage(_nTierHierarchyResponses.TeamId, false, true);
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

            mtEtDashboard.NavigateToPage(_nTierHierarchyResponses.TeamId, false, true);
            Assert.AreEqual("Completed by 0 out of 0 Teams", teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");

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
            Driver.RefreshPage();
            Assert.AreEqual("Completed by 0 out of 1 Teams",
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");

            #endregion

            #endregion
        }
    }
}
