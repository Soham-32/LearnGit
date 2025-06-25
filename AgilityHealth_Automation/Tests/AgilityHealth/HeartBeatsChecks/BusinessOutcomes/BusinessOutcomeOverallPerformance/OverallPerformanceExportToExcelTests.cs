using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.Utilities;


namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes.BusinessOutcomeOverallPerformance
{
    [TestClass]
    [TestCategory("HeartBeatChecks")]
    public class OverallPerformanceExportToExcelTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [DataTestMethod]
        [DataRow("hhc")]
        [DataRow("srca")]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBusinessOutcomesOverallPerformanceStrategicExportToExcel(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var objectivesPage= new BusinessOutcomeObjectivesPage(Driver, Log);

            var fileName ="Export.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            if (env == "srca")
            {
                addBusinessOutcomePage.NavigateToBusinessOutcomesPageForSaDomain(env, companyId);
            }

            else
            {
                addBusinessOutcomePage.NavigateToBusinessOutcomesPageForProd(env, companyId);
            }
            Log.Info("Click on Overall Performance and the Export To Excel is downloaded successfully for selected Objectives");
            
            addBusinessOutcomePage.WaitUntilBusinessOutcomesPageLoaded();
            overallPerformancePage.ClickOnTab("Overall Performance");
            overallPerformancePage.WaitTillOverallPerformanceLoadedSuccessfully();
            overallPerformancePage.ClickOnOutcomesObjectiveProgressBar("Strategic Objectives");
            objectivesPage.ClickOnExportToExcel();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOverallPerformanceObjectivesExcel.png", 10000);
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"<{fileName}> file not downloaded successfully");
            
            
        }

    }
}

