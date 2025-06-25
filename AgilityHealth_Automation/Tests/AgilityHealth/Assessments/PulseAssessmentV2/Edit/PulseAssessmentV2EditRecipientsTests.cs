using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.Utilities;
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
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2EditRecipientsTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static RadarQuestionDetailsV2Response _questionDetailsResponse;
        private static int _team1Id;
        private static int _team2Id;
        private static int _team3Id;
        private static int _team4Id;

        private static TeamResponse _team1Response;
        private static TeamResponse _team2Response;
        private static TeamResponse _team3Response;
        private static TeamResponse _team4Response;

        private static List<TeamV2Response> _team1WithTeamMemberResponses;
        private static List<TeamV2Response> _team2WithTeamMemberResponses;
        private static List<TeamV2Response> _team3WithTeamMemberResponses;
        private static List<TeamV2Response> _team4WithTeamMemberResponses;


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);

                //team1
                var team1 = TeamFactory.GetNormalTeam("PulseV2Team", 1);
                _team1Response = setupApi.CreateTeam(team1).GetAwaiter().GetResult();
                _team1Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team1Response.Name).TeamId;
                var member = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_team1Response.Uid, member);
                _team1WithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_team1Id).ToList();
                _questionDetailsResponse = GetQuestions(_team1Id);

                //team2
                var team2 = TeamFactory.GetNormalTeam("PulseV2Team", 1);
                _team2Response = setupApi.CreateTeam(team2).GetAwaiter().GetResult();
                _team2Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team2Response.Name).TeamId;
                var member2 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_team2Response.Uid, member2);
                _team2WithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_team2Id).ToList();
                _questionDetailsResponse = GetQuestions(_team2Id);

                //team3
                var team3 = TeamFactory.GetNormalTeam("PulseV2Team", 1);
                _team3Response = setupApi.CreateTeam(team3).GetAwaiter().GetResult();
                _team3Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team3Response.Name).TeamId;
                var member3 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_team3Response.Uid, member3);
                _team3WithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_team3Id).ToList();
                _questionDetailsResponse = GetQuestions(_team3Id);

                //team4
                var team4 = TeamFactory.GetNormalTeam("PulseV2Team", 1);
                _team4Response = setupApi.CreateTeam(team4).GetAwaiter().GetResult();
                _team4Id = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team4Response.Name).TeamId;
                var member4 = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(_team3Response.Uid, member4);
                _team4WithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_team4Id).ToList();
                _questionDetailsResponse = GetQuestions(_team4Id);

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
        public void PulseV2_Edit_Recipients_WithRole_Publish()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _team1WithTeamMemberResponses, _team1Id, false);
            AddPulseAssessment(pulseRequest);

            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            Log.Info("Edit draft Pulse Assessment and save as draft with Role");
            PulseV2_Edit_Recipients_WithRole_SaveAsDraft(pulseRequest, _team1Id, _team1Response, _team1WithTeamMemberResponses);

            Log.Info("'Publish' the assessment and go to edit of the assessment and verify selected teams and team members");
            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team1Response.Name), _team1WithTeamMemberResponses.First().SelectedParticipants.Last().Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team1WithTeamMemberResponses.First().SelectedParticipants.Last().Email)}> sent to ");
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team1Response.Name), _team1WithTeamMemberResponses.First().SelectedParticipants.First().Email, "Inbox", "UNREAD", 5), $"Email found with subject <{SharedConstants.PulseEmailSubject(_team1WithTeamMemberResponses.First().SelectedParticipants.First().Email)}> sent to ");

            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team1Response.Name);
            Assert.That.ListContains(_team1WithTeamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            var listOfTeamMembersWithRole = editRecipientsPage.GetTeamMemberRoleByTeamName(_team1Response.Name);
            Assert.That.ListContains(_team1WithTeamMemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Recipients_WithOutRole_Publish()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _team2WithTeamMemberResponses, _team2Id, false);
            AddPulseAssessment(pulseRequest);

            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            Log.Info("Edit draft Pulse Assessment and save as draft with Role");
            PulseV2_Edit_Recipients_WithRole_SaveAsDraft(pulseRequest, _team2Id, _team2Response, _team2WithTeamMemberResponses);

            Log.Info("Remove Selected role from 'Limit To These Roles' dropdown");
            editRecipientsPage.SelectDeselectRoleFromFilterDropDown(_team2WithTeamMemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name, false);

            var listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team2Response.Name);
            Assert.That.ListsAreEqual(_team2WithTeamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers,
                "List of Members does not match");

            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team2Response.Name);
            Assert.That.ListsAreEqual(_team2WithTeamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers,
                "List of Members does not match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 52098
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Recipients_DisabledMembers_Publish()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _team3WithTeamMemberResponses, _team3Id, false);
            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and edit Pulse");
            teamAssessmentDashboardPage.NavigateToPage(_team3WithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Go to 'Edit Questions' tab and select excluded competency");
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();

            var competencyName = filterQuestions.Dimensions.First().SubDimensions.First().Competencies.First().Name;

            Log.Info("Remove selected competency and select another competency which is excluded");
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.DimensionFoundation);
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.SurveySubDimension);

            editQuestionsPage.CheckUncheckQuestionCheckbox(SharedConstants.DimensionFoundation, false);
            Assert.IsFalse(editQuestionsPage.IsQuestionCheckboxSelected(competencyName),
                $"{competencyName} - Competency is selected");

            editQuestionsPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyCompetency);

            Log.Info("Go to 'Edit Recipients' page and verify the team members");
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team3Response.Name);
            Assert.IsTrue(editRecipientsPage.IsTeamMemberEnabled(_team3Response.Name, listOfTeamMembers.Last()),
                "TeamMember is grayed out");
            Assert.IsFalse(editRecipientsPage.IsTeamMemberEnabled(_team3Response.Name, listOfTeamMembers.First()),
                "TeamMember is not grayed out");

            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Click on Pulse edit, go to 'Edit Recipients' page and verify the team members");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(_team3Response.Name);
            Assert.IsTrue(editRecipientsPage.IsTeamMemberEnabled(_team3Response.Name, listOfTeamMembers.Last()),
                "TeamMember is grayed out");
            Assert.IsFalse(editRecipientsPage.IsTeamMemberEnabled(_team3Response.Name, listOfTeamMembers.First()),
                "TeamMember is not grayed out");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Recipients_LinkAndEnvelopeIcon_Publish()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _team4WithTeamMemberResponses, _team4Id, false);
            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and edit Pulse and go to 'Edit Recipients' tab");
            teamAssessmentDashboardPage.NavigateToPage(_team4WithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            var pulseData = PulseV2Factory.GetPulseAddData();
            editPulseCheckPage.FillSchedulerInfo(pulseData);

            Log.Info("Go to 'Edit Recipients' tab, Expand the team and verify 'Cluster  envelope' icon, 'Individual envelope' icon and 'Survey Link' icon is not displayed and 'Publish' the assessment");
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();
            editRecipientsPage.ClickOnTeamExpandIcon(_team4Response.Name);

            Assert.IsFalse(editRecipientsPage.IsBatchEmailDisplayed(), "'Cluster envelope' icon is displayed");

            foreach (var member in _team4WithTeamMemberResponses.First().SelectedParticipants)
            {
                Assert.IsFalse(editRecipientsPage.IsSurveyLinkIconPresent(_team4Response.Name, member.Email), "'Survey Link' icon is displayed");
                Assert.IsFalse(editRecipientsPage.IsIndividualEnvelopePresent(_team4Response.Name, member.Email), "'Individual Envelope' icon is displayed");
            }

            editRecipientsPage.Header.ClickOnPublishButton();
            editRecipientsPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Verify that email for both members are present");
            foreach (var member in _team4WithTeamMemberResponses.First().SelectedParticipants)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team4Response.Name),
                    member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4Response.Name)}> sent to {member.Email}");
            }

            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest.Name), "PulseAssessment is not displayed");

            Log.Info("Go to 'Edit Recipients' tab, Expand the team and verify 'Cluster  envelope' icon, 'Individual envelope' icon and 'Survey Link' icon is displayed");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            editPulseCheckPage.Header.ClickOnEditRecipientsTab();
            editRecipientsPage.ClickOnTeamExpandIcon(_team4Response.Name);

            Assert.IsTrue(editRecipientsPage.IsBatchEmailDisplayed(), "'Cluster envelope' icon is not displayed");

            foreach (var member in _team4WithTeamMemberResponses.First().SelectedParticipants)
            {
                Assert.IsTrue(editRecipientsPage.IsIndividualEnvelopePresent(_team4Response.Name, member.Email), "'Individual Envelope' icon is not displayed");
                Assert.IsTrue(editRecipientsPage.IsSurveyLinkIconPresent(_team4Response.Name, member.Email), "'Survey Link' icon is not displayed");
            }

            Log.Info("Click on 'Cluster envelope' icon and verify toaster message");
            editRecipientsPage.ClickOnClusterEnvelopeIcon();
            Assert.IsTrue(editRecipientsPage.IsEmailSentToasterMessageDisplayed(), "'Reminder has been sent' toaster message is not displayed");

            Log.Info("Verify that email for both members are present");

            foreach (var member in _team4WithTeamMemberResponses.First().SelectedParticipants)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team4Response.Name),
                    member.Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4Response.Name)}> sent to {member.Email}");
            }

            Log.Info("Fill the Pulse survey for first member");
            CompletePulseSurvey(TestContext, filterQuestions, _team4Response.Name, _team4WithTeamMemberResponses.First().SelectedParticipants.First().Email);

            Log.Info("Click on 'Individual envelope' icon and verify toaster message");
            editRecipientsPage.ClickOnIndividualEnvelopeIcon(_team4Response.Name, _team4WithTeamMemberResponses.First().SelectedParticipants.Last().Email);
            Assert.IsTrue(editRecipientsPage.IsEmailSentToasterMessageDisplayed(), "'Reminder has been sent' toaster message is not displayed");

            Log.Info("Verify that email for last member is present");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_team4Response.Name), _team4WithTeamMemberResponses.First().SelectedParticipants.Last().Email, "Inbox", "", 360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_team4WithTeamMemberResponses.First().SelectedParticipants.Last().Email)}> sent to ");

            Log.Info("Click on 'Survey Link' icon");
            var surveyLink = editRecipientsPage.ClickOnSurveyLinkIcon(_team4Response.Name, _team4WithTeamMemberResponses.First().SelectedParticipants.First().Email);

            Log.Info("Verify that user gets navigated to correct survey screen");
            surveyPage.NavigateToUrl(surveyLink);
            Assert.AreEqual(Constants.AlreadySurveyCompletedMessage, surveyPage.GetSurveyAlreadyCompletedText(), "Validation message is not matched on survey page");
        }

        //Common method
        private void PulseV2_Edit_Recipients_WithRole_SaveAsDraft(SavePulseAssessmentV2Request request, int teamId, TeamResponse teamResponse, List<TeamV2Response> _teamWithTeamMemberResponses)
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and edit Pulse");
            teamAssessmentDashboardPage.NavigateToPage(_teamWithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(request.Name);
            editRecipientsPage.Header.ClickOnEditQuestionsTab();

            Log.Info("Go to 'Edit Recipients' tab and verify selected 'Teams' and 'Team Members'");
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();
            Assert.IsTrue(editRecipientsPage.IsTeamSelected(teamResponse.Name), "Team is not selected");

            Log.Info("Deselect team and verify 'Save' button is disabled");
            editRecipientsPage.ClickOnSelectTeamCheckbox(teamResponse.Name, false);
            Assert.IsFalse(editRecipientsPage.Header.IsSaveEnabled(), "'Save' button is enabled");

            Log.Info("Select team and verify all the 'Team Members'");
            editRecipientsPage.ClickOnSelectTeamCheckbox(teamResponse.Name);

            Log.Info("Verify the teamMembers after Select the role from filter");
            editRecipientsPage.SelectDeselectRoleFromFilterDropDown(_teamWithTeamMemberResponses.First().SelectedParticipants.Last().Tags.First().Tags.First().Name);

            var listOfTeamMembers = editRecipientsPage.GetTeamMembersEmailByTeamNames(teamResponse.Name);
            Assert.That.ListContains(_teamWithTeamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), listOfTeamMembers.First(),
                "Member does not match");

            var listOfTeamMembersWithRole = editRecipientsPage.GetTeamMemberRoleByTeamName(teamResponse.Name);
            Assert.That.ListContains(_teamWithTeamMemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRole.First(), "Role does not match");

            Log.Info("Save the assessment and go to edit of created assessment and verify selected teams and team members");
            editRecipientsPage.Header.ClickSaveAsDraftButton();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(request.Name);
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            editPulseCheckPage.Header.ClickOnEditRecipientsTab();

            var listOfTeamMembersAfterSave = editRecipientsPage.GetTeamMembersEmailByTeamNames(teamResponse.Name);
            Assert.That.ListContains(_teamWithTeamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(),
                listOfTeamMembersAfterSave.First(), "Member does not match");

            var listOfTeamMembersWithRoleAfterSave = editRecipientsPage.GetTeamMemberRoleByTeamName(teamResponse.Name);
            Assert.That.ListContains(_teamWithTeamMemberResponses.First().SelectedParticipants.Where(a => a.Tags.Count != 0).Select(a => a.Tags?.First().Tags?.First().Name).ToList(),
                listOfTeamMembersWithRoleAfterSave.First(), "Role does not match");
        }
    }
}