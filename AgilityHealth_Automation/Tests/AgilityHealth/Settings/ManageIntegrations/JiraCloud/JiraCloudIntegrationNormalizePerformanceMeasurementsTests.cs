using AgilityHealth_Automation.Base;
using AtCommon.Api;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraCloudIntegration;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using AtCommon.Dtos.Companies;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.Utilities;
using System.Globalization;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraCloud
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraCloudIntegrationNormalizePerformanceMeasurementsTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static bool _classInitFailed;
        private static SetUpMethods _setupUi;
        private static readonly string JiraCloudInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraCloudIntegrationInfo.json");
        private static readonly JiraCloudIntegration JiraCloudData = JsonConvert.DeserializeObject<JiraCloudIntegration>(JiraCloudInfo);

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("TestIntegration");
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamResponse.TeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _setupUi = new SetUpMethods(_, TestEnvironment);
                _setupUi.JiraIntegrationLinkTeamToBoard(Constants.PlatformJiraCloud, JiraCloudData.JiraBoard, JiraCloudData.JiraProjectName, _teamResponse, Company.Id);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        //[TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyThePerformanceMeasurementsAreNormalizedCorrectly()
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
            var expectedPerformanceMeasurementsNormalized = editTeamMetricsPage.NormalizedPerformanceMeasurementsCalculations(actualPerformanceMeasurements);

            Assert.AreEqual(expectedPerformanceMeasurementsNormalized.NormalizedDeploymentFrequency, actualPerformanceMeasurements.NormalizedDeploymentFrequency, "The normalized 'Deployment Frequency' is incorrect.");
            Assert.AreEqual(expectedPerformanceMeasurementsNormalized.NormalizedDefectRatio, actualPerformanceMeasurements.NormalizedDefectRatio, "The normalized 'Defect Ratio' is incorrect.");
            Assert.AreEqual(expectedPerformanceMeasurementsNormalized.NormalizedPredictability, actualPerformanceMeasurements.NormalizedPredictability, "The normalized 'Predictability' is incorrect.");
            Assert.AreEqual(expectedPerformanceMeasurementsNormalized.NormalizedFeatureCycleTime, actualPerformanceMeasurements.NormalizedFeatureCycleTime, "The normalized 'FeatureCycleTime' is incorrect.");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            _setupUi.JiraIntegrationUnlinkJiraBoard(Constants.PlatformJiraCloud, Company.Id, JiraCloudData.JiraBoard, _teamResponse.Name);
        }
    }
}