using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance
{
    public class BusinessOutcomesOverallPerformancePage : BusinessOutcomeBasePage
    {
        public BusinessOutcomesOverallPerformancePage(IWebDriver driver, ILogger log) : base(driver, log)
        {

        }
        #region Locators

        #region CompanyBannerImage
        private readonly By BannerImageEditIcon = By.XPath("//button[contains(normalize-space(),'Edit')]");
        #endregion

        #region OutcomesTab
        private static By OverallPerformanceSubTab(string tabName) => By.XPath($"//button[text()='{tabName}']");
        private readonly By DownloadPdfIcon = By.XPath("//button[@aria-label='Download PDF']");
        #endregion

        #region ObjectivesDropdown
        private readonly By OutcomesObjectivesDropdown = By.XPath("//div[@id='timeframe-select']");
        private readonly By ObjectivesDropdownValues = By.XPath("//ul[@role='listbox']/li");
        private static By ObjectiveOption(string objective) => By.XPath($"//ul[@role='listbox']//*[text() = '{objective}']");
        private readonly By AnnualObjectiveTeamText = By.XPath("(//div[@class='k-listview-content']//p)[3]");
        private readonly By ObjectiveProgressBar = By.XPath("//div[contains(text(),'Objectives')]/parent::div/../.././following-sibling::div//div[@role='progressbar']");
        private readonly By ObjectiveAndKpisPopupExportToExcelButton = By.XPath("//button[text()='Export To Excel']");
        private readonly By PdfDownloadLoader = By.XPath("//p[text()='Document is downloading, please wait...']");
        #endregion

        #region OutcomesProgressBar
        private static By OverallPerformanceObjectivesProgressBar(string name) => By.XPath($"(//p[text()='{name}']/../following-sibling::div/div/div)[2]");
        #endregion

        #region Outputs/Projects
        private static By GetProjectTimeLine(string companyName, string projectName) =>
            By.XPath($"(//div[contains(@id,'{companyName}')]//p[contains(text(), '{projectName}')]/parent::div/following-sibling::div//div[contains(@class, 'MuiBox-root css-vdogor')])[2]");
        private static By GetProjectHeaderTitle(string titleName) =>
            By.XPath($"//div[contains(@class,'k-dialog')]//span[contains(text(),'{titleName}')]");

        private readonly By OverallProgressText =
            By.XPath("//div[contains(@class,'k-dialog')]//p[contains(text(),'Overall Progress')]");

        private readonly By ShowOrphanCardsText =
            By.XPath("//div[contains(@class,'k-dialog')]//span[contains(text(),'Show orphan cards')]");

        private readonly By FilterOptions = By.XPath("//h6[text()='Projects Timeline']//parent::div//div//span[2]");

        private readonly By FilterIcon = By.XPath("//div[@id='deliverables-tree-view']//div//*[name()='svg' and @data-icon='filter']");

        private readonly By TableHeaderData =
            By.XPath("//thead[contains(@class,'k-grid-header')]//th//span[@class='k-column-title']");
        private readonly By FilterOkButton = By.XPath("//div[contains(@class,'MuiPaper-rounded')]//button[2]");
        private static By InitiativeColumnTitleDropDown(string columnName) => By.XPath($"//span[text()='{columnName}']//ancestor::div[@id='deliverables-tree-view']//div//table//thead//tr//th//span//*[name()='svg']");
        private readonly By ColumnFilterSortDescendingOption = By.XPath("//div[text()='Sort Descending']");
        private static By GetTitle(string titleName) => By.XPath($"//div[text()='{titleName}']");
        private static By GetStatus(string status) => By.XPath($"//div[text()='{status}']//parent::div//parent::td//following-sibling::td//div[contains(@class,'MuiChip-outlinedDefault')]//span");
        private readonly By ExportToPdfButton = By.XPath("//button[@aria-label='Download PDF']");
        private readonly By ExportToExcelButton = By.XPath("//div[@class='k-window-content k-dialog-content']//button[.='Export to Excel']");

        #endregion

        #region Others
        private readonly By LeftNavigationHierarchyTeamName = By.XPath("//a[@role='treeitem']/div");
        private readonly By OverallPerformanceHierarchySection = By.XPath("//div[contains(@class,'k-card-title')]");
        private readonly By OverallStrategyFieldsText = By.XPath("(//div[contains(@class,'k-card-body')])[1]//p[not(descendant::font)] | (//div[contains(@class,'k-card-body')])[1]//p");
        #endregion

        #endregion

        #region Methods

        #region CompanyBannerImage
        public bool IsBannerEditIconDisplayed()
        {
            return Driver.IsElementDisplayed(BannerImageEditIcon);
        }
        #endregion

        #region OutcomesTab
        public void ClickOnOverallPerformanceSubTab(string tabName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on outcomes tab");
            Driver.JavaScriptScrollToElementCenter(OverallPerformanceSubTab(tabName));
            Wait.UntilElementClickable(OverallPerformanceSubTab(tabName)).Click();
        }
        public bool IsOverallProgressOutcomesTabDisplayed(string tabName)
        {
            return Driver.IsElementDisplayed(OverallPerformanceSubTab(tabName));
        }
        public void ClickOnDownloadPdfIcon()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on 'Download PDF' icon");
            Wait.UntilElementEnabled(DownloadPdfIcon);
            Wait.UntilElementClickable(DownloadPdfIcon).Click();
        }
        #endregion

        #region ObjectiveDropdown
        public void ClickObjectivesDropdown()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Clicking Performance Objectives dropdown");
            Driver.MoveToElement(Wait.UntilElementClickable(OutcomesObjectivesDropdown)).Click();
        }
        public bool IsPerformanceObjectivesDropdownDisplayed()
        {
            return Driver.IsElementDisplayed(OutcomesObjectivesDropdown);
        }

        public List<string> GetObjectivesDropdownOptions()
        {
            return Wait.UntilAllElementsLocated(ObjectivesDropdownValues).Select(opt => opt.Text).ToList();
        }
        public void SelectOnObjectivesDropdownOptions(string objectiveValue)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Select card type from dropdown");
            Wait.UntilElementClickable(ObjectiveOption(objectiveValue)).Click();
           
        }
        public string GetAnnualObjectiveTeamText()
        { 
             return Wait.UntilElementVisible(AnnualObjectiveTeamText).GetText().Replace("\r\n", "");
        }
        public void ClickOnObjectiveProgressBar()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on 'Objectives' progress bar");
            Wait.UntilElementClickable(ObjectiveProgressBar).Click();
        }
        public void ClickOnObjectiveAndKpisExportToExcelButton()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on 'Export to Excel' button of objectives and KPIs");
            Wait.UntilElementClickable(ObjectiveAndKpisPopupExportToExcelButton).Click();
        }
        #endregion

        #region OutcomesProgressBar
        public void ClickOnOutcomesObjectiveProgressBar(string outcomesName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on outcomes progress bar");
            Wait.UntilElementClickable(OverallPerformanceObjectivesProgressBar(outcomesName)).Click();
            Wait.UntilJavaScriptReady();
        }
        #endregion

        #region Outputs/Projects

        public void ClickOnExportToExcel()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on the 'Export to Excel' button");
            Wait.UntilElementVisible(ExportToExcelButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnExportToPdf()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on the 'Export To Pdf' button");
            Driver.JavaScriptClickOn(Wait.UntilElementClickable(ExportToPdfButton));
            Wait.UntilElementNotExist(PdfDownloadLoader);
        }

        public List<string> GetProjectHeaderData()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Get table header data");
            var rows = Wait.UntilAllElementsLocated(TableHeaderData);
            return rows.Select(row => row.Text).ToList();
        }

        public void ClickOnFilterButton()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on the filter icon");
            Wait.UntilElementVisible(FilterIcon).Click();
        }
        public void ClickOnFilterOkButton()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Click on 'OK' button in the filter");
            Wait.UntilElementVisible(FilterOkButton).Click();
        }

        public void InitiativeSortByDescending(string columnName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Sorting by descending.");
            Wait.UntilElementClickable(InitiativeColumnTitleDropDown(columnName)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ColumnFilterSortDescendingOption).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetFilterOptions()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Get filter options");
            var filterDropdownOptions = Wait.UntilAllElementsLocated(FilterOptions);
            var optionsText = filterDropdownOptions.Select(e => e.GetText()).ToList();
            return optionsText;
        }

        public bool IsProjectHeaderDisplayed(string titleName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), $"Check if project header '{titleName}' is displayed");
            return Driver.IsElementDisplayed(GetProjectHeaderTitle(titleName));
        }
        public bool IsOverallProgressTextDisplayed()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Check if overall progress text is displayed");
            return Driver.IsElementDisplayed(OverallProgressText);
        }
        public bool IsShowOrphanCardsTextDisplayed()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Check if 'Show orphan cards' text is displayed");
            return Driver.IsElementDisplayed(ShowOrphanCardsText);
        }
        public void ClickOnProjectTimeline(string companyName, string projectName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), $"Click on project timeline for company: {companyName}, project: {projectName}");
            Wait.UntilElementVisible(GetProjectTimeLine(companyName, projectName)).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsTitleDisplayed(string titleName)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), $"Check if title '{titleName}' is displayed");
            return Driver.IsElementDisplayed(GetTitle(titleName));
        }
        public string GetStatusValue(string status)
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), $"Get status for project with status: {status}");
            return Wait.UntilElementVisible(GetStatus(status)).GetText();
        }

        #endregion

        #region Others
        public void WaitTillOverallPerformanceLoadedSuccessfully()
        {
            Wait.UntilElementEnabled(BannerImageEditIcon);
        }
        public List<string> GetLeftNavigationHierarchyTeamName() => Wait.UntilAllElementsLocated(LeftNavigationHierarchyTeamName).Select(e => Driver.JavaScriptScrollToElement(e).GetText()).ToList();
        public List<string> GetHierarchySectionText() => Wait.UntilAllElementsLocated(OverallPerformanceHierarchySection).Select(e => e.GetText().ToUpper()).ToList();

        public List<string> GetCardViewTexts()
        {
            Log.Step(nameof(BusinessOutcomesOverallPerformancePage), "Get list of weight dropdown Values");
            return Wait.UntilAllElementsLocated(OverallStrategyFieldsText).Select(e => e.Text.Replace("\r\n","")).ToList();
        }
        #endregion

        #endregion
    }
}