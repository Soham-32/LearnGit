using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment.GrowthItemTab
{

    [TestClass]
    [TestCategory("GrowthPlan"), TestCategory("GrowthItemTab")]
    public class GrowthItemsTabGridExportTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(), 
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
                _team = teams.GetTeamByName(SharedConstants.Team);

                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessmentAndGi(_team.TeamId, TeamAssessment, new List<GrowthItem> { GrowthItemInfo });
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        [DoNotParallelize]
        public void GrowthItemTab_Grid_ExportToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            //Delete all 'Growth Items' excel file from the local
            const string fileName = "Growth Items.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team Assessment Growth Item Tab' page and added all the columns");
            growthItemsPage.NavigateToPage(Company.Id, _team.TeamId);

            //Select expected columns and get the column header list
            var expectedColumns = GrowthPlanFactory.GetGrowthPlanColumnNameList();
            growthItemsPage.SelectCustomColumns(expectedColumns);
            var columnHeaders = growthItemsPage.GetColumnHeaders();

            Log.Info("Download excel file and verify excel data");
            growthItemsPage.ClickExportToExcelButton();

            //Get data from excel file then compare column counts and names
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);
            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            Assert.AreEqual(columnHeaders.Count, actualColumns.Count, "Column count doesn't match");

            for (var i = 0; i < columnHeaders.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{columnHeaders[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(columnHeaders[i], actualColumns[i], "Column header text doesn't match");
            }

            // Get total row counts from GI grid view and verify first 50 growth items.
            var totalRowCount = growthItemsPage.GetNumberOfGridRows();
            if (totalRowCount < 50)
            {
                Assert.AreEqual(totalRowCount, tbl.Rows.Count, "Total rows doesn't match");
            }
            else
            {
                totalRowCount = 50;
            }

            // Getting all row values
            var expectedRows = new List<List<string>>();
            for (var i = 1; i <= totalRowCount; i++)
            {
                var rowValues = growthItemsPage.GetAllRowValues(i, expectedColumns);
                expectedRows.Add(rowValues);
            }


            // Verify each rows value 
            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // Get the row from the spreadsheet and compare column count for the row
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();
                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Column count doesn't match");

                // Compare each column's value
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    var expectedColumn = expectedColumns[j];
                    var expectedValue = expectedRow[j].Trim();
                    var actualValue = actualRow[j].Trim();

                    if (expectedColumn.Equals("Created"))
                    {
                        Log.Info($"Row {i}, Column {j} - Expected='{expectedValue}' Actual='{actualValue}'");
                        Assert.AreEqual(Math.Abs((DateTime.Parse(expectedValue) - DateTime.Parse(actualValue)).TotalDays) <= 1, true,
                            $"Row {i}, Column {j} Created date value doesn't match");
                    }
                    else if (expectedColumn.Equals("Rank"))
                    {
                        // Treat blank and "0" as equal for Rank
                        bool isEqual = (expectedValue == actualValue) ||
                                       (expectedValue == "0" && string.IsNullOrEmpty(actualValue)) ||
                                       (string.IsNullOrEmpty(expectedValue) && actualValue == "0");

                        Log.Info($"Row {i}, Column {j} - Expected='{expectedValue}' Actual='{actualValue}'");
                        Assert.IsTrue(isEqual, $"Row {i}, Column {j} Rank value doesn't match");
                    }
                    else
                    {
                        Log.Info($"Row {i}, Column {j} - Expected='{expectedValue}' Actual='{actualValue}'");
                        Assert.AreEqual(expectedValue.RemoveWhitespace(), actualValue.RemoveWhitespace(),
                            $"Row {i}, Column {j} value doesn't match");
                    }
                }
            }

            tbl.Dispose();

        }
    }
}
