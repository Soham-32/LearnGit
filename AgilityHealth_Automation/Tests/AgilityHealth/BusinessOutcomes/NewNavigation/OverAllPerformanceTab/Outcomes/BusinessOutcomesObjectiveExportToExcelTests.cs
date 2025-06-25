using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OverAllPerformanceTab.Outcomes
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesObjectiveExportToExcelTests : BusinessOutcomesBaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcome_OverallPerformance_Outcome_Objectives_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomesOverallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            Log.Info("Logging in to the application.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Business outcomes' dashboard");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            Assert.IsTrue(businessOutcomesDashboard.IsPageLoadedCompletely(), "Business Outcome page isn't loaded successfully.Stopping execution");

            Log.Info("Click on 'Overall Performance' tab and go to 'Objectives' progress bar");
            addBusinessOutcomePage.ClickOnOverallPerformanceTab();
            businessOutcomesOverallPerformancePage.ClickOnObjectiveProgressBar();

            Log.Info("Click on 'Export to Excel' button and verify excel file is downloaded successfully.");
            businessOutcomesOverallPerformancePage.ClickOnObjectiveAndKpisExportToExcelButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded($"Export.xlsx"), $"Excel file is not downloaded successfully");

        }
    }
}