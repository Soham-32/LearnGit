using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.TeamDashboard
{
    [TestClass]
    [TestCategory("TeamDashboard"), TestCategory("NewNavigation")]
    public class ExportToExcelTests : BaseTest
    {
        [TestMethod]
        [DoNotParallelize]
        [TestCategory("CompanyAdmin")]
        [TestCategory("ExportToExcel")]
        public void TeamDashboard_GridView_ExportToExcel_All()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            const string fileName = "TeamList.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            Driver.NavigateToPage(ApplicationUrl);

            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SwitchToGridView();

            teamDashboardPage.FilterTeamType("All");

            teamDashboardPage.ExportToExcel();

            // Wait for file download and get the spreadsheet
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            // Get data from Excel
            var excelData = ExcelUtil.GetExcelData(spreadsheet);

            // Get expected data from grid
            var expectedColumns = teamDashboardPage.GetVisibleColumnNamesFromGrid();
            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= teamDashboardPage.GetNumberOfGridRows(); i++)
            {
                var row = expectedColumns.Select(column => teamDashboardPage.GetTeamGridCellValue(i, column)).ToList();
                expectedRows.Add(row);
            }

            // Compare columns
            var actualColumns = excelData.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            // Compare rows
            Assert.AreEqual(expectedRows.Count, excelData.Rows.Count, "Rows count doesn't match");

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];
                var actualRow = excelData.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Columns count doesn't match");

                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"{j}th row text doesn't match");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [TestCategory("CompanyAdmin")]
        [TestCategory("ExportToExcel")]
        public void TeamDashboard_GridView_ExportToExcel_Team()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            const string fileName = "TeamList.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            Driver.NavigateToPage(ApplicationUrl);

            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SwitchToGridView();

            teamDashboardPage.FilterTeamType("Team");

            teamDashboardPage.ExportToExcel();

            // Wait for file download and get the spreadsheet
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            // Get data from Excel
            var excelData = ExcelUtil.GetExcelData(spreadsheet);

            // Get expected data from grid
            var expectedColumns = teamDashboardPage.GetVisibleColumnNamesFromGrid();
            Assert.IsFalse(expectedColumns.Contains("Number of Sub Teams"),
                    "'Number of Sub Teams' column should not display when filtered by 'Team'");

            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= teamDashboardPage.GetNumberOfGridRows(); i++)
            {
                var row = expectedColumns.Select(column => teamDashboardPage.GetTeamGridCellValue(i, column)).ToList();
                expectedRows.Add(row);
            }

            // Compare columns
            var actualColumns = excelData.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            // Compare rows
            Assert.AreEqual(expectedRows.Count, excelData.Rows.Count, "Rows count doesn't match");

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];
                var actualRow = excelData.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Columns count doesn't match");

                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"{j}th row text doesn't match");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [TestCategory("CompanyAdmin")]
        [TestCategory("ExportToExcel")]
        public void TeamDashboard_GridView_ExportToExcel_MultiTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            const string fileName = "TeamList.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            Driver.NavigateToPage(ApplicationUrl);

            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SwitchToGridView();

            teamDashboardPage.FilterTeamType("Multi-Team");

            teamDashboardPage.ExportToExcel();

            // Wait for file download and get the spreadsheet
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            // Get data from Excel
            var excelData = ExcelUtil.GetExcelData(spreadsheet);

            // Get expected data from grid
            var expectedColumns = teamDashboardPage.GetVisibleColumnNamesFromGrid();
            Assert.IsFalse(expectedColumns.Contains("Number of Assessments"),
                    "'Number of Assessments' column should not display when filtered by 'Multi Team'");
            Assert.IsFalse(expectedColumns.Contains("Last Date of Assessment"),
                "'Last Date of Assessment' column should not display when filtered by 'Multi Team'");

            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= teamDashboardPage.GetNumberOfGridRows(); i++)
            {
                var row = expectedColumns.Select(column => teamDashboardPage.GetTeamGridCellValue(i, column)).ToList();
                expectedRows.Add(row);
            }

            // Compare columns
            var actualColumns = excelData.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            // Compare rows
            Assert.AreEqual(expectedRows.Count, excelData.Rows.Count, "Rows count doesn't match");

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];
                var actualRow = excelData.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Columns count doesn't match");

                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"{j}th row text doesn't match");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [TestCategory("CompanyAdmin")]
        [TestCategory("ExportToExcel")]
        [TestCategory("KnownDefect")] // Bug Id : 45564
        public void TeamDashboard_GridView_ExportToExcel_PortfolioTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            const string fileName = "TeamList.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            Driver.NavigateToPage(ApplicationUrl);

            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SwitchToGridView();

            teamDashboardPage.FilterTeamType("Portfolio Team");

            teamDashboardPage.ExportToExcel();

            // Wait for file download and get the spreadsheet
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            // Get data from Excel
            var excelData = ExcelUtil.GetExcelData(spreadsheet);

            // Get expected data from grid
            var expectedColumns = teamDashboardPage.GetVisibleColumnNamesFromGrid();
            Assert.IsFalse(expectedColumns.Contains("Number of Assessments"),
                    "'Number of Assessments' column should not display when filtered by 'Portfolio Team'");
            Assert.IsFalse(expectedColumns.Contains("Last Date of Assessment"),
                "'Last Date of Assessment' column should not display when filtered by 'Portfolio Team'");

            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= teamDashboardPage.GetNumberOfGridRows(); i++)
            {
                var row = expectedColumns.Select(column => teamDashboardPage.GetTeamGridCellValue(i, column)).ToList();
                expectedRows.Add(row);
            }

            // Compare columns
            var actualColumns = excelData.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            // Compare rows
            Assert.AreEqual(expectedRows.Count, excelData.Rows.Count, "Rows count doesn't match");

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];
                var actualRow = excelData.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Columns count doesn't match");

                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"{j}th row text doesn't match");
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [TestCategory("CompanyAdmin")]
        [TestCategory("ExportToExcel")]
        public void TeamDashboard_GridView_ExportToExcel_EnterpriseTeam()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            const string fileName = "TeamList.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            Driver.NavigateToPage(ApplicationUrl);

            loginPage.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.SwitchToGridView();

            teamDashboardPage.FilterTeamType("Enterprise Team");

            teamDashboardPage.ExportToExcel();

            // Wait for file download and get the spreadsheet
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            // Get data from Excel
            var excelData = ExcelUtil.GetExcelData(spreadsheet);

            // Get expected data from grid
            var expectedColumns = teamDashboardPage.GetVisibleColumnNamesFromGrid();
            Assert.IsFalse(expectedColumns.Contains("Number of Assessments"),
                    "'Number of Assessments' column should not display when filtered by 'Enterprise Team'");
            Assert.IsFalse(expectedColumns.Contains("Last Date of Assessment"),
                "'Last Date of Assessment' column should not display when filtered by 'Enterprise Team'");

            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= teamDashboardPage.GetNumberOfGridRows(); i++)
            {
                var row = expectedColumns.Select(column => teamDashboardPage.GetTeamGridCellValue(i, column)).ToList();
                expectedRows.Add(row);
            }

            // Compare columns
            var actualColumns = excelData.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            // Compare rows
            Assert.AreEqual(expectedRows.Count, excelData.Rows.Count, "Rows count doesn't match");

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];
                var actualRow = excelData.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

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
