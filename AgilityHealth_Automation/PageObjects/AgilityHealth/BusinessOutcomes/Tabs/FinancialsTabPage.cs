using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using OpenQA.Selenium;
using System.Collections.Generic;
using AtCommon.Utilities;
using OpenQA.Selenium.Interactions;


namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class FinancialsTabPage : BaseTabPage
    {
        public FinancialsTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Locators

        private readonly By RequestedBudget = By.XPath("//p[text()='Requested Budget:']/following-sibling::div//input[@type='text']");
        private readonly By ApprovedBudget = By.XPath("//p[text()='Approved Budget:']/following-sibling::div//input[@type='text']");
        private readonly By BudgetCategory = By.XPath("//p[text()='Budget Category:']/following::input[1]");
        private readonly By TotalSpent = By.XPath("//p[text()='Total Spent:']//following-sibling::p");
        private readonly By CalculationMethod = By.XPath("//p[text()='Calculation Method:']/following::input[1]");
        private static By MethodAndCategoryValue(string value) => By.XPath($"//li[text()='{value}']");
        private readonly By AddFinancialRowButton = By.XPath("//button[contains(text(),'Add Spend')]");
        private readonly By FinancialRows = By.XPath("//th[text()='Target Spend']//ancestor::table//tr[not(contains(@class, 'MuiTableRow-head'))]");
        private readonly By TableViewButton = AutomationId.Equals("bo-overallprogress-Table-View-Tab");

        private readonly By GraphViewButton = AutomationId.Equals("bo-overallprogress-Graph-View-Tab");
        private static By TargetSpendInput(int targetSpendRowIndex) => By.XPath($"//th[text()='Target Spend']/ancestor::table//tr[{targetSpendRowIndex}]//td[1]//input[@type='text']");
        private static By CurrentSpendInput(int currentSpendRowIndex) => By.XPath($"//th[text()='Target Spend']/ancestor::table//tr[{currentSpendRowIndex}]//td[2]//input[@type='text']");
        private static By AddMonth(string month) => By.XPath($"//button[text()='{month}']");
        private static By AddYear(string year) => By.XPath($"//button[text()='{year}']");
        private static By DateColumnValues(int dateIndex) => By.XPath($"(//input[@placeholder='mmmm yyyy'])[{dateIndex}]");
        private static By CalendarIcon(int calendarIconRowIndex) => By.XPath($"//table//tr[{calendarIconRowIndex}]//button[contains(@aria-label,'Choose date')]");

        private readonly By GraphHighChartPoints = By.XPath("//*[local-name()='g' and contains(@class, 'highcharts-series-0')]//*[local-name()='path' and contains(@class, 'highcharts-point')]");
        private readonly By GraphTooltips = By.XPath("//*[local-name()='g' and contains(@class, 'highcharts-tooltip')]//*[local-name()='text']");
        private readonly By GraphTooltipTextValues = By.XPath("./*[local-name()='tspan'and contains(@style, 'font-weight:bold')]");

        #endregion

        #region Actions

        public void ClickOnAddFinancialRowButton()
        {
            Log.Step(nameof(FinancialsTabPage), "Click on 'Add Financial Row' button");
            ClickOnTableViewTab();
            Wait.UntilElementExists(AddFinancialRowButton).Click();
        }

        public void ClickOnTableViewTab()
        {
            Log.Step(nameof(FinancialsTabPage), "Click on 'Table View' Tab");
            Driver.JavaScriptScrollToElement(TableViewButton);
            Wait.UntilElementClickable(TableViewButton).Click();
        }

        public void ClickOnGraphViewTab()
        {
            Log.Step(nameof(FinancialsTabPage), "Click on 'Graph View' Tab");
            Wait.UntilElementClickable(GraphViewButton).Click();
        }

        public void AddFinancialBudgetData(Financial financial)
        {
            Log.Step(nameof(FinancialsTabPage), "Add Financial Budget Data");
            Wait.UntilElementClickable(RequestedBudget).SendKeys(financial.RequestedBudget.ToString());
            Wait.UntilElementVisible(ApprovedBudget).SetText(financial.ApprovedBudget.ToString());
            Wait.UntilElementClickable(BudgetCategory).Click();
            Wait.UntilElementVisible(MethodAndCategoryValue(financial.BudgetCategory)).Click();
            Wait.UntilTextToBePresent(Wait.UntilElementVisible(TotalSpent), "0");
            Wait.UntilElementClickable(CalculationMethod).Click();
            Wait.UntilElementVisible(MethodAndCategoryValue(financial.CalculationMethod)).Click();
        }

        public void AddFinancialSpendData(BusinessOutcomeFinancialRequest financialData)
        {
            Log.Step(nameof(FinancialsTabPage), $"Adding financial data: TargetSpend={financialData.SpendingTarget}, CurrentSpend={financialData.CurrentSpent}, Date={financialData.FinancialAsOfDate}");
            ClickOnAddFinancialRowButton();
            var newRowIndex = Driver.GetElementCount(FinancialRows);
            Wait.UntilElementVisible(TargetSpendInput(newRowIndex)).SetText(financialData.SpendingTarget.ToString());
            Wait.UntilElementVisible(CurrentSpendInput(newRowIndex)).SetText(financialData.CurrentSpent.ToString());
            Wait.UntilElementClickable(CalendarIcon(newRowIndex)).Click();
            Wait.UntilElementClickable(AddMonth(financialData.FinancialAsOfDate?.ToString("MMM"))).Click();
            Wait.UntilElementClickable(AddYear(financialData.FinancialAsOfDate?.ToString("yyyy"))).Click();
        }

        public Financial GetFinancialBudgetData()
        {
            Log.Step(nameof(FinancialsTabPage), "Retrieving financial rows data");
                var requestedBudget = Wait.UntilElementVisible(RequestedBudget).GetAttribute("value");
                var approvedBudget = Wait.UntilElementVisible(ApprovedBudget).GetAttribute("value");
                var budgetCategory = Wait.UntilElementVisible(BudgetCategory).GetAttribute("value");
                var totalSpent = Wait.UntilElementVisible(TotalSpent).GetText().Replace("$","");
                var calculationMethod = Wait.UntilElementVisible(CalculationMethod).GetAttribute("value");

            return new Financial
            {
                RequestedBudget = requestedBudget.ToInt(),
                ApprovedBudget = approvedBudget.ToInt(),
                BudgetCategory = budgetCategory,
                TotalSpent = totalSpent.ToInt(),
                CalculationMethod = calculationMethod
            };
        }
       
        public List<Dictionary<string, string>> GetFinancialSpendDataRows()
        {
            Log.Step(nameof(FinancialsTabPage), "Retrieving financial rows data");
            var rows = Wait.UntilAllElementsLocated(FinancialRows);
            var financialRows = new List<Dictionary<string, string>>();

            for (var i = 1; i <= rows.Count; i++)
            {
                var targetSpend = Wait.UntilElementVisible(TargetSpendInput(i)).GetAttribute("value");
                var currentSpend = Wait.UntilElementVisible(CurrentSpendInput(i)).GetAttribute("value");
                var date = Wait.UntilElementVisible(DateColumnValues(i)).GetAttribute("value");

                financialRows.Add(new Dictionary<string, string>
                {
                    { "TargetSpend", targetSpend },
                    { "CurrentSpend", currentSpend },
                    { "Date", date }
                });
            }
            return financialRows;
        }

        public List<FinancialGraphData> GetFinancialGraphData()
        {
            Log.Step(nameof(FinancialsTabPage), "Retrieving financial Graph data");
            var actions = new Actions(Driver);
            var dataList = new List<FinancialGraphData>();

            // Use local-name to handle SVG tags
            var points = Driver.FindElements(GraphHighChartPoints);

            foreach (var point in points)
            {
                if (point.Displayed)
                {
                    var attempts = 0;
                    while (attempts < 3)
                    {
                        if (point.Displayed)
                        {
                            actions.MoveToElement(point).Perform();
                        }
                        attempts++;
                    }
                }
                var tooltipData = GetTooltipData();   
                dataList.Add(tooltipData);
            }

            return dataList;
        }

        public FinancialGraphData GetTooltipData()
        {
            Log.Step(nameof(FinancialsTabPage), "Retrieving financial Graph tooltip Values");
            return CSharpHelpers.HandleStaleElement(() =>
            {
                // Wait for the tooltip <text> element to appear inside SVG
                var tooltipElements = Wait.UntilElementVisible(GraphTooltips);

                var graphTooltipValues =
                    tooltipElements.FindElements(GraphTooltipTextValues);

                return new FinancialGraphData
                {
                    MonthYear = graphTooltipValues[0].GetAttribute("textContent"),
                    TargetSpend = graphTooltipValues[1].GetAttribute("textContent"),
                    CurrentSpend = graphTooltipValues[2].GetAttribute("textContent")
                };
            });
        }

        #endregion
    }
}
