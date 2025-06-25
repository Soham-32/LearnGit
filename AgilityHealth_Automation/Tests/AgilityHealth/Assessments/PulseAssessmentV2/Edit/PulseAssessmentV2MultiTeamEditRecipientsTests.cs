using System;
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
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Edit
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2MultiTeamEditRecipientsTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static int _multiTeamId;
        private static int _multiTeamId1;
        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static TeamResponse _team3;
        private static TeamResponse _team4;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _multiTeam1;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static SavePulseAssessmentV2Request _pulseRequest;
        private static SavePulseAssessmentV2Request _pulseRequest1;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);

                var team = TeamFactory.GetNormalTeam("Team", 2);
                _team1 = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                team = TeamFactory.GetNormalTeam("Team", 2);
                _team2 = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                var team1 = TeamFactory.GetNormalTeam("Team", 2);
                _team3 = setupApi.CreateTeam(team1).GetAwaiter().GetResult();

                team1 = TeamFactory.GetNormalTeam("Team", 2);
                _team4 = setupApi.CreateTeam(team1).GetAwaiter().GetResult();

                _multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _team1.Uid, _team2.Uid }).GetAwaiter()
                    .GetResult();

                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeam.Name).TeamId;
                var multiTeamWithMember = setupApi.GetTeamWithTeamMemberResponse(_multiTeamId);

                _multiTeam1 = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse1 = setupApi.CreateTeam(_multiTeam1).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse1.Uid, new List<Guid> { _team3.Uid, _team4.Uid }).GetAwaiter()
                    .GetResult();

                _multiTeamId1 = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_multiTeam1.Name).TeamId;

                var multiTeam1WithMember = setupApi.GetTeamWithTeamMemberResponse(_multiTeamId1);
                var questionDetailsResponse = GetQuestions(_multiTeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, multiTeam1WithMember, _multiTeamId1, false);
                AddPulseAssessment(_pulseRequest);

                _pulseRequest1 = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, multiTeamWithMember, _multiTeamId);
                AddPulseAssessment(_pulseRequest1);
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
        public void PulseV2_MultiTeam_Edit_SelectDeselectTeams_Publish()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard and create 'Pulse Assessment'");
            mtEtDashboardPage.NavigateToPage(_multiTeamId1);

            Log.Info("Go to edit of pulse and verify selected teams are displayed");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            var pulseData = PulseV2Factory.GetPulseAddData();
            editPulseCheckPage.FillSchedulerInfo(pulseData);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var allTeams = selectRecipientsPage.GetListOfTeams();
            foreach (var teams in allTeams)
            {
                Assert.IsTrue(selectRecipientsPage.IsTeamSelected(teams), "Teams are not selected");
            }

            Log.Info("Deselect one team and verify selected and deselected teams");
            editRecipientsPage.ClickOnSelectTeamCheckbox(_team3.Name, false);
            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team3.Name), "Team is selected");
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team4.Name), "Team is not selected");

            Log.Info("Publish the pulse assessment from the 'Review and Publish' page");
            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Verify Emails for selected and deselected teams");
            _team4.Members.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(
                    SharedConstants.PulseEmailSubject(_team4.Name),
                    member.Email, "Inbox","",360),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4.Name)}> sent to {member.Email}"));

            _team3.Members.ForEach(member => Assert.IsFalse(
                GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team3.Name), member.Email, "Inbox", "UNREAD", 5),
                $"Found email with subject <{SharedConstants.PulseEmailSubject(_team3.Name)}> sent to {member.Email}"));

            Log.Info("Go to edit of pulse and verify selected and deselected team");
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);

            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editRecipientsPage.Header.ClickOnEditRecipientsTab();

            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team3.Name), "Team is selected");
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team4.Name), "Team is not selected");

            editRecipientsPage.ClickOnTeamExpandIcon(_team4.Name);
            editRecipientsPage.ClickOnClusterEnvelopeIcon();

            Log.Info("Verify Emails");
            _team4.Members.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(
                    SharedConstants.PulseEmailSubject(_team4.Name),
                    member.Email, "Inbox"),
                $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4.Name)}> sent to {member.Email}"));

            _team3.Members.ForEach(member => Assert.IsFalse(GmailUtil.DoesMemberEmailExist(
                SharedConstants.PulseEmailSubject(_team3.Name),
                member.Email, "Inbox", "UNREAD", 10),
                $"Found email with subject <{SharedConstants.PulseEmailSubject(_team3.Name)}> sent to {member.Email}"));
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_MultiTeam_Edit_PulseCompletionTextVerification()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard and create 'Pulse Assessment'");
            mtEtDashboardPage.NavigateToPage(_multiTeamId);
            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();

            Assert.AreEqual("Completed by 0 out of 4 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(_pulseRequest1.Name), "Participant info doesn't match");
            Assert.AreEqual("Completed by 0 out of 2 Teams", teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(_pulseRequest1.Name), "Participant info doesn't match");

            foreach (var team in _team2.Members)
            {
                CompletePulseSurvey(TestContext, _filteredQuestions, _team2.Name, team.Email);
            }
            
            Driver.RefreshPage();
            Assert.AreEqual("Completed by 2 out of 4 Team Members", teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(_pulseRequest1.Name), "Participant info doesn't match");
            Assert.AreEqual("Completed by 1 out of 2 Teams", teamAssessmentDashboardPage.GetPulseAssessmentTeamCompletedInfo(_pulseRequest1.Name), "Participant info doesn't match");

        }
    }
}
