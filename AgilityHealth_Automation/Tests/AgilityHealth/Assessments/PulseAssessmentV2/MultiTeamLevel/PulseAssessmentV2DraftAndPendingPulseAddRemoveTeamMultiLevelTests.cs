using System;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class PulseAssessmentV2DraftAndPendingPulseAddRemoveTeamMultiLevelTests : PulseV2BaseTest
    {

        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;

        private static TeamResponse _teamIWithDraftpulse;
        private static TeamResponse _teamWithPendingPulse;
        private static TeamResponse _team3;
        private static TeamResponse _team4;

        private static int _teamIdWithDraftPulse;
        private static int _teamIdWithPendingPulse;
        private static int _multiTeamId;

        private static TeamResponse _multiTeamResponse;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static TeamHierarchyResponse _multiTeamHierarchyResponses;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;

        private static IList<TeamV2Response> _teamWithMembersOfDraftPulse;
        private static IList<TeamV2Response> _teamWithMembersOfPendingPulse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                //Create 4 teams with 2 members in each team
                var teamDraftPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamIWithDraftpulse = _setupApi.CreateTeam(teamDraftPulse).GetAwaiter().GetResult();
                _teamIdWithDraftPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamIWithDraftpulse.Name).TeamId;
                _teamWithMembersOfDraftPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithDraftPulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamWithPendingPulse = _setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                _teamIdWithPendingPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithPendingPulse.Name).TeamId;
                _teamWithMembersOfPendingPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithPendingPulse);

                var team3 = TeamFactory.GetNormalTeam("Team", 2);
                _team3 = _setupApi.CreateTeam(team3).GetAwaiter().GetResult();

                var team4 = TeamFactory.GetNormalTeam("Team", 2);
                _team4 = _setupApi.CreateTeam(team4).GetAwaiter().GetResult();

                //Create Multi team and add 2 teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                _multiTeamResponse = _setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _teamIWithDraftpulse.Uid, _teamWithPendingPulse.Uid }).GetAwaiter()
                    .GetResult();
                _multiTeamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeamResponse.Name).TeamId;
               
                #endregion

                #region Getting Team Response
                
                _multiTeamHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeamResponse.Name);
                var questionDetailsResponse = GetQuestions(_multiTeamHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
               
                #endregion

                #region Pulse Requests

                //Pulse request for Draft Pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfDraftPulse, _multiTeamHierarchyResponses.TeamId, false, period: AssessmentPeriod.TwentyFourHours);

                //Pulse request for Pending Pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfPendingPulse, _multiTeamHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

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
        [Description("Verifies add/remove team from draft pulse scenarios.")]
        public void PulseV2_DraftPulse_AddAndRemoveTeams_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_draftPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(_draftPulseRequest, _teamIWithDraftpulse);
        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies add/remove team from pending pulse scenarios.")]
        public void PulseV2_PendingPulse_AddAndRemoveTeams_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_pendingPulseRequest);
            CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(_pendingPulseRequest, _teamWithPendingPulse);

        }

        private void CreatePulseAndVerifyPulseDetailsAfterAddingRemovingTeam(SavePulseAssessmentV2Request pulseRequest,
            TeamResponse team)
        {

            #region Verification before adding/removing teams

            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboard = new MtEtDashboardPage(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            Log.Info("Navigate to Multi team dashboard");
            mtEtDashboard.NavigateToPage(_multiTeamHierarchyResponses.TeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            Assert.AreEqual("Completed by 0 out of 1 Teams",
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");

            Log.Info("Go to pulse edit page and verify team details");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            Assert.IsTrue(editRecipientsPage.IsTeamSelected(team.Name), "Teams are not selected");
            editRecipientsPage.Header.ClickOnCloseIcon();
            editRecipientsPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();

            #region Add team5 and remove 1 team form the added 2 team form the multi team

            Log.Info("Remove 1 team from the multi team");
            editEtMtSubTeamBasePage.NavigateToPage("multiteam", _multiTeamId);
            if (pulseRequest == _draftPulseRequest)
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
            if (pulseRequest == _draftPulseRequest)
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
            Assert.AreEqual("Completed by 0 out of 0 Teams",
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");

            Log.Info("Go to pulse edit page and verify team details");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            Assert.IsFalse(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is displayed");
            if (pulseRequest == _draftPulseRequest)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(_team3.Name), "Team is not displayed");
            }
            else
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(_team4.Name), "Team is not displayed");
            }
            
            editRecipientsPage.ClickOnSelectTeamCheckbox(_team3.Name);
            Assert.IsTrue(editRecipientsPage.IsTeamSelected(_team3.Name), "Team is not selected");
            editRecipientsPage.Header.ClickSaveAsDraftButton();
            Assert.AreEqual("Completed by 0 out of 1 Teams",
                teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(pulseRequest.Name), "Team info doesn't match");
            
            #endregion

            #endregion
        }
    }
}
