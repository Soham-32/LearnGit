using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Facilitator;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Facilitator
{
    [TestClass]
    [TestCategory("Dashboard"), TestCategory("FacilitatorDashboard")]
    public class FacilitatorDashboardTests : BaseTest
    {
        private static readonly User Facilitator = TestEnvironment.UserConfig.GetUserByDescription("facilitator");
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = "FacilitatorDashboard1",
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            Facilitator = Facilitator.FullName
        };

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_FacilitatorDisplays()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.ClickFacilitatorDashboard();
            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);

            Assert.IsTrue(facilitatorDashboard.IsFacilitatorDisplayed(TeamAssessment.Facilitator),
                $"Facilitator <{TeamAssessment.Facilitator}> is not displayed on the Facilitator Dashboard");
            
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_AssessmentDisplays()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.ClickFacilitatorDashboard();
            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);

            facilitatorDashboard.ExpandFacilitator(TeamAssessment.Facilitator);

            Assert.IsTrue(facilitatorDashboard.IsAssessmentDisplayed(TeamAssessment.Facilitator, TeamAssessment.AssessmentName),
                $"Assessment <{TeamAssessment.AssessmentName}> was not found under facilitator <{TeamAssessment.Facilitator}>");

        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);
            
            const string fileName = "Facilitator Feedback.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.ClickFacilitatorDashboard();
            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);

            facilitatorDashboard.ClickExportToExcel();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);
            
            var facilitatorColumns = new List<string>
                {"Facilitator Name", "Email", "Facilitator Avg", "Retrospective Avg", "No of Assessmemts", "Date of Last Assessment"};
            var assessmentColumns = new List<string>
                { "", "Assessment Name", "End Date", "Facilitation Date", "Facilitator Rating", "Retrospective Rating", "", "No Of Participants", "", "No Of Responses" };

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            for (var i = 0; i < facilitatorColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{facilitatorColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(facilitatorColumns[i], actualColumns[i], "Column header text doesn't match");
            }
            var expectedRows = facilitatorDashboard.GetDashboardDataInExcelFormat(facilitatorColumns, assessmentColumns);

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();
              
                // compare them
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"Row {i}, Column {j} value doesn't match");
                }
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_Facilitator_ActiveInactiveToggle()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);

            var user2 = TestEnvironment.UserConfig.GetUserByDescription("user 2");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            teamDashboardPage.ClickFacilitatorDashboard();
            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(false);

            Assert.IsTrue(facilitatorDashboard.IsFacilitatorDisplayed(user2.FullName),
                $"Facilitator <{user2.FullName}> is not displayed on Inactive Facilitator Dashboard");

            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);

            Assert.IsFalse(facilitatorDashboard.IsFacilitatorDisplayed(user2.FullName),
                $"Facilitator <{user2.FullName}> is displayed on Active Facilitator Dashboard");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_Facilitator_AhTrainer_Access()
        {

            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);
            var user7 = TestEnvironment.UserConfig.GetUserByDescription("user 7");

            login.NavigateToPage();
            login.LoginToApplication(user7.Username, user7.Password);

            teamDashboardPage.ClickFacilitatorDashboard();

            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);
            Assert.IsTrue(facilitatorDashboard.IsFacilitatorDisplayed(TeamAssessment.Facilitator),
            $"Facilitator <{TeamAssessment.Facilitator}> is not displayed on Active Facilitator Dashboard");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_Facilitator_AhTrainer_Denied()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            var user4 = TestEnvironment.UserConfig.GetUserByDescription("user 4");

            login.NavigateToPage();
            login.LoginToApplication(user4.Username, user4.Password);

            Assert.IsFalse(teamDashboardPage.IsFacilitatorDashboardDisplayed(),"Facilitator dashboard is displayed");
        }
    }
}
