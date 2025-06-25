using AgilityHealth_Automation.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.DataObjects;
using AtCommon.Api;
using AtCommon.Utilities;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Benchmarking;
using System;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BreadCrumbNavigation.Dashboards;
using AgilityHealth_Automation.Enum.Radar;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Radar
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Radars")]
    public class TeamRadarTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyTeamRadarPageNavigationInProd(string env)
        {
            var radarPage = new RadarPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var teamId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.TeamId).ToList()
                .FirstOrDefault();
            var radarId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.Team.RadarId).ToList()
                .FirstOrDefault();

            radarPage.NavigateToTeamRadarPageForProd(env, teamId, radarId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamRadar.png");
            Assert.IsTrue(radarPage.IsRadarIconOnHeaderPresent(), "Team radar is not displayed");
        }


        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyFilterFunctionalityNavigationInProd(string env)
        {
            var radarPage = new RadarPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var benchmarkPopup = new BenchmarkingPopUp(Driver, Log);
            var benchmarkDetail = new BenchmarkingDetailsPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"Verify filter functionality for the '{env}' environment.");
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(radarPage.IsFilterSidebarDescriptionTextDisplayed(), $"Description text is not displayed for the '{env}' environment.");
            Assert.IsTrue(assessmentDetailPage.DoesFilterItemDisplay("All"), $"'All' filter option is not displayed for the '{env}' environment.");

            Log.Info($"Verify radar is present in 'Detail' view  for the '{env}' environment.");
            Assert.IsTrue(radarPage.IsFilterSidebarIconDisplayed(), $"Radar is not displayed in 'Detail' view for the '{env}' environment.");

            Log.Info($"Verify radar is present in 'Summary' view  for the '{env}' environment.");
            assessmentDetailPage.RadarSwitchView(ViewType.Summary);
            Assert.IsFalse(radarPage.IsFilterSidebarIconDisplayed(), $"Radar is not displayed in 'Summary' view for the '{env}' environment.");

            Log.Info($"Verify radar is present in 'Benchmarking' view  for the '{env}' environment.");
            if (!assessmentDetailPage.DoesBenchmarkingViewExist()) return;
            assessmentDetailPage.RadarSwitchView(ViewType.Benchmarking);
            assessmentDetailPage.SelectBenchmarkingOptionsDropdownOption("All Teams");
            benchmarkPopup.ClickSelectButton();
            Assert.AreEqual("Assessment Benchmarking", benchmarkDetail.GetTitle(), $"Benchmarking Title doesn't match for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyExportToExcelInProd(string env)
        {
            var radarPage = new RadarPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"On radar page, Verify export to excel functionality for the '{env}' environment.");
            const string fileName = "Assessment Results.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickExportToExcel();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"'{fileName}' Excel not downloaded successfully for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyExportToPdfInProd(string env)
        {
            var radarPage = new RadarPage(Driver, Log);
            var breadCrumbNavigationPage = new BreadCrumbNavigationPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            var teamName = breadCrumbNavigationPage.GetTeamName();
            var assessmentName = breadCrumbNavigationPage.GetAssessmentName();

            string baseFileName = $"{teamName} {assessmentName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(baseFileName);

            Log.Info($"On radar page, Verify export to PDF functionality for the '{env}' environment.");
            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();

            var exportedFileName = GetDownloadedFileName(baseFileName,env);

            Log.Info($"Verify PDF is downloaded successfully for the '{env}' environment.");
            if (env.Equals("quad", StringComparison.OrdinalIgnoreCase))
            {
                Assert.IsTrue(exportedFileName.StartsWith(baseFileName.Replace("/", "_").Replace(".pdf", ""), StringComparison.OrdinalIgnoreCase) && exportedFileName.EndsWith(".pdf"), $"PDF file with base name '{baseFileName}' is not downloaded successfully for the '{env}' environment.");
            }
            else if (env.Equals("northerntrust", StringComparison.OrdinalIgnoreCase))
            {
                Assert.IsTrue(exportedFileName.StartsWith(baseFileName.Replace(":", "_").Replace(".pdf", ""), StringComparison.OrdinalIgnoreCase) && exportedFileName.EndsWith(".pdf"), $"PDF file with base name '{baseFileName}' is not downloaded successfully for the '{env}' environment.");
            }
            else
            {
                Assert.IsTrue(exportedFileName.StartsWith(baseFileName.Replace(".pdf", ""), StringComparison.OrdinalIgnoreCase) && exportedFileName.EndsWith(".pdf"), $"PDF file with base name '{baseFileName}' is not downloaded successfully for the '{env}' environment.");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyExportQuestionKeyInProd(string env)
        {
            var radarPage = new RadarPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"On radar page, Verify export to question key functionality for the '{env}' environment.");
            const string fileName = "Assessment Results.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickExportQuestionsButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"'{fileName}' Excel not downloaded successfully for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyHideUnHideCommentsInProd(string env)
        {
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"On radar page, Verify hide-unhide comment functionality for the '{env}' environment.");
            if (!assessmentDetailPage.IsHideAllCommentsIconDisplayed()) return;
            var title = assessmentDetailPage.GetHideAllCommentsIconTitleAttribute();

            if (title.Contains("Remove Hidden Comments from View"))
            {
                assessmentDetailPage.ClickOnHideAllCommentsIcon();
                Assert.IsTrue(assessmentDetailPage.GetPopupHeaderText().Contains("Remove Hidden Comments from View"), $"'Remove Hidden Comments from View' text is not displayed in popup for the '{env}' environment.");
            }
            else if (title.Contains("Display Hidden Comments"))
            {
                assessmentDetailPage.ClickOnHideAllCommentsIcon();
                Assert.IsTrue(assessmentDetailPage.GetPopupHeaderText().Contains("Display Hidden Comments"), $"'Display Hidden Comments' text is not displayed in popup for the '{env}' environment.");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyJumpLinksNavigationInProd(string env)
        {
            var radarPage = new RadarPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            var jumpLinks = new Dictionary<string, string>
            {
                { "Radar", "radar" },
                { "Analytics", "analytics" },
                { "Dimension Notes", "notes" },
                { "Strengths", "item0" },
                { "Achievements", "achievements" },
                { "Growth Opportunities", "growth_opportunities" },
                { "Impediments", "impediments" },
                { "Organizational Obstacles", "organizational_obstacles" },
                { "Growth Plan", "growth_plan" },
                { "Metrics Summary", "metrics_summary" },
                { "Idea Board", "ideaboard" }
            };

            foreach (var link in jumpLinks.Where(link => radarPage.IsJumpLinkPresent(link.Key)))
            {
                if (link.Key == "Idea Board")
                {
                    var originalTab = Driver.CurrentWindowHandle;

                    radarPage.ClickOnJumpLink(link.Key);

                    // Wait and switch to the new tab
                    var allTabs = Driver.WindowHandles;
                    foreach (var tab in allTabs)
                    {
                        if (tab == originalTab) continue;
                        Driver.SwitchTo().Window(tab);
                        break;
                    }

                    Assert.IsTrue(Driver.GetCurrentUrl().Contains(link.Value), $"'{link.Key}' is not displayed correctly in the '{env}' environment.");
                    Driver.Close();
                    Driver.SwitchTo().Window(originalTab);
                }
                else
                {
                    radarPage.ClickOnJumpLink(link.Key);
                    Assert.IsTrue(Driver.GetCurrentUrl().Contains(link.Value), $"'{link.Key}' is not displayed correctly in the '{env}' environment.");
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBurnUpAndReleaseHealthChartNavigationInProd(string env)
        {
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"Verify 'Burn Up Chart' & 'Release Health Chart' tab is present for the '{env}' environment.");

            if (assessmentDetailPage.IsMetricsSummaryDisplayed())
            {
                Assert.IsTrue(assessmentDetailPage.MetricsSummary_IsTabPresent("Burn Up Chart"), $"'Burn Up Chart' tab is not present for the '{env}' environment.");
                Assert.IsTrue(assessmentDetailPage.MetricsSummary_IsTabPresent("Release Health Chart"), $"'Release Health Chart' tab is not present for the '{env}' environment.");
            }
            else
            {
                Log.Info($"'Metrics Summary' section is not available in the '{env}' environment.");
            }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGpExportToExcelInProd(string env)
        {
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var breadCrumbNavigationPage = new BreadCrumbNavigationPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"On '{env}' environment, Verify export to excel functionality for growth plan.");

            assessmentDetailPage.SwitchToGridView();
            var teamName = breadCrumbNavigationPage.GetTeamName();
            var assessmentName = breadCrumbNavigationPage.GetAssessmentName();

            var fileName = $"GrowthPlanFor{teamName+"Assessment"}{assessmentName}.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickOnJumpLink("Growth Plan");
            growthItemGridView.ClickExportToExcel();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"'{fileName}' Excel not downloaded successfully for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGpGridViewAddEditNavigationInProd(string env)
        {
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"On '{env}' environment, Verify Add/Edit page navigation");

            assessmentDetailPage.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsTrue(addGrowthItemPopup.IsAddGrowthPlanItemHeaderTextDisplayed(), $"'Add Growth Plan Item' page is not displayed for the '{env}' environment.");
            addGrowthItemPopup.ClickCancelButton();

            if (!assessmentDetailPage.IsGrowthPlanItemDisplayed()) return;
            assessmentDetailPage.ClickOnGrowthItemEditButton();
            Assert.IsTrue(addGrowthItemPopup.IsEditGrowthPlanItemHeaderTextDisplayed(), $"'Edit Growth Plan Item' page is not displayed for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGpGridViewSortingColumnsInProd(string env)
        {
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var giDashboardGridViewPage = new GiDashboardGridWidgetPage(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);

            Log.Info($"On '{env}' environment, Verify sorting functionality for growth plan section.");

            assessmentDetailPage.SwitchToGridView();

            if (!assessmentDetailPage.IsGrowthPlanItemDisplayed()) return;
            giDashboardGridViewPage.SortGridColumn("Category");
            var sortedData = giDashboardGridViewPage.GetColumnValues("Category");

            var filteredData = sortedData.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var expectedSortedData = filteredData.OrderBy(x => x).ToList();

            Assert.AreEqual(expectedSortedData.First(), sortedData.First(), $"The 'Category' column is not sorted as expected for the '{env}' environment.");
            Assert.AreEqual(expectedSortedData.Last(), sortedData.Last(), $"The 'Category' column is not sorted as expected for the '{env}' environment.");
            Assert.IsTrue(sortedData.Count > 0, $"No data was retrieved from the 'Category' column for the '{env}' environment.");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyGpKanbanViewAddEditHistoryAndCustomizeCardNavigationInProd(string env)
        {
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var addEditGrowthItemPopupPage = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);

            LoginAndNavigateToFirstTeamAndRadar(env);
            assessmentDetailPage.SwitchToKanbanView();

            Log.Info($"On '{env}' environment, Verify 'Add' growth item navigation for the '{env}' environment.");
            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            Assert.IsTrue(addEditGrowthItemPopupPage.IsAddGrowthPlanItemHeaderTextDisplayed(), $"'Add Growth Plan Item' page is not displayed for the '{env}' environment.");
            addEditGrowthItemPopupPage.ClickCancelButton();


            Log.Info($"On the '{env}' environment, Verify 'Edit' & 'History' navigation for the '{env}' environment.");
            if (growthItemKanbanView.IsKanbanGiEditIconDisplayed())
            {
                growthItemKanbanView.ClickOnKanbanGiEditIcon();
                Assert.IsTrue(addEditGrowthItemPopupPage.IsEditGrowthPlanItemHeaderTextDisplayed(), $"'Edit Growth Plan Item' page is not displayed for the '{env}' environment.");
                addEditGrowthItemPopupPage.ClickCancelButton();

                growthItemKanbanView.ClickOnKanbanGiHistoryIcon();
                Assert.IsTrue(growthItemKanbanView.IsKanbanGiHistoryPopupDisplayed(), $"'History Growth Plan Item' page is not displayed for the '{env}' environment.");
                growthItemKanbanView.ClickOnHistoryPopupCloseIcon();
            }
            Log.Info($"On '{env}' environment, Verify 'Customize Growth Plan Cards' navigation for the '{env}' environment.");
            growthItemKanbanView.ClickOnCustomizeGrowthPlanCardsIcon();
            Assert.IsTrue(growthItemKanbanView.IsCustomizeCardPopupDisplayed(), $"'Customize Growth Plan Cards' page is not displayed for the '{env}' environment.");

        }
        private void LoginAndNavigateToFirstTeamAndRadar(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var commonGridPage = new GridPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Login to {env}, Navigate to the Team Dashboard page, select the first team from the list, and then click on the first radar in the Assessment Dashboard.");
            LoginToProductionEnvironment(env);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);
            teamDashboardPage.GridTeamView();
            teamDashboardPage.FilterTeamType("Team");
            commonGridPage.SortGridColumn("Number of Team Assessments", true);
            teamDashboardPage.SelectFirstTeamFromDashboard();
            teamAssessmentDashboard.ClickOnFirstRadarFromAssessmentDashboard();
            assessmentDetailPage.RadarSwitchView(ViewType.Detail);
        }
        private string GetDownloadedFileName(string baseFileName, string env)
        {
            var adjustedBaseName = baseFileName.Replace(".pdf", "");

            if (env.Equals("quad", StringComparison.OrdinalIgnoreCase))
            {
                adjustedBaseName = adjustedBaseName.Replace("/", "_");
            }
            else if (env.Equals("northerntrust", StringComparison.OrdinalIgnoreCase))
            {
                adjustedBaseName = adjustedBaseName.Replace(":", "_");
            }

            var downloadFolderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            var files = Directory.GetFiles(downloadFolderPath);
            var sortedFiles = files.OrderByDescending(file => File.GetLastWriteTime(file));

            foreach (var file in sortedFiles)
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith(adjustedBaseName, StringComparison.OrdinalIgnoreCase) && fileName.EndsWith(".pdf"))
                {
                    return fileName;
                }
            }
            throw new FileNotFoundException($"No PDF file with base name '{baseFileName}' found in the download folder.");
        }
    }
}
