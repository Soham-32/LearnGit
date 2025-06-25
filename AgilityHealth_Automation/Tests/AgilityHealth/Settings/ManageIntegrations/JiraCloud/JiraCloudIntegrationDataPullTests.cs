using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Newtonsoft.Json;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraCloudIntegration;
using System.Globalization;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraCloud
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraCloudIntegrationDataPullTests : BaseTest
    {
        //readonly string 
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static bool _classInitFailed;
        private static SetUpMethods _setupUi;
        private static readonly string JiraCloudInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraCloudIntegrationInfo.json");
        private static readonly JiraCloudIntegration JiraCloudData = JsonConvert.DeserializeObject<JiraCloudIntegration>(JiraCloudInfo);

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("TestIntegration");
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamResponse.TeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _setupUi = new SetUpMethods(testContext, TestEnvironment);
                _setupUi.JiraIntegrationLinkTeamToBoard(Constants.PlatformJiraCloud, JiraCloudData.JiraBoard, JiraCloudData.JiraProjectName, _teamResponse, Company.Id);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyTheIterationDataIsPulledFromJiraCloud()
        {
            VerifySetup(_classInitFailed);

            var editTeamMetricsPage = new EditMetricsBasePage(Driver, Log);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            editTeamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            Driver.RefreshPage();

            Log.Info("Click on the 'Get Data' button and verify that the 'Iteration Data' is pulled correctly from the 'Jira Board'.");
            editTeamMetricsPage.ClickOnGetDataButton();
            Driver.RefreshPage();
            var actualIteration = editTeamMetricsPage.GetIterationDataFromGrid(1);
            Assert.AreEqual(JiraCloudData.IterationData.Name, actualIteration.Name, "Name doesn't match");
            Assert.AreEqual(JiraCloudData.IterationData.From, actualIteration.From, "From doesn't match");
            Assert.AreEqual(JiraCloudData.IterationData.To, actualIteration.To, "To doesn't match");
            Assert.AreEqual(JiraCloudData.IterationData.CompletedPoints, actualIteration.CompletedPoints, "Completed Points doesn't match");
            Assert.AreEqual(JiraCloudData.IterationData.Defects, actualIteration.Defects, "Defects doesn't match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyTheReleaseDataIsPulledFromJiraCloud()
        {
            VerifySetup(_classInitFailed);

            var editTeamMetricsPage = new EditMetricsBasePage(Driver, Log);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            editTeamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            Driver.RefreshPage();

            Log.Info("Click on the 'Get Data' button and verify that the 'Release Data' is pulled correctly from the 'Jira Board'.");
            editTeamMetricsPage.ClickOnGetDataButton();
            Driver.RefreshPage();
            var actualRelease = editTeamMetricsPage.GetReleaseDataFromGrid(1);
            Assert.AreEqual(JiraCloudData.ReleaseData.Name, actualRelease.Name, "Name doesn't match");
            Assert.AreEqual(JiraCloudData.ReleaseData.TargetDate, actualRelease.TargetDate, "Target Date doesn't match");
            Assert.AreEqual(JiraCloudData.ReleaseData.ActualDate, actualRelease.ActualDate, "Actual Date doesn't match");
            Assert.AreEqual(JiraCloudData.ReleaseData.Defects, actualRelease.Defects, "Defects doesn't match");

        }

        //[TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyThePerformanceMeasurementsAreCalculatedCorrectly()
        {
            VerifySetup(_classInitFailed);

            var editTeamMetricsPage = new EditMetricsBasePage(Driver, Log);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Click on the 'Get Data' button and verify that the Performance Measurements table display the data for the calculated Metrics.");
            editTeamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            Driver.RefreshPage();
            editTeamMetricsPage.ClickOnGetDataButton();
            Driver.RefreshPage();
            var expectedDate = DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var actualDate = editTeamMetricsPage.GetPerformanceMeasurementsCalculationDate();
            Assert.AreEqual(expectedDate, actualDate, "The displayed date is incorrect.");

            Log.Info("Verify that the Performance Measurements Calculated and Normalized values for the Metrics are correct.");
            var actualPerformanceMeasurements = editTeamMetricsPage.GetPerformanceMeasurementsDataFromGrid();
            //Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.CalculatedFeatureThroughput, actualPerformanceMeasurements.CalculatedFeatureThroughput);
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.CalculatedFeatureCycleTime, actualPerformanceMeasurements.CalculatedFeatureCycleTime, "The calculated 'Feature Cycle Time' is incorrect.'");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.CalculatedDeploymentFrequency, actualPerformanceMeasurements.CalculatedDeploymentFrequency, "The calculated 'Deployment Frequency' is incorrect.'");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.CalculatedDefectRatio, actualPerformanceMeasurements.CalculatedDefectRatio, "The calculated 'Defect Ratio' is incorrect.'");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.CalculatedPredictability, actualPerformanceMeasurements.CalculatedPredictability, "The calculated 'Predictability' is incorrect.'");
            //Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.NormalizedFeatureThroughput, actualPerformanceMeasurements.NormalizedFeatureThroughput);
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.NormalizedFeatureCycleTime, actualPerformanceMeasurements.NormalizedFeatureCycleTime, "The normalized 'Feature Cycle Time' is incorrect.'");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.NormalizedDeploymentFrequency, actualPerformanceMeasurements.NormalizedDeploymentFrequency, "The normalized 'Deployment Frequency' is incorrect.'");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.NormalizedDefectRatio, actualPerformanceMeasurements.NormalizedDefectRatio, "The normalized 'Defect Ratio' is incorrect.'");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.NormalizedPredictability, actualPerformanceMeasurements.NormalizedPredictability, "The normalized 'Predictability' is incorrect.'");

        }

        //[TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyThePerformanceMeasurementsAreRecalculatedCorrectly()
        {
            VerifySetup(_classInitFailed);

            var editTeamMetricsPage = new EditMetricsBasePage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Click on the 'Get Data' button and verify that the 'Recalculate Metrics' button is not displayed on the 'Metrics' tab.");
            editTeamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            editTeamMetricsPage.ClickOnGetDataButton();
            Driver.RefreshPage();
            editTeamMetricsPage.NavigateToPage(_teamResponse.TeamId);
            Assert.IsFalse(editTeamMetricsPage.IsRecalculateMetricsButtonDisplayed(), "The 'Recalculate metrics' button is displayed.");

            Log.Info("Edit the Iterations data and verify that the 'Recalculate Metrics' button is displayed on the 'Metrics' tab.");
            //Edited Iteration Data
            JiraCloudData.IterationData.Name = "Iteration test edited";
            JiraCloudData.IterationData.CommittedPoints = "11";
            JiraCloudData.IterationData.CompletedPoints = "6";
            JiraCloudData.IterationData.Defects = "21";
            JiraCloudData.IterationData.TotalScope = "8";
            editTeamMetricsPage.ClickIterationDataEditButton(1);
            editTeamMetricsPage.EnterIterationData(JiraCloudData.IterationData);
            Assert.IsTrue(editTeamMetricsPage.IsRecalculateMetricsButtonDisplayed(), "The 'Recalculate metrics' button is not displayed.");

            Log.Info("Click on the 'Recalculate Metrics' button and verify that the Performance Measurements are recalculated.");
            editTeamMetricsPage.ClickOnRecalculateMetricsButton();
            Driver.RefreshPage();
            var actualPerformanceMeasurements = editTeamMetricsPage.GetPerformanceMeasurementsDataFromGrid();
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.RecalculatedDefectRatio, actualPerformanceMeasurements.CalculatedDefectRatio, "The 'Defect Ratio' is not recalculated correctly.");
            Assert.AreEqual(JiraCloudData.PerformanceMeasurementsData.RecalculatedPredictability, actualPerformanceMeasurements.CalculatedPredictability, "The 'Predictability' is not recalculated correctly.");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            _setupUi.JiraIntegrationUnlinkJiraBoard(Constants.PlatformJiraCloud, Company.Id, JiraCloudData.JiraBoard, _teamResponse.Name);
        }
    }
}