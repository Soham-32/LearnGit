using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_FilterByTeamType()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.FilterTeamType("Team");

            Assert.IsTrue(dashboardPage.CheckTeamFilterCorrectly("Team"),
                "Failure !! Filtered data displays incorrectly for Team");

            dashboardPage.FilterTeamType("Multi-Team");

            Assert.IsTrue(dashboardPage.CheckTeamFilterCorrectly("Multi-Team"),
                "Failure !! Filtered data displays incorrectly for Multi-Team");

            dashboardPage.FilterTeamType("Enterprise Team");

            Assert.IsTrue(dashboardPage.CheckTeamFilterCorrectly("Enterprise Team"),
                "Failure !! Filtered data displays incorrectly for Enterprise Team");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_DateRangeFilter()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            var startDate = "11/11/2015";
            dashboardPage.FilterTeamDate(startDate, null);

            Assert.IsTrue(dashboardPage.CheckDateFilterCorrectly(startDate, null),
                "Failure !! Filtered data displays incorrectly for only filter by Start Date");

            var today = DateTime.Now.ToString("MM/d/yyyy", CultureInfo.InvariantCulture);
            dashboardPage.FilterTeamDate(null, today);

            Assert.IsTrue(dashboardPage.CheckDateFilterCorrectly(null, today),
                "Failure !! Filtered data displays incorrectly for only filter by End Date");

            dashboardPage.FilterTeamDate(startDate, today);

            Assert.IsTrue(dashboardPage.CheckDateFilterCorrectly(startDate, today),
                "Failure !! Filtered data displays incorrectly for filter by Start Date and End Date");
        }


        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48327
        [DoNotParallelize]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ExportToExcel_All()
        {
            TeamDashboardExcelValidator("All");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48327
        [DoNotParallelize]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ExportToExcel_Team()
        {
            TeamDashboardExcelValidator("Team");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48327
        [DoNotParallelize]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ExportToExcel_MultiTeam()
        {
            TeamDashboardExcelValidator("Multi-Team");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48327
        [DoNotParallelize]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_ExportToExcel_EnterpriseTeam()
        {
            TeamDashboardExcelValidator("Enterprise Team");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_ImageView_FilterFunctionality()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GoToSwimLaneView();
            dashboardPage.ClickFilter();

            dashboardPage.ClickClearAllLink();
            dashboardPage.ClickFilter();

            Assert.AreEqual(0, dashboardPage.TotalSwimlane(), "All swimlanes are not cleared");

            dashboardPage.ClickFilter();
            dashboardPage.ClickSelectAllLink();
            dashboardPage.ClickFilter();

            Assert.IsTrue(dashboardPage.TotalSwimlane() > 0, "Failure !! Select all function doesn't work properly");

            dashboardPage.ClickFilter();
            dashboardPage.SelectFilterItem("Program Team");
            dashboardPage.ClickFilter();

            Assert.IsFalse(dashboardPage.DoesSwimlaneItemDisplay("Program Team"),
                "Failure !! Selected item doesn't display properly");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamDashboard_GridView_SearchByKeywordAndResetButton()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.SearchTeam("Normal");

            Assert.AreEqual(1, dashboardPage.TotalTeam(), "Failure !! search by doesn't work properly");

            dashboardPage.ResetGridView();

            Assert.IsTrue(dashboardPage.TotalTeam() > 1, "Failure !! Reset button doesn't work properly");

            Assert.IsTrue(string.IsNullOrEmpty(dashboardPage.GetFilterText()),
                "Failure !! Reset button doesn't work properly. Search box is not cleared");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("Member")]
        public void TeamDashboard_VerifySwimeLanesExpandAllCollapseAllFunctionality()
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.MoveToViewButton();

            Assert.IsTrue(dashboardPage.DoesViewItemExist("Grid"), "Grid option should be shown in View");
            Assert.IsTrue(dashboardPage.DoesViewItemExist("Swim Lanes"), "Swim Lanes option should be shown in View");

            dashboardPage.GoToSwimLaneView();
            dashboardPage.MoveToLogo();

            foreach (var et in dashboardPage.EtLaneColorsList())
            {
                Assert.AreEqual("#38B348", et, "Color for Enterprise should be correct");
            }

            foreach (var et in dashboardPage.MtLaneColorsList())
            {
                Assert.AreEqual("#FE9A2E", et, "Color for Multi should be correct");
            }

            foreach (var et in dashboardPage.TeamLaneColorsList())
            {
                Assert.AreEqual("#52AED3", et, "Color for Team should be correct");
            }
            // members can't see archived teams
            if (User.Type != UserType.Member)
            {
                dashboardPage.MoveToStateButton();

                Assert.IsTrue(dashboardPage.DoesStateItemExist("Active"), "Active option should be shown in State");
                Assert.IsTrue(dashboardPage.DoesStateItemExist("Archived"), "Archived option should be shown in State");
            }
            dashboardPage.ExpandAllSwimLane();

            Assert.IsTrue(dashboardPage.IsExpandAllWorkingProperly(), "Expand All should work properly");

            dashboardPage.CollapseAllSwimLane();

            Assert.IsTrue(dashboardPage.IsCollapseAllWorkingProperly(), "Collapse All should work properly");
        }

        private void TeamDashboardExcelValidator(string teamType)
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

            FileUtil.DeleteFilesInDownloadFolder("TeamList.xlsx");

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.GridTeamView();

            dashboardPage.FilterTeamType(teamType);

            //  Some columns are hidden based on the size of the window. so we need get the names of the visible columns
            var expectedColumns = dashboardPage.GetVisibleColumnNamesFromGrid();

            if (teamType == "Team")
            {
                Assert.IsFalse(expectedColumns.Contains("Number of Sub Teams"),
                    "'Number of Sub Teams' column should not display when filtered by 'Team'");
            }
            else if (teamType != "All")
            {
                Assert.IsFalse(expectedColumns.Contains("Number of Assessments"),
                    "'Number of Assessments' column should not display when filtered by 'Enterprise Team'");
                Assert.IsFalse(expectedColumns.Contains("Last Date of Assessment"),
                    "'Last Date of Assessment' column should not display when filtered by 'Enterprise Team'");
            }

            dashboardPage.ClickExportToExcel();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded("TeamList.xlsx");

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            // some times more columns may show up after the export, so you have to get the rows before the export
            var expectedRows = new List<List<string>>();
            for (var i = 1; i <= dashboardPage.GetNumberOfGridRows(); i++)
            {
                // get the expected values from the grid
                var row = expectedColumns.Select(column => dashboardPage.GetCellValue(i, column)).ToList();

                expectedRows.Add(row);
            }

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");
            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }


            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                // compare the row from the application to the row from the spreadsheet
                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Columns count doesn't match");
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"{j}th row text doesn't match");
                }
            }
        }
    }
}