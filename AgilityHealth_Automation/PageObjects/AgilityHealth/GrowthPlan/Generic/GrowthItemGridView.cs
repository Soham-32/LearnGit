using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Generic
{
    public class GrowthItemGridView : BasePage
    {

        public GrowthItemGridView(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        protected readonly By GrowthPlanSectionMarker = By.Id("growth_plan");
        private static By GiColumnHeader(string columnHeader) => By.XPath($"//th[@data-role='columnmenu'][(@style='')or(@style='color: rgb(0, 0, 0);')or not (@style)][@data-title='{columnHeader}']");
        private readonly By GiColumnHeaderDropOutArea = By.XPath("//div[@id='GrowthPlan']/div[@data-role='droptarget']");
        private readonly By ClearFilterButton = By.CssSelector("a.k-grid-ClearFilters");
        private readonly By ColumnMenu = By.CssSelector("span.k-i-arrowhead-s");
        private readonly By ColumnItem = By.CssSelector("li.k-columns-item");
        private readonly By VisibleColumns = By.CssSelector("ul[style*='display: block'] span.k-link");
        private readonly By ColumnHeaders = By.XPath("//th[@role='columnheader'][(@style='')or(@style='color: rgb(0, 0, 0);')or not (@style)][not(@data-field='Action')]");
        private readonly By GridRows = By.CssSelector("div#GrowthPlan tbody>tr");
        private static By GiRowValueByColumn(string giTitle, string columnName) => By.XPath(
            $"//div[@id='GrowthPlan']//tbody//tr/td[text()='{giTitle}']/../td[count(//div[@id='GrowthPlan']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");

        private static By VisibleColumnItem(string item) => By.XPath(
            $"//ul[contains(@style, 'display: block')]//span[contains(@class,'k-link')][contains(normalize-space(), '{item}')]/input");

        private static By ColumnValues(string columnHeader) => By.XPath(
            $"//div[@id='GrowthPlan']//tbody//tr/td[count(//th[@role='columnheader'][@data-title='{columnHeader}']//preceding-sibling::th) + 1]");

        private static By ColumnHeader(string header) =>
            By.XPath($"//th[@role='columnheader'][@style='' or not(@style) or (@style='color: rgb(0, 0, 0);')]//a[text()='{header}'] | //th[@role='columnheader'][@style='' or not(@style) or (@style='color: rgb(0, 0, 0);')]//a//font[text()='{header}']");

        private static By RowValues(int index, string columnHeader) => By.XPath(
            $"//div[@id='GrowthPlan']//tbody//tr[{index}]/td[count(//th[@role='columnheader'][@style='' or not(@style)]//a[@class='k-link'][text()='{columnHeader}']/../preceding-sibling::th | //th[text() = '{columnHeader}']/preceding-sibling::th) + 1]");


        public void DragAndDropColumn(string columnName, int xOffset, int yOffset, bool scrollToGrowthPlanSection = false)
        {
            Wait.UntilElementClickable(GiColumnHeader(columnName));
            Wait.UntilJavaScriptReady();
            var giHeaderDrag = Driver.JavaScriptScrollToElement(GiColumnHeader(columnName));
            var giHeaderDropOut = Driver.JavaScriptScrollToElement(GiColumnHeaderDropOutArea);
            if (scrollToGrowthPlanSection)
            {
                Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            }
            Driver.DragElementToElement(giHeaderDrag, giHeaderDropOut, xOffset, yOffset);
            Wait.UntilJavaScriptReady();
        }

        public void ClearFilter(bool style= true)
        {
            Log.Step(nameof(GrowthItemGridView), "Clear filter");
            if (style)
            {
                var str = Wait.UntilElementExists(ClearFilterButton).GetAttribute("style");
                if (!str.Contains("block")) return;
            }
            Wait.UntilElementClickable(ClearFilterButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsClearFiltersButtonDisplayed() => Driver.IsElementDisplayed(ClearFilterButton);

        public void ClickColumnMenu()
        {
            Log.Step(nameof(GrowthItemGridView), "Click column menu");
            foreach (var ele in Wait.UntilAllElementsLocated(ColumnMenu))
            {
                if (!ele.Displayed) continue;
                ele.Click();
                Wait.HardWait(1000); // need to wait until hover on columns
                if (!Driver.IsElementDisplayed(ColumnItem))
                {
                    ele.Click();
                }
                break;
            }
            Wait.UntilJavaScriptReady();
        }

        public void ClickColumnsMenuItem()
        {
            Log.Step(nameof(GrowthItemGridView), "Click column menu item");
            Wait.UntilElementVisible(ColumnItem).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectColumns(List<string> columns)
        {
            Log.Step(nameof(GrowthItemGridView), $"Select columns <{string.Join(",", columns)}>");
            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
                var elementText = Driver.MoveToElement(ele).GetText();
                var element = Driver.MoveToElement(Wait.UntilElementVisible(VisibleColumnItem(elementText)));
                element.Check(columns.Contains(elementText));
            }
        }

        public void AddColumns(List<string> columns)
        {
            Log.Step(nameof(GrowthItemGridView), $"Add columns <{string.Join(",", columns)}>");
            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
                var elementText = Driver.MoveToElement(ele).GetAttribute("textContent");
                var element = Wait.UntilElementVisible(VisibleColumnItem(elementText));

                if (!columns.Contains(elementText)) continue;

                element.Check();
                Wait.UntilJavaScriptReady();
            }
        }

        public void AddSelectedColumns(List<string> columns)
        {
            ClickColumnMenu();
            ClickColumnsMenuItem();
            AddColumns(columns);
            ClickColumnMenu();
        }

        public List<string> GetColumnHeaders()
        {
            var rawHeaders = Wait.UntilAllElementsLocated(ColumnHeaders).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
            return rawHeaders.Select(header => header.Replace("Column Settings\r\n", "")).ToList();
        }

        public int GetNumberOfGridRows()
        {
            return Wait.UntilAllElementsLocated(GridRows).Count;
        }

        public List<string> GetColumnValues(string columnHeader)
        {
            return Wait.UntilAllElementsLocated(ColumnValues(columnHeader)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        public List<string> GetRowValues(int index, List<string> columns)
        {
            return columns.Select(column => Driver.JavaScriptScrollToElement(Wait.UntilElementExists(RowValues(index, column))).GetText()).ToList();
        }

        public void SortGridColumn(string columnName)
        {
            Log.Step(nameof(GrowthItemGridView), $"Sort grid column <{columnName}>");
            Wait.UntilElementClickable(ColumnHeader(columnName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetGrowthItemValue(string giTitle, string columnName)
        {
            return Wait.UntilElementExists(GiRowValueByColumn(giTitle, columnName)).GetText();
        }
    }
}