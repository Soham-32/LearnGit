using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.Batches.Campaign.Create
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class BatchAssessmentCreateFacilitatorColumnAndTalentDevelopmentTests : BaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug id : 48906
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BatchAssessment_Create_Verify_FacilitatorColumn_And_WorkType()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);
            var batchAssessment = new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = Constants.TeamForBatchAssessment
                    }
                },
                StartDate = DateTime.Today.AddDays(0),
                EndDate = DateTime.Today.AddDays(30)
            };

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
            dashBoardPage.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

            Log.Info("Verify 'Group of Individual' Assessment Type in 'Assessment Type' Dropdown on batches dashboard page");
            var actualAssessmentTypeList = batchesTabPage.GetAllAssessmentTypes();
            foreach (var assessment in talentDevelopmentAssessmentList)
            {
                Assert.That.ListNotContains(actualAssessmentTypeList, assessment, "'Group of Individual' Assessment type is present in 'Assessment Type' Dropdown ");
            }

            Log.Info("Click on '+' icon and Enter values for create Batch Assessment ");
            batchesTabPage.ClickPlusButton();
            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);

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
            actualAssessmentTypeList = createBatchAssessmentPopupPage.GetCreateEditBatchAssessmentPopupAssessmentTypeValueList();
            foreach (var assessment in talentDevelopmentAssessmentList)
            {
                Assert.That.ListNotContains(actualAssessmentTypeList, assessment, "'Group of Individual' Assessment type is present in 'Assessment Type' Dropdown ");
            }

            Log.Info("Click on 'Send and Launch Now' button to create a Batch Assessment");
            createBatchAssessmentPopupPage.ClickSendAndLaunchNowButton();
            createBatchAssessmentPopupPage.ClickOnCreateEditBatchAssessmentPopupYesProceedButton();
            createBatchAssessmentPopupPage.ClickOkButton();

            Log.Info("Search Batch name and verify that created batch is showing correctly in grid");
            batchesTabPage.SearchBatchName(batchAssessment.BatchName);
            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessment.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
        }
    }
}
