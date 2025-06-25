using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.MultiTeamLevel
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("MultiTeamPulse")]
    public class PulseAssessmentEditPageSaveButtonChangesMultiTeamLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;

        private static int _teamIdWithDraftPulse;
        private static int _teamIdWIthActivePulse;
        private static int _teamIdWithPendingPulse;
        private static int _teamIdWithClosedPulse;

        private static TeamResponse _teamWithDraftPulse;
        private static TeamResponse _teamWithActivePulse;
        private static TeamResponse _teamWithPendingPulse;
        private static TeamResponse _teamWIthClosedPulse;

        private static IList<TeamV2Response> _teamWithMembersOfDraftPulse;
        private static IList<TeamV2Response> _teamWithMembersOfActivePulse;
        private static IList<TeamV2Response> _teamWithMembersOfPendingPulse;
        private static IList<TeamV2Response> _teamWithMembersOfClosedPulse;

        private static List<TeamV2Response> _teamWithTeamMemberResponses;
        private static TeamHierarchyResponse _multiTeamHierarchyResponses;
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
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                var teamDraftPulse = TeamFactory.GetNormalTeam("Team", 1);
                _teamWithDraftPulse = _setupApi.CreateTeam(teamDraftPulse).GetAwaiter().GetResult();
                var member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWithDraftPulse.Uid, member1);
                _teamIdWithDraftPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithDraftPulse.Name).TeamId;
                _teamWithMembersOfDraftPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithDraftPulse);

                var teamActivePulse = TeamFactory.GetNormalTeam("Team", 1);
                _teamWithActivePulse = _setupApi.CreateTeam(teamActivePulse).GetAwaiter().GetResult();
                var member2 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWithActivePulse.Uid, member2);
                _teamIdWIthActivePulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithActivePulse.Name).TeamId;
                _teamWithMembersOfActivePulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWIthActivePulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 1);
                _teamWithPendingPulse = _setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                var member3 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWithPendingPulse.Uid, member3);
                _teamIdWithPendingPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithPendingPulse.Name).TeamId;
                _teamWithMembersOfPendingPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithPendingPulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 1);
                _teamWIthClosedPulse = _setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                var member4 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWIthClosedPulse.Uid, member4);
                _teamIdWithClosedPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWIthClosedPulse.Name).TeamId;
                _teamWithMembersOfClosedPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithClosedPulse);

                //Create Multi team and add all teams as a subteams
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeamPulse_");
                var multiTeamResponse = _setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _teamWithDraftPulse.Uid, _teamWithActivePulse.Uid, _teamWithPendingPulse.Uid, _teamWIthClosedPulse.Uid }).GetAwaiter()
                    .GetResult();

                #endregion

                #region Getting Team response

                _multiTeamHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name);

                _teamWithTeamMemberResponses = _setupApi.GetTeamWithTeamMemberResponse(_multiTeamHierarchyResponses.TeamId).ToList();

                var questionDetailsResponse = GetQuestions(_multiTeamHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Pulse Requests

                //Create a Draft Pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfDraftPulse, _multiTeamHierarchyResponses.TeamId, false);

                //Create a Active Pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfActivePulse, _multiTeamHierarchyResponses.TeamId);

                //Create a Pending Pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfPendingPulse, _multiTeamHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                //Create a Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithMembersOfClosedPulse, _multiTeamHierarchyResponses.TeamId);
                
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
        public void PulseV2_DraftPulse_SaveButtonEnableDisable_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            VerifyPulseScenario(_draftPulseRequest);
           
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Active pulse")]
        public void PulseV2_ActivePulse_SaveButtonEnableDisable_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            VerifyPulseScenario(_activePulseRequest);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Pending pulse")]
        public void PulseV2_PendingPulse_SaveButtonEnableDisable_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            VerifyPulseScenario(_pendingPulseRequest);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Closed pulse")]
        public void PulseV2_ClosedPulse_SaveButtonEnableDisable_Verification_MultiTeamLevel()
        {
            VerifySetup(_classInitFailed);
            VerifyPulseScenario(_closedPulseRequest);

        }

        private void VerifyPulseScenario(SavePulseAssessmentV2Request pulseRequest)
        {
            AddPulseAssessment(pulseRequest);

            if (pulseRequest == _closedPulseRequest)
            {
                //Fill survey of both members to close the pulse created within team
                foreach (var participant in pulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamWIthClosedPulse.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamWIthClosedPulse.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestions, _teamWIthClosedPulse.Name, participant.Email);
                }

            }

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info("Login to the application");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Pulse assessment dashboard and edit Pulse and verify 'Save' button");
            mtEtDashboardPage.NavigateToPage(_multiTeamHierarchyResponses.TeamId);
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

            Log.Info("Click on 'Save' button and verify on team assessment dashboard pulse is displayed");
            editRecipientsPage.Header.ClickSaveAsDraftButton();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest.Name), "User is able is not navigated to 'Team assessment dashboard' after clicking on 'Save' button");
        }
    }
}