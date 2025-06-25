using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Facilitator
{
    public class FacilitatorDashboardPage : BasePage
    {
        public FacilitatorDashboardPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        private static By ExpandFacilitatorLink(string facilitatorName) =>
            By.XPath($"//td[text() = '{facilitatorName}']/preceding-sibling::td[2]/a");

        private readonly By ExpandFacilitatorLinks = By.CssSelector("a.k-plus");

        private static By AssessmentsTable(string facilitatorName) => By.XPath($"//td[text() = '{facilitatorName}']/parent::tr/following-sibling::tr//table");
        private static By AssessmentColumnValue(string assessmentName, string columnName) => By.XPath($"//a[text() = '{assessmentName}']/ancestor::tr[1]/td[count(//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1] | //a//font[text() = '{assessmentName}']/ancestor::tr[1]/td[count(//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");
        private static By AssessmentColumnValue(int mainIndex, int subIndex, string columnName) => By.XPath($"//div[@id = 'GrowthPlan']/table/tbody/tr[{mainIndex}]//tbody/tr[{subIndex}]/td[count(//div[@id = 'GrowthPlan']/table/tbody/tr[{mainIndex}]//thead/tr/th[@data-title='{columnName}']/preceding-sibling::th) + 1]");
        private static By FacilitatorColumnValue(string facilitatorName, string columnName) => By.XPath($"//td[text() = '{facilitatorName}']/ancestor::tr[1]/td[count(//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");
        private static By FacilitatorColumnValue(int index, string columnName) => By.XPath($"//div[@id = 'GrowthPlan']/table/tbody/tr[{index}]/td[count(//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");
        private readonly By ExportButton = By.Id("exportDropDown");
        private readonly By ExportExcelButton = By.CssSelector("#exportDropDown-content button.excelBtn");
        private readonly By FacilitatorRows = By.XPath("//div[@id = 'GrowthPlan']/table/tbody/tr");
        private readonly By FacilitatorActiveInactiveToggle = By.Id("toggelanchor");
        private readonly By FacilitatorDashboardTitle = By.XPath("//h1[contains(text(),'Facilitator Feedback')]");
        private static By AssessmentRows(int rowIndex) => By.XPath($"//div[@id = 'GrowthPlan']/table/tbody/tr[{rowIndex}]//tbody/tr");

        //Key-customers verification
        #region
        private static By ExpandIcon(int rowIndex) => By.XPath($"//tr[@class = 'k-master-row']['{rowIndex}']//a[@class = 'k-icon k-plus']");
        private static By CollapseIcon(int rowIndex) => By.XPath($"//tr[@class = 'k-master-row']['{rowIndex}']//a[@class = 'k-icon k-minus']");
        #endregion

        public void ActiveInactiveFacilitatorToggleButton(bool value)
        {
            Log.Step(nameof(FacilitatorDashboardPage), "Active Or Inactive Facilitators Toggle Button as per value = " + value);
            if (value == (Wait.UntilElementExists(FacilitatorActiveInactiveToggle).GetElementAttribute("class") == "handle ico ease on"))
            {
                Wait.UntilElementClickable(FacilitatorActiveInactiveToggle).Click();
                Wait.UntilJavaScriptReady();
            }
        }

        public bool IsFacilitatorDisplayed(string facilitatorName)
        {

            var locator = By.XPath($"//td[contains(normalize-space(),'{GetFacilitatorName(facilitatorName)}')]");
            return Driver.IsElementDisplayed(locator);
        }

        public void ExpandFacilitator(string facilitatorName)
        {
            Log.Step(nameof(FacilitatorDashboardPage), $"Expand facilitator <{facilitatorName}>");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ExpandFacilitatorLink(GetFacilitatorName(facilitatorName))).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsAssessmentDisplayed(string facilitatorName, string assessmentName)
        {
            var tableElement = Wait.UntilElementVisible(AssessmentsTable(GetFacilitatorName(facilitatorName)));
            return Driver.IsChildElementPresent(tableElement, By.XPath($"//td/a[text() = '{assessmentName}']"));
        }

        public string GetAssessmentColumnValue(string facilitatorName, string assessmentName, string column)
        {
            facilitatorName = GetFacilitatorName(facilitatorName);
            var tableElement = Wait.UntilElementVisible(AssessmentsTable(facilitatorName));
            return tableElement.FindElement(AssessmentColumnValue(assessmentName, column)).GetText();
        }

        public string GetAssessmentColumnValue(int facilitatorRowIndex, int assessmentRowIndex, string columnName)
        {
            return Wait.UntilElementVisible(AssessmentColumnValue(facilitatorRowIndex, assessmentRowIndex, columnName))
                .GetText();
        }

        public string GetFacilitatorColumnValue(string facilitatorName, string column)
        {
            facilitatorName = GetFacilitatorName(facilitatorName);
            return Wait.UntilElementVisible(FacilitatorColumnValue(facilitatorName, column)).GetText();
        }

        public string GetFacilitatorColumnValue(int rowIndex, string column)
        {
            return Wait.UntilElementVisible(FacilitatorColumnValue(rowIndex, column)).GetText();
        }

        public string GetFacilitatorDashboardTitle()
        {
            return Wait.UntilElementVisible(FacilitatorDashboardTitle).GetText();
        }

        public void ClickExportToExcel()
        {
            Log.Step(nameof(FacilitatorDashboardPage), "Click on Export to Excel button");
            SelectItem(ExportButton, ExportExcelButton);
        }

        private static string GetFacilitatorName(string facilitatorName) => facilitatorName.Replace(" (ATI Facilitator)", "");

        public void ExpandAllFacilitatorRows()
        {
            var expandlinks = Wait.UntilAllElementsLocated(ExpandFacilitatorLinks);
            foreach (var link in expandlinks)
            {
                Wait.UntilElementClickable(link).Click();
                Wait.UntilJavaScriptReady();
            }
        }

        public List<List<string>> GetDashboardDataInExcelFormat(List<string> facilitatorColumns, List<string> assessmentColumns)
        {
            // the assessment sub-tables won't exist unless the facilitator row is expanded
            ExpandAllFacilitatorRows();

            var facilitatorRowCount = Wait.UntilAllElementsLocated(FacilitatorRows).Count;
            var rows = new List<List<string>>();

            for (var i = 1; i < facilitatorRowCount + 1; i += 2)
            {
                // get the facilitator info
                var facilitatorRow = facilitatorColumns.Select(column => GetFacilitatorColumnValue(i, column)).ToList();
                rows.Add(facilitatorRow);

                // add the row of assessment column headers
                rows.Add(assessmentColumns);

                // get the Assessment Info
                var assessmentRowCount = Wait.UntilAllElementsLocated(AssessmentRows(i + 1)).Count;
                var rowsAdded = 0;
                for (var j = 1; j < assessmentRowCount + 1; j++)
                {
                    var assessmentRow = assessmentColumns.Select(column => column != "" ? GetAssessmentColumnValue(i + 1, j, column) : "").ToList();

                    // if there were no responses, don't add the row
                    if (assessmentRow.Last() != "0")
                    {
                        rows.Add(assessmentRow);
                        rowsAdded++;
                    }
                }

                // if no assessment rows were added, remove the last row wich contains the column headers
                if (rowsAdded == 0)
                {
                    rows.RemoveAt(rows.Count - 1);
                }
            }

            return rows;
        }

        public void WaitForAssessmentToLoad(string facilitator, string assessmentName)
        {
            for (var i = 0; i < 5; i++)
            {
                ActiveInactiveFacilitatorToggleButton(true);
                if (IsFacilitatorDisplayed(facilitator))
                {
                    ExpandFacilitator(facilitator);
                    if (IsAssessmentDisplayed(facilitator, assessmentName))
                        return;
                }
                Wait.HardWait(60000);
                Driver.RefreshPage();
            }
            throw new Exception($"Facilitator <{facilitator}> with assessment <{assessmentName}> was not found on the dashboard.");
        }

        //Navigation
        public void NavigateToFacilitatorDashboardForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/FacilitatorDashboard");
        }

        //Key-cutomers verification
        #region
        // Method to Expand a specific row
        public bool IsExpanIconDisplayed()
        {
            return Driver.IsElementDisplayed(ExpandIcon(1));
        }

        public void ExpandRow(int rowIndex)
        {
            Log.Step(nameof(FacilitatorDashboardPage), $"Expanding row at index {rowIndex}");
            var expandIcon = Wait.UntilElementExists(ExpandIcon(rowIndex));
            if (expandIcon != null && expandIcon.Displayed)
            {
                expandIcon.Click();
                Wait.UntilJavaScriptReady();
            }
        }

        public void CollapseRow(int rowIndex)
        {
            Log.Step(nameof(FacilitatorDashboardPage), $"Collapsing row at index {rowIndex}");
            var collapseIcon = Wait.UntilElementExists(CollapseIcon(rowIndex));
            if (collapseIcon != null && collapseIcon.Displayed)
            {
                collapseIcon.Click();
                Wait.UntilJavaScriptReady();
            }
        }

        // Method to Check if a row is Expanded        
        public bool IsRowExpanded(int rowIndex)
        {
            return Wait.UntilElementExists(CollapseIcon(rowIndex)).GetAttribute("class").Contains("minus");
        }

        // Method to Check if a row is Collaspe
        public bool IsRowCollapse(int rowIndex)
        {
            return Wait.UntilElementExists(ExpandIcon(rowIndex)).GetAttribute("class").Contains("plus");
        }
        #endregion
    }
}
