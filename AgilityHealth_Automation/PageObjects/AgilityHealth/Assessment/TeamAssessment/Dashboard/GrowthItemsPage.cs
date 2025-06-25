using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard
{
    public class GrowthItemsPage : GridPage
    {
        public GrowthItemsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By GrowthItemLoadIcon = By.XPath("//div[@class='k-loading-image']");
        private readonly By AddNewItemButton = By.ClassName("addGIItem");
        private readonly By SyncCommittedItemsButton = By.XPath("//a[text()=' Sync Committed Items ']");
        private readonly By GiRows = By.XPath("//div[@id = 'GrowthPlan']//table/tbody/tr");
        
        private readonly By ExportToExcelButton = By.Id("giexport");
        private readonly By LinkSuccessfullyToasterMessage = By.XPath("//div[text()='Link Successful!']");
        private readonly By UnlinkSuccessfullyToasterMessage = By.XPath("//div[text()='Unlink Successful!']");
        private readonly By GiUpToDateToasterMessage = By.XPath("//div[text()='Growth Items are already up to date!']");
        private static By GiRow(string giTitle) => By.XPath($"//div[@id = 'GrowthPlan']//table/tbody//td[contains(normalize-space(), '{giTitle}')]/..");
        private static By GiDeleteButtonFromGrid(string giTitle) =>
        By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[contains(normalize-space(), '{giTitle}')]/following-sibling::td//a[text()='Delete']");

        private static By GiEditButtonFromGrid(string giTitle) => By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[contains(normalize-space(),'{giTitle}')]/following-sibling::td//a[contains(normalize-space(),'Edit')]");
        private static By GiSyncJiraButtonFromGrid(string giTitle) => By.XPath($"//div[@id='GrowthPlan']//table//td[contains(normalize-space(), '{giTitle}')]/following-sibling::td//a[text()='Sync Jira']");
        private static By GiUnSyncJiraButtonFromGrid(string giTitle) => By.XPath($"//div[@id='GrowthPlan']//table//td[contains(normalize-space(), '{giTitle}')]/following-sibling::td//a[text()='Unsync Jira']");

        private static By GiRowValueByColumn(int rowIndex, string columnName) => By.XPath(
        $"//div[@id='GrowthPlan']//tbody//tr[{rowIndex}]/td[count(//div[@id='GrowthPlan']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");
        private static By GiRowValueByColumn(string giTitle, string columnName) => By.XPath(
        $"//div[@id='GrowthPlan']//tbody//tr/td[contains(normalize-space(), '{giTitle}')]/../td[count(//div[@id='GrowthPlan']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");

        private static By GiCopyButtonFromGrid(string giTitle) =>
        By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[contains(normalize-space(), '{giTitle}')]/following-sibling::td//*[text()='Copy']");

        private readonly By AllDeleteGiButtons = By.XPath("//div[@id = 'GrowthPlan']//table/tbody//tr/td/a[text()='Delete']");

        private static By GiHistoryIcon(string text) =>
            By.XPath($"//td[contains(text(),'{text}')]//preceding-sibling::td/span[@id='History']");
        private static By Origination(int index) => By.XPath($"//table//tbody//tr[{index}]//td[@id='Origination']");
        private static By OriginationHref(int index) => By.XPath($"//table//tbody//tr[{index}]//td[@id='Origination']//a");

        //History Modal
        private readonly By GiHistoryTableUserNameColumnHeader = By.XPath("//a[text()='User Name']");
        private readonly By GiHistoryRows = By.XPath("//*[@id='historyDialog']//tbody/tr");


        //Kanban
        private readonly By GrowthPlanKanbanView = By.Id("growthPlanKannanView");
        private readonly By ToggleButton = By.Id("toggle-header");
        private readonly By KanbanNewGrowthItemButton = By.CssSelector(".fas.fa-plus-square");

        //Key Customer Verification

        #region GI tab elements

        private readonly By GrowthPlanGridView = By.Id("gridWrapperOuter");
        private readonly By AddGrowthPlanItemPopupText = By.XPath("//*[text()='Add Growth Plan Item']");
        private readonly By NotificationsButton = By.XPath("//a[contains(text(),' Notifications')]");
        private readonly By NotificationsPopupTitle = By.Id("notificationDialog_wnd_title");
        private readonly By EditGrowthPlanItemPopup = By.XPath("//div[@aria-label='Edit Growth Plan Item']");

        #endregion

        public void WaitForGrowthItemsPageToLoad()
        {
            Wait.UntilElementNotExist(GrowthItemLoadIcon);
        }
        public void ClickAddNewItemButton()
        {
            Log.Step(nameof(GrowthItemsPage), "Click on Add New Item button");
            Wait.UntilElementVisible(AddNewItemButton).Click();
        }
        public void ClickOnSyncCommittedItemsButton()
        {
            Log.Step(nameof(GrowthItemsPage), "Click on 'Sync Committed Items' button");
            Wait.UntilElementClickable(SyncCommittedItemsButton).Click();
            Wait.HardWait(10000); //It is taking time to sync GI.
        }

        public bool IsSyncCommittedItemsButtonDisplayed()
        {
            return Driver.IsElementDisplayed(SyncCommittedItemsButton);
        }

        public void ClickExportToExcelButton()
        {
            Log.Step(nameof(GrowthItemsPage), "Click on 'Export to Excel' button");
            Wait.UntilElementVisible(ExportToExcelButton).Click();
        }

        public bool IsLinkSuccessfullyToasterMessageDisplayed()
        {
            return Driver.IsElementDisplayed(LinkSuccessfullyToasterMessage, 10);
        }
        public bool IsUnlinkSuccessfullyToasterMessageDisplayed()
        {
            return Driver.IsElementDisplayed(UnlinkSuccessfullyToasterMessage, 10);
        }
        public bool IsGiUpToDateToasterMessageDisplayed()
        {
            return Driver.IsElementDisplayed(GiUpToDateToasterMessage, 10);
        }

        public GrowthItem GetGrowthItemFromGrid(int rowIndex)
        {
            Wait.UntilJavaScriptReady();
            var targetDate = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Target Date"))
            .GetAttribute("textContent");
            DateTime? date = null;
            if (!string.IsNullOrEmpty(targetDate))
            {
                date = DateTime.Parse(targetDate);
            }

            var growthItem = new GrowthItem
            {
                Title = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Title")).GetAttribute("textContent"),
                Status = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Status")).GetAttribute("textContent"),
                Priority =
            Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Priority")).GetAttribute("textContent"),
                Description = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Description"))
            .GetAttribute("textContent"),
                Category =
            Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Category")).GetAttribute("textContent"),
                Type = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Type")).GetAttribute("textContent"),
                TargetDate = date,
                Size = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Size")).GetAttribute("textContent"),
                Color = CSharpHelpers
            .ConvertRgbToHex(Wait.UntilAllElementsLocated(GiRows)[rowIndex - 1].GetCssValue("background-color"))
            .ToLower(),
            };

            return growthItem;
        }

        public GrowthItem GetGrowthItemFromGrid(string giTitle)
        {
            Wait.UntilJavaScriptReady();
            var targetDate = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Target Date"))
            .GetAttribute("textContent");
            DateTime? date = null;
            if (!string.IsNullOrEmpty(targetDate))
            {
                date = DateTime.Parse(targetDate);
            }

            var growthItem = new GrowthItem
            {
                Title = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Title")).GetAttribute("textContent"),
                Status = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Status")).GetAttribute("textContent"),
                Priority =
            Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Priority")).GetAttribute("textContent"),
                Description = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Description"))
            .GetAttribute("textContent"),
                Category = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Category")).GetAttribute("textContent"),
                Assessment = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Assessment")).GetAttribute("textContent"),
                Type = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Type")).GetAttribute("textContent"),
                TargetDate = date,
                Size = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Size")).GetAttribute("textContent"),
                RadarType = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Radar Type")).GetAttribute("textContent"),
                Color = CSharpHelpers
            .ConvertRgbToHex(Wait.UntilElementExists(GiRow(giTitle)).GetCssValue("background-color")).ToLower()
            };

            return growthItem;
        }

        public void DeleteGrowthItem(string giTitle)
        {
            Log.Step(nameof(GrowthItemsPage), $"Delete growth item {giTitle}");
            Wait.UntilElementClickable(GiDeleteButtonFromGrid(giTitle)).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }

        public void ClickGrowthItemEditButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemsPage), $"Click on edit growth item {giTitle}");
            Wait.UntilElementClickable(GiEditButtonFromGrid(giTitle)).Click();
            Wait. HardWait(3000); //Wait until edit popup opens
        }

        public void ClickOnGrowthItemSyncJiraButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemsPage), $"Click on 'Sync Jira' button for {giTitle}.");
            Wait.UntilElementVisible(GiSyncJiraButtonFromGrid(giTitle));
            Wait.UntilElementClickable(GiSyncJiraButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnGrowthItemUnSyncJiraButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemsPage), $"Click on 'UnSync Jira' button for {giTitle}.");
            Wait.UntilElementClickable(GiUnSyncJiraButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsSyncJiraButtonDisplayed(string giTitle)
        {
            return Driver.IsElementDisplayed(GiSyncJiraButtonFromGrid(giTitle), 10);
        }

        public bool IsUnSyncJiraButtonDisplayed(string giTitle)
        {
            return Driver.IsElementDisplayed(GiUnSyncJiraButtonFromGrid(giTitle));
        }

        public int GetGrowthItemCount()
        {
            Wait.UntilJavaScriptReady();
            return Driver.GetElementCount(GiRows);
        }

        public List<string> GetAllRowValues(int index, List<string> columns)
        {
            var getRawValue = GetRowValues(index, columns);
            if (!Driver.IsElementPresent(OriginationHref(index))) return getRawValue;
            var originationText = Wait.UntilElementVisible(Origination(index)).GetText().Replace("'s", "");
            var originationLink = Wait.UntilElementVisible(OriginationHref(index)).GetAttribute("href").Replace("http://", "");
            var originationTextAndLink = originationText + originationLink;
            getRawValue.Remove(getRawValue.Last());
            getRawValue.Add(originationTextAndLink.RemoveWhitespace());
            return getRawValue;
        }

        public void ClickCopyGrowthItemButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemsPage), $"Click on copy growth item {giTitle}");
            Wait.UntilElementClickable(GiCopyButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void DeleteAllGIs()
        {
            if (Wait.InCase(AllDeleteGiButtons) == null) return;
            var numberOfItems = Wait.UntilAllElementsLocated(AllDeleteGiButtons).Count;

            for (var i = 0; i < numberOfItems; i++)
            {

                try
                {
                    // after deleting each GI, the grid refreshes and the elements get stale,
                    // so get the list again and delete the first one
                    Driver.MoveToElement(Wait.UntilAllElementsLocated(AllDeleteGiButtons)[0])
                    .Click(); //If no delete button,but only Edit,Copy buttons then it gives exception. It happens when item copied by next assessment
                    Driver.AcceptAlert();
                }
                catch
                {
                    // ignored
                }

                Wait.UntilJavaScriptReady();
            }
        }

        public string GetPulledValue(string giTitle)
        {
            return Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Pulled")).GetAttribute("textContent");
        }

        public bool DoesGiExist(string title)
        {
            return Wait.InCase(GiRow(title)) != null && Wait.InCase(GiRow(title)).Displayed;

        }

        public int TotalCopiedGi(string title)
        {
            return Wait.UntilAllElementsLocated(GiRow(title)).Count;
        }

        //Kanban View
        public void SwitchToKanbanView()
        {
            Log.Step(nameof(GrowthItemsPage), "Switch to Kanban view");
            Wait.UntilJavaScriptReady();
            if (Wait.UntilElementExists(GrowthPlanKanbanView).Displayed) return;
            Wait.UntilElementClickable(ToggleButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickKanbanAddNewGrowthItem()
        {
            SwitchToKanbanView();
            Log.Step(nameof(GrowthItemsPage), "In Kanban view, click on New Growth Item button");
            Wait.UntilElementClickable(KanbanNewGrowthItemButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickHistoryOfGrowthItem(string text)
        {
            Log.Step(GetType().Name, "Click on history icon for growth item to open up modal");
            Wait.UntilElementClickable(GiHistoryIcon(text)).Click();
        }

        public IList<string> GetRowDataFromHistoryTable()
        {
            return Wait.UntilAllElementsLocated(GiHistoryRows).Select(row => row.GetText()).ToList();
        }

        public IList<string> GetUsernameFromHistoryEventTable()
        {
            Wait.UntilElementVisible(GiHistoryTableUserNameColumnHeader);
            var rows = GetRowDataFromHistoryTable();

            return (from line in rows let startIndex = 0 let statusIndex = line.IndexOf("Status", StringComparison.Ordinal) let endIndex = startIndex + statusIndex select line.Substring(startIndex, endIndex).Trim()).ToList();
        }

        //Key Customer Verification

        #region GI tab elements

        public bool IsAddNewItemButtonDisplayed()
        {
            return Driver.IsElementDisplayed(AddNewItemButton);
        }
        public bool IsGrowthPlanItemPopupDisplayed()
        {
            return Driver.IsElementDisplayed(AddGrowthPlanItemPopupText);
        }
        public void ClickOnNotificationsButton()
        {
            Log.Step(nameof(GrowthItemsPage), "Click on 'Notifications' button");
            Wait.UntilElementClickable(NotificationsButton).Click();
        }
        public bool IsNotificationsPopupDisplayed()
        {
            return Driver.IsElementDisplayed(NotificationsPopupTitle);
        }
        public void SwitchToGridView()
        {
            Log.Step(nameof(GrowthItemsPage), "Switch to grid view");
            Wait.HardWait(1000);//Takes time to load
            if (Wait.UntilElementExists(GrowthPlanGridView).Displayed) return;
            Wait.UntilElementClickable(ToggleButton).Click();
            Wait.HardWait(1000);//Takes time to load
        }
        public bool IsEditGrowthPlanItemPopupDisplayed()
        {
            Driver.JavaScriptScrollToElement(EditGrowthPlanItemPopup);
            return Driver.IsElementDisplayed(EditGrowthPlanItemPopup);
        }
        #endregion

        public void NavigateToPage(int companyId, int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/team/{companyId}/growthitems/{teamId}");
        }

        //Team assessment Growth Item tab Navigation in prod
        public void NavigateToTeamGrowthItemTabForProd(string env, int companyId, int teamId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/team/{companyId}/growthitems/{teamId}");
        }
    }
}
