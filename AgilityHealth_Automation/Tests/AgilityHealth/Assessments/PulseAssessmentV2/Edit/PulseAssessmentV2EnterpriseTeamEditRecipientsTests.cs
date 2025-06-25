using System;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Edit
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2EnterpriseTeamEditRecipientsTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;
        private static SetupTeardownApi _setupApi;
        private static TeamHierarchyResponse _enterpriseHierarchyResponses;
        private static IList<TeamV2Response> _enterpriseTeamWithMemberResponses;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static SavePulseAssessmentV2Request _pulseRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setupApi = new SetupTeardownApi(TestEnvironment);

                var team = TeamFactory.GetNormalTeam("Team", 2);
                _team1 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                team = TeamFactory.GetNormalTeam("Team", 2);
                _team2 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                team = TeamFactory.GetNormalTeam("Team", 2);
                _team3 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                team = TeamFactory.GetNormalTeam("Team", 2);
                _team4 = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                var multiTeam1 = TeamFactory.GetMultiTeam("MultiTeam1");
                var multiTeamResponse1 = _setupApi.CreateTeam(multiTeam1).GetAwaiter().GetResult();
                _setupApi.AddSubteams(multiTeamResponse1.Uid, new List<Guid> { _team1.Uid, _team2.Uid }).GetAwaiter()
                    .GetResult();

                var multiTeam2 = TeamFactory.GetMultiTeam("MultiTeam2");
                var multiTeamResponse2 = _setupApi.CreateTeam(multiTeam2).GetAwaiter().GetResult();
                _setupApi.AddSubteams(multiTeamResponse2.Uid, new List<Guid> { _team3.Uid, _team4.Uid }).GetAwaiter()
                    .GetResult();

                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("Enterprise");
                var enterpriseTeamResponse = _setupApi.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                _setupApi.AddSubteams(enterpriseTeamResponse.Uid, new List<Guid> { multiTeamResponse1.Uid, multiTeamResponse2.Uid }).GetAwaiter()
                    .GetResult();
                _enterpriseHierarchyResponses = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name);

                _enterpriseTeamWithMemberResponses = _setupApi.GetTeamWithTeamMemberResponse(_enterpriseHierarchyResponses.TeamId);

                var questionDetailsResponse = GetQuestions(_enterpriseHierarchyResponses.TeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _enterpriseTeamWithMemberResponses, _enterpriseHierarchyResponses.TeamId, false);
                AddPulseAssessment(_pulseRequest);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_EnterpriseTeam_Edit_SelectDeselectTeams_Publish()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            mtEtDashboardPage.NavigateToPage(_enterpriseHierarchyResponses.TeamId, true);

            Log.Info("Go to edit of pulse and verify selected teams and teamMembers are displayed");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            var pulseData = PulseV2Factory.GetPulseAddData();
            editPulseCheckPage.FillSchedulerInfo(pulseData);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var allMembers = _enterpriseTeamWithMemberResponses.SelectMany(a => a.SelectedParticipants.Select(s => s.Email))
                .ToList();

            // verify that all teams are showing
            foreach (var team in _enterpriseTeamWithMemberResponses)
            {
                Assert.IsTrue(selectRecipientsPage.IsTeamDisplayed(team.Name), "Team is not displayed");
                Assert.AreEqual("Incomplete", editRecipientsPage.GetAssessmentStatusOfTeam(team.Name), "Assessment status for team does not show");
                Assert.IsTrue(selectRecipientsPage.IsTeamSelected(team.Name), "Teams are not selected");

                var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(team.Name);
                foreach (var member in listOfTeamMembers)
                {
                    Assert.That.ListContains(allMembers, member, "member does not match");
                }
            }

            Log.Info("Publish the pulse assessment from the 'Review and Publish' page");
            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Verify Emails for selected and deselected teams");

            foreach (var team in _enterpriseTeamWithMemberResponses)
            {
                foreach (var member in team.SelectedParticipants)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(team.Name), member.Email, "Inbox", "", 360),
                        $"Could not find email with subject <{SharedConstants.PulseEmailSubject(team.Name)}> sent to {member.Email}");
                }

            }

            Log.Info("Fill pulse survey for multi team");
            var multiTeam1 = _enterpriseHierarchyResponses.Children.First().Children;
            foreach (var teams in multiTeam1)
            {
                Log.Info("Fill the Pulse survey for members");
                var listOfTeamMembers = _setupApi.GetTeamWithTeamMemberResponse(teams.TeamId);
                foreach (var members in listOfTeamMembers.First().SelectedParticipants)
                {
                    CompletePulseSurvey(TestContext, _filteredQuestions, teams.Name, members.Email);
                }

            }

            Log.Info("Go to edit of pulse and verify Complete and Incomplete teams and teamMembers of assessment");
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editRecipientsPage.Header.ClickOnEditRecipientsTab();

            foreach (var team in multiTeam1)
            {
                Assert.AreEqual("Completed", editRecipientsPage.GetAssessmentStatusOfTeam(team.Name), "Assessment status for team does not show");

                var listOfTeamMembers = _setupApi.GetTeamWithTeamMemberResponse(team.TeamId);
                foreach (var member in listOfTeamMembers.First().SelectedParticipants)
                {
                    editRecipientsPage.ClickOnTeamExpandIcon(team.Name);
                    Assert.IsFalse(editRecipientsPage.IsIndividualEnvelopePresent(team.Name, member.Email), "Envelope icon is showing");
                    Assert.AreEqual("Completed", editRecipientsPage.GetSurveyCompletedTag(team.Name, member.FirstName + " " + member.LastName), "Assessment is not completed");
                }
            }

            var multiTeam2 = _enterpriseHierarchyResponses.Children.Last().Children;

            foreach (var team in multiTeam2)
            {
                Assert.AreEqual("Incomplete", editRecipientsPage.GetAssessmentStatusOfTeam(team.Name), "Assessment status for team does not show");
            }
        }
    }
}
