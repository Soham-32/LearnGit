using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Reports;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Report
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Reports")]
    public class ReportTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyReportsPageNavigationInProd(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\Reports.png");
            Assert.IsTrue(reportDashboard.IsPageTitleDisplayed(), "Page title is not displayed");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyMySchedulesPopupInReports(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            reportDashboard.ClickOnMySchedulesButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\MySchedulesPopupReports.png");
            Assert.AreEqual("My Schedules", reportDashboard.GetMySchedulesPopupTitle(), $"'My Schedule' popup title is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyReportPadInReports(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            reportDashboard.ClickOnReportPadButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\ReportPadReports.png");
            Assert.IsTrue(reportDashboard.IsViewButtonDisplayed(), $"'View' button is not displayed for the client - {env}");
            Assert.IsTrue(reportDashboard.IsSaveButtonDisplayed(), $"'Save' button is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifySearchFunctionalityInReports(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            var reportName = reportDashboard.GetReportName();
            reportDashboard.SearchWithReportName(reportName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SearchFunctionalityReports.png");
            Assert.IsTrue(reportDashboard.IsReportDisplayed(), $"Searched element is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGenerateReportFunctionality(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            var reportName = reportDashboard.GetReportName();
            reportDashboard.ClickOnGenerateReportIcon(reportName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GenerateIndividualReports.png");

            reportDashboard.ClickOnGenerateReportExportButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded("Assessment Info.xlsx"), $"Excel file is not downloaded successfully for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyIndividualScheduleReportPopup(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            reportDashboard.ClickOnIndividualScheduleReportIcon();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\IndividualScheduleReports.png");

            Assert.AreEqual("Schedule", reportDashboard.GetIndividualSchedulesPopupTitle(), $"'Schedule' popup title is not displayed for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyFiltersFunctionalityInReports(string env)
        {
            var reportDashboard = new ReportsDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            reportDashboard.NavigateToReportPageForProd(env, companyId);
            var filterOptions = reportDashboard.GetAllDropDownOptionList();
            foreach (var filterOption in filterOptions)
            {
                reportDashboard.SelectDropDownOption(filterOption);

                TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\FilterFunctionalityReports_{filterOption}.png");
                Assert.IsTrue(reportDashboard.IsSelectedDropDownOptionDisplayed(filterOption),
                    $"Filtered options are not displayed for the client - {env} with filter {filterOption}");
            }
        }
    }
}
