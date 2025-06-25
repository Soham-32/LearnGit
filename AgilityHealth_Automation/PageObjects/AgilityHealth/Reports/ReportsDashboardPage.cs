using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Reports
{
    public class ReportsDashboardPage : BasePage
    {
        public ReportsDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By PageTitle = By.CssSelector("div.pg-title h1");
        private readonly By SelectCompanies = By.XPath("//span[text()='Select A Company']");
        private static By CompanyListItem(string companyName) => By.XPath($"//ul[@id='Companies_listbox']//li[text()='{companyName}']");
        private readonly By SelectButton = By.XPath("//input[@id='company']");

        //Key Customer Validation

        #region ReportDashboardElements

        // My Schedules
        private readonly By MySchedulesButton = By.XPath("//span[text()='My Schedules']");
        private readonly By MySchedulesPopupTitle = By.Id("report_schedules_dialog_wnd_title");
        
        // Report Pad
        private readonly By ReportPadButton = By.XPath("//span[contains(@class,'create_report')]");
        private readonly By ReportPadViewButton = By.XPath("//span[text()='View']");
        private readonly By ReportPadSaveButton = By.XPath("//span[text()='Save']");
        
        // Filters
        private readonly By SearchReportsTextbox = By.Id("reportFilterbox");
        private readonly By ReportDropdown = By.XPath("//span[text()='select']");
        private static By SelectedDropDownOption(string option) => By.XPath($"//span[text()='{option}']");
        private readonly By DropdownList = By.XPath("//ul[@id='DropdownCategory_listbox']//li[text()]");
        private static By DropDownOption(string option) =>
            By.XPath($"//ul[@id='DropdownCategory_listbox']//li[text()='{option}']");
        private readonly By ReportName =
            By.XPath("//div[@id='ReportsGrid']//tbody//tr[1]//span | //div[@id='ReportsGrid']//tbody//tr[1]//span//font//font");

        // Generate Report
        private static By GenerateReport(string reportName) =>
            By.XPath($"//div[@id='ReportsGrid']//table//span[text()='{reportName}']//ancestor::tr//td//img[@class='action-icon']");
        private readonly By ExportButtonOfGenerateReport = By.XPath("//button[@class='exportBtn']");
        private readonly By ExcelButtonOfGenerateReport = By.XPath("//button[@class='excelBtn k-grid-excel']");

        //Individual Schedule
        private readonly By ScheduleIcon =
            By.XPath("//div[@id='ReportsGrid']//table//tbody//tr[1]//td[3]//img[@class='schedule-icon']");
        private readonly By SchedulePopupTitle = By.Id("schedule_window_wnd_title");

        #endregion

        public string GetTitleText()
        {
            Log.Step(nameof(ReportsDashboardPage), "Get page title text.");
            return Wait.UntilElementVisible(PageTitle).GetText();
        }

        public bool IsPageTitleDisplayed()
        {
            return Driver.IsElementDisplayed(PageTitle);
        }

        public void SelectCompany(string companyName)
        {
            Log.Info($"Select {companyName} company from company dropdown");
            SelectItem(SelectCompanies, CompanyListItem(companyName));
            Wait.UntilElementClickable(SelectButton).Click();
        }

        //Key Customer Validation

        #region ReportDashboardElements

        // My Schedules
        public void ClickOnMySchedulesButton()
        {
            Log.Step(nameof(ReportsDashboardPage), "Click On 'My Schedules' button");
            Wait.UntilElementClickable(MySchedulesButton).Click();
        }
        public string GetMySchedulesPopupTitle()
        {
            return Wait.UntilElementVisible(MySchedulesPopupTitle).GetText();
        }

        // Report Pad
        public void ClickOnReportPadButton()
        {
            Log.Step(nameof(ReportsDashboardPage), "Click On 'Report Pad' button");
            Wait.UntilElementClickable(ReportPadButton).Click();
        }
        public bool IsViewButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ReportPadViewButton);
        }
        public bool IsSaveButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ReportPadSaveButton);
        }

        // Filters
        public void SearchWithReportName(string reportName)
        {
            Log.Step(nameof(ReportsDashboardPage), "Search report from text box");
            Wait.UntilElementVisible(SearchReportsTextbox).SetText(reportName);
            Wait.HardWait(3000);// need to wait till search items shows in grid
        }
        public IList<string> GetAllDropDownOptionList()
        {
            Wait.UntilElementClickable(ReportDropdown).Click();
            var elements = Wait.UntilAllElementsLocated(DropdownList).Where(e => e.Displayed).Select(e => e.Text).ToList();
            return elements;
        }
        public bool IsSelectedDropDownOptionDisplayed(string option)
        {
            return Driver.IsElementDisplayed(SelectedDropDownOption(option));
        }
        public void SelectDropDownOption(string option)
        {
            Log.Step(nameof(ReportsDashboardPage), "Select Option from the 'Filter' dropdown");
            SelectItem(ReportDropdown, DropDownOption(option));
        }
        public bool IsReportDisplayed()
        {
            return Driver.IsElementDisplayed(ReportName);
        }

        public string GetReportName()
        {
            return Wait.UntilElementVisible(ReportName).GetText();
        }

        // Generate Report
        public void ClickOnGenerateReportIcon(string reportName)
        {
            Log.Step(nameof(ReportsDashboardPage), "Click on 'Generate Report' icon");
            Wait.UntilElementClickable(GenerateReport(reportName)).Click();
        }
        public void ClickOnGenerateReportExportButton()
        {
            Log.Step(nameof(ReportsDashboardPage), "Click on 'Export' and 'Excel' button of generate report popup");
            Wait.UntilElementClickable(ExportButtonOfGenerateReport).Click();
            Wait.UntilElementClickable(ExcelButtonOfGenerateReport).Click();
        }

        //Individual Schedule
        public void ClickOnIndividualScheduleReportIcon()
        {
            Log.Step(nameof(ReportsDashboardPage), "Click on individual 'Schedule' icon");
            Wait.UntilElementClickable(ScheduleIcon).Click();
        }

        public string GetIndividualSchedulesPopupTitle()
        {
            return Wait.UntilElementVisible(SchedulePopupTitle).GetText();
        }
        #endregion

        public string GetPageUrl(int companyId, bool isNonSaPaUser = true)
        {

            if (isNonSaPaUser)
            {
                return $"{BaseTest.ApplicationUrl}/company/{companyId}/reports";
            }
            return $"{BaseTest.ApplicationUrl}/reporting?companyId={companyId}";

        }
        public void NavigateToReportPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/reports");
        }
    }
}