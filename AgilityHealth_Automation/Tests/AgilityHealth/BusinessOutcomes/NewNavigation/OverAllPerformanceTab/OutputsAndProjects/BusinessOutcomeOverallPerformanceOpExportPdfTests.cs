using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OverAllPerformanceTab.
    OutputsAndProjects
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomeOverallPerformanceOpExportPdfTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OverallPerformance_OutputsProjects_ExportPdf()
        {
            var loginPage = new LoginPage(Driver, Log);
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var businessOutcomesOverallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);

            var fileName = $"{User.CompanyName}OverallPerformance.pdf".RemoveWhitespace();
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Log.Info("Logging in to the application.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to Business Outcomes Dashboard.");
            dashboardPage.NavigateToPage(Company.Id);

            Log.Info("Opening the 'Overall Performance' tab.");
            overallPerformancePage.ClickOnOverallPerformanceTab();
            overallPerformancePage.WaitTillOverallPerformanceLoadedSuccessfully();

            Log.Info("Switching to 'Outputs/Projects' sub-tab.");
            overallPerformancePage.ClickOnOverallPerformanceSubTab("Outputs/Projects");

            Log.Info("Exporting data to PDF.");
            businessOutcomesOverallPerformancePage.ClickOnExportToPdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"Exported file {fileName} was not downloaded successfully.");
        }
    }
}

