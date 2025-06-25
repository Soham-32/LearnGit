using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.Batches.Campaign.Edit
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class BatchAssessmentEditFacilitatorColumnAndTalentDevelopmentTests : BaseTest
    {
        private static int _assessmentId;
        private static SetupTeardownApi _setupApi;
        private static int _teamId;
        private static bool _classInitFailed;

        public static BatchAssessment BatchAssessment = new BatchAssessment
        {
            BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            AssessmentType = SharedConstants.TeamAssessmentType,
            TeamAssessments = new List<TeamAssessmentInfo>
            {
                new TeamAssessmentInfo
                {
                    TeamName = SharedConstants.TeamForBatchAssessment
                }
            },
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30)
        };

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.CreateTeamBatchAssessment(BatchAssessment);
                _setupApi = new SetupTeardownApi(TestEnvironment);
                _teamId = _setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.TeamForBatchAssessment).TeamId;
                _assessmentId = _setupApi.GetAssessmentResponse(Constants.TeamForBatchAssessment, BatchAssessment.AssessmentName).Result.AssessmentId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug id : 48906 
        [TestCategory("CompanyAdmin")]
        public void BatchAssessment_Edit_Verify_FacilitatorColumn_WorkType_Navigation()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            var talentDevelopmentAssessmentList = new List<string>
            {
                "Agile Team Coach V2.0",
                "EBAS 360 v2",
                "Leadership Agility V2.0",
                "Product Owner 360 V2_0",
                "Release Train Engineer (RTE) Radar",
                "Scrum Master  3.0"
            };

            Log.Info("Login into the Application");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Batch Dashboard page");
            batchesTabPage.NavigateToPage(Company.Id);

            Log.Info($"Search {BatchAssessment.BatchName} Batch on batch Assessment dashboard and Click on it's Edit button");
            batchesTabPage.SearchBatchName(BatchAssessment.BatchName);
            batchesTabPage.ClickBatchEditButton(BatchAssessment.BatchName);

            Log.Info("Verify 'Facilitator' column is present in Batch Assessment Popup Column Header List ");
            var batchPopupColumnHeaderList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupColumnHeadersValueList();
            Assert.That.ListContains(batchPopupColumnHeaderList, "Facilitator(s)", "Facilitator(s) column is not displayed");

            Log.Info("Verify 'Group of Individual' work Type in 'WorkType' column values List ");
            var workTypeColumnList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupColumnValueList("Work Type");
            Assert.That.ListNotContains(workTypeColumnList, "Group Of Individuals", "'Group Of Individuals' work type is present in 'WorkType' column values List");

            Log.Info("Verify 'Filter By Work Type' List ");
            var filterByWorkTypeList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupWorkTypeValueList();
            Assert.That.ListNotContains(filterByWorkTypeList, "Group Of Individuals", "List contains Group of Individuals");

            Log.Info("Verify 'Group of Individual' Assessment Type in 'Assessment Type' Dropdown ");
            var actualAssessmentTypeList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupAssessmentTypeValueList();
            foreach (var assessment in talentDevelopmentAssessmentList)
            {
                Assert.That.ListNotContains(actualAssessmentTypeList, assessment, "'Group of Individual' Assessment type is present in 'Assessment Type' Dropdown ");
            }

            Log.Info($"Verify 'Edit assessment' page URL after clicking on the '{Constants.TeamForBatchAssessment}' team ");
            Thread.Sleep(180000); // Time required for Batch Assessment to switch from Draft to Active State
            batchesTabPage.ClickOnBatchTeam(Constants.TeamForBatchAssessment);
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/teams/{_teamId}/assessments/{_assessmentId}/edit", Driver.GetCurrentUrl(), "'Edit Team Assessment' page URL didn't match");
        }
    }
}
