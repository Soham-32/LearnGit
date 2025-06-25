using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    internal class GrowthPlanDashboardPage : BasePage
    {
        public GrowthPlanDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        public static readonly Dictionary<string, string> ColumnLocators = new Dictionary<string, string>
        {
            { "Title", "Title_" },
            { "Priority", "Priority_" },
            {"Status","Status_" },
            {"Category","Category_" },
            {"Created Date","CreatedDate_"},
            {"Owner(s)","Owners_"},
            {"Id","Id_"},
            {"Type","Type_" },
            {"Location","Location_" },
            {"Assessment","Assessment_" },
            {"Team","Team_" },
            {"Competency Target","CompetencyTarget_" },
            {"Radar Type","RadarType_" },
            {"Updated By","UpdatedBy_" },
            {"Target Date","TargetDate_" },
            {"Completion Date","CompletionDate_" },
            {"Size","Size_"},
            {"Affected Teams","AffectedTeams_" },
            {"Tags","Tags_" },
            {"Origination","Origination_" },
            {"External Identifier", "ExternalIdentifier_"}
        };


        // grid locators
        private static By AllColumnValues(string columnName) => AutomationId.StartsWith(ColumnLocators[columnName]);
        private static By ColumnValueByTitleName(string titleName, string columnName) =>
        By.XPath($"//div[starts-with(@automation-id,'{ColumnLocators["Title"]}')][text()='{titleName}']//ancestor::div[@automation-id='gridData']//div[starts-with(@automation-id,'{ColumnLocators[columnName]}')]");

        private readonly By DashboardSettingButton = AutomationId.Equals("btnSettings");
        private readonly By DashboardColumnSettingList = AutomationId.Equals("checkboxPanel");
        private static By ColumnCheckbox(string columnName) =>
        AutomationId.Equals("checkboxPanel", $"input[name = '{columnName}']");
        private static By ColumnHeaderList(string columnName) => AutomationId.StartsWith($"gridHeader_{columnName}_");
        private static By ColumnHeaderAngleIcon(string columnName) => By.XPath($"//div[text()='{columnName}']/button/span//*[local-name()='svg']");
        private static By NthRowTitle(int nth) => AutomationId.Equals($"Title_{nth - 1}");
        private readonly By AddGrowthItemButton = AutomationId.Equals("btnAddGrowthItem");
        private static By GrowthItemTitle(string title) => By.XPath($"*//div[@id='containerGrid']//div[text()='{title}']");

        //Export To Excel
        private readonly By ExportToExcelButton = AutomationId.Equals("btnExportExcel");
        private readonly By GridColumnHeaders = By.XPath("//div[@automation-id='gridHeader']//div");
        private static By GridRowValuesByColumnHeader(string columnHeader) => By.XPath($"*//div[@id='containerGrid']//div[@automation-id='{columnHeader}_0']");


        public void ClickOnAddGrowthItemButton()
        {
            Log.Step(nameof(GrowthPlanDashboardPage), "Click on Add Growth Item Button");
            Wait.UntilElementVisible(AddGrowthItemButton);
            Wait.UntilElementClickable(AddGrowthItemButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsGrowthItemDisplayed(string giTitle, bool waitTillNotExits = false)
        {
            if (waitTillNotExits)
            {
                Wait.UntilElementNotExist(GrowthItemTitle(giTitle));
            }
            return Driver.IsElementDisplayed(GrowthItemTitle(giTitle));
        }

        public string GetColumnValueByGiTitleName(string titleName, string columnName)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Getting {columnName} values by given Title Name- {titleName}");
            return Wait.UntilElementVisible(ColumnValueByTitleName(titleName, columnName)).GetText();
        }
        public void ClickOnColumnValueByGiTitleName(string titleName, string columnName)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Click on {columnName} column for given GI by title - {titleName}");
            Wait.UntilElementClickable(ColumnValueByTitleName(titleName, columnName)).Click();
        }

        public List<string> GetAllColumnValues(string columnName)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Getting All values by column name - {columnName}");
            return Wait.UntilAllElementsLocated(AllColumnValues(columnName)).Select(e => e.GetText()).ToList();
        }

        public void ShowColumnSettings()
        {
            Log.Step(nameof(GrowthPlanDashboardPage), "Open the Column Settings menu");
            if (Driver.IsElementDisplayed(DashboardColumnSettingList)) return;
            Wait.UntilElementClickable(DashboardSettingButton).Click();
            Wait.UntilElementVisible(DashboardColumnSettingList);
        }
        public void HideColumnSettings()
        {
            Log.Step(nameof(GrowthPlanDashboardPage), "Close the Column Settings menu");
            if (!Driver.IsElementDisplayed(DashboardColumnSettingList)) return;
            Wait.UntilElementClickable(DashboardSettingButton).Click();
            Wait.UntilElementNotExist(DashboardColumnSettingList);
        }
        public bool IsColumnCheckboxSelected(string columnName)
        {
            Wait.HardWait(1000); //Grid takes time to refresh default columns.Dev will not work to optimize this -so put hard wait to avoid any false failures
            return Driver.IsElementSelected(ColumnCheckbox(columnName));
        }

        public void RemoveColumns(IList<string> columns)
        {
            ShowColumnSettings();
            foreach (var columnName in columns)
            {
                Log.Step(nameof(GrowthPlanDashboardPage), $"Un-Check the checkbox for column <{columnName}>");
                Wait.UntilElementVisible(ColumnCheckbox(columnName));
                Wait.UntilElementClickable(ColumnCheckbox(columnName)).Check(false);
            }
            HideColumnSettings();
        }
        public bool IsColumnVisible(string columnName)
        {
            Log.Info($"Verify <{columnName}> column is showing.");
            return Driver.IsElementDisplayed(ColumnHeaderList(columnName));
        }
        public void AddColumns(IList<string> columns)
        {
            ShowColumnSettings();
            foreach (var columnName in columns)
            {
                Log.Step(nameof(GrowthPlanDashboardPage), $"Check the checkbox for column <{columnName}>");
                Wait.UntilElementVisible(ColumnCheckbox(columnName));
                Wait.UntilElementClickable(ColumnCheckbox(columnName)).Check();
            }
            HideColumnSettings();
        }

        public void NavigateToPage(int companyId, int teamId = 0)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/v2/growthplan/company/{companyId}/team/{teamId}");
        }

        public string GetPageUrl(int companyId, int teamId = 0)
        {
            return $"{BaseTest.ApplicationUrl}/V2/growthplan/company/{companyId}/team/{teamId}";
        }

        public void WaitUntilGrowthPlanLoaded(int numberOfItemsToBeLoaded = 1, int timeout = 60)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Waiting until {numberOfItemsToBeLoaded} growth items loaded");
            for (var i = 0; i < timeout; i++)
            {
                if (Driver.IsElementDisplayed(NthRowTitle(numberOfItemsToBeLoaded)))
                {
                    break;
                }
                Wait.HardWait(1000);
            }
        }

        public void ClickOnColumn(string columnName)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Click on {columnName} Column");
            Wait.UntilElementClickable(ColumnHeaderList(columnName)).Click();
        }

        public void SortColumnIntoAscendingOrder(string column)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Sorted a {column} column into ascending order");
            ClickOnColumn(column);

            if (!IsAngleIconVisible(column))
            {
                ClickOnColumn(column);
            }
            else if (GetColumnAngleValue(column) == "angle-up")
            {
                ClickOnColumn(column);
                ClickOnColumn(column);
                ClickOnColumn(column);
            }
            else
            {
                ClickOnColumn(column);
                ClickOnColumn(column);
            }
        }

        public void SortColumnIntoDescendingOrder(string column)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Sorted a {column} column into descending order");
            ClickOnColumn(column);

            if (!IsAngleIconVisible(column))
            {
                ClickOnColumn(column);
                ClickOnColumn(column);
            }
            else if (GetColumnAngleValue(column) == "angle-up")
            {
                ClickOnColumn(column);
            }
            else
            {
                ClickOnColumn(column);
                ClickOnColumn(column);
                ClickOnColumn(column);
            }
        }

        private string GetColumnAngleValue(string columnName)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Getting {columnName}'s angle value");
            return Wait.UntilElementVisible(ColumnHeaderAngleIcon(columnName)).GetElementAttribute("data-icon");
        }

        private bool IsAngleIconVisible(string columnName)
        {
            return Driver.IsElementDisplayed(ColumnHeaderAngleIcon(columnName));
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

        //Export To Excel
        public void ClickOnExportToExcelButton()
        {
            Log.Step(nameof(GrowthPlanDashboardPage), "Click on 'Export to Excel' button");
            Wait.UntilElementClickable(ExportToExcelButton).Click();
        }
        internal List<string> GetVisibleColumnHeaderNamesFromGrid()
        {
            Log.Step(nameof(GrowthPlanDashboardPage), "Getting all visible column's name");
            Wait.UntilElementVisible(GridColumnHeaders);
            var displayedElements = Wait.UntilAllElementsLocated(GridColumnHeaders).Where(e => e.Displayed);
            return displayedElements.Select(e => e.GetText()).ToList();
        }
        public string GetRawValueByColumnHeader(string columnHeader)
        {
            Log.Step(nameof(GrowthPlanDashboardPage), $"Getting raw value of {columnHeader}");
            return Wait.UntilElementVisible(GridRowValuesByColumnHeader(columnHeader)).GetText();
        }
    }
}