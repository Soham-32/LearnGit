using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardKanbanExportTests : BaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51227
        [DoNotParallelize]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GrowthItemDashboard_Kanban_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var growthItemDashBoard = new GrowthItemsDashboardPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickGrowthItemDashBoard();

            growthItemDashBoard.ChangeViewWidget(GrowthItemWidget.Kanban);
            const string fileName = "Growth Items.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            giDashboardKanbanView.FilterBySurveyType("(All)");

            giDashboardKanbanView.ClickExportToExcel();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            //Expected growth items title            
            var expectedGrowthItemTitles = giDashboardKanbanView.GetAllGrowthItemsTitles();

            //Actual growth itmes data
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedGrowthItemTitles.Count, tbl.Rows.Count,
                "All created GIs should be shown in exported Excel file");

            //Verifying growth item titles

            //Getting title column index
            var titleColumnNumber = 0;
            for (var i = 0; i< tbl.Columns.Count; i++)
            {
                if (tbl.Columns[i].ColumnName.Equals("Title"))
                {
                    titleColumnNumber = i;
                    break;
                }
            }
            for (var i = 0; i < expectedGrowthItemTitles.Count; i++)
            {
                var actualRow = tbl.Rows[i][titleColumnNumber].ToString();
                Assert.AreEqual(expectedGrowthItemTitles.Contains(actualRow), true, 
                    $"Growth item not present. Actual : {actualRow} , Expected Growth Items List : \n" + string.Join( Environment.NewLine, expectedGrowthItemTitles.ToArray()));
            }
                
        }
    }
}
