using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2EmailCheckForCommonMembersTests : PulseV2BaseTest
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
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Pulse", 2);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(team.Name).TeamId;

                team.Name = "Pulse" + Guid.NewGuid();
                team.Members.Add(MemberFactory.GetTeamMember());
                _teamResponse1 = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _teamResponse.Uid, _teamResponse1.Uid }).GetAwaiter().GetResult();

                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeam.Name).TeamId;
                _multiTeam = setupApi.GetTeamWithTeamMemberResponse(_multiTeamId);

                _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId).ToList();

                var questionDetailsResponse = GetQuestions(_multiTeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, _teamWithTeamMemberResponses, _multiTeamId);
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        [Description("Members belonging to two different teams receive emails corresponding to each respective team." +
                     "If a member has completed the survey for one team, clicking the Cluster envelope should only send an email for the other team, and not for the team for which the member has already completed the survey.")]
        public void PulseV2_MultiTeam_EmailCheckForCommonMembers()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var firstTeamMemberName = _teamResponse.Members.First().FirstName + " " + _teamResponse.Members.First().LastName;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();

            Log.Info("Verify that email for one team1 is present and for team2 does not");
            _multiTeam.First().SelectedParticipants.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                   member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {member.Email}"));

            _multiTeam.Last().SelectedParticipants.ForEach(member => Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse1.Name),
                member.Email, "Inbox", "", 5), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse1.Name)}> sent to {member.Email}"));

            Log.Info("Edit pulse assessment, select team2 and save the assessment");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnNextButton();
            editRecipientsPage.Header.ClickOnEditRecipientsTab();
            editRecipientsPage.ClickOnSelectTeamCheckbox(_multiTeam.Last().Name);
            Assert.IsTrue(editRecipientsPage.IsTeamSelected(_multiTeam.Last().Name));
            editRecipientsPage.Header.ClickSaveAsDraftButton();

            Log.Info("Verify that email for team2 is present and for team2 is not present");
            _multiTeam.Last().SelectedParticipants.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse1.Name),
                member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse1.Name)}> sent to {member.Email}"));

            _multiTeam.First().SelectedParticipants.ForEach(member => Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                member.Email, "Inbox", "UNREAD", 5), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {member.Email}"));

            Log.Info("Fill the survey for first member");
            surveyPage.NavigateToUrl(GmailUtil.GetPulseSurveyLink(SharedConstants.PulseEmailSubject(_teamResponse.Name), _multiTeam.First().SelectedParticipants.First().Email, "inbox"));
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(5);
            surveyPage.ClickFinishButton();

            Log.Info("Edit the pulse and go to edit recipients page to verify the 'Completed' check mark");
            teamAssessmentDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnNextButton();
            editRecipientsPage.Header.ClickOnEditRecipientsTab();

            editRecipientsPage.ClickOnTeamExpandIcon(_teamResponse.Name);
            Assert.IsFalse(editRecipientsPage.IsIndividualEnvelopePresent(_teamResponse.Name, _multiTeam.First().SelectedParticipants.First().Email), "Envelope icon is showing");
            Assert.AreEqual("Completed", editRecipientsPage.GetSurveyCompletedTag(_teamResponse.Name, firstTeamMemberName), "Assessment is not completed");

            Log.Info("Click on 'Email to All incomplete teams' envelope icon to send reminder to all members who have not completed the pulse. Verify the same.");
            editRecipientsPage.ClickOnEmailAllInCompleteTeamsEnvelopeIcon();
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                    _multiTeam.First().SelectedParticipants.First().Email, "Inbox", "UNREAD", 5),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {_multiTeam.First().SelectedParticipants.First().Email}");

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                    _multiTeam.First().SelectedParticipants.Last().Email, "Inbox", "", 360),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {_multiTeam.First().SelectedParticipants.Last().Email}");

            _multiTeam.Last().SelectedParticipants.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse1.Name),
                member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse1.Name)}> sent to {member.Email}"));
        }
    }
}
