using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.NTierLevel
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("NTierTeamPulse")]
    public class PulseAssessmentEditPageSaveButtonChangesNTierLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;

        private static int _team1Id;
        private static int _team2Id;
        private static int _team3Id;
        private static int _team4Id;

        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;

        private static IList<TeamV2Response> _team1WithMembers;
        private static IList<TeamV2Response> _team2WithMembers;
        private static IList<TeamV2Response> _team3WithMembers;
        private static IList<TeamV2Response> _team4WithMembers;

        private static SetupTeardownApi _setupApi;
        private static TeamResponse _enterpriseTeamResponse;
        private static List<TeamV2Response> _teamWithTeamMemberResponses;
        private static TeamHierarchyResponse _nTierHierarchyResponses;
        private static RadarQuestionDetailsV2Response _filteredQuestions;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                var team = TeamFactory.GetNormalTeam("Team", 1);
                _team1 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();
                var member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_team1.Uid, member1);
                _team1Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team1.Name).TeamId;
                _team1WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team1Id);

                team = TeamFactory.GetNormalTeam("Team", 1);
                _team2 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();
                member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_team2.Uid, member1);
                _team2Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team2.Name).TeamId;
                _team2WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team2Id);

                team = TeamFactory.GetNormalTeam("Team", 1);
                _team3 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();
                member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_team3.Uid, member1);
                _team3Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team3.Name).TeamId;
                _team3WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team3Id);

                team = TeamFactory.GetNormalTeam("Team", 1);
                _team4 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();
                member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_team4.Uid, member1);
                _team4Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team4.Name).TeamId;
                _team4WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team4Id);

                //Create Multi team and add both teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = _setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _team1.Uid, _team2.Uid, _team3.Uid, _team4.Uid }).GetAwaiter()
                    .GetResult();

                //Create Enterprise team and add multi team as a subteams
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("Enterprise");
                _enterpriseTeamResponse = _setupApi.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(_enterpriseTeamResponse.Uid, new List<Guid> { multiTeamResponse.Uid }).GetAwaiter()
                    .GetResult();

                // Create n-tier team
                var nTierTeamName = "N-Tier" + RandomDataUtil.GetTeamName();
                setupUi.NTier_CreateNTierTeam(nTierTeamName, new List<string> { _enterpriseTeamResponse.Name });
                
                #endregion

                #region Getting Team response

                _nTierHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_enterpriseTeamResponse.Name);

                _teamWithTeamMemberResponses = _setupApi.GetTeamWithTeamMemberResponse(_nTierHierarchyResponses.TeamId).ToList();

                var questionDetailsResponse = GetQuestions(_nTierHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region MyRePulse Requestsgion

                //Create a Draft Pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team1WithMembers, _nTierHierarchyResponses.TeamId, false);

                //Create a Active Pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team2WithMembers, _nTierHierarchyResponses.TeamId);

                //Create a Pending Pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team3WithMembers, _nTierHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                //Create a Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team4WithMembers, _nTierHierarchyResponses.TeamId);

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
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Draft pulse")]
        public void PulseV2_DraftPulse_SaveButtonEnableDisable_Verification_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_draftPulseRequest, _team1);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Active pulse")]
        public void PulseV2_ActivePulse_SaveButtonEnableDisable_Verification_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_activePulseRequest, _team2);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Pending pulse")]
        public void PulseV2_PendingPulse_SaveButtonEnableDisable_Verification_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_pendingPulseRequest, _team3);

        }

        [TestMethod]
        [TestCategory("KnownDefect")]//Bug Id: 53394
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Closed pulse")]
        public void PulseV2_ClosedPulse_SaveButtonEnableDisable_Verification_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_closedPulseRequest, _team4);

        }

        private void VerifyPulseScenario(SavePulseAssessmentV2Request pulseRequest, TeamResponse team)
        {
            AddPulseAssessment(pulseRequest);

            if (pulseRequest == _closedPulseRequest)
            {
                //Fill survey of both members to close the pulse created within team
                foreach (var participant in _closedPulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team4.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestions, _team4.Name, participant.Email);
                }
            }

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Pulse assessment dashboard and edit Pulse and verify 'Save' button");
            mtEtDashboardPage.NavigateToPage(_nTierHierarchyResponses.TeamId, true);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            Assert.IsTrue(editPulseCheckPage.Header.IsSaveEnabled(), "'Save' button is disabled");
            Assert.IsTrue(editPulseCheckPage.Header.IsNextButtonEnabled(), "'Next' button is disabled");

            Log.Info("Click on 'Next' button and verify 'Next' button of 'Edit Questions' tab");
            editPulseCheckPage.Header.ClickOnNextButton();
            Assert.IsTrue(editPulseCheckPage.Header.IsNextButtonEnabled(), "'Next' button is disabled");
            Assert.IsTrue(editPulseCheckPage.Header.IsBackButtonEnabled(), "'Back' button is disabled");

            Log.Info("Go to 'Edit Recipients' tab and verify 'Save' button is disabled");
            editQuestionsPage.Header.ClickOnNextButton();
            Assert.IsTrue(editPulseCheckPage.Header.IsSaveEnabled(), "'Save' button is disabled");
            Assert.IsTrue(editPulseCheckPage.Header.IsBackButtonEnabled(), "'Back' button is disabled");

            Log.Info("Select role from filter dropdown and verify 'Save' button is enabled");
            editRecipientsPage.SelectDeselectRoleFromFilterDropDown(_teamWithTeamMemberResponses.First().SelectedParticipants
                .Last().Tags.First().Tags.First().Name);
            Assert.IsTrue(editPulseCheckPage.Header.IsSaveEnabled(), "'Save' button is disabled");

            Log.Info("Click on 'Envelope' icon and verify save button");
            if (pulseRequest == _closedPulseRequest)
            {
                editRecipientsPage.ClickOnIndividualEnvelopeIcon(team.Name, _teamWithTeamMemberResponses.First().SelectedParticipants
                    .Last().Tags.First().Tags.First().Name);
                Assert.IsTrue(editRecipientsPage.IsEmailSentToasterMessageDisplayed(), "Toaster message is not displayed");
                Assert.IsTrue(editRecipientsPage.Header.IsSaveEnabled(), "'Save' button is disabled");
            }

            Log.Info("Click on 'Save' button and verify on team assessment dashboard pulse is displayed");
            editRecipientsPage.Header.ClickSaveAsDraftButton();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest.Name), "User is able is not navigated to 'Team assessment dashboard' after clicking on 'Save' button");
        }
    }
}