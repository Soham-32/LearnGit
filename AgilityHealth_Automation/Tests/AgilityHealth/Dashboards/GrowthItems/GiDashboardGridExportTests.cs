using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardGridExportTests : BaseTest
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [DoNotParallelize]
        public void GrowthItemDashboard_Grid_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);

            const string fileName = "Growth Items.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickGrowthItemDashBoard();

            giDashboardGridView.ClearFilter();

            var expectedColumns = GrowthPlanFactory.GetGrowthPlanColumnNameList();

            giDashboardGridView.ClickColumnMenu();
            giDashboardGridView.ClickColumnsMenuItem();
            giDashboardGridView.SelectColumns(expectedColumns);
            giDashboardGridView.ClickColumnMenu();

            var columnHeaders = giDashboardGridView.GetColumnHeaders();
            Assert.AreEqual(expectedColumns.Count, columnHeaders.Count, "Column count doesn't match");

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{columnHeaders[i]}'");
                Assert.AreEqual(expectedColumns[i], columnHeaders[i], $"{i}th column header should display properly");
            }

            giDashboardGridView.ClickExportToExcel();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            Assert.AreEqual(columnHeaders.Count, actualColumns.Count, "Column count doesn't match");
            for (var i = 0; i < columnHeaders.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{columnHeaders[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(columnHeaders[i], actualColumns[i], "Column header text doesn't match");
            }

            var totalRowCount = giDashboardGridView.GetNumberOfGridRows();
            //Only verifying first 50 growth items.
            if (totalRowCount < 50)
            {
                Assert.AreEqual(totalRowCount, tbl.Rows.Count, "Total rows doesn't match");
            }
            else
            {
                totalRowCount = 50;
            }

            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= totalRowCount; i++)
            {
                var rowValues = growthItemsPage.GetAllRowValues(i, expectedColumns);
                if (rowValues[1].Equals(""))
                    rowValues[1] = "0";
                expectedRows.Add(rowValues);
            }

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString().Trim().RemoveWhitespace()).ToList();

                // compare them
                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Row count doesn't match");
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    if (expectedColumns[j].Equals("Created"))
                    {
                        Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                        Assert.AreEqual(Math.Abs((DateTime.Parse(expectedRow[j]) - DateTime.Parse(actualRow[j])).TotalDays) <= 1, true, $"Row {i}, Column {j} Target Date value doesn't match");
                    }
                    else
                    {
                        Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                        Assert.AreEqual(expectedRow[j].RemoveWhitespace(), actualRow[j].RemoveWhitespace(), $"Row {i}, Column {j} value doesn't match");
                    }
                }
            }

            tbl.Dispose();
        }
    }
}
