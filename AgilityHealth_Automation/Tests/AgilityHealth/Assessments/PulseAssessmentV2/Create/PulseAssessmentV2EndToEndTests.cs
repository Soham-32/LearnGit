using System;
using System.Linq;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2EndToEndTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static int _teamId;
        private static TeamResponse _teamResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51186
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_EndToEnd_Functionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var firstTeamMemberName = _teamResponse.Members.First().FirstName + " " + _teamResponse.Members.First().LastName;
            var secondTeamMemberName = _teamResponse.Members.Last().FirstName + " " + _teamResponse.Members.Last().LastName;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Log.Info("Fill the valid data on 'Create Pulse Check' page");
            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Select Questions and verify the selected question on 'Select Questions' page");
            selectQuestionPage.FillForm(pulseData);
            var dimensionName = pulseData.Questions.First().DimensionName;
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox isn't selected");

            selectQuestionPage.Header.ClickOnNextButton();

            Log.Info("Verify the selected teams form 'Select Recipients' page");
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            Assert.IsTrue(selectRecipientsPage.Header.IsNextButtonEnabled(), "Next button is disabled");

            var allTeams = selectRecipientsPage.GetListOfTeams();
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(allTeams.First()), "Team is not selected");

            selectRecipientsPage.Header.ClickOnNextButton();

            Log.Info("Publish the pulse assessment from the 'Review and Publish' page");
            reviewAndPublishPage.ClickOnPublishButton();
            reviewAndPublishPage.Header.ClickOnPublishPopupCancelButton();

            Assert.IsTrue(reviewAndPublishPage.IsPublishButtonDisplayed(), "Publish button is not available");

            reviewAndPublishPage.ClickOnPublishButton();
            reviewAndPublishPage.Header.ClickOnPublishPopupPublishButton();

            var participant = teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseData.Name);
            Assert.AreEqual("Completed by 0 out of 2 Team Members", participant, "Participant info doesn't match");

            Log.Info("Verify that email for both members are present");
            _teamResponse.Members.ForEach(member => Assert.IsTrue(GmailUtil.DoesMemberEmailExist(SharedConstants.PulseEmailSubject(_teamResponse.Name),
                   member.Email, "Inbox","",360), $"Could not find email with subject <{SharedConstants.PulseEmailSubject(_teamResponse.Name)}> sent to {member.Email}"));

            Log.Info("Fill the Pulse survey for first member and verify the dimension");
            surveyPage.NavigateToUrl(GmailUtil.GetPulseSurveyLink(SharedConstants.PulseEmailSubject(_teamResponse.Name), _teamResponse.Members.First().Email, "inbox"));

            surveyPage.ConfirmIdentity();
            var dimensions = surveyPage.GetLeftNavDimensionsList();
            Assert.AreEqual(dimensionName, dimensions.First(), "Dimension does not match");

            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(5);
            surveyPage.ClickFinishButton();

            Log.Info("Navigate to team assessment dashboard, Go to Pulse Growth Journey and verify");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();

            participant = teamAssessmentDashboardPage.GetPulseAssessmentTeamMemberCompletedInfo(pulseData.Name);
            Assert.AreEqual("Completed by 1 out of 2 Team Members", participant, "Participant info doesn't match");

            teamAssessmentDashboardPage.ClickOnPulseRadar(pulseData.Name);

            var listOfPulseAssessments = pulseGrowthJourneyPage.GetPulseAssessmentNames();
            Assert.AreEqual(pulseData.Name, listOfPulseAssessments.First(), "Pulse assessment name is not showing");

            Log.Info("Navigate to team assessment dashboard, Edit the pulse assessment and verify assessment completed tag");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            editPulseCheckPage.Header.ClickOnEditRecipientsTab();
            editRecipientsPage.ClickOnTeamExpandIcon(_teamResponse.Name);
            Assert.IsFalse(editRecipientsPage.IsIndividualEnvelopePresent(_teamResponse.Name, _teamResponse.Members.First().Email), "Envelope icon is showing");
            Assert.AreEqual("Completed", editRecipientsPage.GetSurveyCompletedTag(_teamResponse.Name, firstTeamMemberName), "Assessment is not completed");

            Assert.IsTrue(editRecipientsPage.IsIndividualEnvelopePresent(_teamResponse.Name, _teamResponse.Members.Last().Email), "Envelope icon is not showing");
            Assert.IsFalse(editRecipientsPage.IsSurveyCompletedTagDisplayed(_teamResponse.Name, secondTeamMemberName), "Completed tag is display");
        }
    }
}
