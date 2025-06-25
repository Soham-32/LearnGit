using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Delete
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2DeleteTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static RadarQuestionDetailsV2Response _questionDetailsResponse;
        private static TeamResponse _teamResponse;
        private static int _teamId;
        private static List<TeamV2Response> _teamWithTeamMemberResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("PulseV2Team", 1);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                //Get team profile 
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId).ToList();

                _questionDetailsResponse = GetQuestions(_teamId);
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
        public void PulseV2_Edit_Delete_SaveAsDraft()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _teamWithTeamMemberResponses, _teamId, false);
            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team assessment dashboard and edit drafted pulse assessment");
            teamAssessmentDashboardPage.NavigateToPage(_teamWithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Click on 'Cancel' button of 'Delete Assessment' popup and verify pulse is not deleted");
            editPulseCheckPage.Header.ClickDeleteButton();
            editPulseCheckPage.Header.ClickDeleteAssessmentPopupCancelButton();
            Assert.IsTrue(editRecipientsPage.Header.IsEditPulseCheckTitleDisplayed(), "Edit pulse check title is not displayed");

            Log.Info("Click on 'Delete' button of 'Delete Assessment' popup and verify pulse is not displayed on teamAssessment dashboard");
            editPulseCheckPage.Header.ClickDeleteButton();
            editPulseCheckPage.Header.ClickOnDeleteAssessmentPopupDeleteButton();
            Assert.IsFalse(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest.Name), "Pulse assessment is still displayed");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Delete_Publish()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _teamWithTeamMemberResponses, _teamId);
            AddPulseAssessment(pulseRequest);

            var pulseRequest1 = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _teamWithTeamMemberResponses, _teamId);
            AddPulseAssessment(pulseRequest1);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var pulseGrowthJourneyPage = new PulseGrowthJourneyPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editRecipientsPage = new EditRecipientsPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Fill the survey for first member");
            CompletePulseSurvey(TestContext,filterQuestions, _teamResponse.Name, _teamResponse.Members.First().Email);

            Log.Info("Go to team assessment dashboard and click on edit of pulse assessment");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Click on 'Cancel' button of 'Delete Assessment' popup and verify pulse is not deleted");
            editPulseCheckPage.Header.ClickDeleteButton();
            editPulseCheckPage.Header.ClickDeleteAssessmentPopupCancelButton();
            Assert.IsTrue(editRecipientsPage.Header.IsEditPulseCheckTitleDisplayed(), "Edit pulse check title is not displayed");

            Log.Info("Click on 'Delete' button of 'Delete Assessment' popup and verify pulse is not displayed on teamAssessment dashboard");
            editPulseCheckPage.Header.ClickDeleteButton();
            editPulseCheckPage.Header.ClickOnDeleteAssessmentPopupDeleteButton();
            Assert.IsFalse(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest.Name), "Pulse assessment is still displayed");
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseRequest1.Name), "Pulse assessment is not displayed");

            Log.Info("Go to pulse radar and verify deleted pulse is not displayed");
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseRadar(pulseRequest1.Name);
            Assert.IsFalse(pulseGrowthJourneyPage.IsPulseResultsDisplayed(pulseRequest.Name), "Pulse assessment is not displayed");
        }
    }
}
