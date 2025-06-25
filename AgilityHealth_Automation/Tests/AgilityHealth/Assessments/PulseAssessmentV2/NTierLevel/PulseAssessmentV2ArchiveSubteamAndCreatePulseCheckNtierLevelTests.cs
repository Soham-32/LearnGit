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
using AgilityHealth_Automation.SetUpTearDown;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.NTierLevel
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments"), TestCategory("NTierTeamPulse")]
    public class PulseAssessmentV2ArchiveSubteamAndCreatePulseCheckNtierLevelTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static int _multiTeamId;
        private static int _enterpriseTeamId;
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static TeamResponse _teamResponse1;
        private static IList<TeamV2Response> _multiTeam;
        private static AddTeamWithMemberRequest _enterpriseTeam;
        private static int _nTierTeamId;
        private static SavePulseAssessmentV2Request _pulseRequest;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static List<TeamV2Response> _teamWithTeamMemberResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Pulse", 2);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(team.Name).TeamId;

                team.Name = RandomDataUtil.GetTeamName() + Guid.NewGuid();
                team.Members.Add(MemberFactory.GetTeamMember());
                _teamResponse1 = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _teamResponse.Uid, _teamResponse1.Uid }).GetAwaiter().GetResult();

                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeam.Name).TeamId;
                _multiTeam = setupApi.GetTeamWithTeamMemberResponse(_multiTeamId);

                _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId).ToList();

                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeam");
                var enterpriseTeamResponse = setupApi.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(enterpriseTeamResponse.Uid, new List<Guid> { multiTeamResponse.Uid }).GetAwaiter()
                    .GetResult();

                _enterpriseTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_enterpriseTeam.Name).TeamId;

                // Create n-tier team
                var nTierTeamName = "N-Tier" + RandomDataUtil.GetTeamName();
                setupUi.NTier_CreateNTierTeam(nTierTeamName, new List<string> { _enterpriseTeam.Name });
                _nTierTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(nTierTeamName).TeamId;

                var questionDetailsResponse = GetQuestions(_enterpriseTeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithTeamMemberResponses, _nTierTeamId);
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
        [TestCategory("CompanyAdmin")]
        [Description("Members belonging to two different teams receive emails corresponding to each respective team.")]
        public void PulseV2_ArchiveTeamAndCheckEmailSent_NTierLevel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.GridTeamView();

            Log.Info("Archive one of the created teams");
            dashBoardPage.SearchTeam(_teamResponse1.Name);
            dashBoardPage.DeleteTeam(_teamResponse1.Name, RemoveTeamReason.ArchiveProjectCompleted);

            Log.Info("Navigate to N-tier team, pulse assessment dashboard");
            teamAssessmentDashboardPage.NavigateToPage(_nTierTeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();

            Log.Info("Click on 'Email to All incomplete teams' envelope icon to send reminder to all members who have not completed the pulse.Verify that email for one team1 is present and for team2(Archived Team) is not sent.");
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