using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;

using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.GridViewTimeLineView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesTimelineViewExportToExcelTests : BusinessOutcomesBaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcome_TimeLineView_Week_ExportToExcel()
        {
            VerifyExportToExcel("Week");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcome_TimeLineView_Month_ExportToExcel()
        {
            VerifyExportToExcel("Month");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcome_TimeLineView_Year_ExportToExcel()
        {
            VerifyExportToExcel("Year");
        }

        private void VerifyExportToExcel(string timePeriod)
        {
            Log.Info("Initializing required pages");
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomesViewPage = new BusinessOutcomesViewPage(Driver, Log);
            var downloadPath = FileUtil.GetDownloadPath();

            Log.Info("Navigating to Login Page and Logging into the application");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to Business Outcomes Dashboard and Clicking on Grid View tab");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            addBusinessOutcomePage.ClickOnGridViewTab();

            Log.Info("Verifying Grid View dropdown options");
            var expectedGridViewDropdownValues = new List<string> { "Grid View", "Timeline View" };
            var actualGridViewDropdownValues = businessOutcomesViewPage.GetGridViewDropdownOptions();
            Assert.That.ListsAreEqual(expectedGridViewDropdownValues, actualGridViewDropdownValues);
            businessOutcomesViewPage.ClickOnGridViewDropdownOptions("Timeline View");

            Log.Info("Clicking on Time Period filter and verifying Time Period options");
            var expectedTimePeriodValues = new List<string> { "WEEK", "MONTH", "YEAR" };
            var actualTimePeriodValues = businessOutcomesViewPage.GetTimePeriodFilterOptions();
            Assert.That.ListsAreEqual(expectedTimePeriodValues, actualTimePeriodValues);
            businessOutcomesViewPage.ClickOnTimePeriodFilter(timePeriod);

            Log.Info("Clicking on Export to Excel button");
            addBusinessOutcomePage.ClickOnExportToExcelButton();

            Log.Info("Verifying if the Excel file is downloaded");
            var files = Directory.GetFiles(downloadPath, "Timeline-view-*.xlsx");
            var spreadsheet = files.OrderByDescending(File.GetCreationTime).First();
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Log.Info("Verifying column headers in the exported file");
            var expectedColumns = new List<string>
            {
                "ID", "Card Title", "Owners", "Start Date", "End Date", "Progress"
            };

            var actualColumns = tbl.Columns.Cast<DataColumn>()
                .Select(col => col.ColumnName.Trim())
                .Where(name => !string.IsNullOrEmpty(name) && !name.Contains("Column")) // Exclude unwanted columns
                .ToList();
            Assert.That.ListsAreEqual(expectedColumns, actualColumns, $"Column matched.");
        }

    }
}
