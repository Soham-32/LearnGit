using System;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AtCommon.Utilities;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Api.Enums;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.TeamLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("TeamPulse")]
    public class PulseAssessmentV2EveryTeamLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;

        private static int _teamIdWithActivePulse;
        private static int _teamIdWithDraftPulse;
        private static int _teamIdWithPendingPulse;
        private static int _teamIdWithClosedPulse;

        private static int _multiTeamId;
        private static int _enterpriseTeamId;
        private static int _nTierTeamId;

        private static TeamResponse _teamResponseActivePulse;
        private static TeamResponse _teamResponseDraftPulse;
        private static TeamResponse _teamResponsePendingPulse;
        private static TeamResponse _teamResponseClosedPulse;

        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam1;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam2;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam3;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam4;

        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;
        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                var setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                // Create team
                var teamActivePulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponseActivePulse = setupApi.CreateTeam(teamActivePulse).GetAwaiter().GetResult();
                _teamIdWithActivePulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponseActivePulse.Name).TeamId;
                var teamMembersOfActivePulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithActivePulse);

                var teamDraftPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponseDraftPulse = setupApi.CreateTeam(teamDraftPulse).GetAwaiter().GetResult();
                _teamIdWithDraftPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponseDraftPulse.Name).TeamId;
                var teamMembersOfDraftPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithDraftPulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponsePendingPulse = setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                _teamIdWithPendingPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponsePendingPulse.Name).TeamId;
                var teamMembersOfPendingPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithPendingPulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponseClosedPulse = setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                _teamIdWithClosedPulse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponseClosedPulse.Name).TeamId;
                var teamMembersOfClosedPulse = setupApi.GetTeamWithTeamMemberResponse(_teamIdWithClosedPulse);

                // Create multi team
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeamPulse_");
                var multiResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiResponse.Uid, new List<Guid> { _teamResponseActivePulse.Uid , _teamResponseDraftPulse.Uid, _teamResponsePendingPulse.Uid, _teamResponseClosedPulse.Uid }).GetAwaiter().GetResult();
                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiResponse.Name).TeamId;

                // Create enterprise team
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeamPulse_");
                var enterpriseResponse = setupApi.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(enterpriseResponse.Uid, new List<Guid> { multiResponse.Uid }).GetAwaiter().GetResult();
                _enterpriseTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseResponse.Name).TeamId;

                // Create n-tier team
                var nTierTeamName = "N-Tier" + RandomDataUtil.GetTeamName();
                setupUi.NTier_CreateNTierTeam(nTierTeamName, new List<string> { enterpriseResponse.Name });
                _nTierTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(nTierTeamName).TeamId;

                #endregion

                #region Getting team response

                var questionDetailsResponseTeam1 = GetQuestions(_teamIdWithActivePulse);
                _filteredQuestionsTeam1 = questionDetailsResponseTeam1.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam2 = GetQuestions(_teamIdWithDraftPulse);
                _filteredQuestionsTeam2 = questionDetailsResponseTeam2.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam3 = GetQuestions(_teamIdWithPendingPulse);
                _filteredQuestionsTeam3 = questionDetailsResponseTeam3.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam4 = GetQuestions(_teamIdWithClosedPulse);
                _filteredQuestionsTeam4 = questionDetailsResponseTeam4.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Getting pulse requests

                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam1, teamMembersOfActivePulse, _teamIdWithActivePulse, period: AssessmentPeriod.OneWeek);

                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam2, teamMembersOfDraftPulse, _teamIdWithDraftPulse, false, period: AssessmentPeriod.TwoWeeks);

                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam3, teamMembersOfPendingPulse, _teamIdWithPendingPulse, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam4, teamMembersOfClosedPulse, _teamIdWithClosedPulse, period: AssessmentPeriod.SeventyTwoHours);

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
        [Description("Verifies that a draft, active, pending and closed Pulse Assessment are created at the Team Level and its appears at different levels in the company hierarchy, including N-Tier, Enterprise, Multi-Team, and Team levels.")]
        public void PulseV2_ActiveDraftPendingClose_Pulse_At_MtLevel_And_VerifyAtEveryLevel()
        {
            VerifySetup(_classInitFailed);

            #region Create pulses for all state and fill the survey for closing pulse

            AddPulseAssessment(_activePulseRequest);
            AddPulseAssessment(_closedPulseRequest);
            AddPulseAssessment(_pendingPulseRequest);
            AddPulseAssessment(_draftPulseRequest);

            //Fill survey of both members to close the pulse created
            foreach (var participant in _closedPulseRequest.SelectedTeams.First().SelectedParticipants)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponseClosedPulse.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponseClosedPulse.Name)}> sent to {participant.Email}"); CompletePulseSurvey(TestContext, _filteredQuestionsTeam4, _teamResponseClosedPulse.Name, participant.Email);
            }

            #endregion

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var mtEtDashboard = new MtEtDashboardPage(Driver, Log);
            var activePulseName = _activePulseRequest.Name;
            var draftPulseName = _draftPulseRequest.Name;
            var pendingPulseName = _pendingPulseRequest.Name;
            var closedPulseName = _closedPulseRequest.Name;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            #region Verify all pulse at n-tier level

            Log.Info("Navigate to N-Tier dashboard and verify pulse details");
            mtEtDashboard.NavigateToPage(_nTierTeamId, false, true);
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on N-Tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), $"Edit icon is displayed for {activePulseName} Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on N-Tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), $"Edit icon is displayed for {draftPulseName} Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName} does not exist on N-Tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), $"Edit icon is displayed for {pendingPulseName} Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(closedPulseName, "Closed"), $"{closedPulseName} does not exist on N-Tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(closedPulseName),
                $"Edit icon is displayed for {closedPulseName} Pulse Assessment");

            #endregion

            #region Verify all pulse at enterprise level
            
            Log.Info("Navigate to enterprise team assessment dashboard verify pulse details");
            mtEtDashboard.NavigateToPage(_enterpriseTeamId, true);
            Driver.RefreshPage();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on enterprise level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on enterprise level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName}  does not exist on enterprise level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(closedPulseName, "Closed"), $"{closedPulseName} does not exist on enterprise level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(closedPulseName), "Edit icon is displayed on Pulse Assessment");

            #endregion

            #region Verify all pulse at multi team level

            Log.Info("Navigate to multi team assessment dashboard and verify verify pulse details");
            mtEtDashboard.NavigateToPage(_multiTeamId);
            Driver.RefreshPage();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on multi team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on multi team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName} does not exist on multi team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(closedPulseName, "Closed"), $"{closedPulseName} does not exist on multi team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(closedPulseName), "Edit icon is displayed on Pulse Assessment");

            #endregion

            #region Verify all pulse at team level

            Log.Info($"Navigate to team assessment dashboard of {activePulseName} and verify verify pulse details");
            teamAssessmentDashboardPage.NavigateToPage(_teamIdWithActivePulse);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on team");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), "Edit icon is not displayed on Pulse Assessment");

            Log.Info($"Navigate to team assessment dashboard of {draftPulseName} and verify verify pulse details");
            teamAssessmentDashboardPage.NavigateToPage(_teamIdWithDraftPulse);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on team");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), "Edit icon is not displayed on Pulse Assessment");

            Log.Info($"Navigate to team assessment dashboard of {pendingPulseName} and verify verify pulse details");
            teamAssessmentDashboardPage.NavigateToPage(_teamIdWithPendingPulse);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName} does not exist on team");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), "Edit icon is not displayed on Pulse Assessment");

            Log.Info($"Navigate to team assessment dashboard of {closedPulseName} and verify verify pulse details");
            teamAssessmentDashboardPage.NavigateToPage(_teamIdWithClosedPulse);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(closedPulseName, "Closed"), $"{closedPulseName} does not exist on team");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(closedPulseName), "Edit icon is not displayed on Pulse Assessment");

            #endregion

        }
    }
}
