using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList
{
    internal class AssessmentDashboardListTabPage : AssessmentDashboardBasePage
    {
        public AssessmentDashboardListTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        public By AssessmentDashboardVisibleColumns = By.XPath("//ul//li[@class='k-item k-state-default']//span[@class='k-link']");
        private readonly By AssessmentLoadIcon = By.XPath("//div[@class='k-loading-image']");
        private readonly By AssessmentTypeDropdown = By.CssSelector("span[aria-owns='ddlAssessmentType_listbox']");
        private readonly By SearchTextBox = By.Id("teamFilterBox1");
        private static By AssessmentTypeListItem(string item) => By.XPath($"//ul[@id = 'ddlAssessmentType_listbox']/li[text() = '{item}'] | //ul[@id = 'ddlAssessmentType_listbox']/li//font[text() = '{item}']");
        private readonly By AssessmentTypeLIstItemAll = By.XPath("//div[@id='ddlAssessmentType-list']/div[text()='All'] | //div[@id='ddlAssessmentType-list']/div//font[text()='All']");
        private readonly By ColumnMenu = By.XPath("//th[@data-field='AssessmentId'][1]/a");
        private readonly By ColumnItem = By.CssSelector("li.k-columns-item");
        private readonly By VisibleColumns = By.CssSelector("ul[style*='display: block'] span.k-link");
        private static By VisibleColumnItem(string item) => By.XPath(
            $"//ul[contains(@style, 'display: block')]//span[@class='k-link'][text()='{item}']/input");
        private static By ColumnValues(string columnHeader) => By.XPath(
            $"//div[@id='assessmentManagement']//tbody//tr/td[count(//th[@data-role='columnsorter'][@style='' or not(@style)]//a[@class='k-link'][text()='{columnHeader}']/../preceding-sibling::th) + 1]");
        private readonly By GridRows = By.CssSelector("div#assessmentManagement tbody>tr");
        private static By RowValues(int index, string columnHeader) => By.XPath(
            $"//div[@id='assessmentManagement']//tbody//tr[{index}]/td[count(//th[@data-role='columnsorter'][@style='' or not(@style)]//a[@class='k-link'][text()='{columnHeader}']/../preceding-sibling::th) + 1]");
        private readonly By ColumnHeaders = By.XPath("//th[@data-role='columnsorter'][@style='' or not(@style)]/a[@class='k-link']");

        private readonly By EditButtons =
            By.XPath("//span[contains(@class,'k-link')][contains(normalize-space(),'Edit')]");
        private readonly By EditMenuButton = By.XPath("//span[@class = 'k-link'][text() = 'Edit']/parent::li/following-sibling::li/span | //font[text() = 'Edit']/ancestor::li/following-sibling::li/span");
        private readonly By EditMenuDelete = By.XPath("//li[starts-with(@id, 'menu_Delete')]/span");
        private readonly By ExportButton = By.Id("exportDropDown");
        private readonly By ExportExcelButton = By.CssSelector("#exportDropDown-content button.excelBtn");
        private readonly By BatchListItemAll = By.XPath("//div[@id='ddlBatchName-list']/div[text()='All'] | //div[@id='ddlBatchName-list']/div//font[text()='All']");
        private readonly By BatchFilterDropdown = By.CssSelector("span[aria-owns='ddlBatchName_listbox']");
        private static By TeamAssessmentName(string assessmentName) => By.XPath($"*//div[@id='assessmentManagement']//table//tr//td[text()='{assessmentName}']");
        private readonly By AssessmentFilterMessage = By.Id("teamsSelected");
        private readonly By ManageAssessmentsTitle = By.XPath("//h1[text()='Manage Assessments']");

        //delete assessment popup
        private readonly By DeleteAssessmentPopup = By.Id("showDeleteView");
        private readonly By DeleteAssessmentYesButton = By.Id("do_SaveAssessment");

        private static By BatchFilterListItem(string item) => By.XPath($"//ul[@id='ddlBatchName_listbox']/li[text()='{item}']");

        public void WaitForAssessmentPageToLoad()
        {
            Wait.UntilElementNotExist(AssessmentLoadIcon);
        }
        public void FilterByAssessmentType(string assessmentType)
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Filter by assessment type <{assessmentType}>");
            Wait.UntilJavaScriptReady();
            var locator = assessmentType.Equals("All") ? AssessmentTypeLIstItemAll : AssessmentTypeListItem(assessmentType);
            SelectItem(AssessmentTypeDropdown, locator);
            Wait.UntilJavaScriptReady();
        }

        public void FilterByBatch(string batch)
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Filter by batch <{batch}>");
            Wait.UntilJavaScriptReady();
            var locator = (batch.Equals("All")) ? BatchListItemAll : BatchFilterListItem(batch);
            SelectItem(BatchFilterDropdown, locator);
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetColumnValues(string columnHeader)
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Get all columns for {columnHeader} column header");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(ColumnValues(columnHeader)).Select(header => header.GetText()).ToList();
        }

        public void FilterBySearchTerm(string text)
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Filter by search term <{text}>");
            Wait.UntilJavaScriptReady();

            Wait.UntilElementClickable(SearchTextBox).SetText(text + Keys.Enter);
            Wait.UntilJavaScriptReady();
            Wait.UntilJavaScriptReady();
        }

        public void OpenColumnMenu()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Open column Menu");
            SelectItem(ColumnMenu, ColumnItem);
        }

        public void CloseColumnMenu()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Close column Menu");
            Wait.UntilElementClickable(ColumnMenu).Click();
        }

        public void AddColumns(List<string> columns)
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Add column to the widget <{string.Join(",", columns)}>");
            Wait.UntilJavaScriptReady();
            OpenColumnMenu();
            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
                var elementText = ele.GetText();

                if (!columns.Contains(elementText)) continue;
                Wait.UntilElementVisible(VisibleColumnItem(elementText)).Check();
                Wait.UntilJavaScriptReady();

            }
            CloseColumnMenu();
        }

        public void ClickExcelButton()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Click on Excel button");
            Wait.UntilJavaScriptReady();
            SelectItem(ExportButton, ExportExcelButton);

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
        }

        public List<string> GetColumnHeaders()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Get all the column headers list");
            return Wait.UntilAllElementsLocated(ColumnHeaders).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        public int GetNumberOfGridRows()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Get the number of Grid Rows");
            return Wait.UntilAllElementsLocated(GridRows).Count;
        }

        public List<string> GetRowValues(int index, List<string> columns)
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Get row values");
            return columns.Select(column => Driver.JavaScriptScrollToElement(RowValues(index, column)).GetText()).ToList();
        }

        internal void ClickOnEditAssessment(string assessmentName)
        {
            FilterBySearchTerm(assessmentName);
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Click on Edit assessment <{assessmentName}>");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(EditButtons).Click();
        }

        public void ResetAllFilters()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Reset all filters");
            FilterByAssessmentType("All");
            FilterByBatch("All");
            FilterBySearchTerm("");
        }

        public void DeleteAssessment(string assessmentName)
        {
            FilterBySearchTerm(assessmentName);
            Log.Step(nameof(AssessmentDashboardListTabPage), $"Delete assessment <{assessmentName}>");
            Wait.UntilJavaScriptReady();
            SelectItem(EditMenuButton, EditMenuDelete);
            Wait.UntilElementVisible(DeleteAssessmentPopup);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(DeleteAssessmentYesButton).Click();
            Wait.UntilElementInvisible(DeleteAssessmentPopup);

        }

        public string GetAssessmentFilterMessage()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Get the Assessment Filter message");
            return Wait.UntilElementVisible(AssessmentFilterMessage).GetText();
        }

        public string GetManageAssessmentsTitle()
        {
            Log.Step(nameof(AssessmentDashboardListTabPage), "Get the Manage assessments title");
            return Wait.UntilElementVisible(ManageAssessmentsTitle).GetText();
        }
        public bool IsAssessmentDisplayed(string assessmentName)
        {
            return Driver.IsElementDisplayed(TeamAssessmentName(assessmentName));
        }

        //Navigation
        public void NavigateToAssessmentListTabPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/AssessmentManagementDashboard");
        }
    }
}
