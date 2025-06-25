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
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraDataCenterIntegration;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraDataCenter
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraDataCenterIntegrationDataPullTests : BaseTest
    {
        //readonly string 
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static bool _classInitFailed;
        private static SetUpMethods _setupUi;
        private static readonly string JiraCenterInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraDataCenterIntegrationInfo.json");
        private static readonly JiraDataCenterIntegration JiraDataCenter = JsonConvert.DeserializeObject<JiraDataCenterIntegration>(JiraCenterInfo);

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
                _setupUi.JiraIntegrationLinkTeamToBoard(Constants.PlatformJiraDataCenter, JiraDataCenter.JiraBoard, JiraDataCenter.JiraProjectName, _teamResponse, Company.Id);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyTheIterationDataIsPulledFromJiraDataCenter()
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
            Assert.AreEqual(JiraDataCenter.IterationData.Name, actualIteration.Name, "Name doesn't match");
            Assert.AreEqual(JiraDataCenter.IterationData.From, actualIteration.From, "From doesn't match");
            Assert.AreEqual(JiraDataCenter.IterationData.To, actualIteration.To, "To doesn't match");
            Assert.AreEqual(JiraDataCenter.IterationData.CompletedPoints, actualIteration.CompletedPoints, "Completed Points doesn't match");
            Assert.AreEqual(JiraDataCenter.IterationData.Defects, actualIteration.Defects, "Defects doesn't match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyTheReleaseDataIsPulledFromJiraDataCenter()
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
            Assert.AreEqual(JiraDataCenter.ReleaseData.Name, actualRelease.Name, "Name doesn't match");
            Assert.AreEqual(JiraDataCenter.ReleaseData.TargetDate, actualRelease.TargetDate, "Target Date doesn't match");
            Assert.AreEqual(JiraDataCenter.ReleaseData.ActualDate, actualRelease.ActualDate, "Actual Date doesn't match");
            Assert.AreEqual(JiraDataCenter.ReleaseData.Defects, actualRelease.Defects, "Defects doesn't match");

        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            _setupUi.JiraIntegrationUnlinkJiraBoard(Constants.PlatformJiraDataCenter, Company.Id, JiraDataCenter.JiraBoard, _teamResponse.Name);
        }
    }
}