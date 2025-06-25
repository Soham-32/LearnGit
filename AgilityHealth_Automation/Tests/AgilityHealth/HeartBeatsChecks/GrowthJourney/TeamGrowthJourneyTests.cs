using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BreadCrumbNavigation.Dashboards;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.GrowthJourney
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_GrowthJourney")]
    public class TeamGrowthJourneyTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyTeamGrowthJourneyPageNavigationInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamGrowthJourney.png");
            Assert.IsTrue(growthJourneyPage.IsGrowthJourneyTabPresent(), $"Team growth journey tab is not present for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyExportToExcelInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            const string fileName = "Analysis.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Log.Info($"Export to excel for the '{env}' environment.");
            growthJourneyPage.ClickExportToExcel();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"'{fileName}' Excel not downloaded successfully for the '{env}' environment.");

        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyExportToPdfInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            var breadcrumbNavigationPage = new BreadCrumbNavigationPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            var companyName = breadcrumbNavigationPage.GetCompanyName();
            var teamName = breadcrumbNavigationPage.GetTeamName();

            string fileName = $"{teamName} {companyName}.pdf";
            fileName = fileName.Replace('/', '_');
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Log.Info($"Export to PDF");
            growthJourneyPage.ClickExportToPdf();
            growthJourneyPage.ClickCreatePdf();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"'{fileName}' PDF not downloaded successfully for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyDimensionSubdimensionFiltersInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            Log.Info($"Verify that we can choose dimensions and subdimensions from dropdown for the '{env}' environment.");
            var dropdownDimension = growthJourneyPage.GetFirstDimension();
            growthJourneyPage.ClickOnFirstDimension();
            var dropdownSubdimension = growthJourneyPage.GetFirstSubDimension();
            growthJourneyPage.ClickOnFirstSubDimension();
            var radarAnalysisDimension = growthJourneyPage.GetDimensionFromCompareRadarAnalysis();
            var radarAnalysisSubdimension = growthJourneyPage.GetSubDimensionFromCompareRadarAnalysis();
            Assert.IsTrue(radarAnalysisDimension.Contains(dropdownDimension), $"Dimension does not match for the '{env}' environment.");
            Assert.IsTrue(radarAnalysisSubdimension.Contains(dropdownSubdimension), $"SubDimension does not match for the '{env}' environment.");

        }
        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyRadarTypeDropdownInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            Log.Info($"Open radar type dropdown and check if page exists for the '{env}' environment.");
            var doesRadarTypeDropdownExist = growthJourneyPage.IsRadarTypeDropdownDisplayed();
            if (doesRadarTypeDropdownExist == false)
            {
                Log.Info($"Dropdown does not exist {env}");

                return;
            }
            growthJourneyPage.ClickOnTheFirstRadarTypeFromDropdown();
            var desc = growthJourneyPage.GetTeamRadarDescription();
            Assert.IsNotNull(desc, $"Page was not found for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyFilterAssessmentInLeftNavInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            Log.Info($"Open left nav and check if assessment tab is visible for the '{env}' environment.");
            growthJourneyPage.OpenFilterSidebar();
            Assert.IsTrue(growthJourneyPage.IsAssessmentTabVisible(), $"Assessment tab not visible  for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyShowHideMetricsFilterInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            Log.Info("Click on 'Show/Hide Metrics' button ");
            var text = growthJourneyPage.GetShowHideMetricsText();
            if (text.Contains("Hide"))
            {
                Assert.IsTrue(growthJourneyPage.IsHideMetricsButtonOn(), $"'Hide Metrics' button is not on for the '{env}' environment.");
            }
            else if (text.Contains("Show"))
            {
                Assert.IsTrue(growthJourneyPage.IsShowMetricsButtonOn(), $"'Show Metrics' button is not on for the '{env}' environment.");
            }
            growthJourneyPage.ClickOnShowHideMetricsButton();
            text = growthJourneyPage.GetShowHideMetricsText();
            if (text.Contains("Hide"))
            {
                Assert.IsTrue(growthJourneyPage.IsHideMetricsButtonOn(), $"'Hide Metrics' button is not on for the '{env}' environment.");
            }
            else if (text.Contains("Show"))
            {
                Assert.IsTrue(growthJourneyPage.IsShowMetricsButtonOn(), $"'Show Metrics' button is not on for the '{env}' environment.");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyDimensionSubDimensionExpandAndCollapseInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToTeamGrowthJourneyPage(env)) return;
            Log.Info($"Click on dimension dropdown for the '{env}' environment.");
            var dimensionClass = growthJourneyPage.GetDimensionClass();
            if (dimensionClass.Contains("expand"))
            {
                growthJourneyPage.ClickOnDimensionDropdown();
                dimensionClass = growthJourneyPage.GetDimensionClass();
                Assert.IsTrue(dimensionClass.Contains("collapse"), $"Dimension class does not contains 'collapse' for the '{env}' environment.");
            }
            else if (dimensionClass.Contains("collapse"))
            {
                growthJourneyPage.ClickOnDimensionDropdown();
                dimensionClass = growthJourneyPage.GetDimensionClass();
                Assert.IsTrue(dimensionClass.Contains("expand"), $"Dimension class does not contains 'expand' for the '{env}' environment.");
            }
            Driver.RefreshPage();

            Log.Info($"Click on sub-dimension dropdown for the '{env}' environment.");
            var subdimensionClass = growthJourneyPage.GetSubDimensionClass();
            if (subdimensionClass.Contains("expand"))
            {
                growthJourneyPage.ClickOnSubDimensionDropdown();
                subdimensionClass = growthJourneyPage.GetSubDimensionClass();
                Assert.IsTrue(subdimensionClass.Contains("collapse"), $"Sub-dimension class does not contains 'collapse' for the '{env}' environment.");
            }
            else if (subdimensionClass.Contains("collapse"))
            {
                growthJourneyPage.ClickOnSubDimensionDropdown();
                subdimensionClass = growthJourneyPage.GetSubDimensionClass();
                Assert.IsTrue(subdimensionClass.Contains("expand"), $"Sub-dimension class does not contains 'expand' for the '{env}' environment.");
            }
        }

        public bool NavigateToTeamGrowthJourneyPage(string env)
        {
            Log.Info($"Login to the environment {env} and go to team 'Growth Journey' tab");
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            var commonGridPage = new GridPage(Driver, Log);

            Log.Info($"Login to {env}, Navigate to 'Team dashboard' page and click on first 'Team' from list.");
            LoginToProductionEnvironment(env);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);
            teamDashboardPage.GridTeamView();
            teamDashboardPage.FilterTeamType("Team");
            if (teamDashboardPage.IsTeamDisplayed() == false)
            {
                Log.Info($"No teams in this environment {env}");
                return false;
            }
            commonGridPage.SortGridColumn("Number of Team Assessments", true);
            teamDashboardPage.SelectFirstTeamFromDashboard();

            Log.Info($"Go to 'Growth Journey' page and switch to timeline view for the '{env}' environment.");
            if (radarPage.IsGrowthJourneyTabDisplayed() == false)
            {
                Log.Info($"Growth Journey tab not displayed for the '{env}' environment.");
                return false;
            }
            radarPage.ClickGrowthJourneyTab();
            growthJourneyPage.SwitchToTimelineView();
            return true;

        }
    }
}
