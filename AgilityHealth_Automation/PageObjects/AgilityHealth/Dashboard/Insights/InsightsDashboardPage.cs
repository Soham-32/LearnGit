using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights
{
    public class InsightsDashboardPage : BasePage
    {
        public InsightsDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        protected readonly By TeamAgilityTab = AutomationId.Equals("insights-Team-Agility-Tab");
        protected readonly By StructuralAgilityTab = AutomationId.Equals("insights-Structural-Agility-Tab");
        protected readonly By EnterpriseAgilityTab = AutomationId.Equals("insights-Enterprise-Agility-Tab");
        protected readonly By FourLenzTab = AutomationId.Equals("insights-4-Lenz-Dashboard-Tab");
        protected readonly By MyDashboardTab = AutomationId.Equals("insights-My-Dashboard-Tab");
        protected readonly By AskAiAgentTab = AutomationId.Equals("insights-Ask-AI-Agent-Tab");
        protected readonly By OverallInsightsTab = AutomationId.Equals("insights-Overall-Insights-Tab");

        protected readonly By RefreshButton = AutomationId.Equals("refresh_button");
        protected readonly By LastSyncDate = AutomationId.Equals("last_sync");

        protected readonly By TabContainer = By.Id("id_tab_container");

        protected static By Widget(string widgetId) => By.Id($"id_widget_{widgetId}");

        protected static By WidgetOptions(string widgetId) =>
            By.XPath($"//div[@id='id_widget_{widgetId}']//button[contains(@class,'id_widget_options')]");

        protected static By DownloadItem(string item) => By.XPath($"//li[@role='menuitem']/p[text()='{item}']");

        protected readonly By LoadingSpinner =
            By.XPath("//p[text() = 'Loading...']//img[contains(@src, 'load') and contains(@src, '.gif')]");

        protected static By WidgetYAxisValues(string widgetId) => By.CssSelector($"#id_widget_{widgetId} .highcharts-yaxis-labels>text");
        protected static By WidgetLegends(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} .highcharts-legend-item tspan");
        protected static By WidgetTitle(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} h5");
        protected static By EnterpriseAgilityWidgetsTitle(string widgetTitle) =>
            By.XPath($"//div[@id='react-dash-rewamp']//span[@class='ctitle89'][text()='{widgetTitle}']");
        protected static By WidgetSubtitle(string widgetId) =>
            By.CssSelector($"#id_widget_{widgetId} p");

        // methods

        public void ScrollWidgetIntoView(InsightsWidget widget)
        {
            Log.Step(GetType().Name, $"Scrolling widget <{widget.Title}> into view");
            Driver.JavaScriptScrollToElement(Widget(widget.Id), widget.Id.EndsWith("gi"));
        }
        public void ScrollWidgetIntoView(String widgetTitle)
        {
            Log.Step(GetType().Name, $"Scrolling widget <{widgetTitle}> into view");
            Driver.JavaScriptScrollToElement(EnterpriseAgilityWidgetsTitle(widgetTitle));
        }

        public bool IsWidgetDisplayed(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(WidgetTitle(widget.Id)).GetText() == widget.Title;
        }
        public bool IsWidgetDisplayed(String widgetTitle)
        {
            ScrollWidgetIntoView(widgetTitle);
            return Driver.IsElementDisplayed(EnterpriseAgilityWidgetsTitle(widgetTitle));
        }

        public void SelectDownloadOption(InsightsWidget widget, InsightsDownloadOption option)
        {
            ScrollWidgetIntoView(widget);
            Log.Step(GetType().Name, $"Select download option <{option:G}> for widget <{widget.Title}>");

            var item = option switch
            {
                InsightsDownloadOption.Jpeg => "Download JPEG Image",
                InsightsDownloadOption.Pdf => "Download PDF Document",
                InsightsDownloadOption.Png => "Download PNG Image",
                InsightsDownloadOption.Svg => "Download SVG Vector Image",
                _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
            };

            SelectItem(WidgetOptions(widget.Id), DownloadItem(item));
            Wait.HardWait(3000);
        }

        public void ClickOnStructuralAgilityTab(int timeOut = 120)
        {
            Log.Step(GetType().Name, "Click on Structural Agility tab");
            Driver.JavaScriptScrollToElement(StructuralAgilityTab, false);
            Wait.UntilElementClickable(StructuralAgilityTab).Click();
            WaitUntilWidgetsLoaded(timeOut);
        }
        public void ClickOnEnterpriseAgilityTab()
        {
            Log.Step(GetType().Name, "Click on 'Enterprise Agility' tab");
            Wait.UntilElementClickable(EnterpriseAgilityTab).Click();
            CloseDeploymentPopup();
        }
        public void ClickOnTeamAgilityTab()
        {
            Log.Step(GetType().Name, "Click on Team Agility tab");
            Wait.UntilElementClickable(TeamAgilityTab).Click();
        }

        public void ClickOnAskAiAgentTab() {
            Log.Step(GetType().Name, "Click on Ask Ai Agent tab");
            Wait.UntilElementClickable(AskAiAgentTab).Click();
        }
        public void ClickOnOverallInsightsTab() {
            Log.Step(GetType().Name, "Click on Overall Insights tab");
            Wait.UntilElementClickable(OverallInsightsTab).Click();
        }

        public void WaitUntilWidgetsLoaded(int timeOut = 120)
        {
            new LeftNavPage(Driver, Log).WaitUntilLoaded(timeOut);
            Wait.UntilElementVisible(TabContainer);
            new SeleniumWait(Driver, timeOut).UntilElementNotExist(LoadingSpinner);
            Wait.HardWait(1500);
        }
        public void WaitForTeamAgilityPageToLoad()
        {
            Wait.UntilElementVisible(TabContainer);
            Wait.UntilElementNotExist(LoadingSpinner);
        }

        public IList<string> GetWidgetYAxisValues(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Driver.GetTextFromAllElements(WidgetYAxisValues(widget.Id));
        }

        public IList<string> GetWidgetLegends(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Driver.GetTextFromAllElements(WidgetLegends(widget.Id));
        }

        public string GetWidgetTitle(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(WidgetTitle(widget.Id)).GetText();
        }

        public string GetWidgetSubtitle(InsightsWidget widget)
        {
            ScrollWidgetIntoView(widget);
            return Wait.UntilElementExists(WidgetSubtitle(widget.Id)).GetText();
        }

        public void ClickRefreshButton()
        {
            Log.Step(GetType().Name, "Click on 'Refresh' button");
            Wait.UntilElementClickable(RefreshButton).Click();
            Wait.UntilAttributeEquals(RefreshButton, "textContent", "Refreshing");
            Wait.HardWait(10000); // Wait for the refresh
            Wait.UntilAttributeEquals(RefreshButton, "textContent", "Refresh");
            WaitUntilWidgetsLoaded();
        }
        public void ClickRefreshButtonForEnterpriseAgility()
        {
            Log.Step(GetType().Name, "Click on 'Refresh' button");
            Wait.UntilElementClickable(RefreshButton).Click();
        }

        public string GetLastSyncDate()
        {
            return Wait.UntilElementVisible(LastSyncDate).GetText();
        }

        public bool IsTeamAgilityDashboardTabDisplayed()
        {
            return Driver.IsElementDisplayed(TeamAgilityTab);
        }

        public bool IsStructuralAgilityDashboardTabDisplayed()
        {
            return Driver.IsElementDisplayed(StructuralAgilityTab);
        }
        public bool IsEnterpriseAgilityDashboardTabDisplayed()
        {
            return Driver.IsElementDisplayed(EnterpriseAgilityTab);
        }
        public bool IsFourLenzDashboardTabDisplayed()
        {
            return Driver.IsElementDisplayed(FourLenzTab);
        }
        public bool IsMyDashboardTabDisplayed()
        {
            return Driver.IsElementDisplayed(MyDashboardTab);
        }

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/V2/insights/company/{companyId}/parents/0/team/0/tab/teamagility");
            WaitUntilWidgetsLoaded();
            CloseDeploymentPopup();
        }
        public void NavigateToStructuralAgilityPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/V2/insights/company/{companyId}/parents/0/team/0/tab/structuralagility");
            WaitUntilWidgetsLoaded();
            CloseDeploymentPopup();
        }
        public void NavigateToEnterpriseAgilityPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/V2/insights/company/{companyId}/parents/0/team/0/tab/enterpriseagility");
            WaitUntilWidgetsLoaded();
            CloseDeploymentPopup();
        }

        public string GetPageUrl(int companyId)
        {
            return $"{BaseTest.ApplicationUrl}/V2/insights/company/{companyId}/parents/0/team/0/tab/teamagility";
        }
    }


    public class InsightsWidget
    {
        public InsightsWidget(string title, string id)
        {
            Title = title;
            Id = id;
        }
        public InsightsWidget(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
        public string Id { get; set; }
    }

    public enum InsightsDownloadOption
    {
        Png,
        Jpeg,
        Pdf,
        Svg
    }
}
