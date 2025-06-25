using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.TeamLevel
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("TeamPulse")]
    public class PulseAssessmentEditPageSaveButtonChangesTeamLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;

        private static int _teamIdWithDraftPulse;
        private static int _teamIdWithActivePulse;
        private static int _teamIdWithPendingPulse;
        private static int _teamIdWithClosedPulse;

        private static TeamResponse _teamWithDraftPulse;
        private static TeamResponse _teamWithActivePulse;
        private static TeamResponse _teamWithPendingPulse;
        private static TeamResponse _teamWithClosedPulse;

        private static IList<TeamV2Response> _teamWithMembersOfDraftPulse;
        private static IList<TeamV2Response> _teamWithMembersOfActivePulse;
        private static IList<TeamV2Response> _teamWithMembersOfPendingPulse;
        private static IList<TeamV2Response> _teamWithMembersOfClosedPulse;
       
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam1;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam2;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam3;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam4;

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
                member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWithActivePulse.Uid, member1);
                _teamIdWithActivePulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithActivePulse.Name).TeamId;
                _teamWithMembersOfActivePulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithActivePulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 1);
                _teamWithPendingPulse = _setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWithPendingPulse.Uid, member1);
                _teamIdWithPendingPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithPendingPulse.Name).TeamId;
                _teamWithMembersOfPendingPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithPendingPulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 1);
                _teamWithClosedPulse = _setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                member1 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                _setupApi.AddTeamMembers(_teamWithClosedPulse.Uid, member1);
                _teamIdWithClosedPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithClosedPulse.Name).TeamId;
                _teamWithMembersOfClosedPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithClosedPulse);

                #endregion

                #region Getting Team response
                var questionDetailsResponseTeam1 = GetQuestions(_teamIdWithDraftPulse);
                _filteredQuestionsTeam1 = questionDetailsResponseTeam1.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam2 = GetQuestions(_teamIdWithActivePulse);
                _filteredQuestionsTeam2 = questionDetailsResponseTeam2.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam3 = GetQuestions(_teamIdWithPendingPulse);
                _filteredQuestionsTeam3 = questionDetailsResponseTeam3.FilterQuestions(QuestionSelectionPreferences.Dimension);

                var questionDetailsResponseTeam4 = GetQuestions(_teamIdWithClosedPulse);
                _filteredQuestionsTeam4 = questionDetailsResponseTeam4.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Pulse Requests

                //Create a Draft Pulse
                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam1, _teamWithMembersOfDraftPulse, _teamIdWithDraftPulse, false);

                //Create a Active Pulse
                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam2, _teamWithMembersOfActivePulse, _teamIdWithActivePulse);

                //Create a Pending Pulse
                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam3, _teamWithMembersOfPendingPulse, _teamIdWithPendingPulse, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                //Create a Closed Pulse
                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam4, _teamWithMembersOfClosedPulse, _teamIdWithClosedPulse);

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
        public void PulseV2_DraftPulse_SaveButtonEnableDisable_Verification_TeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_draftPulseRequest, _teamIdWithDraftPulse, _teamWithMembersOfDraftPulse);
           
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Active pulse")]
        public void PulseV2_ActivePulse_SaveButtonEnableDisable_Verification_TeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_activePulseRequest, _teamIdWithActivePulse, _teamWithMembersOfActivePulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Pending pulse")]
        public void PulseV2_PendingPulse_SaveButtonEnableDisable_Verification_TeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_pendingPulseRequest, _teamIdWithPendingPulse, _teamWithMembersOfPendingPulse);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("This test verifies the creation, editing, and saving of a Pulse assessment, ensuring correct email notifications, survey completion, and UI behavior. It checks button states across tabs and validates role-based recipient selection before confirming changes on the Team Assessment Dashboard - For a Closed pulse")]
        public void PulseV2_ClosedPulse_SaveButtonEnableDisable_Verification_TeamLevel()
        {
            VerifySetup(_classInitFailed);

            VerifyPulseScenario(_closedPulseRequest, _teamIdWithClosedPulse, _teamWithMembersOfClosedPulse);

        }

        private void VerifyPulseScenario(SavePulseAssessmentV2Request pulseRequest, int teamId , IList<TeamV2Response>  teamWithMembers)
        {
            AddPulseAssessment(pulseRequest);

            if (pulseRequest == _closedPulseRequest)
            {
                //Fill survey of both members to close the pulse created within team
                foreach (var participant in pulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamWithClosedPulse.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamWithClosedPulse.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestionsTeam4, _teamWithClosedPulse.Name, participant.Email);
                }

            }

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Pulse assessment dashboard and edit Pulse and verify 'Save' button");
            teamAssessmentDashboardPage.NavigateToPage(teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
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
            var filterTag = teamWithMembers.First().SelectedParticipants.Last().Tags.First().Tags.First().Name;
            editRecipientsPage.SelectDeselectRoleFromFilterDropDown(filterTag);
            Assert.IsTrue(editPulseCheckPage.Header.IsSaveEnabled(), "'Save' button is disabled");

            Log.Info("Click on 'Save' button and verify on team assessment dashboard pulse is displayed");
            editRecipientsPage.Header.ClickSaveAsDraftButton();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest.Name), "User is able is not navigated to 'Team assessment dashboard' after clicking on 'Save' button");
        }
    }
}