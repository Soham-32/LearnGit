using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
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
    public class MultiTeamGrowthJourneyTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyMultiTeamTeamGrowthJourneyPageNavigationInProd(string env)
        {
            Log.Info($"Verify that we are able to navigate to the 'Growth Journey' page for the '{env}' environment.");

            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\MultiTeamGrowthJourney.png");
            Assert.IsTrue(growthJourneyPage.IsGrowthJourneyTabPresent(), $"Multi team growth journey tab is not present for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyExportToExcelInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
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
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
            var companyName = breadcrumbNavigationPage.GetCompanyName();
            var teamName = breadcrumbNavigationPage.GetTeamName();

            string fileName = $"{teamName} {companyName}.pdf";
            fileName = fileName.Replace('/', '_');
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Log.Info($"Export to pdf for the '{env}' environment.");
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
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
            Log.Info($"Verify that we can choose 'Dimensions' and 'Subdimensions' from dropdown for the '{env}' environment.");
            var dropdownDimension = growthJourneyPage.GetFirstDimension();
            growthJourneyPage.ClickOnFirstDimension();
            var dropdownSubdimension = growthJourneyPage.GetFirstSubDimension();
            growthJourneyPage.ClickOnFirstSubDimension();
            var radarAnalysisDimension = growthJourneyPage.GetDimensionFromCompareRadarAnalysis();
            var radarAnalysisSubdimension = growthJourneyPage.GetSubDimensionFromCompareRadarAnalysis();
            Assert.IsTrue(radarAnalysisDimension.Contains(dropdownDimension), $"Dimension does not match  for the '{env}' environment.");
            if (dropdownDimension == "Measurements" || dropdownDimension == "Metrics")
            {
                Assert.IsTrue(radarAnalysisSubdimension.Contains("Measurements") || radarAnalysisSubdimension.Contains("Metrics"));
            }
            else
            {
                Assert.IsTrue(radarAnalysisSubdimension.Contains(dropdownSubdimension), $"Sub-dimension does not match for the '{env}' environment.");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyFilterAssessmentInLeftNavInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
            Log.Info($"Choose each period one time for the '{env}' environment.");
            growthJourneyPage.OpenFilterSidebar();
            growthJourneyPage.ClickCompareType();
            var tabs = growthJourneyPage.GetAllCompareDropdownValues();
            foreach (var tab in tabs)
            {
                growthJourneyPage.SelectCompareTypeFromFilter(tab);
                growthJourneyPage.OpenFilterSidebar();
                var filterText = growthJourneyPage.GetCompareDropdownDetailText();
                if (tab == "Monthly")
                {
                    Assert.IsTrue(filterText == "Filter the radar by clicking on the month you want to compare below.", $"Monthly period not selected for the '{env}' environment.");
                }
                else if (tab == "Quarterly")
                {
                    Assert.IsTrue(filterText == "Filter the radar by clicking on the quarter you want to compare below.", $"Quaterly period not selected for the '{env}' environment.");
                }
                else if (tab == "Bi-Annually")
                {
                    Assert.IsTrue(filterText == "Filter the radar by clicking on the half year you want to compare below.", $"Bi-Annually period not selected for the '{env}' environment.");
                }
                else if (tab == "Annually")
                {
                    Assert.IsTrue(filterText == "Filter the radar by clicking on the year you want to compare below.", $"Annually period not selected for the '{env}' environment.");
                }
                else if (tab == "Campaigns")
                {
                    Assert.IsTrue(filterText.Contains("campaign"), $"Campaigns period not selected for the '{env}' environment.");
                }
            }

        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyShowHideMetricsFilterInProd(string env)
        {
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
            Log.Info($"Click on 'Show/Hide Metrics' button for the '{env}' environment.");
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
            if (!NavigateToMultiTeamTeamGrowthJourneyPage(env)) return;
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

            Log.Info("Click on sub-dimension dropdown");
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

        public bool NavigateToMultiTeamTeamGrowthJourneyPage(string env)
        {
            Log.Info($"Login to environment {env} and go to 'Multi-Team' growth journey Tab");
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            var commonGridPage = new GridPage(Driver, Log);

            Log.Info($"Login to {env}, Navigate to 'Team dashboard' page and click on first multiteam from list.");
            LoginToProductionEnvironment(env);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);
            teamDashboardPage.GridTeamView();
            teamDashboardPage.FilterTeamType("Multi-Team");
            if (teamDashboardPage.IsTeamDisplayed() == false)
            {
                Log.Info($"No multi-teams in this environment {env}");
                return false;
            }
            commonGridPage.SortGridColumn("Number of Sub Teams", true);
            teamDashboardPage.SelectFirstTeamFromDashboard();

            Log.Info($"Go to 'Growth Journey' page and switch to timeline view for the '{env}' environment.");
            if (teamAssessmentDashboard.IsSwimLaneDisplayed())
            {
                teamAssessmentDashboard.ClickOnFirstRadarFromAssessmentDashboard();
            }
            else
            {
                Log.Info($"No radars available for the '{env}' environment.");

            }
            if (radarPage.IsGrowthJourneyTabDisplayed() == false)
            {
                Log.Info($"Growth Journey tab not present for the '{env}' environment.");
                return false;
            }
            radarPage.ClickGrowthJourneyTab();
            growthJourneyPage.SwitchToTimelineView();
            return true;
        }
    }
}
