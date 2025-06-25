using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Financials;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_BusinessOutcomes")]
    public class BusinessOutcomesTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBusinessOutcomesPageNavigationInProd(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            addBusinessOutcomePage.NavigateToBusinessOutcomesPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomes.png", 10000);
            Assert.AreEqual("3 YEAR OUTCOMES", addBusinessOutcomePage.GetDefaultCardLabelTitle(), $"3 years outcomes label Title does not matched after navigating in 'Business outcomes page' for the client - {env}");
        }

        [TestMethod]
        [TestCategory("BusinessOutcomesSaudiChecks")]
        public void VerifyBusinessOutcomesPageNavigationForSrca()
        {
            VerifyBusinessOutcomesPageNavigationForSaudiDomains("srca");
        }

        [TestMethod]
        [TestCategory("BusinessOutcomesSaudiChecks")]
        public void VerifyBusinessOutcomesPageNavigationForHhc()
        {
            VerifyBusinessOutcomesPageNavigationForSaudiDomains("hhc");
        }

        [TestMethod]
        [TestCategory("BusinessOutcomesSaudiChecks")]
        public void VerifyBusinessOutcomesPageNavigationForRcmc()
        {
            VerifyBusinessOutcomesPageNavigationForSaudiDomains("rcmc");
        }
       
        private void VerifyBusinessOutcomesPageNavigationForSaudiDomains(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var financialsPage = new BusinessOutcomesFinancialPage(Driver, Log);
            var viewPage = new BusinessOutcomesViewPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ListToString();

            LoginToProductionEnvironment(env);
            addBusinessOutcomePage.NavigateToBusinessOutcomesPageForSaDomain(env, companyId);
            if (addBusinessOutcomePage.IsPendoPopUpDisplayed())
            {
                addBusinessOutcomePage.ClickOnPendoCloseIcon();
            }
            Log.Info("Navigate to the Business Outcomes page and verify that Business Outcomes page loaded successfully");
            if (env == "srca" || env == "rcmc")
            {
                Assert.AreEqual("STRATEGIC OBJECTIVES", addBusinessOutcomePage.GetDefaultCardLabelTitle(), $"STRATEGIC OBJECTIVES label Title does not matched after navigating in 'Business outcomes page' for the client - {env}");
            }

            else
            {
                Assert.AreEqual("2028 OUTCOMES", addBusinessOutcomePage.GetDefaultCardLabelTitle(), $"2028 OUTCOMES label Title does not matched after navigating in 'Business outcomes page' for the client - {env}");
            }
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomes.png", 3000);

            Log.Info("Verify the Business Outcome Page headers");
            var expectedHeaderTexts = new List<string>() { "Overall Performance", "Card View", "Financials", "Grid View", "Meeting Notes" };
            var actualHeaderTexts = addBusinessOutcomePage.GetBusinessOutcomesHeaderTabs();

            foreach (var headerText in actualHeaderTexts)
            {
                Assert.That.ListContains(expectedHeaderTexts, headerText, "Header list doesn't match");
            }

            Log.Info("Click on Overall Performance tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnOverallPerformanceTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOverallPerformanceTab.png", 3000);
            Assert.IsTrue(overallPerformancePage.IsBannerEditIconDisplayed(), "Image Banner Edit icon is not displayed");
            var companyNameFromCardTitle = overallPerformancePage.GetCompanyNameFromCardTitle(companyName);            
            Assert.IsTrue(companyNameFromCardTitle.Equals("Saudi Red Crescent") || companyNameFromCardTitle.Equals("Health Holding Co") || companyNameFromCardTitle.Equals("RCMC"), $"Company name - {companyNameFromCardTitle} is not displayed");

            Log.Info("Click on Outputs/Projects tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnOutputsProjectsTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOutputsProjectsTab.png", 3000);
            Assert.IsTrue(companyNameFromCardTitle.Equals("Saudi Red Crescent") || companyNameFromCardTitle.Equals("Health Holding Co") || companyNameFromCardTitle.Equals("RCMC"), $"Company name - {companyNameFromCardTitle} is not displayed");

            Log.Info("Click on financials tab from the overall performance tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnFinancialsTabOnOverallPerformanceTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesfinancialsTab.png", 3000);
            Assert.AreEqual("Current Financial Progress", overallPerformancePage.GetCurrentFinancialProgressText(), "The Current Financial Progress text is not displayed");

            Log.Info("Click on Maturity tab and verify the Dashboard is loaded Successfully");
            if (addBusinessOutcomePage.IsMaturityTabDisplayed())
            {
                addBusinessOutcomePage.ClickOnMaturityTab();
                TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMaturityTab.png", 3000);
                Assert.IsTrue(companyNameFromCardTitle.Equals("Saudi Red Crescent") || companyNameFromCardTitle.Equals("Health Holding Co") || companyNameFromCardTitle.Equals("RCMC"), $"Company name - {companyNameFromCardTitle} is not displayed");
            }

            Log.Info("Click on Financial tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnFinancialTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesFinancialTab.png", 3000);
            Assert.IsTrue(financialsPage.IsFinancialPageTitleDisplayed(), "Financial page title is not displayed");

            Log.Info("Click on Outcome Tree View tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnOutcomeTreeViewTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOutcomeTreeViewTab.png", 3000);
            Assert.AreEqual("ID", addBusinessOutcomePage.GetIdTextFromOutcomeTreeViewTab(), "The ID text is not displayed on the Outcome Tree View tab");

            Log.Info("Click on Grid View tab and verify the Dashboard is loaded Successfully");
            addBusinessOutcomePage.ClickOnGridViewTab();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesGridView.png", 3000);
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
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomes.png", 3000);
            Assert.AreEqual("Total Cards", viewPage.GetGridViewTitleText(), "Grid View page title is not matched");

            var expectedTimeLineViewHeadersValues = new List<string>
            {
                "WEEK",
                "MONTH",
                "YEAR"
            };
            var actualTimeLineViewHeadersValues = viewPage.GetTimeLineViewHeadersText();
            Assert.That.ListsAreEqual(expectedTimeLineViewHeadersValues, actualTimeLineViewHeadersValues, "Timeline Header Value is incorrect");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomes.png", 3000);
        }
    }
}
