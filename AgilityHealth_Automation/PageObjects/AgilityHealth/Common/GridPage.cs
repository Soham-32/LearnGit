using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Common
{
    public class GridPage : BasePage
    {
        public GridPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By ColumnMenu = By.CssSelector("span.k-i-arrowhead-s");
        private readonly By ColumnItem = By.CssSelector("li.k-columns-item");
        private readonly By VisibleColumns = By.CssSelector("ul[style*='display: block'] span.k-link");
        private readonly By ColumnHeaders = By.XPath("//th[@role='columnheader'][(@style='')or(@style='color: rgb(0, 0, 0);')or not (@style)][not(@data-field='Action')][@data-field]");
        private readonly By GridRows = By.CssSelector("div#GrowthPlan tbody>tr");

        private static By VisibleColumnItem(string item) => By.XPath(
            $"//ul[contains(@style, 'display: block')]//span[contains(@class,'k-link')][text()='{item}']/input | //ul[contains(@style, 'display: block')]//span[contains(@class,'k-link')]//font[text()='{item}']/ancestor::span/input");
        private static By ColumnValue(string columnHeader) => By.XPath($"//div[@id='gridView']//tbody/tr/td[count(//div[@id='gridView']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1] | //div[@id='gridView']//table//tr/th[count(//div[@id='gridView']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]");
        private static By GridColumnHeader(string columnName) => By.CssSelector($"th[data-title='{columnName}'] a:last-child");
        private static By RowValues(int index, string columnHeader) => By.XPath(
            $"//div[@id='GrowthPlan']//tbody//tr[{index}]/td[count(//th[@role='columnheader']/a[text()='{columnHeader}']/../preceding-sibling::th | //th[text() = '{columnHeader}']/preceding-sibling::th) + 1]");

        //Key Customer Verification

        #region Gi Elements
        private static By GrowthItemColumnValues(string columnHeader) => By.XPath($"//div[@id='gridWrapper']//th[count(//div[@id='gridWrapper']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]//ancestor::table//tbody//td[6]");

        #endregion

        public void ClickColumnMenu()
        {
            Log.Step(nameof(GridPage), "Click column menu");
            Wait.UntilJavaScriptReady();
            foreach (var ele in Wait.UntilAllElementsLocated(ColumnMenu))
            {
                if (!ele.Displayed) continue;
                ele.Click();
                Wait.HardWait(2000); // Need to wait until column dropdown gets open
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
            Log.Step(nameof(GridPage), "Click column menu item");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(ColumnItem).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectColumns(List<string> columns, By by)
        {
            Log.Step(nameof(GridPage), $"Select columns <{string.Join(",", columns)}>");
            foreach (var ele in Wait.UntilAllElementsLocated(by))
            {
                var elementText = Driver.MoveToElement(ele).GetText();
                var element = Driver.MoveToElement(Wait.UntilElementVisible(VisibleColumnItem(elementText)));
                element.Check(columns.Contains(elementText));
            }
        }
        public void AddSelectedColumns(List<string> columns, By by)
        {
            ClickColumnMenu();
            ClickColumnsMenuItem();
            SelectColumns(columns, by);
            ClickColumnMenu();
        }
        public void SelectCustomColumns(List<string> columns)
        {
            ClickColumnMenu();
            ClickColumnsMenuItem();
            //Log.Step(nameof(GiGridPage), $"Select columns <{string.Join(",", columns)}>");
            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
                var elementText = Driver.MoveToElement(ele).GetText();
                var element = Driver.MoveToElement(Wait.UntilElementVisible(VisibleColumnItem(elementText)));
                element.Check(columns.Contains(elementText));
            }
            ClickColumnMenu();
        }

        public List<string> GetColumnHeaders()
        {
            var rawHeaders = Wait.UntilAllElementsLocated(ColumnHeaders).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
            return rawHeaders.Select(header => header.Replace("Column Settings\r\n", "")).ToList();
        }

        public int GetNumberOfGridRows()
        {
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(GridRows).Count;
        }

        public List<string> GetColumnValues(string columnHeader)
        {
            Log.Info("Get all column values");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(ColumnValue(columnHeader)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }
        public List<string> GetColumnPercentageValues(string columnHeader)
        {
            Log.Info("Get all column's percentage values");
            var getColumnValueList = Wait.UntilAllElementsLocated(ColumnValue(columnHeader)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
            return getColumnValueList.Select(columnValue => !columnValue.Equals("")
                    ? columnValue.Split(' ').LastOrDefault().GetDigits().ToString()
                    : "")
                .ToList();
        }
        public List<string> GetRowValues(int index, List<string> columns)
        {
            return columns.Select(column => Driver.JavaScriptScrollToElement(Wait.UntilElementExists(RowValues(index, column))).GetText().RemoveWhitespace()).ToList();
        }
        public void SortGridColumn(string columnName, bool isDescending = false)
        {
            Log.Step(nameof(GridPage), $"Sort grid column <{columnName}>");
            Wait.UntilElementClickable(GridColumnHeader(columnName)).Click();
            Wait.UntilJavaScriptReady();
            if (isDescending)
            {
                Wait.UntilElementClickable(GridColumnHeader(columnName)).Click();
            }
        }

        public IList<string> FormatColumnDates(IList<string> actualColumnText)
        {
            for (var i = 0; i < actualColumnText.Count; i++)
            {
                if (!string.IsNullOrEmpty(actualColumnText[i]))
                {
                    actualColumnText[i] = DateTime.Parse(actualColumnText[i]).ToString("yyyy/MM/dd");
                }
            }

            return actualColumnText;
        }

        //Key Customer Verification

        #region Gi Elements
        public List<string> GetGrowthItemsColumnValues(string columnHeader)
        {
            Log.Info("Get all growth items column values");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(GrowthItemColumnValues(columnHeader)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        #endregion
    }
}