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
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes.BusinessOutcomeOverallPerformance
{
    [TestClass]
    [TestCategory("HeartBeatChecks")]
    public class BusinessOutcomesOverallPerformanceTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DataRow("hhc")]
        [DataRow("srca")]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBusinessOutcomesOverallPerformance(string env)
        {
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
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

            Log.Info("Click on Overall Performance and verify it is loaded successfully");
            overallPerformancePage.ClickOnTab("Overall Performance");
            overallPerformancePage.WaitTillOverallPerformanceLoadedSuccessfully();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOverallPerformanceTab.png", 10000);
            Assert.IsTrue(overallPerformancePage.IsBannerEditIconDisplayed(), "Company Banner Edit Icon Is Not Displayed");
            Assert.IsTrue(overallPerformancePage.IsOverallProgressOutcomesTabDisplayed("Outcomes"), "Outcome Tab Is Not Displayed");
            Assert.IsTrue(overallPerformancePage.IsOverallProgressOutcomesTabDisplayed("Outputs/Projects"), "Outputs/Projects Tab Is Not Displayed");
            Assert.IsTrue(overallPerformancePage.IsOverallProgressOutcomesTabDisplayed("Financials"), "Financials Tab Is Not Displayed");

            Log.Info("Click on Outputs/Projects and verify it is loaded successfully");
            overallPerformancePage.ClickOnOverallPerformanceSubTab("Outputs/Projects");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOverallPerformanceOutputsProjectsTab.png", 10000);
            var actualOutPutProjectWeightFieldTexts = overallPerformancePage.GetCardViewTexts();
            Assert.That.ListContains(actualOutPutProjectWeightFieldTexts, "Obstacles:", "Obstacles is not displayed");
            Assert.IsFalse(overallPerformancePage.IsPerformanceObjectivesDropdownDisplayed(), "Objectives Dropdown Is Displayed");

            Log.Info("Click on Outcomes tab and verify hierarchy is displayed");
            overallPerformancePage.ClickOnOverallPerformanceSubTab("Outcomes");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesOverallPerformanceOutcomesTab.png", 10000);
            var leftNavTeams = overallPerformancePage.GetLeftNavigationHierarchyTeamName().ConvertAll(text => text.ToUpper());
            var hierarchySectionTeams = overallPerformancePage.GetHierarchySectionText();
            Assert.That.ListsAreEqual(leftNavTeams, hierarchySectionTeams,"Hierarchy displayed is incorrect");

            Log.Info("Verify the Company Hierarchy Card View ");
            var outcomesCompanyLevelCardViewTexts = overallPerformancePage.GetCardViewTexts();
            var expectedOutcomesCompanyLevelCardViewTexts = new List<string>
            {
                "Overall Strategy Progress",
                "Strategic Objectives:",
                "KPIs:"
            };

            Assert.That.ListsAreEqual(expectedOutcomesCompanyLevelCardViewTexts, outcomesCompanyLevelCardViewTexts,"Outcomes Company's Card view Title is incorrect");//

            Log.Info("Verify the Objectives dropdown Values");
            overallPerformancePage.ClickObjectivesDropdown();
            var dropdownOptions = overallPerformancePage.GetObjectivesDropdownOptions();
            var expectedOptions = new List<string> { "Annual Objectives", "Quarterly Objectives" };
            Assert.That.ListsAreEqual(expectedOptions, dropdownOptions, "Dropdown options does not match");

            Log.Info("Verify the Annual Objectives Filter is applied to the Hierarchy view");
            overallPerformancePage.SelectOnObjectivesDropdownOptions("Annual Objectives");
            var t = overallPerformancePage.GetAnnualObjectiveTeamText();
            Assert.AreEqual("Annual Objectives:", overallPerformancePage.GetAnnualObjectiveTeamText(),"Selected Annual Objective is not displayed");

            Log.Info("Verify the Quarterly Objectives Filter is applied to the Hierarchy view");
            overallPerformancePage.ClickObjectivesDropdown();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesQuarterlyObjectives.png", 10000);
            overallPerformancePage.SelectOnObjectivesDropdownOptions("Quarterly Objectives");
            Assert.AreEqual("Quarterly Objectives:", overallPerformancePage.GetAnnualObjectiveTeamText(), "Selected Annual Objective is not displayed");
            
            
        }
    }
}
