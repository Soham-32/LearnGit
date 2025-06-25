using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;
using System;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure
{
    internal class GrowthItemGridViewPage : AddGrowthItemPopupBasePage
    {
        public GrowthItemGridViewPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;
        private static By GiRow(string giTitle) => By.XPath($"//div[@id = 'GrowthPlan']//table/tbody//td[text()='{giTitle}']/..");
        private static By GiRowValueByColumn(string giTitle, string columnName) => By.XPath($"//div[@id='GrowthPlan']//tbody//tr/td[text()='{giTitle}']/../td[count(//div[@id='GrowthPlan']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");
        private static By GiEditButtonFromGrid(string giTitle) =>
            By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[text()='{giTitle}']/following-sibling::td//li[contains(@id, 'menu_Edit')]");

        public GrowthItem GetGrowthItemFromGrid(string giTitle)
        {
            Wait.UntilJavaScriptReady(Timeout);
            var targetDate = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Target Date"))
            .GetAttribute("textContent");
            var date = !string.IsNullOrEmpty(targetDate) ? DateTime.Parse(targetDate) : (DateTime?)null;

            var growthItem = new GrowthItem
            {
                Title = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Title")).GetAttribute("textContent"),
                Status = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Status")).GetAttribute("textContent"),
                Priority =
            Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Priority")).GetAttribute("textContent"),
                Description = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Description"))
            .GetAttribute("textContent"),
                Category =
            Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Category")).GetAttribute("textContent").Trim(),
                Type = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Type")).GetAttribute("textContent"),
                TargetDate = date,
                Size = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Size")).GetAttribute("textContent"),
                Color = CSharpHelpers
            .ConvertRgbToHex(Wait.UntilElementExists(GiRow(giTitle)).GetCssValue("background-color")).ToLower(),
            };
            return growthItem;
        }

        public void ClickGrowthItemEditButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), $"Click on Edit button for <{giTitle}>");
            Wait.UntilElementClickable(GiEditButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public bool IsGiPresent(string title) => Driver.IsElementPresent(GiRow(title), 15);
    }
}