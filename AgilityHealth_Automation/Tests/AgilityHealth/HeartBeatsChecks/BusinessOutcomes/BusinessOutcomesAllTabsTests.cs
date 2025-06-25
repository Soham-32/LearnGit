using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Financials;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes

{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_BusinessOutcomes")]
    public class BusinessOutcomesAllTabsTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DataRow("srca")]
        [DataRow("hhc")]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBusinessOutcomesPageHeaders(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var financialsPage = new BusinessOutcomesFinancialPage(Driver, Log);
            var viewPage = new BusinessOutcomesViewPage(Driver, Log);

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
            addBusinessOutcomePage.WaitUntilBusinessOutcomesPageLoaded();

            Log.Info("Verify the Business Outcome Page headers");
            var expectedHeaderTexts = new List<string>() { "Overall Performance", "Card View", "Financials", "Grid View", "Meeting Notes" };
            var actualHeaderTexts = addBusinessOutcomePage.GetBusinessOutcomesHeaderTabs();

            foreach (var headerText in actualHeaderTexts)
            {
                Assert.That.ListContains(expectedHeaderTexts, headerText, "Header list doesn't match");
            }

            Log.Info("Click on Overall Performance tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnOverallPerformanceTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOverallPerformanceTab.png", 10000);
            Assert.IsTrue(overallPerformancePage.IsBannerEditIconDisplayed(),"Image Banner Edit icon is not displayed");
            Assert.AreEqual(overallPerformancePage.GetLeftNavigationHierarchyTeamName()[1].ToUpper(), overallPerformancePage.GetHierarchySectionText()[1], "Company name is not displayed");

            Log.Info("Click on Financial tab and verify the Dashboard is loaded Successfully");
            if (actualHeaderTexts.Contains("Financials"))
            {
                addBusinessOutcomePage.ClickOnFinancialTab();
                TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesFinancialTab.png", 10000);
                Assert.IsTrue(financialsPage.IsFinancialPageTitleDisplayed(), "Financial page title is not displayed");
            }

            Log.Info("Click on Grid View tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnGridViewTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesGridView.png", 10000);
            var expectedGridViewDropdownValues = new List<string>
            {
                "Grid View",
                "Timeline View"
            };
            var actualGridViewDropdownValues = viewPage.GetGridViewDropdownOptions();
            Assert.That.ListsAreEqual(expectedGridViewDropdownValues, actualGridViewDropdownValues);

            viewPage.ClickOnGridViewDropdownOptions("Grid View");
            Assert.AreEqual("Total Cards", viewPage.GetGridViewTitleText(), "Grid View page title is not matched");

            addBusinessOutcomePage.ClickOnGridViewTab();
            Log.Info("Click on Timeline View and verify the Dashboard is loaded Successfully");
            viewPage.ClickOnGridViewDropdownOptions("Timeline View");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomes.png", 10000);
            Assert.AreEqual("Total Cards", viewPage.GetGridViewTitleText(), "Grid View page title is not matched");

            var expectedTimeLineViewHeadersValues = new List<string>
            {
                "WEEK",
                "MONTH",
                "YEAR"
            };
            var actualTimeLineViewHeadersValues = viewPage.GetTimeLineViewHeadersText();
            Assert.That.ListsAreEqual(expectedTimeLineViewHeadersValues, actualTimeLineViewHeadersValues,"Timeline Header Value is incorrect");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomes.png", 10000);
        }
    }
}
