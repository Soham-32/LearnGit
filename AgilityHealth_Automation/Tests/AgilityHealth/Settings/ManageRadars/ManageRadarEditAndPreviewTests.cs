using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Dtos.Radars.Custom;
using AgilityHealth_Automation.SetUpTearDown;
using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageRadars
{
    [TestClass]
    [TestCategory("Settings")]
    [TestCategory("SiteAdmin")]
    public class ManageRadarEditAndPreviewTests : ManageRadarBaseTests
    {
        private static bool _classInitFailed;
        private static SetUpMethods _setupUi;
        private static readonly RadarDetails RadarInfo = ManageRadarFactory.GetValidRadarDetails();
        private static readonly RadarDimensionResponse FirstDimensionResponse = RadarInfo.RadarDimensions[0];
        private static readonly RadarDimensionResponse SecondDimensionResponse = RadarInfo.RadarDimensions[1];
        private static readonly RadarDimensionResponse ThirdDimensionResponse = RadarInfo.RadarDimensions[2];
        private static readonly RadarDimensionResponse ForthDimensionResponse = RadarInfo.RadarDimensions[3];
        private static readonly RadarDimensionResponse FifthDimensionResponse = RadarInfo.RadarDimensions[4];

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {

            try
            {
                _setupUi = new SetUpMethods(testContext, TestEnvironment);
                _setupUi.CreateRadar(RadarInfo);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        //Test Approach!

        //Created 5 Dimensions with SubDimension, Competencies and Questions.
        //Deleted Dimension 1.
        //Deleted SubDimension of Dimension 2.
        //Deleted Competency of Dimension 3.
        //Deleted Questions of Dimension 4.
        //Deleted last open ended question.
        //Verify all in preview survey.

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageRadar_Parameters_Add_Verify_Delete()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver, Log);
            var addRadarDimensionsPage = new AddRadarDimensionsPage(Driver, Log);
            var addRadarSubDimensionsPage = new AddRadarSubDimensionsPage(Driver, Log);
            var addRadarCompetenciesPage = new AddRadarCompetenciesPage(Driver, Log);
            var addRadarQuestionsPage = new AddRadarQuestionsPage(Driver, Log);
            var addRadarOpenEndedPage = new AddRadarOpenEndedPage(Driver, Log);
            var addRadarReviewAndFinishBasePage = new RadarReviewAndFinishBasePage(Driver, Log);
            var previewAssessmentPage = new PreviewAssessmentPage(Driver, Log);
            var differentDimensionResponses = new List<RadarDimensionResponse>
            {
                FirstDimensionResponse,
                SecondDimensionResponse,
                ThirdDimensionResponse,
                ForthDimensionResponse,
                FifthDimensionResponse
            };

            //Expected Radar Dimension and Open Ended List values from DTO
            var expectedRadarDimensionList =
                ManageRadarFactory.GetValidRadarDetails().RadarDimensions.Select(a => a.Name).ToList();
            var expectedRadarOpenEndedList = ManageRadarFactory.GetValidRadarDetails().RadarDimensions
                .SelectMany(a => a.RadarSubDimensions).SelectMany(s => s.RadarCompetencies)
                .SelectMany(c => c.RadarQuestions).SelectMany(q => q.RadarOpenEnded).Select(o => o.Text).ToList();

            Log.Info("Login to the application and edit radar");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();
            manageRadarPage.ClickOnRadarEditIcon(RadarInfo.Name);
            editRadarDetailsPage.SelectWizardStep("Dimensions");

            // Add Dimensions
            Log.Info("Add 'Dimensions' and click on 'Continue To' Button");
            foreach (var dimensionResponse in differentDimensionResponses)
            {
                addRadarDimensionsPage.ClickOnAddNewButton();
                addRadarDimensionsPage.EnterDimensionInfo(dimensionResponse);
                addRadarDimensionsPage.ClickOnUpdateButton();
            }

            addRadarDimensionsPage.ClickOnContinueToButton();

            // Add Sub Dimensions
            Log.Info("Add Sub Dimensions and click on 'Continue To' Button");
            foreach (var dimensionResponse in differentDimensionResponses)
            {
                addRadarSubDimensionsPage.ClickOnAddNewButton();
                addRadarSubDimensionsPage.EnterSubDimensionInfo(dimensionResponse.RadarSubDimensions.FirstOrDefault());
                addRadarSubDimensionsPage.ClickOnUpdateButton();
            }

            addRadarSubDimensionsPage.ClickOnContinueToButton();

            // Add Competencies
            Log.Info("Add Competencies and click on 'Continue To' Button");
            foreach (var dimensionResponse in differentDimensionResponses)
            {
                addRadarCompetenciesPage.ClickOnAddNewButton();
                Assert.IsFalse(addRadarCompetenciesPage.IsSubDimensionDropdownEnabled(),
                    "Sub Dimension Dropdown is 'Enabled'");
                Assert.IsTrue(addRadarCompetenciesPage.IsSubDimensionDropdownValidationMessageDisplayed(),
                    "Sub Dimension Dropdown Validation Message is not being displayed");
                addRadarCompetenciesPage.EnterCompetencyInfo(dimensionResponse.RadarSubDimensions.FirstOrDefault()
                    ?.RadarCompetencies.FirstOrDefault());
                Assert.IsTrue(addRadarCompetenciesPage.IsSubDimensionDropdownEnabled(),
                    "Sub Dimension Dropdown is 'Disabled'");
                Assert.IsFalse(addRadarCompetenciesPage.IsSubDimensionDropdownValidationMessageDisplayed(),
                    "Sub Dimension Dropdown Validation Message is being displayed");
                addRadarCompetenciesPage.ClickOnUpdateButton();
            }

            addRadarCompetenciesPage.ClickOnContinueToButton();

            // Add Questions
            Log.Info("Add Questions and click on 'Continue To' Button");
            foreach (var dimensionResponse in differentDimensionResponses)
            {
                addRadarQuestionsPage.ClickOnAddNewButton();
                Assert.IsFalse(addRadarQuestionsPage.IsSubDimensionDropdownEnabled(),
                    "Sub Dimension Dropdown is 'Enabled'");
                Assert.IsTrue(addRadarQuestionsPage.IsSubDimensionDropdownValidationMessageDisplayed(),
                    "Sub Dimension Dropdown Validation Message is not being displayed");
                Assert.IsFalse(addRadarQuestionsPage.IsCompetencyDropdownEnabled(), "Competency Dropdown is 'Enabled'");
                Assert.IsTrue(addRadarQuestionsPage.IsCompetencyDropdownValidationMessageDisplayed(),
                    "Competency Dropdown Validation Message is not being displayed");
                addRadarQuestionsPage.EnterQuestionInfo(dimensionResponse.RadarSubDimensions.FirstOrDefault()
                    ?.RadarCompetencies.FirstOrDefault()?.RadarQuestions.FirstOrDefault());
                Assert.IsTrue(addRadarQuestionsPage.IsSubDimensionDropdownEnabled(),
                    "Sub Dimension Dropdown is 'Disabled'");
                Assert.IsFalse(addRadarQuestionsPage.IsSubDimensionDropdownValidationMessageDisplayed(),
                    "Sub Dimension Dropdown Validation Message is being displayed");
                Assert.IsTrue(addRadarQuestionsPage.IsCompetencyDropdownEnabled(), "Competency Dropdown is 'Disabled'");
                Assert.IsFalse(addRadarQuestionsPage.IsCompetencyDropdownValidationMessageDisplayed(),
                    "Competency Dropdown Validation Message is being displayed");
                addRadarQuestionsPage.ClickOnUpdateButton();
            }

            addRadarQuestionsPage.ClickOnContinueToButton();

            // Add Open Ended Questions
            Log.Info("Add Open Ended Question and click on 'Continue To' Button");
            addRadarOpenEndedPage.ClickOnAddNewButton();
            addRadarOpenEndedPage.EnterOpenEndedInfo(FirstDimensionResponse.RadarSubDimensions.FirstOrDefault()
                ?.RadarCompetencies.FirstOrDefault()?.RadarQuestions.FirstOrDefault()?.RadarOpenEnded.FirstOrDefault());
            addRadarOpenEndedPage.ClickOnUpdateButton();

            addRadarOpenEndedPage.ClickOnAddNewButton();
            addRadarOpenEndedPage.EnterOpenEndedInfo(SecondDimensionResponse.RadarSubDimensions.FirstOrDefault()
                ?.RadarCompetencies.FirstOrDefault()?.RadarQuestions.FirstOrDefault()?.RadarOpenEnded.FirstOrDefault());
            addRadarOpenEndedPage.ClickOnUpdateButton();

            addRadarOpenEndedPage.ClickOnContinueToButton();

            Log.Info("Preview Assessment and verify all parameters are present");
            addRadarReviewAndFinishBasePage.SelectActionType(
                RadarReviewAndFinishBasePage.SelectAction.PreviewAssessment);
            Driver.SwitchToLastWindow();
            var actualDimensionList = previewAssessmentPage.GetDimensionList();
            Assert.That.ListsAreEqual(expectedRadarDimensionList, actualDimensionList, "'Dimensions' list not matched");

            foreach (var radarDimensionResponse in differentDimensionResponses)
            {
                var actualRadarParameters =
                    previewAssessmentPage.GetAssessmentDetailsForDimension(radarDimensionResponse.Name);

                var actualSubDimensionList = actualRadarParameters["SubDimensions"].ToList();
                var actualCompetencyList = actualRadarParameters["Competencies"].ToList();
                var actualQuestionList = actualRadarParameters["Questions"].ToList();

                var expectedSubDimensionList = radarDimensionResponse.RadarSubDimensions.Select(x => x.Name).ToList();
                var expectedCompetencyList = radarDimensionResponse.RadarSubDimensions
                    .SelectMany(a => a.RadarCompetencies)
                    .Select(w => w.Name).ToList();
                var expectedQuestionList = radarDimensionResponse.RadarSubDimensions
                    .SelectMany(a => a.RadarCompetencies)
                    .SelectMany(b => b.RadarQuestions).Select(c => c.QuestionText).ToList();
                
                Assert.That.ListsAreEqual(expectedSubDimensionList, actualSubDimensionList,
                    "SubDimension List doesn't match");
                Assert.That.ListsAreEqual(expectedCompetencyList, actualCompetencyList,
                    "Competency List doesn't match");
                Assert.That.ListsAreEqual(expectedQuestionList, actualQuestionList, "Question List doesn't match");
            }

            var actualOpenEndedList = previewAssessmentPage.GetOpenEndedList();
            Assert.That.ListsAreEqual(expectedRadarOpenEndedList, actualOpenEndedList, "Open Ended list not matched");

            //Remove unnecessary elements from Parameters List by DELETE 'Dimension' parameter and then verify it
            Log.Info("Remove 'Dimension' parameter then verify all Radar parameters.");
            addRadarReviewAndFinishBasePage.CloseCurrentWindowAndSwitchToFirstWindow();

            Log.Info("Delete 'Dimension' parameter from First Dimension ");
            addRadarReviewAndFinishBasePage.SelectWizardStep("Dimensions");
            addRadarDimensionsPage.ClickOnDeleteButton(expectedRadarDimensionList.FirstOrDefault());
            addRadarDimensionsPage.ClickOnDeleteConfirmationYesButton();

            Log.Info("Delete 'Sub-Dimension' parameter from Second Dimension ");
            addRadarReviewAndFinishBasePage.SelectWizardStep("Sub-Dimensions");
            addRadarDimensionsPage.ClickOnDeleteButton(SecondDimensionResponse.RadarSubDimensions.Select(x => x.Name)
                .ToList().FirstOrDefault());
            addRadarDimensionsPage.ClickOnDeleteConfirmationYesButton();

            Log.Info("Delete 'Competency' parameter from Third Dimension ");
            addRadarReviewAndFinishBasePage.SelectWizardStep("Competencies");
            addRadarDimensionsPage.ClickOnDeleteButton(ThirdDimensionResponse.RadarSubDimensions
                .SelectMany(q => q.RadarCompetencies).Select(s => s.Name).ToList().FirstOrDefault());
            addRadarDimensionsPage.ClickOnDeleteConfirmationYesButton();

            Log.Info("Delete 'Question' parameter from Forth Dimension ");
            addRadarReviewAndFinishBasePage.SelectWizardStep("Questions");
            addRadarDimensionsPage.ClickOnDeleteButton(ForthDimensionResponse.RadarSubDimensions
                .SelectMany(q => q.RadarCompetencies).SelectMany(w => w.RadarQuestions).Select(s => s.QuestionText)
                .ToList().FirstOrDefault());
            addRadarDimensionsPage.ClickOnDeleteConfirmationYesButton();

            Log.Info("Delete 'Open Ended' parameter from Second Dimension and Preview the Assessment");
            addRadarReviewAndFinishBasePage.SelectWizardStep("Open Ended");
            addRadarDimensionsPage.ClickOnDeleteButton(expectedRadarOpenEndedList.LastOrDefault());
            addRadarDimensionsPage.ClickOnDeleteConfirmationYesButton();

            Log.Info("Verify that expected parameters are present on Survey");
            addRadarReviewAndFinishBasePage.SelectWizardStep("Review & Finish");
            addRadarReviewAndFinishBasePage.SelectActionType(
                RadarReviewAndFinishBasePage.SelectAction.PreviewAssessment);
            Driver.SwitchToLastWindow();

            var finalExpectedRadarDimensionList = new List<string>() { expectedRadarDimensionList.LastOrDefault() };
            actualDimensionList = previewAssessmentPage.GetDimensionList();
            Assert.That.ListsAreEqual(finalExpectedRadarDimensionList, actualDimensionList,
                "Dimensions list not matched");

            var actualFinalRadarParameters =
                previewAssessmentPage.GetAssessmentDetailsForDimension(actualDimensionList.FirstOrDefault());

            var actualFinalSubDimensionList = actualFinalRadarParameters["SubDimensions"].ToList();
            var actualFinalCompetencyList = actualFinalRadarParameters["Competencies"].ToList();
            var actualFinalQuestionList = actualFinalRadarParameters["Questions"].ToList();
            
            var expectedFinalSubDimensionList = FifthDimensionResponse.RadarSubDimensions.Select(x => x.Name).ToList();
            var expectedFinalCompetencyList = FifthDimensionResponse.RadarSubDimensions
                .SelectMany(a => a.RadarCompetencies)
                .Select(w => w.Name).ToList();
            var expectedFinalQuestionList = FifthDimensionResponse.RadarSubDimensions
                .SelectMany(a => a.RadarCompetencies)
                .SelectMany(b => b.RadarQuestions).Select(c => c.QuestionText).ToList();
            
            Assert.That.ListsAreEqual(expectedFinalSubDimensionList, actualFinalSubDimensionList,
                "SubDimension List doesn't match");
            Assert.That.ListsAreEqual(expectedFinalCompetencyList, actualFinalCompetencyList,
                "Competency List doesn't match");
            Assert.That.ListsAreEqual(expectedFinalQuestionList, actualFinalQuestionList,
                "Question List doesn't match");

            var actualFinalOpenEndedList = previewAssessmentPage.GetOpenEndedList();
            var expectedFinalOpenEndedList = new List<string>() { expectedRadarOpenEndedList.FirstOrDefault() };
            Assert.That.ListsAreEqual(expectedFinalOpenEndedList, actualFinalOpenEndedList,
                "Open Ended list not matched");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _setupUi.DeleteRadar(RadarInfo.Name);
        }
    }
}

