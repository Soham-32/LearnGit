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
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AtCommon.Api.Enums;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.SetUpTearDown;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.NTierLevel
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("NTierTeamPulse")]
    public class PulseAssessmentV2AddMemberVerificationNTierLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;

        private static int _team1Id;
        private static int _team2Id;
        private static int _team3Id;
        private static int _team4Id;

        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;

        private static SavePulseAssessmentV2Request _draftPulseRequest;
        private static SavePulseAssessmentV2Request _activePulseRequest;
        private static SavePulseAssessmentV2Request _pendingPulseRequest;
        private static SavePulseAssessmentV2Request _closedPulseRequest;

        private static IList<TeamV2Response> _team1WithMembers;
        private static IList<TeamV2Response> _team2WithMembers;
        private static IList<TeamV2Response> _team3WithMembers;
        private static IList<TeamV2Response> _team4WithMembers;

        private const int SurveyedMemberCounts = 2;
        private static TeamResponse _enterpriseTeamResponse;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static TeamHierarchyResponse _nTierHierarchyResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                _setupApi = new SetupTeardownApi(TestEnvironment);

                #region Team Creation

                //Create 4 teams
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
                _team3Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team3.Name).TeamId;
                _team3WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team3Id);

                var team4 = TeamFactory.GetNormalTeam("Team", 2);
                _team4 = _setupApi.CreateTeam(team4).GetAwaiter().GetResult();
                _team4Id = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team4.Name).TeamId;
                _team4WithMembers = _setupApi.GetTeamWithTeamMemberResponse(_team4Id);

                //Create Multi team and add 4 teams as a subteams
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

                #region Getting Team Response

                _nTierHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(nTierTeamName);

                _setupApi.GetTeamWithTeamMemberResponse(_nTierHierarchyResponses.TeamId);
                
                var questionDetailsResponse = GetQuestions(_nTierHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);

                #endregion

                #region Pulse Requests

                _draftPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team1WithMembers, _nTierHierarchyResponses.TeamId, false, period: AssessmentPeriod.TwentyFourHours);

                _activePulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team2WithMembers, _nTierHierarchyResponses.TeamId, period: AssessmentPeriod.OneWeek);

                var futureStartDate = DateTime.UtcNow.AddDays(1);
                _pendingPulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _team3WithMembers, _nTierHierarchyResponses.TeamId, true, null, AssessmentPeriod.TwentyFourHours, RepeatIntervalId.Never, futureStartDate);

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
        [Description("Verifies adding a team member in Draft pulse scenarios.")]
        public void PulseV2_AddMember_DraftPulse_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_draftPulseRequest);
            VerifyPulseScenario(_draftPulseRequest, _team1, _team1Id, _team1WithMembers);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies adding a team member in Active pulse scenarios.")]
        public void PulseV2_AddMember_ActivePulse_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_activePulseRequest);
            VerifyPulseScenario(_activePulseRequest, _team2, _team2Id, _team2WithMembers);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [Description("Verifies adding a team member in Pending pulse scenarios.")]
        public void PulseV2_AddMember_PendingPulse_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_pendingPulseRequest);
            VerifyPulseScenario(_pendingPulseRequest, _team3, _team3Id, _team3WithMembers);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53394
        [TestCategory("CompanyAdmin")]
        [Description("Verifies adding a team member in Closed pulse scenarios.")]
        public void PulseV2_AddMember_ClosedPulse_NTierTeamLevel()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            AddPulseAssessment(_closedPulseRequest);
            VerifyPulseScenario(_closedPulseRequest, _team4, _team4Id, _team4WithMembers);
        }


        private void VerifyPulseScenario(SavePulseAssessmentV2Request pulseRequest, TeamResponse team, int teamId, IList<TeamV2Response> teamWithMembers)
        {

            #region Fill survey of bothe members to close the pulse 

            if (pulseRequest == _closedPulseRequest)
            {
                //Fill survey of both members to close the pulse created within team
                foreach (var participant in _closedPulseRequest.SelectedTeams.First().SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team4.Name), participant.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4.Name)}> sent to {participant.Email}");
                    CompletePulseSurvey(TestContext, _filteredQuestions, _team4.Name, participant.Email);
                }
            }

            #endregion

            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editTeamMemberPage = new EditTeamTeamMemberPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            #region Verification before adding team member
           
            Log.Info("Navigate to enterprise team dashboard");
            mtEtDashboardPage.NavigateToPage(_nTierHierarchyResponses.TeamId, false, true);
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Verify team and team member details");
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(team.Name), "Team is not displayed");
            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(team.Name);
            Assert.That.ListsAreEqual(teamWithMembers.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers, "List of Members does not match");
            editRecipientsPage.Header.ClickOnCloseIcon();
            reviewAndPublishPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();

            Log.Info("Verify team members count on Pulse thumbnail");
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
                Email = "memberemail" + CSharpHelpers.RandomNumber() + "@sharklasers.com"
            };

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();
            var actualTeamMember = addTeamMemberPage.GetTeamMemberInfoFromGrid(3);
            Assert.AreEqual(teamMemberInfo.FirstName, actualTeamMember.FirstName, "Firstname doesn't match");

            #endregion

            #region Verification after adding team member

            Log.Info("Verify team and team member details on 'Edit Recipients' page");
            mtEtDashboardPage.NavigateToPage(_nTierHierarchyResponses.TeamId, false, true);
            
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
            var aa = teamWithMembers.First().SelectedParticipants.Select(a => a.Email).ToList();
            var combinedList = teamMemberEmailList.Concat(aa).ToList();

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
