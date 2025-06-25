using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AgilityHealth_Automation.Utilities;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company
{
    internal class CompanyDashboardPage : BasePage
    {
        public CompanyDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By PageTitle = By.XPath("//div[@id='mainPanel']//p[contains(@class,'jss')]");
        private readonly By AddCompanyButton = AutomationId.Equals("btnAddCompany");
        private readonly By SearchFilterTextbox = AutomationId.Equals("searchFilter", "input");
        private readonly By FilterClearButton = AutomationId.Equals("clearFilter");
        private readonly By ColumnSettingsButton = AutomationId.Equals("btnSettings", "svg");
        private readonly By ColumnSettingsList = AutomationId.Equals("checkboxPanel");
        private static By ColumnCheckbox(string columnName) =>
            AutomationId.Equals("checkboxPanel", $"input[name = '{columnName}']");

        //Key-customer verification
        #region Elements
        //Get all company list
        private readonly By CompanyNameList = By.XPath("//*[contains(@automation-id , 'companyName')]");
        private readonly By CompanyShowHideColumnsList = By.XPath("//div[@automation-id='checkboxPanel']//input[@name]");
        private readonly By ShowHideColumnsText = By.XPath("//div[text() = 'Show/Hide Columns']");
        #endregion

        // grid locators
        private readonly By LifeCycleListbox = AutomationId.StartsWith("lifeCycle_rollover", "div");
        private static By LifeCycleListItem(string item) => By.XPath($"//ul/li[text() = '{item}']");
        private static By AllColumnValues(string columnName) => AutomationId.StartsWith(ColumnLocators[columnName]);
        private static By ColumnValueByCompanyName(string companyName, string columnLocator) =>
            By.XPath($"//div[starts-with(@automation-id, '{ColumnLocators["Company Name"]}')][text() = '{companyName}']//ancestor::div[starts-with(@automation-id, 'gridData_')]//*[starts-with(@automation-id, '{columnLocator}')] | //div[starts-with(@automation-id, '{ColumnLocators["Company Name"]}')]//font[text() = '{companyName}']//ancestor::div[starts-with(@automation-id, 'gridData_')]//*[starts-with(@automation-id, '{columnLocator}')] | //div[starts-with(@automation-id, '{ColumnLocators["Company Name"]}')][contains(text(), '{companyName}')]//ancestor::div[starts-with(@automation-id, 'gridData_')]//*[starts-with(@automation-id, '{columnLocator}')]");
        private static By ColumnHeader(string columnName) => AutomationId.StartsWith($"gridHeader_{columnName}_");
        private static By CompanyRowByIndex(int index) => AutomationId.Equals($"gridData_{index}");
        private static By CompanyRowByCompanyName(string companyName) =>
            By.XPath($"//div[starts-with(@automation-id, '{ColumnLocators["Company Name"]}')][text() = '{companyName}']//ancestor::div[starts-with(@automation-id, 'gridData_')]");

        private static By CompanyName(string companyName) => By.XPath($"//*[contains(text(),'{companyName}')]");

        // delete popup
        private readonly By DeletePopupDeleteButton = AutomationId.Equals("deleteBtn");

        // draft company popup
        private readonly By DraftCompanyPopup = By.XPath("//div[@role='dialog']//h2[text()='Draft Company']");
        private readonly By DraftCompanyPopupEditButton = By.XPath("//div[@role='dialog']//h2[text()='Draft Company']//parent::div//following-sibling::div//button[normalize-space(text()) = 'Edit']");
        private readonly By DraftCompanyPopupCancelButton = By.XPath("//div[@role='dialog']//h2[text()='Draft Company']//parent::div//following-sibling::div//button[normalize-space(text()) = 'Cancel']");

        private static readonly Dictionary<string, string> ColumnLocators = new Dictionary<string, string>
        {
            { "Company Name", "companyName_" },
            { "Industry", "industry_" },
            { "Comp Admin", "partnerMgt_" },
            { "Ref. Partner", "refPartner_" },
            { "Last Asmt", "lastAsmt_" },
            { "Asmt", "asmt_" },
            { "Teams", "teams_" },
            { "Subscript", "subscript_" },
            { "Life Cycle", "lifeCycle_" },
            { "Comp Type", "compType_" },
            { "Edit", "edit_" },
            { "Delete", "delete_" }
        };

        public void WaitUntilLoaded()
        {
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SearchFilterTextbox);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(FilterClearButton).Click();
            Wait.UntilElementExists(CompanyRowByIndex(0));
            Wait.UntilJavaScriptReady();
        }

        public void WaitUntilCompanyLoaded(int timeout = 60)
        {
            for (var i = 0; i < timeout; i++)
            {
                var companyStorage = (string)Driver.JsExecutor().ExecuteScript("return window.localStorage.getItem('persist:company');");
                var isLoaded = JObject.Parse(companyStorage)["loaded"]?.ToString();
                if (isLoaded != null && bool.Parse(isLoaded))
                {
                    break;
                }
                Wait.HardWait(1000);
            }
        }

        public string GetPageTitleText()
        {
            Log.Step(nameof(CompanyDashboardPage), "Get page title text");
            return Wait.UntilElementVisible(PageTitle).GetText();
        }
        public string GetAddACompanyButtonText()
        {
            Log.Step(nameof(CompanyDashboardPage), "Get 'Add a Company' Button text");
            return Wait.UntilElementVisible(AddCompanyButton).GetText();
        }

        public void Search(string searchTerm)
        {
            Log.Step(nameof(CompanyDashboardPage), $"Enter Search term <{searchTerm}>");
            Wait.UntilElementClickable(SearchFilterTextbox);
            Wait.UntilElementClickable(FilterClearButton).Click();
            Wait.UntilElementClickable(SearchFilterTextbox).SetText(searchTerm);
            Wait.UntilJavaScriptReady();
        }

        public IList<string> GetColumnValues(string columnName)
        {
            Wait.HardWait(2000);  // Wait until company dashboard load
            Driver.JavaScriptScrollToElement(AllColumnValues(columnName));
            return Driver.GetTextFromAllElements(AllColumnValues(columnName));
        }


        public void ShowColumnSettings()
        {
            Log.Step(nameof(CompanyDashboardPage), "Open the Column Settings menu");
            if (Driver.IsElementPresent(ColumnSettingsList)) return;
            Wait.UntilElementClickable(ColumnSettingsButton).Click();
            Wait.UntilElementVisible(ColumnSettingsList);
        }

        public void HideColumnSettings()
        {
            Log.Step(nameof(CompanyDashboardPage), "Close the Column Settings menu");
            if (!Driver.IsElementPresent(ColumnSettingsList)) return;
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ColumnSettingsButton).Click();
            Wait.UntilElementNotExist(ColumnSettingsList);
        }

        public void RemoveColumns(IList<string> columns)
        {
            ShowColumnSettings();
            foreach (var columnName in columns)
            {
                Log.Step(nameof(CompanyDashboardPage), $"Un-Check the checkbox for column <{columnName}>");
                Wait.UntilElementClickable(ColumnCheckbox(columnName)).Check(false);
            }
            HideColumnSettings();
        }

        public bool IsColumnVisible(string columnName)
        {
            return Wait.UntilElementExists(ColumnHeader(columnName)).Displayed;
        }

        public void AddColumns(IList<string> columns)
        {
            ShowColumnSettings();
            foreach (var columnName in columns)
            {
                Log.Step(nameof(CompanyDashboardPage), $"Check the checkbox for column <{columnName}>");
                Wait.UntilElementClickable(ColumnCheckbox(columnName)).Check();
            }
            HideColumnSettings();
        }

        public void ClickEditIconByCompanyName(string companyName)
        {
            HoverOverCompanyName(companyName);
            Log.Step(nameof(CompanyDashboardPage), $"Click on the 'Edit' button for <{companyName}>");
            RemovePendoHelp();
            Driver.JavaScriptScrollToElement(ColumnValueByCompanyName(companyName, ColumnLocators["Edit"]));
            Wait.UntilElementClickable(ColumnValueByCompanyName(companyName, ColumnLocators["Edit"])).Click();
            Wait.UntilJavaScriptReady();
            new EditCompanyProfilePage(Driver, Log).WaitUntilLoaded();
        }

        public void ClickDeleteIconByCompanyName(string companyName)
        {
            HoverOverCompanyName(companyName);
            Log.Step(nameof(CompanyDashboardPage), $"Click on the 'Delete' button for <{companyName}>");
            RemovePendoHelp();
            Wait.UntilElementClickable(ColumnValueByCompanyName(companyName, ColumnLocators["Delete"])).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(DeletePopupDeleteButton).Click();
            Wait.UntilElementNotExist(DeletePopupDeleteButton);
        }

        public void SelectLifecycle(string companyName, string lifeCycle)
        {
            HoverOverCompanyName(companyName);
            Log.Step(nameof(CompanyDashboardPage), $"Select Life Cycle <{lifeCycle}> for <{companyName}>");
            var companyRow = Wait.UntilElementExists(CompanyRowByCompanyName(companyName));
            Wait.UntilElementClickable(companyRow.FindElement(LifeCycleListbox)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(LifeCycleListItem(lifeCycle)).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetCompanyDetail(string companyName, string fieldName)
        {
            return Wait.UntilElementExists(ColumnValueByCompanyName(companyName, ColumnLocators[$"{fieldName}"])).GetAttribute("textContent");
        }

        public bool IsLifeCycleUpdated(string companyName, string expectedValue)
        {
            try
            {
                Wait.UntilAttributeEquals(ColumnValueByCompanyName(companyName, ColumnLocators["Life Cycle"]),
                    "textContent", expectedValue);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void ClickOnCompanyName(string companyName)
        {
            Log.Step(nameof(CompanyDashboardPage), $"Click on company name <{companyName}>");
            WaitUntilCompanyLoaded();
            Wait.UntilElementClickable(CompanyName(companyName)).Click();
        }

        public void ClickOnAddCompanyButton()
        {
            Log.Step(nameof(CompanyDashboardPage), "Click on the 'Add Company' button");
            Wait.UntilElementClickable(AddCompanyButton).Click();
        }
        public bool IsAddCompanyButtonDisplayed()
        {
            return Driver.IsElementDisplayed(AddCompanyButton, 20);
        }

        public bool IsCompanyPresent(string companyName)
        {
            return Driver.IsElementPresent(CompanyName(companyName), 60);
        }

        public void HoverOverCompanyName(string companyName)
        {
            Log.Step(nameof(CompanyDashboardPage), $"Hover over company name <{companyName}>");
            Driver.JavaScriptScrollToElement(ColumnValueByCompanyName(companyName, ColumnLocators["Company Name"]));
            var element = Wait.UntilElementVisible(ColumnValueByCompanyName(companyName,
                    ColumnLocators["Company Name"]));
            Driver.MoveToElement(element);
        }
        public bool IsCompanySearchFilterTextBoxDisplayed()
        {
            return Driver.IsElementDisplayed(SearchFilterTextbox);
        }

        // Draft Company Popup
        public void ClickOnDraftCompanyPopupEditButton()
        {
            Log.Step(nameof(CompanyDashboardPage), "Click on 'Edit' button for draft company popup");
            Wait.UntilElementClickable(DraftCompanyPopupEditButton).Click();
        }

        public void ClickOnDraftCompanyPopupCancelButton()
        {
            Log.Step(nameof(CompanyDashboardPage), "Click on 'Cancel' button for draft company popup");
            Wait.UntilElementClickable(DraftCompanyPopupCancelButton).Click();
        }

        public bool IsDraftCompanyPopupDisplayed()
        {
            return Driver.IsElementDisplayed(DraftCompanyPopup);
        }

        public void NavigateToPage()
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/v2/company");
        }

        //Key-customer verification
        #region Elements
        public bool IsCompanyNameListDisplayed()
        {
            var companyNameList = Wait.UntilAllElementsLocated(CompanyNameList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();

            if (companyNameList.All(string.IsNullOrEmpty))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsCompanyShowHideColumnsListDisplayed()
        {
            var columnSettingsNameList = Wait.UntilAllElementsLocated(CompanyShowHideColumnsList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();

            if (columnSettingsNameList == null || columnSettingsNameList.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsShowHideColumnsTextDisplayed()
        {
            return Driver.IsElementDisplayed(ShowHideColumnsText);
        }
        #endregion
    }
}
