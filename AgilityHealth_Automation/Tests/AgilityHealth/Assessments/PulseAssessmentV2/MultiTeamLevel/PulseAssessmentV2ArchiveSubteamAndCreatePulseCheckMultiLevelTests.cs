using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.MultiTeamLevel
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("MultiTeamPulse")]
    public class PulseAssessmentV2ArchiveSubteamAndCreatePulseCheckMultiLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static int _multiTeamId;
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static TeamResponse _teamResponse1;
        private static IList<TeamV2Response> _multiTeam;
        private static SavePulseAssessmentV2Request _pulseRequest;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static List<TeamV2Response> _teamWithTeamMemberResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                #region Team Creation

                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Pulse", 2);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(team.Name).TeamId;

                team.Name = "Pulse" + Guid.NewGuid();
                team.Members.Add(MemberFactory.GetTeamMember());
                _teamResponse1 = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                var multiTeam = TeamFactory.GetMultiTeam("MultiTeamPulse_");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _teamResponse.Uid, _teamResponse1.Uid }).GetAwaiter().GetResult();
                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name).TeamId;

                #endregion

                #region Getting Team response

                _multiTeam = setupApi.GetTeamWithTeamMemberResponse(_multiTeamId);
                _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId).ToList();
                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeam.Name).TeamId;

                #endregion

                #region Pulse Requests

                var questionDetailsResponse = GetQuestions(_multiTeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithTeamMemberResponses, _multiTeamId);
                AddPulseAssessment(_pulseRequest);

                #endregion

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        [Description("Members belonging to two different teams receive emails corresponding to each respective team.")]
        public void PulseV2_ArchiveTeamAndCheckEmailSentAtMultiTeamLevel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamDashBoardPage = new TeamDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            teamDashBoardPage.GridTeamView();

            Log.Info("Archive one of the created teams");
            teamDashBoardPage.SearchTeam(_teamResponse1.Name);
            teamDashBoardPage.DeleteTeam(_teamResponse1.Name, RemoveTeamReason.ArchiveProjectCompleted);

            Log.Info("Navigate to Multi team, pulse assessments dashboard");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();

            Log.Info("Click on 'Email to All Incomplete Teams' icon. Verify email sent for Team1 and not for archived Team2.");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnNextButton();
            editQuestionsPage.Header.ClickOnNextButton();
            editRecipientsPage.ClickOnEmailAllInCompleteTeamsEnvelopeIcon();
            _multiTeam.First().SelectedParticipants.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                   member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {member.Email}"));

            _multiTeam.Last().SelectedParticipants.ForEach(member => Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse1.Name),
                member.Email, "Inbox", "", 5), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse1.Name)}> sent to {member.Email}"));

        }
    }
}
