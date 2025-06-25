using System;
using System.Collections.Generic;
using System.Globalization;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.Batches
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class BatchesDashboardEditTests : BaseTest
    {
        public static BatchAssessment BatchAssessment = new BatchAssessment
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
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today.AddDays(30)
        };

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            // add an assessment
            var setup = new SetUpMethods(testContext, TestEnvironment);
            setup.ScheduleTeamBatchAssessment(BatchAssessment);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BatchesDashboard_EditTeamBatchAssessment()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

            batchesTabPage.SearchBatchName(BatchAssessment.BatchName);

            batchesTabPage.ClickBatchEditButton(BatchAssessment.BatchName);

            var batchAssessmentEdited = new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = SharedConstants.Team
                    },
                    new TeamAssessmentInfo
                    {
                        TeamName = Constants.TeamForBatchAssessment
                    }
                },
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(14)
            };

            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessmentEdited);
            
            createBatchAssessmentPopupPage.ClickScheduleButton();

            Assert.IsTrue(createBatchAssessmentPopupPage.IsScheduleConfirmationPopupDisplayed(),
                "Confirmation popup should display on popup");
            Assert.AreEqual(batchAssessmentEdited.AssessmentType, createBatchAssessmentPopupPage.GetSchedulePopupSurveyType(),
                "Assessment Type should be shown on popup");
            Assert.AreEqual(batchAssessmentEdited.TeamAssessments.Count, createBatchAssessmentPopupPage.GetSchedulePopupSelectedTeam(),
                "Total selected team should be shown correctly on popup");
            Assert.AreEqual(batchAssessmentEdited.StartDate.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                createBatchAssessmentPopupPage.GetPopupStartDate(), "Start Date should be shown correctly on popup");

            createBatchAssessmentPopupPage.ClickScheduleYesProceedButton();

            Assert.AreEqual("Batch process has successfully been completed.",
                createBatchAssessmentPopupPage.GetSuccessPopupText(), "Success popup should be shown correctly");

            createBatchAssessmentPopupPage.ClickOkButton();

            batchesTabPage.SearchBatchName(batchAssessmentEdited.BatchName);

            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessmentEdited.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
            Assert.AreEqual(batchAssessmentEdited.AssessmentName, actualBatchAssessment.AssessmentName, "Assessment name should be shown correctly");
            Assert.AreEqual(batchAssessmentEdited.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), actualBatchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), "Start Date should be shown correctly");
            Assert.AreEqual(batchAssessmentEdited.AssessmentType, actualBatchAssessment.AssessmentType, "Assessment type should be shown correctly");


        }
    }
}
