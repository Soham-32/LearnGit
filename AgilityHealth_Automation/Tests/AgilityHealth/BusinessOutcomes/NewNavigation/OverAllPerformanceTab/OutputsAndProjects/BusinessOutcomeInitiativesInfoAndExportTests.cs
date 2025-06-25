using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OverAllPerformanceTab.OutputsAndProjects
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomeInitiativesInfoAndExportTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _initiativeCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);

            var initiatives = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives);

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OverallPerformance_OutputsProjects_Initiative_GridViewHeadersAndData()
        {
            var loginPage = new LoginPage(Driver, Log);
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var businessOutcomesOverallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);

            Log.Info("Logging in to the application.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to Business Outcomes Dashboard.");
            dashboardPage.NavigateToPage(Company.Id);

            Log.Info("Opening the 'Overall Performance' tab.");
            overallPerformancePage.ClickOnOverallPerformanceTab();
            overallPerformancePage.WaitTillOverallPerformanceLoadedSuccessfully();

            Log.Info("Switching to 'Outputs/Projects' sub tab.");
            overallPerformancePage.ClickOnOutputsProjectsTab();

            Log.Info("Verifying 'Initiatives' timeline header information.");
            businessOutcomesOverallPerformancePage.ClickOnProjectTimeline(User.CompanyName, "Initiatives");

            Assert.IsTrue(businessOutcomesOverallPerformancePage.IsProjectHeaderDisplayed("Initiatives"), "'Initiatives' Header is not displayed");
            Assert.IsTrue(businessOutcomesOverallPerformancePage.IsOverallProgressTextDisplayed(), "'Overall Progress' text is not displayed in the header");
            Assert.IsTrue(businessOutcomesOverallPerformancePage.IsShowOrphanCardsTextDisplayed(), "'Show Orphan Card' text is not displayed in the header");

            Log.Info("Verifying Initiatives Grid headers.");
            var expectedProjectHeaders = new List<string> { "ID", "Title", "Card Type", "Progress", "Team", "Owners", "Status", "Start Date", "End Date" };
            CollectionAssert.AreEqual(expectedProjectHeaders, businessOutcomesOverallPerformancePage.GetProjectHeaderData(), "Mismatch in Initiatives Grid headers");
            businessOutcomesOverallPerformancePage.InitiativeSortByDescending("ID");

            Log.Info("Validating project data in the grid.");
            Assert.IsTrue(businessOutcomesOverallPerformancePage.IsTitleDisplayed(_initiativeCardResponse.Title), $"Initiatives title '{_initiativeCardResponse.Title}' is not displayed");
            Assert.AreEqual("Not Started", businessOutcomesOverallPerformancePage.GetStatusValue(_initiativeCardResponse.Title), $"Status value mismatch for Initiatives '{_initiativeCardResponse.Title}'");
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OverallPerformance_OutputsProjects_Initiative_ExportToExcel()
        {
            var loginPage = new LoginPage(Driver, Log);
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var businessOutcomesOverallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);

            const string exportFileName = "Export.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(exportFileName);

            Log.Info("Logging in to the application.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to the Business Outcomes Dashboard.");
            dashboardPage.NavigateToPage(Company.Id);

            Log.Info("Opening the 'Overall Performance' tab.");
            overallPerformancePage.ClickOnOverallPerformanceTab();
            overallPerformancePage.WaitTillOverallPerformanceLoadedSuccessfully();

            Log.Info("Switching to the 'Outputs/Projects' sub tab.");
            overallPerformancePage.ClickOnOutputsProjectsTab();

            Log.Info("Clicking on the Initiatives timeline.");
            businessOutcomesOverallPerformancePage.ClickOnProjectTimeline(User.CompanyName, "Initiatives");

            Log.Info("Clicking on the 'Export to Excel' button.");
            businessOutcomesOverallPerformancePage.ClickOnExportToExcel();

            Log.Info($"Checking if the file '{exportFileName}' has been downloaded.");
            Assert.IsTrue(FileUtil.IsFileDownloaded(exportFileName), $"<{exportFileName}> file not downloaded successfully");

            Log.Info("Waiting for the downloaded file and loading the spreadsheet.");
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(exportFileName);

            Log.Info("Adjusting the columns in the Excel file for proper viewing.");
            ExcelUtil.ExcelColumnAutoAdjustExcel(spreadsheet);

            Log.Info("Fetching data from the Excel file.");
            var excelDataTable = ExcelUtil.GetExcelData(spreadsheet);

            Log.Info("Verifying that the correct columns are present in the exported Excel file.");
            var exportColumns = new List<string> { "ID", "Title", "Card Type", "Progress", "Team", "Owners", "Status", "Start Date", "End Date" };
            var actualColumns = (from DataColumn item in excelDataTable.Columns select item.ColumnName).ToList();
            actualColumns = actualColumns.Where(c => !c.StartsWith("Column")).ToList();

            Log.Info("Verify that data in exported excel file showing properly");
            for (var i = 0; i < exportColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{exportColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(exportColumns[i], actualColumns[i], "Column header text doesn't match");
            }

            Log.Info($"Checking if the {_initiativeCardResponse.Title} title is displayed in the exported Excel file.");
            var titles = excelDataTable.AsEnumerable().Select(row => row["Title"].ToString()).ToList();
            Assert.IsTrue(titles.Contains(_initiativeCardResponse.Title), $"The created card with title '{_initiativeCardResponse.Title}' is not displayed in the exported Excel file.");

        }
    }
}

