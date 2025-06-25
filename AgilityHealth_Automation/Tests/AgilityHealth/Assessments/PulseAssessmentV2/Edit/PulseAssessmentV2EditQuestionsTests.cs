using System;
using System.Collections;
using System.Collections.Generic;
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
    public class PulseAssessmentV2EditQuestionsTests : PulseV2BaseTest
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
                var team = TeamFactory.GetNormalTeam("PulseV2Team", 2);
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Questions_DimensionLevel()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Dimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _teamWithTeamMemberResponses, _teamId, false);
            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and edit Pulse");
            teamAssessmentDashboardPage.NavigateToPage(_teamWithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Go to 'Edit Questions' tab  and verify selected dimension and all the beneath subDimensions, competencies and  questions");
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            var dimensionName = filterQuestions.Dimensions.First().Name;

            Assert.IsTrue(editQuestionsPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is not selected");

            var expectedTotalQuestionCount = _questionDetailsResponse.Dimensions.Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(d => d.SubDimensions).SelectMany(z => z.Competencies).SelectMany(c => c.Questions).ToList().Count;

            var subDimensions = _questionDetailsResponse.Dimensions.Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(d => d.SubDimensions.Select(x => x.Name)).ToList();
            foreach (var subDimension in subDimensions)
            {
                //Sub Dimension Verification

                Assert.IsTrue(editQuestionsPage.IsQuestionSectionDisplayed(subDimension), $"{subDimension} - sub dimension is not displayed");

                //Competency Verification
                var competencies = _questionDetailsResponse.Dimensions
                .Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimension)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();
                foreach (var competency in competencies)
                {
                    Assert.IsTrue(editQuestionsPage.IsQuestionSectionDisplayed(competency), $"{competency} - competency is not displayed");

                    var actualPulseQuestions = editQuestionsPage.GetAllQuestionsByCompetency(competency);
                    QuestionsVerification(_questionDetailsResponse, SharedConstants.SurveyDimension, subDimension, competency, actualPulseQuestions);
                }
            }

            var actualTotalQuestionCount = editQuestionsPage.GetTotalCountOfQuestions();
            Assert.AreEqual(expectedTotalQuestionCount, actualTotalQuestionCount, "Number Of Questions is not matched");

            Log.Info("Remove selected dimension and select another dimension then publish the pulse assessment");
            editQuestionsPage.CheckUncheckQuestionCheckbox(dimensionName, false);
            Assert.IsFalse(editQuestionsPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension is selected");

            //Select another question
            editQuestionsPage.CheckUncheckQuestionCheckbox(SharedConstants.DimensionClarity);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            editQuestionsPage.Header.ClickOnPublishButton();
            editQuestionsPage.Header.ClickOnPublishPopupPublishButton();

            //edit
            Log.Info("Switch to pulse view and edit recently created pulse assessment");
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();

            Log.Info("Verify that updated dimensions and all the beneath subDimensions, competencies and questions should be displayed");
            _questionDetailsResponse = GetQuestions(_teamId);

            var updatedSubDimension = _questionDetailsResponse.Dimensions.Where(s => s.Name.Equals(SharedConstants.DimensionClarity)).SelectMany(d => d.SubDimensions.Select(x => x.Name)).ToList();
            foreach (var subDimension in updatedSubDimension)
            {
                var competencies = _questionDetailsResponse.Dimensions
                .Where(s => s.Name.Equals(SharedConstants.DimensionClarity)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimension)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();
                foreach (var competency in competencies)
                {
                    Assert.IsTrue(editQuestionsPage.IsCompetencyDisplayed(competency), $"{competency} - competency is not displayed");

                    var actualPulseQuestions = editQuestionsPage.GetAllQuestionsByCompetencyAfterPublish(competency);
                    QuestionsVerification(_questionDetailsResponse, SharedConstants.DimensionClarity, subDimension, competency, actualPulseQuestions);
                }
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Questions_SubDimensionLevel()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.SubDimension);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _teamWithTeamMemberResponses, _teamId, false);

            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and edit Pulse");
            teamAssessmentDashboardPage.NavigateToPage(_teamWithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Go to 'Edit Questions' tab  and verify selected subDimension and all the beneath competencies and questions");
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            var dimensionName = filterQuestions.Dimensions.First().Name;
            var subDimensionName = filterQuestions.Dimensions.First().SubDimensions.First().Name;

            Assert.IsTrue(editQuestionsPage.IsQuestionCheckboxSelected(subDimensionName), $"{subDimensionName} - Sub dimension  is not selected");

            var expectedTotalQuestionCount = _questionDetailsResponse.Dimensions.Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(d => d.SubDimensions.Where(s => s.Name.Equals(SharedConstants.SurveySubDimension))).SelectMany(z => z.Competencies).SelectMany(c => c.Questions).ToList().Count;

            //Sub Dimension Verification
            Assert.IsTrue(editQuestionsPage.IsQuestionSectionDisplayed(SharedConstants.SurveySubDimension), $"{SharedConstants.SurveySubDimension} - sub dimension is not displayed");

            //Competency Verification
            var competencies = _questionDetailsResponse.Dimensions
                    .Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(SharedConstants.SurveySubDimension)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();
            foreach (var competency in competencies)
            {
                Assert.IsTrue(editQuestionsPage.IsQuestionSectionDisplayed(competency), $"{competency} - competency is not displayed");

                var actualPulseQuestions = editQuestionsPage.GetAllQuestionsByCompetency(competency);
                QuestionsVerification(_questionDetailsResponse, dimensionName, SharedConstants.SurveySubDimension, competency, actualPulseQuestions);
            }

            var actualTotalQuestionCount = editQuestionsPage.GetTotalCountOfQuestions();
            Assert.AreEqual(expectedTotalQuestionCount, actualTotalQuestionCount, "Number Of Questions is not matched");
            Assert.IsTrue(editQuestionsPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is not selected");

            Log.Info("Remove selected subDimension and select another subDimension then publish the pulse assessment");
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.DimensionFoundation);
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.SurveySubDimension);
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.SurveySubDimensionAgility);

            editQuestionsPage.CheckUncheckQuestionCheckbox(subDimensionName, false);
            Assert.IsFalse(editQuestionsPage.IsQuestionCheckboxSelected(subDimensionName), $"{subDimensionName} - Sub dimension is selected");

            //Select another question
            editQuestionsPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveySubDimensionAgility);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            editQuestionsPage.Header.ClickOnPublishButton();
            editQuestionsPage.Header.ClickOnPublishPopupPublishButton();

            //edit
            Log.Info("Switch to pulse view and edit recently created pulse assessment");
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();

            Log.Info("Verify that updated subDimensions and all the beneath competencies and questions should be displayed");
            _questionDetailsResponse = GetQuestions(_teamId);

            var updatedCompetency = _questionDetailsResponse.Dimensions
                    .Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(SharedConstants.SurveySubDimensionAgility)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();

            foreach (var competency in updatedCompetency)
            {
                Assert.IsTrue(editQuestionsPage.IsCompetencyDisplayed(competency), $"{competency} - competency is not displayed");

                var actualPulseQuestions = editQuestionsPage.GetAllQuestionsByCompetencyAfterPublish(competency);
                QuestionsVerification(_questionDetailsResponse, dimensionName, SharedConstants.SurveySubDimensionAgility, competency, actualPulseQuestions);
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_Questions_CompetencyLevel()
        {
            VerifySetup(_classInitFailed);

            var filterQuestions = _questionDetailsResponse.FilterQuestions(QuestionSelectionPreferences.Competency);
            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(filterQuestions, _teamWithTeamMemberResponses, _teamId, false);

            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var editQuestionsPage = new EditQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and edit Pulse");
            teamAssessmentDashboardPage.NavigateToPage(_teamWithTeamMemberResponses.First().TeamId);
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);

            Log.Info("Go to 'Edit Questions' tab  and verify selected competency and all the beneath questions");
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();
            var competencyName = filterQuestions.Dimensions.First().SubDimensions.First().Competencies.First().Name;

            Assert.IsTrue(editQuestionsPage.IsQuestionCheckboxSelected(competencyName), $"{competencyName} - Competency is not selected");

            var expectedTotalQuestionCount = _questionDetailsResponse.Dimensions.Where(s => s.Name.Equals(SharedConstants.SurveyDimension)).SelectMany(d => d.SubDimensions.Where(s => s.Name.Equals(SharedConstants.SurveySubDimension))).SelectMany(z => z.Competencies.Where(s => s.Name.Equals(SharedConstants.SurveyCompetency))).SelectMany(c => c.Questions).ToList().Count;

            //Sub Dimension Verification
            Assert.IsTrue(editQuestionsPage.IsQuestionSectionDisplayed(SharedConstants.SurveySubDimension), $"{SharedConstants.SurveySubDimension} - sub dimension is not displayed");

            //Competency Verification
            Assert.IsTrue(editQuestionsPage.IsQuestionSectionDisplayed(SharedConstants.SurveyCompetency), $"{SharedConstants.SurveyCompetency} - competency is not displayed");

            //Questions Verification
            var actualPulseQuestions = editQuestionsPage.GetAllQuestionsByCompetency(SharedConstants.SurveyCompetency);
            QuestionsVerification(_questionDetailsResponse, SharedConstants.SurveyDimension, SharedConstants.SurveySubDimension, SharedConstants.SurveyCompetency, actualPulseQuestions);

            var actualTotalQuestionCount = editQuestionsPage.GetTotalCountOfQuestions();
            Assert.AreEqual(expectedTotalQuestionCount, actualTotalQuestionCount, "Number Of Questions is not matched");

            Log.Info("Remove selected competency and select another competency then publish the pulse assessment");
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.DimensionFoundation);
            editQuestionsPage.ExpandCollapseQuestion(SharedConstants.SurveySubDimension);

            editQuestionsPage.CheckUncheckQuestionCheckbox(competencyName, false);
            Assert.IsFalse(editQuestionsPage.IsQuestionCheckboxSelected(competencyName), $"{competencyName} - Competency is selected");

            //Select another question
            editQuestionsPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyCompetencyWorkSpace);
            editQuestionsPage.Header.ClickOnEditRecipientsTab();
            editQuestionsPage.Header.ClickOnPublishButton();
            editQuestionsPage.Header.ClickOnPublishPopupPublishButton();

            //edit
            Log.Info("Switch to pulse view and edit recently created pulse assessment");
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseRequest.Name);
            editPulseCheckPage.Header.ClickOnEditQuestionsTab();

            Log.Info("Verify that updated competency and all the beneath questions should be displayed");
            _questionDetailsResponse = GetQuestions(_teamId);

            var actualUpdatedPulseQuestions = editQuestionsPage.GetAllQuestionsByCompetencyAfterPublish(SharedConstants.SurveyCompetencyWorkSpace);
            QuestionsVerification(_questionDetailsResponse, SharedConstants.SurveyDimension, SharedConstants.SurveySubDimension, SharedConstants.SurveyCompetencyWorkSpace, actualUpdatedPulseQuestions);
        }

        private static void QuestionsVerification(RadarQuestionDetailsV2Response questionsDetailsResponse, string dimension, string subDimension, string competency, IList actualPulseQuestionsList)
        {
            //Questions Verification
            var expectedCompetencyQuestionsList = questionsDetailsResponse.Dimensions.Where(s => s.Name.Equals(dimension))
                .SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimension)).SelectMany(z => z.Competencies
                .Where(c => c.Name.Equals(competency)).SelectMany(c => c.Questions.Select(rq => rq.Text.FormatSurveyQuestions())))).ToList();

            var index = 1;
            var modifiedCompetencyQuestionsList = expectedCompetencyQuestionsList.Select(q =>
                $"Q{index++}:{competency.Replace(" ", "")}{q}").ToList();

            Assert.That.ListsAreEqual(modifiedCompetencyQuestionsList, actualPulseQuestionsList, "Pulse Question list is not matched.");
        }
    }
}