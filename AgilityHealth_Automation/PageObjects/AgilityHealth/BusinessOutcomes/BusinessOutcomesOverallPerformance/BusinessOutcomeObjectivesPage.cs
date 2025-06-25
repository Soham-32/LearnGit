using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance
{
    public class BusinessOutcomeObjectivesPage : BusinessOutcomeBasePage
    {
        public BusinessOutcomeObjectivesPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region ObjectivesExportToExcel
        private readonly By ExportToExcelButton = By.XPath("//div[@class='k-window-content k-dialog-content']//button[.='Export to Excel']");
        private static By PopupHeaderTitle(string name) =>
            By.XPath($"//div[contains(@class,'k-dialog')]//span[contains(text(),'{name}')]");
        private readonly By OverallProgressText =
            By.XPath("//div[contains(@class,'k-dialog')]//p[contains(text(),'Overall Progress')]");
        private readonly By ChangeSinceDropdownButton =
            By.XPath("//span[contains(text(),'Change Since')]//following-sibling::div");
        private static By TableHeaderData(string name) => By.XPath($"//span[text()='{name}']//ancestor::thead//span[@class='k-cell-inner']");
        private readonly By ObjectiveCloseButton = By.XPath("//button[@aria-label='Close']");
        private static By ObjectivesColumnTitleDropDown(string columnName) => By.XPath($"//span[text()='{columnName}']//parent::span//following-sibling::a/span");
        private readonly By ColumnFilterSortDescendingOption = By.XPath("//div[text()='Sort Descending']");
        private static By CardTitle(string name) => By.XPath($"//p[text()='{name}']");
        private static By Status(string status) => By.XPath($"//p[text()='{status}']//ancestor::tr//td[@aria-colindex='4']//div//span");
        private static By KeyResultTitle(string name) => By.XPath($"//div[text()='{name}']");
        #endregion

        #endregion

        #region ExportToExcel
        public void ClickOnExportToExcel()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on export to excel");
            Wait.UntilElementVisible(ExportToExcelButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsHeaderDisplayed(string name)
        {
            return Driver.IsElementDisplayed(PopupHeaderTitle(name));
        }
        public bool IsOverallProgressTextDisplayed()
        {
            return Driver.IsElementDisplayed(OverallProgressText);
        }
        public void ClickOnChangeSinceDropdownButton()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on 'Change since dropdown'");
            Wait.UntilElementVisible(ChangeSinceDropdownButton).Click();
        }
        public List<string> GetPopupHeaderData(string name)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Get header names of 'Objectives' popup and 'KPIs' popup");
            var rows = Wait.UntilAllElementsLocated(TableHeaderData(name));
            var rowTexts = new List<string>();

            foreach (var row in rows)
            {
                rowTexts.Add(row.Text);
            }

            return rowTexts;
        }

        public void ClickOnObjectiveCloseButton()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on 'Close' button of 'Objectives' and 'KPIs' popup");
            Wait.UntilElementClickable(ObjectiveCloseButton).Click();
        }
        public void ObjectivesSortByDescending(string columnName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Sorting by descending.");
            Wait.UntilElementClickable(ObjectivesColumnTitleDropDown(columnName)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ColumnFilterSortDescendingOption).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsTitleDisplayed(string name)
        {
            return Driver.IsElementDisplayed(CardTitle(name));
        }
        public string GetCardStatus(string name)
        {
            return Wait.UntilElementVisible(Status(name)).GetText();
        }
        public bool IsKeyResultTitleDisplayed(string name)
        {
            return Driver.IsElementDisplayed(KeyResultTitle(name));
        }
        #endregion
    }
}
