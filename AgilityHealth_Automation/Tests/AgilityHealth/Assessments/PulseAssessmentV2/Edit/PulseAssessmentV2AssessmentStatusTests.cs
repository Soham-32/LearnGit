using System;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Dtos.Assessments.PulseV2;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Edit
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2AssessmentStatusTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team1;
        private static TeamResponse _team2;
        private static int _multiTeamId;
        private static RadarQuestionDetailsV2Response _filteredQuestions;
        private static SavePulseAssessmentV2Request _pulseRequest;

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

                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setupApi.CreateTeam(multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _team1.Uid, _team2.Uid }).GetAwaiter()
                    .GetResult();
                _multiTeamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name).TeamId;
                var multiTeamWithMember = setupApi.GetTeamWithTeamMemberResponse(_multiTeamId);

                var questionDetailsResponse = GetQuestions(_multiTeamId);
                _filteredQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_filteredQuestions, multiTeamWithMember, _multiTeamId, false);
                AddPulseAssessment(_pulseRequest);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_MultiTeam_Edit_AssessmentStatusCheck_Publish()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard");
            mtEtDashboardPage.NavigateToPage(_multiTeamId);

            Log.Info("Go to edit of pulse and select option from the status dropdown to verify teams accordingly");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);
            var pulseData = PulseV2Factory.GetPulseAddData();
            editPulseCheckPage.FillSchedulerInfo(pulseData);

            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var allTeams = editRecipientsPage.GetListOfTeams();
            editRecipientsPage.SelectStatusOptionFromStatusDropdown("All");
            foreach (var teams in allTeams)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(teams), "Teams are not displayed");
            }

            editRecipientsPage.SelectStatusOptionFromStatusDropdown("Incomplete");

            Log.Info("Hover on the (I) icon and bulk envelope icon then verify teams");
            foreach (var teams in allTeams)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(teams), "Teams are not displayed");
                Assert.AreEqual("Incomplete", editRecipientsPage.GetAssessmentStatusOfTeam(teams), "Completed is showing");
                Assert.AreEqual("Team Member Completion: 0/2 - (May include response(s) from deleted team members)", editRecipientsPage.GetTeamMemberCompletionText(_team1.Name), "Tooltip text does not match");
                Assert.AreEqual("Send reminder to incomplete team members", editRecipientsPage.GetTooltipMessageOfBulkEnvelope(_team1.Name), "Tooltip message is not displayed");
            }

            editRecipientsPage.SelectStatusOptionFromStatusDropdown("Complete");
            foreach (var teams in allTeams)
            {
                Assert.IsFalse(editRecipientsPage.IsTeamDisplayed(teams), "Teams are not displayed");
            }

            editRecipientsPage.SelectStatusOptionFromStatusDropdown("All");
            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Fill the Pulse survey for one team's members and edit pulse assessment");
            foreach (var team in _team1.Members)
            {
                CompletePulseSurvey(TestContext, _filteredQuestions, _team1.Name, team.Email);
            }

            teamAssessmentDashboardPage.ClickOnMtEtToggleButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);

            editRecipientsPage.Header.ClickOnEditQuestionsTab();
            editRecipientsPage.Header.ClickOnEditRecipientsTab();

            Log.Info("Verify status of teams from dropdown");
            editRecipientsPage.SelectStatusOptionFromStatusDropdown("Incomplete");

            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(_team2.Name), "Team is not displayed");
            Assert.IsFalse(editRecipientsPage.IsTeamDisplayed(_team1.Name), "Team is displayed");
            Assert.AreEqual("Incomplete", editRecipientsPage.GetAssessmentStatusOfTeam(_team2.Name), "Completed text is displayed");
            Assert.IsTrue(editRecipientsPage.IsBulKEnvelopeButtonDisplayed(_team2.Name), "Envelope Icon is not displayed");
            Assert.AreEqual("Send reminder to incomplete team members", editRecipientsPage.GetTooltipMessageOfBulkEnvelope(_team2.Name), "Tooltip message is not displayed");

            Log.Info("Click on 'Bulk Envelope' icon and verify toaster message and email");
            editRecipientsPage.ClickOnBulkEnvelopeIcon(_team2.Name);

            foreach (var member in _team2.Members)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team2.Name),
                    member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team2.Name)}> sent to {member.Email}");
            }

            editRecipientsPage.SelectStatusOptionFromStatusDropdown("Complete");
            Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(_team1.Name), "Team is not displayed");
            Assert.IsFalse(editRecipientsPage.IsTeamDisplayed(_team2.Name), "Team is displayed");
            Assert.AreEqual("Completed", editRecipientsPage.GetAssessmentStatusOfTeam(_team1.Name), "Incomplete text is displayed");

            editRecipientsPage.SelectStatusOptionFromStatusDropdown("All");
            foreach (var teams in allTeams)
            {
                Assert.IsTrue(editRecipientsPage.IsTeamDisplayed(teams), "Teams are not displayed");
            }

            Assert.AreEqual("Completed", editRecipientsPage.GetAssessmentStatusOfTeam(_team1.Name), "Incomplete text is displayed");
            Assert.AreEqual("Incomplete", editRecipientsPage.GetAssessmentStatusOfTeam(_team2.Name), "Completed text is displayed");
        }
    }
}
