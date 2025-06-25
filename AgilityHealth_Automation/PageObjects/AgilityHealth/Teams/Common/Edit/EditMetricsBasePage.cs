using System;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Integrations.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit
{

    public class EditMetricsBasePage : BasePage
    {
        public EditMetricsBasePage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Header
        private readonly By GetDataButton = By.XPath("//button[text()='Get Data']");
        private readonly By RecalculateMetricsButton = By.XPath("//button[text()='Recalculate Metrics']");

        //Jira Logo
        private readonly By JiraLogo = By.CssSelector("img[src='/images/jira_software.png']");

        //Table
        private static By Team(string teamName) => By.XPath($"//table//td[text()='{teamName}']");

        private static By JiraBoard(string teamName) => By.XPath($"//td[text()='{teamName}']//following-sibling::td");
        // Iteration Data
        private readonly By AddNewIterationDataButton = By.CssSelector("#iteration-data-grid a.k-grid-add");
        private readonly By IterationBarExpanded = By.CssSelector("#upDownIconIterationGrid");

        // Iteration Data Grid
        private readonly By IterationGrid = By.Id("IterationGrid");
        private static By IterationDataEditButton(int index) =>
            By.XPath($"//div[@id='iteration-data-grid']//table//tr[{index}]//a[contains(@class,'k-grid-edit')]");
        private static By IterationDataName(int index) => By.XPath($"//div[@id='iteration-data-grid']//table/tbody/tr[{index}]/td[2]");
        private static By IterationDataFrom(int index) => By.XPath($"//div[@id='iteration-data-grid']//table/tbody/tr[{index}]/td[3]");
        private static By IterationDataTo(int index) => By.XPath($"//div[@id='iteration-data-grid']//table/tbody/tr[{index}]/td[4]");
        private static By IterationDataCommittedPoints(int index) => By.XPath($"//div[@id='iteration-data-grid']//table/tbody/tr[{index}]/td[5]");
        private static By IterationDataCompletedPoints(int index) => By.XPath($"//div[@id='iteration-data-grid']//table/tbody/tr[{index}]/td[6]");
        private static By IterationDataEscapedDefect(int index) => By.XPath($"//div[@id='iteration-data-grid']//table/tbody/tr[{index}]/td[11]");

        // Edit Iteration popup
        private const string FromDateCalendarId = "From_dateview";
        private const string ToDateCalendarId = "To_dateview";
        private readonly By TargetPointTextbox = By.Id("TargetPoints");
        private readonly By TargetPointTextboxMark = By.XPath("//input[@id='TargetPoints']/preceding-sibling::input");
        private readonly By ActualPointsTextbox = By.Id("ActualPoints");
        private readonly By ActualPointsTextboxMark = By.XPath("//input[@id='ActualPoints']/preceding-sibling::input");
        private readonly By DefectsTextbox = By.Id("TotalDefects");
        private readonly By DefectsTextboxMark = By.XPath("//input[@id='TotalDefects']/preceding-sibling::input");
        private readonly By TotalScopeTextbox = By.Id("TotalScope");
        private readonly By TotalScopeTextboxMark = By.XPath("//input[@id='TotalScope']/preceding-sibling::input");

        // Release Data
        private readonly By AddNewReleaseDataButton = By.CssSelector("#release-data-grid a.k-grid-add");
        private readonly By ReleaseBarExpanded = By.CssSelector("#upDownIconReleaseGrid");

        // Release Data Grid
        private readonly By ReleaseGrid = By.Id("ReleaseGrid");
        private static By ReleaseDataEditButton(int index) =>
            By.XPath($"//div[@id='release-data-grid']//table//tr[{index}]//a[contains(@class,'k-grid-edit')]");
        private static By ReleaseDataName(int index) => By.XPath($"//div[@id='release-data-grid']//table/tbody/tr[{index}]/td[2]");
        private static By ReleaseDataTargetDate(int index) => By.XPath($"//div[@id='release-data-grid']//table/tbody/tr[{index}]/td[3]");
        private static By ReleaseDataActualDate(int index) => By.XPath($"//div[@id='release-data-grid']//table/tbody/tr[{index}]/td[4]");
        private static By ReleaseDataEscapedDefects(int index) => By.XPath($"//div[@id='release-data-grid']//table/tbody/tr[{index}]/td[12]");

        // Edit Release popup
        private const string TargetDateCalendarId = "TargetDate_dateview";
        private const string ActualDateCalendarId = "ActualDate_dateview";
        private readonly By ReleaseActualPointsTextbox = By.Id("ActualPoints");
        private readonly By ReleaseActualPointsTextboxMark = By.XPath("//input[@id='ActualPoints']/preceding-sibling::input");

        //Performance Measurements Data Grid
        private readonly By CalculationDate = By.XPath("//div[@id='performanceMeasurementDataGrid']//td");
        private static By CalculatedValue(string metricName) => By.XPath($"//th[text()='{metricName} ']//following-sibling::td");
        private static By NormalizedValue(string metricName) => By.XPath($"//th[text()='{metricName} ']//following-sibling::td[2]");

        // shared between the popups
        private readonly By NameTextbox = By.Id("Name");
        private readonly By UpdateButton = By.CssSelector("a.k-grid-update");

        //Header
        public bool IsGetDataButtonPresent()
        {
            return Driver.IsElementPresent(GetDataButton);
        }

        public void ClickOnGetDataButton()
        {
            Log.Step(nameof(EditMetricsBasePage), "Click on the 'Get Data' button.");
            Wait.UntilElementClickable(GetDataButton).Click();
            Wait.HardWait(90000);//It takes at least 1.5 minutes to get the Jira data on the metrics tab.
        }

        public bool IsRecalculateMetricsButtonDisplayed()
        {
            return Driver.IsElementDisplayed(RecalculateMetricsButton);
        }

        public void ClickOnRecalculateMetricsButton()
        {
            Log.Step(nameof(EditMetricsBasePage), "Click on the 'Recalculate Metrics' button.");
            Wait.UntilElementClickable(RecalculateMetricsButton).Click();
            Wait.HardWait(5000); //It takes some time to recalculate the performance measurements metrics.
        }

        //Jira Logo
        public bool IsJiraLogoPresent()
        {
            return Driver.IsElementPresent(JiraLogo);
        }

        //Table
        public string GetTeamName(string teamName)
        {
            return Wait.UntilElementVisible(Team(teamName)).GetText();
        }

        public string GetJiraBoardName(string teamName)
        {
            return Wait.UntilElementVisible(JiraBoard(teamName)).GetText();
        }

        // Iteration Data methods
        public void ClickAddNewIterationDataButton()
        {
            Log.Step(GetType().Name, "On Edit Team page, Metrics tab, click Add New Iteration Data button");
            ExpandIteration();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(AddNewIterationDataButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ExpandIteration()
        {
            Wait.UntilJavaScriptReady();
            var iterationGrid = Wait.UntilElementExists(IterationGrid);

            if (!iterationGrid.Displayed)
            {
                Wait.UntilElementClickable(IterationBarExpanded).Click();
                Wait.UntilJavaScriptReady();
            }
        }

        public void ClickIterationDataEditButton(int index)
        {
            Log.Step(GetType().Name, $"On Edit Team page, Metrics tab, click edit Iteration at index ${index}");
            Wait.UntilElementClickable(IterationDataEditButton(index)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void EnterIterationData(Iteration iteration)
        {
            Log.Step(GetType().Name, "On Edit Team page, Metrics tab, New Iteration popup, enter Iteration data");
            if (!string.IsNullOrEmpty(iteration.TotalScope))
            {
                ClickClearSet(TotalScopeTextboxMark, TotalScopeTextbox, iteration.TotalScope);
            }

            if (!string.IsNullOrEmpty(iteration.Defects))
            {
                ClickClearSet(DefectsTextboxMark, DefectsTextbox, iteration.Defects);
            }

            if (!string.IsNullOrEmpty(iteration.CompletedPoints))
            {
                ClickClearSet(ActualPointsTextboxMark, ActualPointsTextbox, iteration.CompletedPoints);
            }

            if (!string.IsNullOrEmpty(iteration.CommittedPoints))
            {
                ClickClearSet(TargetPointTextboxMark, TargetPointTextbox, iteration.CommittedPoints);
            }

            if (!string.IsNullOrEmpty(iteration.From))
            {
                var fromDate = DateTime.Parse(iteration.From);
                var fromCal = new CalendarWidget(Driver, FromDateCalendarId);
                fromCal.SetDate(fromDate);
            }

            if (!string.IsNullOrEmpty(iteration.To))
            {
                var toDate = DateTime.Parse(iteration.To);
                var toCal = new CalendarWidget(Driver, ToDateCalendarId);
                toCal.SetDate(toDate);
            }

            if (!string.IsNullOrEmpty(iteration.Name))
            {
                Wait.UntilElementClickable(NameTextbox).SetText(iteration.Name);
            }

            ClickUpdateButton();
        }

        public Iteration GetIterationDataFromGrid(int index)
        {
            var iterationData = new Iteration
            {
                CompletedPoints = Wait.UntilElementVisible(IterationDataCompletedPoints(index)).Text,
                Defects = Wait.UntilElementVisible(IterationDataEscapedDefect(index)).Text,
                From = Wait.UntilElementVisible(IterationDataFrom(index)).Text,
                Name = Wait.UntilElementVisible(IterationDataName(index)).Text,
                CommittedPoints = Wait.UntilElementVisible(IterationDataCommittedPoints(index)).Text,
                To = Wait.UntilElementVisible(IterationDataTo(index)).Text
            };

            return iterationData;
        }

        // release data methods
        public void ClickAddNewReleaseDataButton()
        {
            Log.Step(GetType().Name, "On Edit Team page, Metrics tab, click Add New Release Data button");
            ExpandRelease();
            Wait.UntilElementClickable(AddNewReleaseDataButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ExpandRelease()
        {
            var releaseGrid = Wait.UntilElementExists(ReleaseGrid);
            if (!releaseGrid.Displayed)
            {
                Wait.UntilElementClickable(ReleaseBarExpanded).Click();
                Wait.UntilJavaScriptReady();
            }
        }

        public void EnterReleaseData(Release release)
        {
            Log.Step(GetType().Name, "On Edit Team page, Metrics tab, New Release popup, enter Release data");
            if (!string.IsNullOrEmpty(release.Defects))
            {
                ClickClearSet(DefectsTextboxMark, DefectsTextbox, release.Defects);
            }

            if (!string.IsNullOrEmpty(release.ActualPoints))
            {
                ClickClearSet(ReleaseActualPointsTextboxMark, ReleaseActualPointsTextbox, release.ActualPoints);
            }

            if (!string.IsNullOrEmpty(release.TargetPoints))
            {
                ClickClearSet(TargetPointTextboxMark, TargetPointTextbox, release.TargetPoints);
            }

            if (!string.IsNullOrEmpty(release.TargetDate))
            {
                var targetDate = DateTime.Parse(release.TargetDate);
                var targetCal = new CalendarWidget(Driver, TargetDateCalendarId);
                targetCal.SetDate(targetDate);
            }

            if (!string.IsNullOrEmpty(release.ActualDate))
            {
                var actualDate = DateTime.Parse(release.ActualDate);
                var actualCal = new CalendarWidget(Driver, ActualDateCalendarId);
                actualCal.SetDate(actualDate);
            }

            if (!string.IsNullOrEmpty(release.Name))
            {
                Wait.UntilElementClickable(NameTextbox).SetText(release.Name);
            }

            ClickUpdateButton();
        }

        public Release GetReleaseDataFromGrid(int index)
        {
            var release = new Release
            {
                Name = Wait.UntilElementVisible(ReleaseDataName(index)).Text,
                TargetDate = Wait.UntilElementVisible(ReleaseDataTargetDate(index)).Text,
                ActualDate = Wait.UntilElementVisible(ReleaseDataActualDate(index)).Text,
                Defects = Wait.UntilElementVisible(ReleaseDataEscapedDefects(index)).Text
            };

            return release;
        }

        public void ClickReleaseDataEditButton(int index)
        {
            Log.Step(GetType().Name, $"On Edit Team page, Metrics tab, click edit Release at index ${index}");
            Wait.UntilElementClickable(ReleaseDataEditButton(index)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickUpdateButton()
        {
            Wait.UntilElementClickable(UpdateButton).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }

        //Performance Measurements Data Grid
        public string GetPerformanceMeasurementsCalculationDate()
        {
            Log.Step(GetType().Name, "Get the Performance Measurements calculation date.");
            return Wait.UntilElementVisible(CalculationDate).GetText();
        }

        public PerformanceMeasurements GetPerformanceMeasurementsDataFromGrid()
        {
            var performanceMeasurements = new PerformanceMeasurements()
            {
                //CalculatedFeatureThroughput = Wait.UntilElementVisible(CalculatedValue("Feature Throughput")).GetText(),
                CalculatedFeatureCycleTime = Wait.UntilElementVisible(CalculatedValue("Feature Cycle Time")).GetText(),
                CalculatedDeploymentFrequency = Wait.UntilElementVisible(CalculatedValue("Deployment Frequency")).GetText(),
                CalculatedDefectRatio = Wait.UntilElementVisible(CalculatedValue("Defect Ratio")).GetText(),
                CalculatedPredictability = Wait.UntilElementVisible(CalculatedValue("Predictability")).GetText(),
                //NormalizedFeatureThroughput = Wait.UntilElementVisible(NormalizedValue("Feature Throughput")).GetText(),
                NormalizedFeatureCycleTime = Wait.UntilElementVisible(NormalizedValue("Feature Cycle Time")).GetText(),
                NormalizedDeploymentFrequency = Wait.UntilElementVisible(NormalizedValue("Deployment Frequency")).GetText(),
                NormalizedDefectRatio = Wait.UntilElementVisible(NormalizedValue("Defect Ratio")).GetText(),
                NormalizedPredictability = Wait.UntilElementVisible(NormalizedValue("Predictability")).GetText()
            };

            return performanceMeasurements;
        }

        public string NormalizeCalculatedPredictability(string calculatedPredictability)
        {
            var normalizedPredictability = int.Parse((calculatedPredictability.Split(" ".ToCharArray()).ToList())[0]);
            return normalizedPredictability <= 50 ? "Pre - Crawl" :
                normalizedPredictability <= 70 ? "Crawl" :
                normalizedPredictability <= 80 ? "Walk" :
                normalizedPredictability <= 90 ? "Run" :
                "Fly";
        }

        public string NormalizeCalculatedDefectRatio(string calculatedDefectRatio)
        {
            var normalizedDefectRatio = int.Parse((calculatedDefectRatio.Split(" ".ToCharArray()).ToList())[0]);
            return normalizedDefectRatio >= 50 ? "Pre - Crawl" :
                normalizedDefectRatio >= 30 ? "Crawl" :
                normalizedDefectRatio >= 20 ? "Walk" :
                normalizedDefectRatio >= 5 ? "Run" :
                "Fly";
        }

        public string NormalizeCalculatedDeploymentFrequency(string calculatedDeploymentFrequency)
        {
            var deploymentFrequency = int.Parse((calculatedDeploymentFrequency.Split("".ToCharArray()).ToList())[0]);
            var normalizedDeploymentFrequency = (calculatedDeploymentFrequency.Contains("Weeks")) ? deploymentFrequency * 7 : deploymentFrequency;
            return normalizedDeploymentFrequency >= 90 ? "Pre - Crawl" :
                normalizedDeploymentFrequency >= 30 ? "Crawl" :
                normalizedDeploymentFrequency >= 7 ? "Walk" :
                normalizedDeploymentFrequency >= 4 ? "Run" :
                "Fly";
        }

        public string NormalizeCalculatedFeatureCycleTime(string calculatedFeatureCycleTime)
        {
            var featureCycleTime = int.Parse((calculatedFeatureCycleTime.Split("".ToCharArray()).ToList())[0]);
            var normalizedFeatureCycleTime = (calculatedFeatureCycleTime.Contains("Weeks")) ? featureCycleTime * 7 : featureCycleTime;
            return normalizedFeatureCycleTime <= 14 ? "Fly" :
                normalizedFeatureCycleTime <= 28 ? "Run" :
                normalizedFeatureCycleTime <= 42 ? "Walk" :
                normalizedFeatureCycleTime <= 56 ? "Crawl" :
                "Pre - Crawl";
        }


        public PerformanceMeasurements NormalizedPerformanceMeasurementsCalculations(PerformanceMeasurements calculatedValues)
        {
            var performanceMeasurements = new PerformanceMeasurements()
            {
                NormalizedDeploymentFrequency = NormalizeCalculatedDeploymentFrequency(calculatedValues.CalculatedDeploymentFrequency),
                NormalizedFeatureCycleTime = NormalizeCalculatedFeatureCycleTime(calculatedValues.CalculatedFeatureCycleTime),
                NormalizedDefectRatio = NormalizeCalculatedDefectRatio(calculatedValues.CalculatedDefectRatio),
                NormalizedPredictability = NormalizeCalculatedPredictability(calculatedValues.CalculatedPredictability)
            };

            return performanceMeasurements;
        }

        public void ClickClearSet(By clickLocator, By textBoxLocator, string inputText)
        {
            Wait.UntilElementClickable(clickLocator).Click();
            Wait.UntilElementClickable(textBoxLocator).Clear();
            Wait.UntilElementClickable(clickLocator).Click();
            Wait.UntilElementClickable(textBoxLocator).SendKeys(inputText);
        }

        //Navigate
        public void NavigateToPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/metrics/{teamId}");
        }
    }
}
