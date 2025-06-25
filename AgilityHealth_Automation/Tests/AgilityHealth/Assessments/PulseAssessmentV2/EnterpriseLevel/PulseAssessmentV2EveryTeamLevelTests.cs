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

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.EnterpriseLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("EnterpriseTeamPulse")]
    public class PulseAssessmentV2EveryTeamLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;

        private static int _teamId;
        private static int _team1Id;
        private static int _multiTeamId;
        private static int _enterpriseTeamId;
        private static int _nTierTeamId;
        private static TeamResponse _teamResponse;
        private static TeamResponse _team1Response;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;
        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static RadarQuestionDetailsV2Response _filteredQuestions;


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                var setupApi = new SetupTeardownApi(TestEnvironment);


                #region Team Creation

                // Create team
                var team = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                var teamMembers = setupApi.GetTeamWithTeamMemberResponse(_teamId);

                var team1 = TeamFactory.GetNormalTeam("Team", 2);
                _team1Response = setupApi.CreateTeam(team1).GetAwaiter().GetResult();
                _team1Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team1Response.Name).TeamId;
                var team1Members = setupApi.GetTeamWithTeamMemberResponse(_team1Id);

                // Create multi team
                var multiTeam = TeamFactory.GetMultiTeam("Multi Team");
                var multiResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiResponse.Uid, new List<Guid> { _teamResponse.Uid , _team1Response.Uid}).GetAwaiter().GetResult();
                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiResponse.Name).TeamId;

                // Create enterprise team
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("Enterprise Team");
                var enterpriseResponse = setupApi.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(enterpriseResponse.Uid, new List<Guid> { multiResponse.Uid }).GetAwaiter().GetResult();
                _enterpriseTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseResponse.Name).TeamId;

                // Create n-tier team
                var nTierTeamName = "N-Tier" + RandomDataUtil.GetTeamName();
                setupUi.NTier_CreateNTierTeam(nTierTeamName, new List<string> { enterpriseResponse.Name });
                _nTierTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(nTierTeamName).TeamId;

                #endregion


                #region Getting enterprise team response

                var enterpriseHierarchyResponses = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseResponse.Name);
                var questionDetailsResponse = GetQuestions(enterpriseHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion


                #region Getting pulse requests

                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, teamMembers, enterpriseHierarchyResponses.TeamId, period: AssessmentPeriod.OneWeek);

                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, team1Members, enterpriseHierarchyResponses.TeamId, period: AssessmentPeriod.SeventyTwoHours);

                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, teamMembers, enterpriseHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, teamMembers, enterpriseHierarchyResponses.TeamId, false, period: AssessmentPeriod.TwoWeeks);

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
        [Description("Verifies that a draft, active, pending and closed Pulse Assessment are created at the Enterprise Team Level and its appears at different levels in the company hierarchy, including N-Tier, Enterprise, Multi-Team, and Team levels.")]
        public void PulseV2_ActiveDraftPendingClose_Pulse_At_EtLevel_And_VerifyAtEveryLevel()
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
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team1Response.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team1Response.Name)}> sent to {participant.Email}");
                CompletePulseSurvey(TestContext, _filteredQuestions, _team1Response.Name, participant.Email);
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
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on n-tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), $"Edit icon is displayed for {activePulseName} Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on n-tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), $"Edit icon is displayed for {draftPulseName} Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName} does not exist on n-tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), $"Edit icon is displayed for {pendingPulseName} Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(closedPulseName, "Closed"), $"{closedPulseName} does not exist on n-tier level");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(closedPulseName),
                $"Edit icon is displayed for {closedPulseName} Pulse Assessment");

            #endregion

            #region Verify all pulse at enterprise level

            Log.Info("Navigate to enterprise team assessment dashboard verify pulse details");
            mtEtDashboard.NavigateToPage(_enterpriseTeamId, true);
            Driver.RefreshPage();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on n-tier level");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), "Edit icon is not displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on n-tier level");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName}  does not exist on n-tier level");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(closedPulseName, "Closed"), $"{closedPulseName} does not exist on n-tier level");
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(closedPulseName), "Edit icon is displayed on Pulse Assessment");


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

            Log.Info("Navigate to team assessment dashboard and verify verify pulse details");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(activePulseName, "Open"), $"{activePulseName} does not exist on team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(activePulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(draftPulseName, "Draft"), $"{draftPulseName} does not exist on team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(draftPulseName), "Edit icon is displayed on Pulse Assessment");

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseStatusDisplayed(pendingPulseName, "Pending"), $"{pendingPulseName} does not exist on team");
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentEditIconDisplayed(pendingPulseName), "Edit icon is displayed on Pulse Assessment");

            #endregion

        }
    }
}
