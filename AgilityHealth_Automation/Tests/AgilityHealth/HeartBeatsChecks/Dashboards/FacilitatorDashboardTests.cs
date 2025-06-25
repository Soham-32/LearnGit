using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Facilitator;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Dashboard")]
    public class FacilitatorDashboardTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyFacilitatorDashboardPageNavigationInProd(string env)
        {
            if (env == "eu" || env == "truist") { return; }
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboardPage = new FacilitatorDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            teamDashboardPage.IsFacilitatorDashboardDisplayed();
            facilitatorDashboardPage.NavigateToFacilitatorDashboardForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\FacilitatorDashboard.png");
            Assert.AreEqual("Facilitator Feedback", facilitatorDashboardPage.GetFacilitatorDashboardTitle(), $"Facilitator dashboard Title does not matched after navigating in 'Facilitator Dashboard' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyFacilitatorDashboardExportToExcelInProd(string env)
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);

            const string fileName = "Facilitator Feedback.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            Log.Info("Navigate to the facilitator dashboard and verify that the excel is downloaded successfully for the active facilitators");
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);
            if (teamDashboardPage.IsFacilitatorDashboardDisplayed())
            {
                facilitatorDashboard.NavigateToFacilitatorDashboardForProd(env, companyId);
                facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);
                facilitatorDashboard.ClickExportToExcel();
                FileUtil.WaitUntilFileDownloaded(fileName);
                Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} Excel is not downloaded successfully - {env}");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyActiveFacilitatorExpandAndCollaseInProd(string env)
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            Log.Info("Navigate to the facilitator dashboard page and verify that the row is expanded and collapsed for the Active facilitators");
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);
            if (teamDashboardPage.IsFacilitatorDashboardDisplayed())
            {
                facilitatorDashboard.NavigateToFacilitatorDashboardForProd(env, companyId);
                facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);
                if (facilitatorDashboard.IsExpanIconDisplayed())
                {
                    facilitatorDashboard.ExpandRow(1);
                    Assert.IsTrue(facilitatorDashboard.IsRowExpanded(1), $"The first row of facilitator dashboard is not expanded for - {env}");

                    facilitatorDashboard.CollapseRow(1);
                    Assert.IsTrue(facilitatorDashboard.IsRowCollapse(1), $"The first row of facilitator dashboard is not collapse for - {env}");
                }
            }       
        }


        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyInActiveFacilitatorExpandAndCollaseInProd(string env)
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            LoginToProductionEnvironment(env);

            Log.Info("Navigate to the facilitator dashboard page and verify that the row is expanded for the Inactive facilitators");
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);
            if (teamDashboardPage.IsFacilitatorDashboardDisplayed())
            {
                facilitatorDashboard.NavigateToFacilitatorDashboardForProd(env, companyId);
                facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(false);
                if (facilitatorDashboard.IsExpanIconDisplayed())
                {
                    facilitatorDashboard.ExpandRow(1);
                    Assert.IsTrue(facilitatorDashboard.IsRowExpanded(1), $"The first row of facilitator dashboard is not expanded for - {env}");

                    Log.Info("Verify that the row is collapse for the Inactive facilitators");
                    facilitatorDashboard.CollapseRow(1);
                    Assert.IsTrue(facilitatorDashboard.IsRowCollapse(1), $"The first row of facilitator dashboard is not collapse for - {env}");
                }
            }            
        }
    }
}
