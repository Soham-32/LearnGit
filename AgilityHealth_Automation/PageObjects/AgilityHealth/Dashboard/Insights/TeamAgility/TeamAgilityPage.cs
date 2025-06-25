using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility
{
    internal class TeamAgilityPage : InsightsDashboardPage
    {
        public TeamAgilityPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private static By IndexWidgetMaturityText(string widgetId) =>
            By.XPath($"//div[@id = 'id_widget_{widgetId}']//div[@dir='ltr']//span/div[@class = 'pivot-text1']");
        private static By WidgetNoData(string widgetId) =>
            By.XPath($"//div[@id = 'id_widget_{widgetId}']//*[contains(text(), 'There is no data to display.')]");

        private readonly By RadarFilter = AutomationId.Equals("id_filter_radar");
        private readonly By BenchmarkFilter = AutomationId.Equals("id_filter_benchmark", "div");
        private readonly By NumberOfQuartersFilter = AutomationId.Equals("id_filter_number of quarters", "div");
        private readonly By LastQuarterFilter = AutomationId.Equals("id_filter_last quarter", "div");
        private readonly By CampaignsDropdown = AutomationId.Equals("id_filter_campaigns");
        private readonly By ViewByCampaigns = By.XPath("//p[text()='View By Campaigns']/../following-sibling::span");
        private readonly By ViewByQuarters = By.XPath("//p[text()='View By Quarters']/../following-sibling::span");
        private readonly By TeamAgilityTabText = By.XPath("//button[text()='Team Agility']");

        private readonly By AgilityIndexPieChartSelect = By.XPath(
            $"//div[@id='id_widget_{TeamAgilityWidgets.AgilityIndex.Id}']//div[contains(@class, 'container')]//*[contains(@class, 'band ')][1]");
        private static By GenericListItem(string item) =>
            By.XPath($"//*[@role='listbox']//*[contains(text(), '{item}')]");
        private readonly By GenericListItems = By.XPath("//ul[@role='listbox']/li/p");
        private static By WidgetPieChartSelect(string widgetId) =>
            By.XPath($"(//div[@id='id_widget_{widgetId}']//*[text()=1])[1]");
        private static By WidgetPieChartSelect0(string widgetId) =>
            By.XPath($"//div[@id='id_widget_{widgetId}']//div[text()=0] | //div[@id='id_widget_{widgetId}']//div//font[text()=0]");
        private static By WidgetBarSelect(string widgetId, int index = 1) => By.XPath(
            $"//div[@id='id_widget_{widgetId}']//*[not(contains(@class,'legend'))]/*[contains(@class, 'highcharts-point')][{index}]");
        private static By WidgetBarXaxisLabels(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} .highcharts-xaxis-labels text");
        private static By WidgetSortButton(string widgetId) => By.XPath(
            $"//div[@id='id_widget_{widgetId}']//div[contains(@class, 'id_widget_dimensions_sort')]//p");
        private static By WidgetSortButtonListItem(string item) =>
            By.XPath($"//div[@id='menu-']//li/p[text()='{item}'] | //div[@id='menu-']//li/p//font[text()='{item}']");
        private static By GiWidgetShowFilterText(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} div.id_widget_xgi_show p");
        private static By GrowthItemStage(string widgetId) =>
            By.XPath($"//div[@id='id_widget_{widgetId}']//*[contains(@class, 'legend-item')]");
        private static By GiWidgetSegmentByFilterText(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} div.id_widget_xgi_segmentby p");
        private static By GiWidgetGiCount(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} div.id_widget_xgi_segmentby + div");


        // Methods

        public string GetIndexWidgetMaturity(InsightsWidget widget)
        {
            if (Wait.InCase(IndexWidgetMaturityText(widget.Id))?.Displayed ?? false)
            {
                return Wait.UntilElementExists(IndexWidgetMaturityText(widget.Id)).GetText();
            }

            throw new Exception($"<{widget.Title}> maturity is not displayed.");
        }

        public bool IsWidgetDataNotDisplayed(InsightsWidget widget)
        {
            return Wait.InCase(WidgetNoData(widget.Id))?.Displayed ?? false;
        }

        public string GetRadarText()
        {
            return Wait.UntilElementExists(RadarFilter).GetText();
        }

        public void SelectRadarItem(string item)
        {
            Log.Step(nameof(TeamAgilityPage), $"Select item <{item}> from Radar dropdown");
            if (GetRadarText() != item)
                SelectItem(RadarFilter, GenericListItem(item));
            WaitUntilWidgetsLoaded();
        }

        public string GetBenchmarkText()
        {
            return Wait.UntilElementExists(BenchmarkFilter).GetText();
        }

        public void SelectBenchmark(string item)
        {
            Log.Step(nameof(TeamAgilityPage), $"Select item <{item}> from Benchmark dropdown");
            if (GetBenchmarkText() != item)
                SelectItem(BenchmarkFilter, GenericListItem(item));
            WaitUntilWidgetsLoaded();
        }

        public string GetNumberOfQuartersText() => Wait.UntilElementExists(NumberOfQuartersFilter).GetText();

        public void SelectNumberOfQuarters(string item)
        {
            Log.Step(nameof(TeamAgilityPage), $"Select item <{item}> from Number of Quarters dropdown");
            if (GetNumberOfQuartersText() != $"{item} Quarters")
                SelectItem(NumberOfQuartersFilter, GenericListItem(item));
            WaitUntilWidgetsLoaded();
        }

        public string GetLastQuarterText() => Wait.UntilElementExists(LastQuarterFilter).GetText();

        public void SelectLastQuarter(string item)
        {
            Log.Step(nameof(TeamAgilityPage), $"Select item <{item}> from Last Quarter dropdown");
            if (GetLastQuarterText() != item)
                SelectItem(LastQuarterFilter, GenericListItem(item));
            WaitUntilWidgetsLoaded();
        }
        public string GetTeamAgilityTabText()
        {
            return Wait.UntilElementVisible(TeamAgilityTabText).GetText();
        }
        //Widget Sort Box
        public void SelectWidgetSortHighToLow(InsightsWidget widget)
        {
            Log.Step(nameof(TeamAgilityPage), $"Select item 'Highest to Lowest' for widget <{widget.Title}>");
            if (GetSortText(widget) != "Highest to Lowest")
                SelectItem(WidgetSortButton(widget.Id), WidgetSortButtonListItem("Highest to Lowest"));
            WaitUntilWidgetsLoaded();
        }

        public void SelectWidgetSortLowToHigh(InsightsWidget widget)
        {
            Log.Step(nameof(TeamAgilityPage), $"Select item 'Lowest to Highest' for widget <{widget.Title}>");
            if (GetSortText(widget) != "Lowest to Highest")
                SelectItem(WidgetSortButton(widget.Id), WidgetSortButtonListItem("Lowest to Highest"));
            WaitUntilWidgetsLoaded();
        }

        public string GetSortText(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(WidgetSortButton(widget.Id)).GetText();
        }

        //Widget Modals
        public void OpenWidgetModal(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            Log.Step(nameof(TeamAgilityTab), $"Open widget modal for widget <{widget.Title}>");
            switch (widget.Title)
            {
                case "Maturity Index":
                case "Performance Index":
                    Wait.UntilElementClickable(WidgetPieChartSelect(widget.Id)).Click();
                    break;
                case "Agility Index":
                    Wait.UntilElementClickable(AgilityIndexPieChartSelect).Click();
                    break;
                default:
                    Wait.UntilElementClickable(WidgetBarSelect(widget.Id)).Click();
                    break;
            }
            Wait.UntilJavaScriptReady();
        }

        public void ClickChartBar(InsightsWidget widget, string barLabel)
        {
            ScrollWidgetIntoView(widget);
            Log.Step(nameof(TeamAgilityTab), $"Click on bar <{barLabel}> for <{widget.Title}>");
            var labels = Wait.UntilAllElementsLocated(WidgetBarXaxisLabels(widget.Id));
            var index = labels.ToList().FindIndex(l => l.Text.RemoveWhitespace() == barLabel.RemoveWhitespace());
            if (index < 0) throw new Exception($"{barLabel} was not found");

            Wait.UntilElementClickable(WidgetBarSelect(widget.Id, index + 1)).Click();

        }

        public void Select0OnWidgetPieChart(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            Log.Step(nameof(TeamAgilityTab), $"Click on the first section in the pie chart for widget <{widget.Title}>");
            Wait.UntilElementClickable(WidgetPieChartSelect0(widget.Id)).Click();
            WaitUntilWidgetsLoaded();
        }



        //Widget Show Filter
        public string GetGiStatusFilterText(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(GiWidgetShowFilterText(widget.Id)).GetText();
        }

        public void SelectGiStatusFilter(InsightsWidget widget, string listItem)
        {
            Log.Step(nameof(TeamAgilityTab), $"Select Growth Item Status item <{listItem}> for widget <{widget.Title}>");
            if (GetGiStatusFilterText(widget) != listItem)
                SelectItem(GiWidgetShowFilterText(widget.Id), GenericListItem(listItem));
            WaitUntilWidgetsLoaded();
        }

        public string GetGiStageLegendText(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(GrowthItemStage(widget.Id)).GetText();
        }


        //Widget Segment By Filter
        public string GetSegmentByFilterText(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(GiWidgetSegmentByFilterText(widget.Id)).GetText();
        }

        public void SelectGiSegmentByFilter(InsightsWidget widget, string listItem)
        {
            Log.Step(nameof(TeamAgilityTab), $"Select Growth Item Segment item <{listItem}> for widget <{widget.Title}>");
            if (GetSegmentByFilterText(widget) != listItem)
                SelectItem(GiWidgetSegmentByFilterText(widget.Id), GenericListItem(listItem));
            WaitUntilWidgetsLoaded();
        }

        private IList<string> GetListItems() => Driver.GetTextFromAllElements(GenericListItems);

        public IList<string> GetNumberOfQuartersList()
        {
            Wait.UntilAllElementsLocated(NumberOfQuartersFilter);
            Wait.UntilElementClickable(NumberOfQuartersFilter).Click();
            var items = GetListItems();
            Wait.UntilAllElementsLocated(GenericListItems)[0].Click();

            return items;
        }

        public IList<string> GetLastQuarterList()
        {
            Wait.UntilAllElementsLocated(NumberOfQuartersFilter);
            Wait.HardWait(3000);//Waiting for dropdown loading
            Wait.UntilElementClickable(LastQuarterFilter).Click();
            var items = GetListItems();
            Wait.UntilAllElementsLocated(GenericListItems)[0].Click();

            return items;
        }

        public string GetGrowthItemCount(InsightsWidget widget)
        {
            return Wait.UntilElementVisible(GiWidgetGiCount(widget.Id)).GetText();
        }

        public void SwitchToViewByCampaigns()
        {
            Log.Step(nameof(TeamAgilityTab), "Switch to View By Campaigns");
            if (Driver.IsElementDisplayed(ViewByCampaigns)) return;
            Wait.UntilElementClickable(ViewByQuarters).Click();
        }

        public bool DoesViewByCampaignsDisplay()
        {
            return Driver.IsElementDisplayed(ViewByCampaigns);
        }

        public bool DoesViewByQuartersDisplay()
        {
            return Driver.IsElementDisplayed(ViewByQuarters);
        }

        public bool DoesCampaignsDropdownDisplay()
        {
            return Driver.IsElementDisplayed(CampaignsDropdown);
        }

        //Navigation
        public void NavigateToInsightsTeamAgilityPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/V2/insights/company/{companyId}/parents/0/team/0/tab/teamagility");
        }

    }


    public static class TeamAgilityWidgets
    {
        public static InsightsWidget AgilityIndex = new InsightsWidget("Agility Index", "agilityindex");
        public static InsightsWidget AgilityIndexOverTime = new InsightsWidget("Agility Index Over Time", "agilityovertime");
        public static InsightsWidget AgilityDimensions = new InsightsWidget("Agility Dimensions", "agilitydimensions");
        public static InsightsWidget MaturityIndex = new InsightsWidget("Maturity Index", "maturityindex");
        public static InsightsWidget MaturityOverTime = new InsightsWidget("Maturity Over Time", "maturityovertime");
        public static InsightsWidget MaturityDimensions = new InsightsWidget("Maturity Dimensions", "maturitydimensions");
        public static InsightsWidget PerformanceIndex = new InsightsWidget("Performance Index", "performanceindex");
        public static InsightsWidget PerformanceOverTime = new InsightsWidget("Performance Over Time", "performanceovertime");
        public static InsightsWidget PerformanceMetrics = new InsightsWidget("Performance Metrics", "performancemetrics");
        public static InsightsWidget TeamGrowthItems = new InsightsWidget("Team Growth Items", "teamgi");
        public static InsightsWidget OrganizationalGrowthItems = new InsightsWidget("Organizational Growth Items", "organizationgi");
        public static InsightsWidget EnterpriseGrowthItems = new InsightsWidget("Enterprise Growth Items", "enterprisegi");


        public static List<InsightsWidget> GetList()
        {
            return new List<InsightsWidget>
            {
                AgilityIndex,
                AgilityIndexOverTime,
                AgilityDimensions,
                MaturityIndex,
                MaturityOverTime,
                MaturityDimensions,
                PerformanceIndex,
                PerformanceMetrics,
                PerformanceOverTime,
                TeamGrowthItems,
                OrganizationalGrowthItems,
                EnterpriseGrowthItems
            };
        }
    }
}