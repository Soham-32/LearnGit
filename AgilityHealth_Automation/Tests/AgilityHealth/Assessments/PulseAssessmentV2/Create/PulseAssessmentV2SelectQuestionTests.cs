using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2SelectQuestionTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("PulseV2Team");
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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_DisplayByDimension()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select questions under 'Select Questions' page and verify");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);
            var dimensionName = pulseData.Questions.First().DimensionName;
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox isn't selected");

            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();
            var expectedTotalQuestionCount = 0;

            foreach (var questionDetails in pulseData.Questions)
            {
                expectedTotalQuestionCount += surveyResponse.Dimensions.Where(s => s.Name.Equals(questionDetails.DimensionName)).SelectMany(d => d.SubDimensions).SelectMany(z => z.Competencies).SelectMany(c => c.Questions).ToList().Count;

                var subDimension = surveyResponse.Dimensions.Where(s => s.Name.Equals(questionDetails.DimensionName)).SelectMany(d => d.SubDimensions.Select(x => x.Name)).ToList(); foreach (var subDimensionName in subDimension)
                {
                    //Sub Dimension Verification
                    Assert.IsTrue(selectQuestionPage.IsQuestionSectionDisplayed(subDimensionName), $"{subDimensionName} - sub dimension isn't displayed");

                    //Competency Verification
                    var competency = surveyResponse.Dimensions.Where(s => s.Name.Equals(questionDetails.DimensionName)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimensionName)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();
                    foreach (var competencyName in competency)
                    {
                        Assert.IsTrue(selectQuestionPage.IsQuestionSectionDisplayed(competencyName), $"{competencyName} - competency isn't displayed");

                        var actualPulseQuestionList = selectQuestionPage.GetAllQuestionsByCompetency(competencyName);
                        QuestionsVerification(surveyResponse, questionDetails.DimensionName, subDimensionName, competencyName, actualPulseQuestionList);
                    }
                }
            }
            var actualTotalQuestionCount = selectQuestionPage.GetTotalCountOfQuestions();
            Assert.AreEqual(expectedTotalQuestionCount, actualTotalQuestionCount, "Number Of Questions is not matched");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_DisplayBySubDimension()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select questions under 'Select Questions' page and verify");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.SubDimension;
            selectQuestionPage.FillForm(pulseData);

            var subDimensionName = pulseData.Questions.First().SubDimensionName;
            var expectedTotalQuestionCount = 0;
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(subDimensionName), $"{subDimensionName} - sub dimension checkbox isn't selected");

            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();

            foreach (var questionDetails in pulseData.Questions)
            {
                expectedTotalQuestionCount += surveyResponse.Dimensions.Where(s => s.Name.Equals(questionDetails.DimensionName)).SelectMany(d => d.SubDimensions.Where(s => s.Name.Equals(questionDetails.SubDimensionName))).SelectMany(z => z.Competencies).SelectMany(c => c.Questions).ToList().Count;

                //Sub Dimension Verification
                Assert.IsTrue(selectQuestionPage.IsQuestionSectionDisplayed(questionDetails.SubDimensionName), $"{questionDetails.SubDimensionName} - sub dimension isn't displayed");

                //Competency Verification
                var competency = surveyResponse.Dimensions.Where(s => s.Name.Equals(questionDetails.DimensionName)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(questionDetails.SubDimensionName)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();
                foreach (var competencyName in competency)
                {
                    Assert.IsTrue(selectQuestionPage.IsQuestionSectionDisplayed(competencyName), $"{competencyName} - competency isn't displayed");

                    var actualPulseQuestionList = selectQuestionPage.GetAllQuestionsByCompetency(competencyName);
                    QuestionsVerification(surveyResponse, questionDetails.DimensionName, subDimensionName, competencyName, actualPulseQuestionList);
                }
            }
            var actualTotalQuestionCount = selectQuestionPage.GetTotalCountOfQuestions();
            Assert.AreEqual(expectedTotalQuestionCount, actualTotalQuestionCount, "Number Of Questions is not matched");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_DisplayByCompetency()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select questions under 'Select Questions' page and verify");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.Competency;
            selectQuestionPage.FillForm(pulseData);

            var competency = pulseData.Questions.First().CompetencyName;
            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();
            var expectedTotalQuestionCount = 0;
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(competency), $"{competency} - competency checkbox isn't selected");

            foreach (var questionDetails in pulseData.Questions)
            {
                expectedTotalQuestionCount += surveyResponse.Dimensions.Where(s => s.Name.Equals(questionDetails.DimensionName)).SelectMany(d => d.SubDimensions.Where(s => s.Name.Equals(questionDetails.SubDimensionName))).SelectMany(z => z.Competencies.Where(s => s.Name.Equals(questionDetails.CompetencyName))).SelectMany(c => c.Questions).ToList().Count;

                //Sub Dimension Verification
                Assert.IsTrue(selectQuestionPage.IsQuestionSectionDisplayed(questionDetails.SubDimensionName), $"{questionDetails.SubDimensionName} - sub dimension isn't displayed");

                //Competency Verification
                Assert.IsTrue(selectQuestionPage.IsQuestionSectionDisplayed(questionDetails.CompetencyName), $"{questionDetails.CompetencyName} - competency isn't displayed");

                var actualPulseQuestionList = selectQuestionPage.GetAllQuestionsByCompetency(questionDetails.CompetencyName);
                QuestionsVerification(surveyResponse,questionDetails.DimensionName,questionDetails.SubDimensionName,questionDetails.CompetencyName,actualPulseQuestionList);
            }
            var actualTotalQuestionCount = selectQuestionPage.GetTotalCountOfQuestions();
            Assert.AreEqual(expectedTotalQuestionCount, actualTotalQuestionCount, "Number Of Questions is not matched");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_SelectByDimension()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select questions under 'Select Questions' page and verify");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);

            var dimensionName = pulseData.Questions.First().DimensionName;

            //Dimension Verification
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox isn't selected");

            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();
            var subDimensions = surveyResponse.Dimensions.Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions.Select(x => x.Name)).ToList();

            //Expanding Dimension section
            selectQuestionPage.ExpandCollapseQuestion(dimensionName);

            foreach (var subDimension in subDimensions)
            {
                //Sub Dimension Verification
                Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(subDimension), $"{subDimension} - sub dimension checkbox isn't selected");

                var competencies = surveyResponse.Dimensions
                    .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).Where(d => d.Name.Equals(subDimension)).SelectMany(z => z.Competencies.Select(c => c.Name)).ToList();

                //Expanding Sub-Dimension section
                selectQuestionPage.ExpandCollapseQuestion(subDimension);
                
                //Competency Verification
                competencies.ForEach(competency => Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(competency), $"{competency} - competency checkbox isn't selected"));
            }

            selectQuestionPage.CheckUncheckQuestionCheckbox(dimensionName, false);

            //Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is selected");

            //Sub Dimension Verification
            subDimensions.ForEach(subDimension => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(subDimension), $"{subDimension} - sub dimension checkbox is selected"));

            var allCompetencies = surveyResponse.Dimensions
                .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).SelectMany(z => z.Competencies.Select(c => c.Name)).ToList();
            
            //Competency Verification
            allCompetencies.ForEach(competency => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(competency), $"{competency} - competency checkbox is selected"));
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_SelectBySubDimension()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select questions under 'Select Questions' page and verify");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.SubDimension;
            selectQuestionPage.FillForm(pulseData);

            var dimensionName = pulseData.Questions.First().DimensionName;
            var subDimensionName = pulseData.Questions.First().SubDimensionName;

            //Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is selected");

            //Sub-Dimension Verification
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(subDimensionName), $"{subDimensionName} - sub dimension checkbox isn't selected");

            //Sub-Dimension Competency Verification
            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();
            var subDimensionCompetencies = surveyResponse.Dimensions
                .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).Where(d => d.Name.Equals(subDimensionName)).SelectMany(z => z.Competencies.Select(c => c.Name)).ToList();

            selectQuestionPage.ExpandCollapseQuestion(subDimensionName);

            //Competency Verification
            subDimensionCompetencies.ForEach(competency => Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(competency), $"{competency} - competency checkbox isn't selected"));

            //Other Sub-Dimensions / Competencies Verification
            var otherSubDimensions = surveyResponse.Dimensions.Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions.Where(x => !x.Name.Equals(subDimensionName)).Select(x => x.Name)).ToList();

            foreach (var subDimension in otherSubDimensions)
            {
                //Sub Dimension Verification
                Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(subDimension), $"{subDimension} - sub dimension checkbox is selected");

                var competencies = surveyResponse.Dimensions
                    .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).Where(d => d.Name.Equals(subDimension)).SelectMany(z => z.Competencies.Select(c => c.Name)).ToList();

                //Expanding Sub-Dimension section
                selectQuestionPage.ExpandCollapseQuestion(subDimension);

                //Competency Verification
                competencies.ForEach(competency => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(competency), $"{competency} - competency checkbox is selected"));
            }

            selectQuestionPage.CheckUncheckQuestionCheckbox(subDimensionName, false);

            //Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is selected");

            var subDimensions = surveyResponse.Dimensions.Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions.Select(x => x.Name)).ToList();

            //Sub Dimension Verification
            subDimensions.ForEach(subDimension => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(subDimension), $"{subDimension} - sub dimension checkbox is selected"));

            var allCompetencies = surveyResponse.Dimensions
                .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).SelectMany(z => z.Competencies.Select(c => c.Name)).ToList();

            //Competency Verification
            allCompetencies.ForEach(competency => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(competency), $"{competency} - competency checkbox is selected"));
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_SelectByCompetency()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select questions under 'Select Questions' page and verify");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.Competency;
            selectQuestionPage.FillForm(pulseData);

            var dimensionName = pulseData.Questions.First().DimensionName;
            var subDimensionName = pulseData.Questions.First().SubDimensionName;
            var competencyName = pulseData.Questions.First().CompetencyName;

            //Competency Verification
            Assert.IsTrue(selectQuestionPage.IsQuestionCheckboxSelected(competencyName), $"{competencyName} - competency checkbox isn't selected");

            //Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is selected");

            //Sub-Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(subDimensionName), $"{dimensionName} - sub dimension checkbox is selected");

            //Other Competencies Verification
            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();
            var otherCompetencies = surveyResponse.Dimensions
                .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).Where(d => d.Name.Equals(subDimensionName)).SelectMany(z => z.Competencies.Where(c => !c.Name.Equals(competencyName)).Select(c => c.Name)).ToList();

            //Competency Verification
            otherCompetencies.ForEach(otherCompetency => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(otherCompetency), $"{otherCompetency} - competency checkbox is selected"));

            selectQuestionPage.CheckUncheckQuestionCheckbox(competencyName, false);

            //Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(dimensionName), $"{dimensionName} - dimension checkbox is selected");

            //Sub-Dimension Verification
            Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(subDimensionName), $"{dimensionName} - sub dimension checkbox is selected");

            //All Competencies Verification
            var competencies = surveyResponse.Dimensions
                .Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions).Where(d => d.Name.Equals(subDimensionName)).SelectMany(z => z.Competencies.Select(c => c.Name)).ToList();

            //Competency Verification
            competencies.ForEach(otherCompetency => Assert.IsFalse(selectQuestionPage.IsQuestionCheckboxSelected(otherCompetency), $"{otherCompetency} - competency checkbox is selected"));
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_Questions_NextDisabled()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, select/deselect questions under 'Select Questions' page and verify 'Next' button");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Assert.IsFalse(createPulseCheckPage.Header.IsNextButtonEnabled(), "Next button is enabled");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);

            createPulseCheckPage.Header.ClickOnNextButton();

            pulseData.Questions = null;

            selectQuestionPage.FillForm(pulseData);
            Assert.IsFalse(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is enabled");

            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyDimension);
            Assert.IsTrue(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is disabled");

            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyDimension, false);
            Assert.IsFalse(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is enabled");

            selectQuestionPage.ExpandCollapseQuestion(SharedConstants.SurveyDimension);
            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveySubDimension);
            Assert.IsTrue(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is disabled");

            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveySubDimension, false);
            Assert.IsFalse(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is enabled");

            selectQuestionPage.ExpandCollapseQuestion(SharedConstants.SurveySubDimension);
            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyCompetency);
            Assert.IsTrue(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is disabled");

            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyCompetency, false);
            Assert.IsFalse(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is enabled");

            selectQuestionPage.CheckUncheckQuestionCheckbox(SharedConstants.SurveyDimension);
            Assert.IsTrue(selectQuestionPage.Header.IsNextButtonEnabled(), "Next button is disabled");
        }

        private static void QuestionsVerification(RadarQuestionDetailsV2Response questionsDetailsResponse, string dimension, string subDimension, string competency, IList actualPulseQuestionsList)
        {
            //Questions Verification
            var expectedCompetencyQuestionsList1 = questionsDetailsResponse.Dimensions.Where(s => s.Name.Equals(dimension))
                .SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimension)).SelectMany(z => z.Competencies
                    .Where(c => c.Name.Equals(competency)).SelectMany(c => c.Questions.Select(rq => rq.Text.FormatSurveyQuestions())))).ToList();

            var modifiedCompetencyQuestionsList1 = new List<string>();
            var index1 = 1;
            foreach (var question in expectedCompetencyQuestionsList1.Select(q => $"Q{index1}:{competency.Replace(" ", "")}{q}"))
            {
                modifiedCompetencyQuestionsList1.Add(question);
                index1++;
            }
            
            Assert.That.ListsAreEqual(modifiedCompetencyQuestionsList1, actualPulseQuestionsList, "Pulse Question list is not matched");
        }
    }
}