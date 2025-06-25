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
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Assessments.PulseV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AtCommon.Api.Enums;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.TeamLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("TeamPulse")]
    public class PulseAssessmentV2AddMemberVerificationTeamLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;
        private const int SurveyedMemberCounts = 2;

        private static TeamResponse _teamWithDraftPulse;
        private static TeamResponse _teamWithActivePulse;
        private static TeamResponse _teamwithPendingPulse;
        private static TeamResponse _teamWithClosedPulse;

        private static int _teamIdWithDraftPulse;
        private static int _teamIdWithActivePulse;
        private static int _teamIdWithPendingPulse;
        private static int _teamIdWithClosedPulse;

        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam1;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam2;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam3;
        private static RadarQuestionDetailsV2Response _filteredQuestionsTeam4;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static IList<TeamV2Response> _team1WithMembersOfDraftPulse;
        private static IList<TeamV2Response> _teamWithMembersOfActivePulse;
        private static IList<TeamV2Response> _teamWithMembersOfPendingPulse;
        private static IList<TeamV2Response> _teamWithMembersOfClosedPulse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                //Create 4 teams
                var teamDraftPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamWithDraftPulse = _setupApi.CreateTeam(teamDraftPulse).GetAwaiter().GetResult();
                _teamIdWithDraftPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithDraftPulse.Name).TeamId;
                _team1WithMembersOfDraftPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithDraftPulse);

                var teamActivePulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamWithActivePulse = _setupApi.CreateTeam(teamActivePulse).GetAwaiter().GetResult();
                _teamIdWithActivePulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithActivePulse.Name).TeamId;
                _teamWithMembersOfActivePulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithActivePulse);

                var teamPendingPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamwithPendingPulse = _setupApi.CreateTeam(teamPendingPulse).GetAwaiter().GetResult();
                _teamIdWithPendingPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamwithPendingPulse.Name).TeamId;
                _teamWithMembersOfPendingPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithPendingPulse);

                var teamClosedPulse = TeamFactory.GetNormalTeam("Team", 2);
                _teamWithClosedPulse = _setupApi.CreateTeam(teamClosedPulse).GetAwaiter().GetResult();
                _teamIdWithClosedPulse = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamWithClosedPulse.Name).TeamId;
                _teamWithMembersOfClosedPulse = _setupApi.GetTeamWithTeamMemberResponse(_teamIdWithClosedPulse);

                #endregion

                #region Getting Team Response

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

                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam1, _team1WithMembersOfDraftPulse, _teamIdWithDraftPulse, false, period: AssessmentPeriod.TwentyFourHours);

                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam2, _teamWithMembersOfActivePulse, _teamIdWithActivePulse, period: AssessmentPeriod.OneWeek);

                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam3, _teamWithMembersOfPendingPulse, _teamIdWithPendingPulse, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

                _closedPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestionsTeam4, _teamWithMembersOfClosedPulse, _teamIdWithClosedPulse, period: AssessmentPeriod.SeventyTwoHours);

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
        [Description("Verifies adding a team member in Draft pulse scenarios.")]
        public void PulseV2_AddMember_DraftPulse_TeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_draftPulseRequest);
            VerifyPulseScenario(_draftPulseRequest, _teamWithDraftPulse, _teamIdWithDraftPulse, _team1WithMembersOfDraftPulse);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies adding a team member in Active pulse scenarios.")]
        public void PulseV2_AddMember_ActivePulse_TeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_activePulseRequest);
            VerifyPulseScenario(_activePulseRequest, _teamWithActivePulse, _teamIdWithActivePulse, _teamWithMembersOfActivePulse);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies adding a team member in Pending pulse scenarios.")]
        public void PulseV2_AddMember_PendingPulse_TeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_pendingPulseRequest);
            VerifyPulseScenario(_pendingPulseRequest, _teamwithPendingPulse, _teamIdWithPendingPulse, _teamWithMembersOfPendingPulse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53394
        [TestCategory("CompanyAdmin")]
        [Description("Verifies adding a team member in Closed pulse scenarios.")]
        public void PulseV2_AddMember_ClosedPulse_TeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_closedPulseRequest);
            VerifyPulseScenario(_closedPulseRequest, _teamWithClosedPulse, _teamIdWithClosedPulse, _teamWithMembersOfClosedPulse);
        }

        private void VerifyPulseScenario(SavePulseAssessmentV2Request pulseRequest, TeamResponse team, int teamId, IList<TeamV2Response> teamWithMembers)
        {

            #region Fill survey of bothe members to close the pulse 

            if (pulseRequest == _closedPulseRequest)
            {
                //Fill survey of both members to close the pulse created within team
                foreach (var participant in _closedPulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamWithClosedPulse.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamWithClosedPulse.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestionsTeam4, _teamWithClosedPulse.Name, participant.Email);
                }
            }

            #endregion

            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            #region Verification before adding team member

            Log.Info("Navigate to team dashboard and click on pulse edit link");
            teamAssessmentDashboardPage.NavigateToPage(teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Verify team and team member details");
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is not displayed");
            
            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(team.Name);
            Assert.That.ListsAreEqual(teamWithMembers.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers, "List of Members does not match");
           
            editRecipientsPage.Header.ClickOnCloseIcon();
            reviewAndPublishPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();

            Log.Info("Verify team members count on Pulse thumbnail");
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 2 out of 2 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Member count doesn't match");
            }
            else
            {
                Assert.AreEqual("Completed by 0 out of 2 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Member count doesn't match");
            }

            #endregion

            #region Add team member

            Log.Info("Add new Team member and verify");
            editTeamMemberPage.NavigateToPage(teamId);
            addTeamMemberPage.ClickAddNewTeamMemberButton();

            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = RandomDataUtil.GetLastName(),
                Email = RandomDataUtil.GetEmail()
            };

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();
            var actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(3);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");

            #endregion

            #region Verification after adding team member

            Log.Info("Verify team and team member details on 'Edit Recipients' page");
            teamAssessmentDashboardPage.NavigateToPage(teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();

            if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 2 out of 2 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Member count doesn't match");
            }
            else
            {
                Assert.AreEqual("Completed by 0 out of 2 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Member count doesn't match");
            }

            Log.Info("Go to edit of pulse and verify count and added member ");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is not displayed");
            
            listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(team.Name);
            var teamMemberEmailList = new List<string> { teamMemberInfo.Email };
            var existingTeamMembersEmailList = teamWithMembers.First().SelectedParticipants.Select(a => a.Email).ToList();
            var combinedList = teamMemberEmailList.Concat(existingTeamMembersEmailList).ToList();

            Assert.That.ListsAreEqual(combinedList, listOfTeamMembers, "List of Members does not match");

            editRecipientsPage.Header.ClickOnCloseIcon();
            reviewAndPublishPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();
            Driver.RefreshPage();

            Log.Info("Go to edit of pulse and verify count of added member on pulse logo");

            if (pulseRequest == _closedPulseRequest)
            {
                Assert.AreEqual("Completed by 2 out of 3 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Member count doesn't match");
            }
            else
            {
                Assert.AreEqual("Completed by 0 out of 3 Team Members",
                    teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseRequest.Name), "Member count doesn't match");
            }

            Log.Info("Go to pulse growth journey page and verify details of team members");
            if (pulseRequest == _closedPulseRequest)
            {
                Log.Info("Go to pulse growth journey page and verify details of team members");
                teamAssessmentDashboardPage.ClickOnPulseRadar(_closedPulseRequest.Name);
                Assert.AreEqual(SurveyedMemberCounts, pulseGrowthJourneyPage.GetTheCountOfSurveyedParticipants(), "Completed participants does not match");
                Assert.AreEqual("2/3",
                    pulseGrowthJourneyPage.GetTheCountOfSurveyedVsTotalMembers("Summary Row"), "Total and Surveyed members does not match");
            }
            #endregion

        }
    }
}
