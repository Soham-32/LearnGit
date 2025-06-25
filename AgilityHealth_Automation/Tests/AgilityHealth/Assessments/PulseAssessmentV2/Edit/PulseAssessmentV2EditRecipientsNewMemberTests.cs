using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Edit
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2EditRecipientsNewMemberTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setupApi;
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static SavePulseAssessmentV2Request _pulseRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("PulseV2Team", 2);
                _teamResponse = _setupApi.CreateTeam(team).GetAwaiter().GetResult();

                //Get team profile 
                _teamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                var teamWithTeamMemberResponses = _setupApi.GetTeamWithTeamMemberResponse(_teamId).ToList();
                var questionDetailsResponse = GetQuestions(_teamId);

                var filterQuestions = questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
                _pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, teamWithTeamMemberResponses, _teamId);
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
        public void PulseV2_Edit_Recipients_Add_NewMember_Publish()
        {
            VerifySetup(_classInitFailed);

            foreach (var participant in _pulseRequest.SelectedTeams.First().SelectedParticipants)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                        participant.Email, "Inbox", "", 360),
                    $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {participant.Email}");
            }

            var member = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
            _setupApi.AddTeamMembers(_teamResponse.Uid, member);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard, edit Pulse and send reminder email to new member");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(_pulseRequest.Name);

            editQuestionsPage.Header.ClickOnEditRecipientsTab();

            Assert.IsTrue(editRecipientsPage.IsTeamSelected(_teamResponse.Name), "Team isn't selected");

            editRecipientsPage.ClickOnTeamExpandIcon(_teamResponse.Name);

            var listOfMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_teamResponse.Name);
            Assert.That.ListContains(listOfMembers, member.Email, "Member does not match");

            editRecipientsPage.ClickOnIndividualEnvelopeIcon(_teamResponse.Name, member.Email);

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                    member.Email, "Inbox"),
                 $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {member.Email}");

            foreach (var participant in _pulseRequest.SelectedTeams.First().SelectedParticipants)
            {
                Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                        participant.Email, "Inbox", "UNREAD", 10),
                    $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {member.Email}");
            }

        }
    }
}