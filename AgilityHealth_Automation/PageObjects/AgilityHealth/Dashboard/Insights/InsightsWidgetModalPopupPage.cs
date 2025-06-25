using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights
{
    internal class InsightsWidgetModalPopupPage : InsightsDashboardPage
    {
        public InsightsWidgetModalPopupPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By WidgetModalCLoseButton = By.XPath("//button[@aria-label='close']");
        private static By MaturityWidgetModalTitle(string widgetName) => By.XPath($"//h6[contains(text(),'{widgetName}')] | //h6//font[contains(text(),'{widgetName}')]");
        private static By WidgetModalMaturity(string maturity) => By.XPath($"//h5[contains(text(), '{maturity}')]");
        private static By WidgetModalColumn(string columnName) =>
            By.XPath($"//div[@role='dialog']//th//p[text()='{columnName}']");
        private static By RowValues(string columnName) => By.XPath(
            $"//div[@role = 'dialog']//tbody//td[count(//div[@role = 'dialog']//th//p[text() = '{columnName}']/ancestor::th[1]/preceding-sibling::th) + 1]");

        private readonly By FilterTextbox = By.XPath("//label[text() = 'Search / Filter']/following-sibling::div/input");
        private readonly By ExportExcelButton = AutomationId.Equals("export-excel-button");

        public bool IsMaturityWidgetModalTitleDisplayed(string title)
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementPresent(MaturityWidgetModalTitle(title)) &&
                   Wait.UntilElementExists(MaturityWidgetModalTitle(title)).Displayed;
        }

        public bool IsMaturityWidgetModalMaturityDisplayed()
        {
            return Driver.IsElementPresent(WidgetModalMaturity("PRE-CRAWL")) &&
                   Wait.UntilElementExists(WidgetModalMaturity("PRE-CRAWL")).Displayed;
        }

        public bool IsColumnDisplayed(string columnName)
        {
            return Driver.IsElementPresent(WidgetModalColumn(columnName)) &&
                   Wait.UntilElementExists(WidgetModalColumn(columnName)).Displayed;
        }

        public void SelectColumnToSort(string columnName)
        {
            Log.Step(nameof(InsightsWidgetModalPopupPage), 
                $"Click on column <{columnName}>");
            Wait.UntilElementClickable(WidgetModalColumn(columnName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetRowValues(string columnName)
        {
            Log.Step(nameof(InsightsWidgetModalPopupPage), 
                $"Get the values of column <{columnName}>");
            return Driver.GetTextFromAllElements(RowValues(columnName)).ToList();
        }

        public void CloseWidgetModal()
        {
            Log.Step(nameof(InsightsWidgetModalPopupPage), 
                "Click the close button on the widget modal");
            Wait.UntilElementClickable(WidgetModalCLoseButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void Search(string searchTerm)
        {
            Log.Step(nameof(InsightsWidgetModalPopupPage), 
                $"Entering search term <{searchTerm}> in the filter textbox");
            Wait.UntilElementClickable(FilterTextbox).SetText(searchTerm);
            try
            {
                Wait.UntilElementNotExist(RowValues("Title"), 2);
            }
            catch (WebDriverTimeoutException)
            {}
            Wait.UntilElementExists(RowValues("Title"));
        }

        public void ClickExportToExcelButton()
        {
            Log.Step(nameof(InsightsWidgetModalPopupPage), "Clicking on the 'Excel' button");
            Wait.UntilElementExists(RowValues("Title"));
            Wait.UntilElementClickable(ExportExcelButton).Click();
        }
    }
}
