using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OverAllPerformanceTab.Outcomes
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation"), TestCategory("KnownDefect")]
    public class BusinessOutcomeKpisExportToExcelTests : BusinessOutcomesBaseTest
    {

         [TestMethod]
         [TestCategory("KnownDefect")] //Bug Id: 51590
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_OverallPerformance_Outcome_Kpis_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomesOverallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            Log.Info("Log in in to the application.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Business outcomes' dashboard");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            Assert.IsTrue(businessOutcomesDashboard.IsPageLoadedCompletely(), "Business Outcome page isn't loaded successfully.Stopping execution");

            Log.Info("Click on 'Overall Performance' tab and go to 'KPIs' progress bar");
            addBusinessOutcomePage.ClickOnOverallPerformanceTab();
            businessOutcomesOverallPerformancePage.ClickOnOutcomesObjectiveProgressBar("KPIs");

            Log.Info("Click on 'Export to Excel' button and verify excel file is download successfully.");
            businessOutcomesOverallPerformancePage.ClickOnObjectiveAndKpisExportToExcelButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded("Export.xlsx"), $"Excel file is not downloaded successfully");

        }
    }
}