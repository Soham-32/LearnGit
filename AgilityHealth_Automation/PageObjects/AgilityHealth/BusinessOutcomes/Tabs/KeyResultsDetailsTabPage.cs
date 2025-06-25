
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.BusinessOutcomes.Custom;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class KeyResultsDetailsTabPage : BusinessOutcomeBasePage
    {
        public KeyResultsDetailsTabPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        private readonly By DescriptionTextArea = By.XPath("//textarea[@placeholder='Description']");
        private readonly By FormulaTextArea = By.XPath("//textarea[@placeholder='Formula']");
        private readonly By SourceTextArea = By.XPath("//textarea[@placeholder='Source']");
        private readonly By FrequencyDropdown = By.XPath("//p[contains(text(),'Frequency')]/../following-sibling::div");
        private static By FrequencyListItem(string frequency) => By.XPath($"//li[text()='{frequency}']");
        private readonly By CommentTextArea = By.XPath("//textarea[@placeholder='Comment']");
        private readonly By AddSubTargetLink = By.XPath("//span[contains(text(),'Add Sub Target')]/..");
        private readonly By EndDatePickerInput = By.XPath("(//p[contains(text(),'End Date')]//following-sibling::div//input)[1]");
        private readonly By GraphViewTab = By.XPath("//button[contains(text(),'Graph View')]");
        private static By SubTargetInputByRow(int rowIndex) => By.XPath($"//table//tr[{rowIndex + 1}]//td[2]//input");
        private static By ByWhenDatePickerByRow(int rowIndex) => By.XPath($"//table//tr[{rowIndex + 1}]//td[3]//input[contains(@placeholder,'mm/dd/yyyy')]");
        private readonly By SubTargetDots = By.XPath("//*[contains(@class, 'highcharts-point')]");
        private readonly By SubTargetDotTooltipValue = By.XPath("//*[local-name()='g' and @class ='highcharts-label highcharts-tooltip highcharts-color-undefined']//*[local-name()='tspan' and text()=' Target: ']//..//*[local-name()='tspan']");
        #endregion

        #region Methods

        public  void EnterSubTargetDescription(string description)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Enter Description");
            Wait.UntilElementVisible(DescriptionTextArea).SendKeys(description);
        }

        public void EnterFormula(string formula)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Enter Formula");
            Wait.UntilElementVisible(FormulaTextArea).SendKeys(formula);
        }

        public void EnterSource(string source)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Enter Source");
            Wait.UntilElementVisible(SourceTextArea).SendKeys(source);
        }

        public void SelectFrequency(string frequency)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), $"Select Frequency: {frequency}");
            SelectItem(FrequencyDropdown, FrequencyListItem(frequency));
        }


        public void EnterComment(string comment)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Enter Comment");
            Wait.UntilElementVisible(CommentTextArea).SendKeys(comment);
        }

        public void ClickAddSubTarget()
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Click on Add Sub Target");
            Wait.UntilElementClickable(AddSubTargetLink).Click();
        }
        public void SetEndDate(string date)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), $"Set End Date: {date}");
            Wait.UntilElementClickable(EndDatePickerInput).SetText(date, isReact: true);
        }

        public void EnterSubTarget(int rowIndex, string value)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), $"Enter Sub Target at row {rowIndex}");
            var input = Wait.UntilElementVisible(SubTargetInputByRow(rowIndex));
            input.Clear();
            input.SendKeys(value);
        }

        public void SetSubTargetByWhenDate(int rowIndex, string date)
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), $"Set 'By When' date at row {rowIndex}");
            Wait.UntilElementClickable(ByWhenDatePickerByRow(rowIndex)).Click();
            var input = Wait.UntilElementClickable(ByWhenDatePickerByRow(rowIndex));
            input.Clear();
            input.SetText(date, isReact: true);
        }


        public void ClickGraphViewTab()
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Click on 'Graph View' tab");
            Wait.UntilElementClickable(GraphViewTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void FillKeyResultDetails(BusinessOutcomeDetailsTabs businessOutcomesDetails)
        {
            EnterSubTargetDescription(businessOutcomesDetails.Description);
            EnterFormula(businessOutcomesDetails.Formula);
            EnterSource(businessOutcomesDetails.Source);
            SelectFrequency(businessOutcomesDetails.Frequency);
            EnterComment(businessOutcomesDetails.Comment);
            SetEndDate(businessOutcomesDetails.EndDate);

            foreach (var (value, date, i) in businessOutcomesDetails.SubTargets.Select((v, i) => (v.SubTargetValue, v.ByWhenDate, i)))
            {
                ClickAddSubTarget();
                EnterSubTarget(i, value);
                SetSubTargetByWhenDate(i, date);
            }

        }

        public Dictionary<string, string> GetSubTargetTooltipData()
        {
            Log.Step(nameof(KeyResultsDetailsTabPage), "Get SubTarget Tooltip Data from Highcharts");

            var result = new Dictionary<string, string>();
            var dots = Driver.FindElements(SubTargetDots);

            foreach (var dot in dots)
            {
                var actions = new Actions(Driver);
                actions.MoveToElement(dot).Perform();

                var tooltipElements = Driver.FindElements(SubTargetDotTooltipValue).ToList();
                if (tooltipElements.Count == 0)
                {
                    actions.MoveToElement(dot).Perform();
                    tooltipElements = Driver.FindElements(SubTargetDotTooltipValue).ToList();
                }

                if (tooltipElements.Count >= 4)
                {
                    var date = tooltipElements[0].Text.Trim();    // e.g., "Apr 2025"
                    var value = tooltipElements[3].Text.Trim();   // e.g., "Target: 80"

                    if (!result.ContainsKey(date))
                    {
                        result.Add(date, value);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
