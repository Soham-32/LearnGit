using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Scheduler;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.Scheduler
{
    
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class SchedulerDashboardTests : BaseTest
    {
        private static bool _classInitFailed;
        private static string _classInitFailedMessage;
        private static TeamResponse _teamResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _) 
        {
            try
            {
                _teamResponse = new SetupTeardownApi(TestEnvironment).GetTeamResponse(SharedConstants.Team);
            }
            catch (Exception e)
            {
                _classInitFailed = true;
                _classInitFailedMessage = e.ToLogString(e.StackTrace);
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void SchedulerDashboard_SendBatchAssessment()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var schedulerTabPage = new SchedulerTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.SchedulerTab);

            schedulerTabPage.ClickSendBatchAssessment();

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
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(30)
            };
            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);
            createBatchAssessmentPopupPage.ClickScheduleButton();

            Assert.IsTrue(createBatchAssessmentPopupPage.IsScheduleConfirmationPopupDisplayed(),
                "Confirmation popup should display on popup");
            Assert.AreEqual(batchAssessment.AssessmentType, createBatchAssessmentPopupPage.GetSchedulePopupSurveyType(),
                "Assessment Type should be shown on popup");
            Assert.AreEqual(1, createBatchAssessmentPopupPage.GetSchedulePopupSelectedTeam(),
                "Total selected team should be shown correctly on popup");
            Assert.AreEqual(batchAssessment.StartDate.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                createBatchAssessmentPopupPage.GetPopupStartDate(), "Start Date should be shown correctly on popup");

            createBatchAssessmentPopupPage.ClickScheduleYesProceedButton();

            Assert.AreEqual("Batch process has successfully been completed.",
                createBatchAssessmentPopupPage.GetSuccessPopupText(), "Success popup should be shown correctly");

            createBatchAssessmentPopupPage.ClickOkButton();

            batchesTabPage.NavigateToPage(Company.Id);
            batchesTabPage.SearchBatchName(batchAssessment.BatchName);

            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessment.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentName, actualBatchAssessment.AssessmentName, "Assessment name should be shown correctly");
            Assert.AreEqual(batchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), actualBatchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), "Start Date should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentType, actualBatchAssessment.AssessmentType, "Assessment type should be shown correctly");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void SchedulerDashboard_CreateDraftAssessment()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var schedulerTabPage = new SchedulerTabPage(Driver, Log);
            var createDraftAssessmentPopup = new CreateDraftAssessmentPopupPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var reviewAndLaunchPage = new ReviewAndLaunchPage(Driver, Log);

            var facilitator = TestEnvironment.UserConfig.GetUserByDescription("facilitator");

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashBoardPage.GetTeamIdFromLink(_teamResponse.Name).ToInt();
            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.SchedulerTab);


            schedulerTabPage.ClickCreateDraftAssessment();
            
            var teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = $"Scheduled{RandomDataUtil.GetAssessmentName()}",
                TeamName = _teamResponse.Name,
                Facilitator = facilitator.FullName,
                FindFacilitator = false,
                FacilitationDate = DateTime.Today,
                FacilitationDuration = 2,
                Location = "On Mars",
                TeamMembers = _teamResponse.Members.Select(m => m.Email).ToList(),
                StakeHolders = _teamResponse.Stakeholders.Select(s => s.Email).ToList()
            };

            createDraftAssessmentPopup.EnterAssessmentInfo(teamAssessment);
            teamAssessment.StartDate = DateTime.UtcNow.ToLocalTime();
            teamAssessment.EndDate = teamAssessment.StartDate.AddDays(7);
            createDraftAssessmentPopup.ClickCreateDraftAssessmentButton();

            teamAssessmentDashboard.NavigateToPage(teamId);
            teamAssessmentDashboard.ClickOnRadar(teamAssessment.AssessmentName);

            var actualProfile = reviewAndLaunchPage.GetAssessmentProfile();

            Assert.AreEqual(teamAssessment.AssessmentType, actualProfile.AssessmentType, "Assessment Type doesn't match.");
            Assert.AreEqual(teamAssessment.AssessmentName, actualProfile.AssessmentName, "Assessment Name doesn't match.");
            Assert.AreEqual(teamAssessment.Facilitator, actualProfile.Facilitator, "Facilitator doesn't match.");
            Assert.AreEqual(teamAssessment.FacilitationDate, actualProfile.FacilitationDate, "Facilitation Date doesn't match.");
            Assert.AreEqual(teamAssessment.FacilitationDuration, actualProfile.FacilitationDuration, "Facilitation Duration doesn't match.");
            Assert.AreEqual(teamAssessment.Location, actualProfile.Location, "Location doesn't match.");

            Assert.That.ListsAreEqual(teamAssessment.TeamMembers.ToList(), reviewAndLaunchPage.GetTeamMembers().Values.ToList(),
                "Team Members list does not match.");

            Assert.That.ListsAreEqual(teamAssessment.StakeHolders.ToList(), reviewAndLaunchPage.GetStakeholders().Values.ToList(),
                "Stakeholders list does not match.");

            Assert.IsTrue(reviewAndLaunchPage.IsSendToEveryoneRadioButtonSelected(), 
                "'Send To Everyone' radiobutton should be selected.");
            Assert.That.TimeIsClose(teamAssessment.StartDate, reviewAndLaunchPage.GetAssessmentStartDate(),8,
                message:"'Assessment Start' doesn't match.");
            Assert.That.TimeIsClose(teamAssessment.EndDate, reviewAndLaunchPage.GetAssessmentEndDate(),8,
                message:"'Assessment End' doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void SchedulerDashboard_CreateDraftAssessment_FindFacilitator()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var schedulerTabPage = new SchedulerTabPage(Driver, Log);
            var createDraftAssessmentPopup = new CreateDraftAssessmentPopupPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var reviewAndLaunchPage = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashBoardPage.GetTeamIdFromLink(_teamResponse.Name).ToInt();
            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.SchedulerTab);

            schedulerTabPage.ClickCreateDraftAssessment();

            var teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = $"Scheduled{RandomDataUtil.GetAssessmentName()}",
                TeamName = _teamResponse.Name,
                FindFacilitator = true,
                FacilitationDate = DateTime.Today,
                FacilitationDuration = 2,
                Location = "On Mars"
            };

            createDraftAssessmentPopup.EnterAssessmentInfo(teamAssessment);

            createDraftAssessmentPopup.ClickCreateDraftAssessmentButton();

            teamAssessmentDashboard.NavigateToPage(teamId);
            teamAssessmentDashboard.ClickOnRadar(teamAssessment.AssessmentName);

            Assert.IsTrue(reviewAndLaunchPage.IsFindFacilitatorCheckboxChecked(), 
                "'Find a facilitator' checkbox is not checked.");
        }
    }
}