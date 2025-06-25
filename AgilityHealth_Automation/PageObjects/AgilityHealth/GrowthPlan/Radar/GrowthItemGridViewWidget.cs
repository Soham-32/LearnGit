using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar
{
    internal class GrowthItemGridViewWidget : AssessmentDetailsCommonPage
    {
        private readonly RadarPage Radar;
        public GrowthItemGridViewWidget(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Radar = new RadarPage(driver, Log);
        }

        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;

        private readonly By AddNewGrowthItemButton = By.CssSelector("#GrowthPlan a.k-grid-add");
        private readonly By ExportButton = By.Id("exportDropDown");
        private readonly By ExportExcelButton = By.CssSelector("#exportDropDown-content button.excelBtn");
        private readonly By AllDeleteGiButtons = By.XPath("//div[@id = 'GrowthPlan']//table/tbody/tr//a[text()='Delete']");
        // Growth Item Grid
        private readonly By GiRows = By.XPath("//div[@id = 'GrowthPlan']//table/tbody/tr");
        private static By GiRow(string giTitle) => By.XPath($"//div[@id = 'GrowthPlan']//table/tbody//td[contains(normalize-space(),'{giTitle}')]/..");
        private static By GiByDescriptionAndCategory(string giDescription, string growthItemType) => By.XPath(
            $"//div[@id = 'GrowthPlan']//table/tbody//td[contains(normalize-space(),'{giDescription}')]/..//td[contains(normalize-space(),'{growthItemType}')]");
        private readonly By SelectFilterSymbol = By.CssSelector("th[data-title='Title'] span[class='k-icon k-i-arrowhead-s']");
        private readonly By ColumnFilterOption =
        By.XPath("//ul[@role='menubar']/li[contains(@class,'k-columns-item k-state-default')]/span");
        private readonly By ColumnCheckboxes =
        By.XPath("//ul[@role='menubar']/li[contains(@class,'k-columns-item k-state-default')]/div//input");
        private readonly By VisibleColumns = By.CssSelector("ul[style*='display: block'] span.k-link");
        private static By GiCopyButtonFromGrid(string giTitle) => By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[contains(normalize-space(),'{giTitle}')]/following-sibling::td//*[contains(normalize-space(),'Copy')]");
        private static By GiDeleteButtonFromGrid(int rowIndex) => By.XPath($"//div[@id = 'GrowthPlan']//table/tbody/tr[{rowIndex}]//a[text()='Delete']");
        private static By GiDeleteButtonFromGrid(string giTitle) => By.XPath($"//div[@id = 'GrowthPlan']//table/tbody/tr/td[contains(normalize-space(),'{giTitle}')]/following-sibling::td//a[contains(normalize-space(),'Delete')]");
        private static By GiEditButtonFromGrid(string giTitle) =>
            By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[contains(normalize-space(),'{giTitle}')]/following-sibling::td//*[contains(normalize-space(),'Edit')]");
        private static By GiRowValueByColumn(int rowIndex, string columnName) => By.XPath($"//div[@id='GrowthPlan']//tbody//tr[{rowIndex}]/td[count(//div[@id='GrowthPlan']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");
        private static By GiRowValueByColumn(string giTitle, string columnName) => By.XPath($"//div[@id='GrowthPlan']//tbody//tr/td[contains(normalize-space(), '{giTitle}')]/../td[count(//div[@id='GrowthPlan']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");

        // Growth Items Pull/Push Dialog
        private readonly By PullGiFromPushPullDialogBox = By.CssSelector("a.k-grid-PullItemsFromSubTeams");
        private readonly By ClassicPullItemCountLabel = By.CssSelector(".k-grid-PullItemsFromSubTeams #pullableCount");
        private readonly By PullDialogCloseButton = By.XPath("//div[@id='pullDialog']/preceding-sibling::div//span[@class='k-icon k-i-close']");
        private readonly By ShowPullableItemButton = By.CssSelector(".k-grid-checkBoxPull");
        private readonly By PullUnpullDownArrow = By.XPath("//div[@id='pullDialog']//span[text()='Column Settings']");
        private readonly By ColumnTabForCheckboxes = By.XPath("//div[@class='k-animation-container']//span[text()='Columns']");
        private static By GiUnpullButtonFromGrid(string giTitle) => By.XPath($"//div[@id = 'GrowthPlan']//table/tbody/tr/td[contains(normalize-space(),'{giTitle}')]/following-sibling::td//*[text()='Unpull']");
        private static By PullFromSubTeamButton(string item) => By.XPath(
            $"//div[@id='pullGrowthPlanData']//table//td[contains(normalize-space(), '{item}')]/preceding-sibling::td//span[@class='pullbutton']/..");
        private static By UnPullFromSubTeamButton(string item) => By.XPath(
            $"//div[@id='pullGrowthPlanData']//table//td[contains(normalize-space(),'{item}')]/preceding-sibling::td//span[@class='unpullbutton']/..");
        private readonly By UnpullGrowthPlanItemPopUpYesButton = By.Id("yesButton");
        private static By GiValueByColumnInPushPullDialogBox(string giTitle, string columnName) => By.XPath(
            $"//div[@id='pullDialog']//tbody//tr/td[contains(normalize-space(), '{giTitle}')]/../td[count(//div[@id='pullDialog']//th[@data-title='{columnName}']/a/../preceding-sibling::th) + 1]");


        //Growth Item Grid
        public void ClickAddNewGrowthItem()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Click on Add Growth Item button");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker, false);
            SwitchToGridView();
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilElementClickable(AddNewGrowthItemButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public string GetAddNewItemButtonText()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Get 'Add New Item' button text");
            return Wait.UntilElementVisible(AddNewGrowthItemButton).GetText();
        }

        public void ClickGrowthItemEditButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), $"Click on Edit button for <{giTitle}>");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            Wait.UntilElementClickable(GiEditButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void DeleteGrowthItem(int rowIndex)
        {
            SwitchToGridView();
            Log.Step(nameof(GrowthItemGridViewWidget), $"Click on Delete button at index <{rowIndex}>");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            Wait.UntilElementClickable(GiDeleteButtonFromGrid(rowIndex)).Click();

            AcceptDelete();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void DeleteGrowthItem(string giTitle)
        {
            SwitchToGridView();
            Log.Step(nameof(GrowthItemGridViewWidget), $"Delete growth item <{giTitle}>");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            Wait.UntilElementClickable(GiDeleteButtonFromGrid(giTitle)).Click();
            AcceptDelete();
        }

        public int GetGrowthItemCount()
        {
            Wait.UntilJavaScriptReady(Timeout);
            return Driver.GetElementCount(GiRows);
        }

        public GrowthItem GetGrowthItemFromGrid(int rowIndex)
        {
            Wait.UntilJavaScriptReady(Timeout);
            var targetDate = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Target Date"))
            .GetAttribute("textContent");
            var date = !string.IsNullOrEmpty(targetDate) ? DateTime.Parse(targetDate) : (DateTime?)null;

            var growthItem = new GrowthItem
            {
                Title = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Title")).GetAttribute("textContent"),
                Status = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Status")).GetAttribute("textContent"),
                Priority = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Priority")).GetAttribute("textContent"),
                Description = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Description")).GetAttribute("textContent"),
                Category = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Category")).GetAttribute("textContent"),
                Type = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Type")).GetAttribute("textContent"),
                TargetDate = date,
                Size = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Size")).GetAttribute("textContent"),
                Color = CSharpHelpers
               .ConvertRgbToHex(Wait.UntilAllElementsLocated(GiRows)[rowIndex - 1].GetCssValue("background-color")).ToLower(),
                CompetencyTargets = Wait.UntilElementExists(GiRowValueByColumn(rowIndex, "Competency Target")).GetAttribute("textContent").Split(',').ToList()
            };

            return growthItem;
        }

        public GrowthItem GetGrowthItemFromGrid(string giTitle, bool isGiDashboard = false)
        {
            Wait.UntilJavaScriptReady(Timeout);
            var targetDate = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Target Date"))
            .GetAttribute("textContent");
            var date = !string.IsNullOrEmpty(targetDate) ? DateTime.Parse(targetDate) : (DateTime?)null;

            var growthItem = new GrowthItem
            {
                Title = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Title")).GetAttribute("textContent"),
                Status = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Status")).GetAttribute("textContent"),
                Priority =
            Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Priority")).GetAttribute("textContent"),
                Description = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Description"))
            .GetAttribute("textContent"),
                Category =
            Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Category")).GetAttribute("textContent"),
                Type = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Type")).GetAttribute("textContent"),
                TargetDate = date,
                Size = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Size")).GetAttribute("textContent"),
                Color = CSharpHelpers
            .ConvertRgbToHex(Wait.UntilElementExists(GiRow(giTitle)).GetCssValue("background-color")).ToLower(),
            };
            if (!isGiDashboard) return growthItem;
            growthItem.Dimension = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Dimension")).GetAttribute("textContent");
            growthItem.SubDimension = Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Sub-Dimension")).GetAttribute("textContent");
            return growthItem;
        }

        public string GetGrowthItemId(string giTitle)
        {
            Log.Step(nameof(AddGrowthItemPopupPage), $"Get Growth Item Id by {giTitle}");
            return Wait.UntilElementExists(GiRowValueByColumn(giTitle, "Id")).GetAttribute("textContent");
        }

        public void ClickExportToExcel()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Click Export to Excel button");
            Radar.ClickOnGrowthPlanQuickLink();
            SelectItem(ExportButton, ExportExcelButton);
            Wait.UntilJavaScriptReady(Timeout);

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
        }

        public void SelectAllColumn()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Select all columns");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            SelectItem(SelectFilterSymbol, ColumnFilterOption);

            var elements = Wait.UntilAllElementsLocated(ColumnCheckboxes);

            foreach (var element in elements.Where(element => element.Displayed))
            {
                element.Check();
                Wait.UntilJavaScriptReady(Timeout);
            }

        }

        public void AddColumns(List<string> columns)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), $"Add columns <{string.Join(",", columns)}>");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            SelectItem(SelectFilterSymbol, ColumnFilterOption);

            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
                var elementText = Driver.MoveToElement(ele).GetAttribute("textContent");
                var checkbox = Wait.ForSubElement(ele, By.TagName("input"));
                checkbox.Check(columns.Contains(elementText));

                Wait.UntilJavaScriptReady(Timeout);
            }

        }

        public void DeleteAllGIs()
        {
            SwitchToGridView();
            Log.Step(nameof(GrowthItemGridViewWidget), "Delete all growth items");
            try
            {
                Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
                if (Wait.InCase(AllDeleteGiButtons) == null) return;
                int numberOfItems;
                do
                {
                    Wait.UntilAllElementsLocated(AllDeleteGiButtons)[0].Click(); //If no delete button,but only Edit,Copy buttons then it gives exception. It happens when item copied by next assessment
                    AcceptDelete();
                    Wait.UntilJavaScriptReady();
                    numberOfItems = Wait.UntilAllElementsLocated(AllDeleteGiButtons).Count;
                } while (numberOfItems > 0);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public string GetCellValue(string giTitle, string column) =>
            Wait.UntilElementExists(GiRowValueByColumn(giTitle, column)).GetAttribute("textContent");

        public bool IsGiPresent(string title) => Driver.IsElementPresent(GiRow(title), 15);

        public bool IsGiPresentWithDescriptionAndCategory(string giDescription, GrowthItemType growthItemType) =>
            Driver.IsElementPresent(GiByDescriptionAndCategory(giDescription, growthItemType.GetDescription()));

        // Growth Items Pull/Push Popup
        public void ClickPullItemFromSubTeam()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Click on Pull Item From Subteam button");
            SwitchToGridView();
            Driver.JavaScriptScrollToElement(PullGiFromPushPullDialogBox, false);
            Wait.UntilElementEnabled(PullGiFromPushPullDialogBox); //Added as it was unable to click Pull button 
            Wait.UntilElementClickable(PullGiFromPushPullDialogBox).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void PullItemFromSubTeam(string itemTitle)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), $"Click on Pull From Subteam button for <{itemTitle}>");
            Wait.UntilElementClickable(PullFromSubTeamButton(itemTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public void UnPullItemFromSubTeam(string itemTitle)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), $"Click on UnPull From Subteam button for <{itemTitle}>");
            Wait.UntilElementClickable(UnPullFromSubTeamButton(itemTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public bool IsUnpullGrowthItemButtonDisplayed(string growthItemTitle) => Driver.IsElementDisplayed(GiUnpullButtonFromGrid(growthItemTitle));

        public void ClickOnUnpullItemButtonFromGrid(string growthItemTitle)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Click on 'UnPull' button from Growth item grid.");
            Wait.UntilElementClickable(GiUnpullButtonFromGrid(growthItemTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void ClickOnUnpullGrowthPlanPopUpYesButton()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Click on 'Yes' button from Unpull Growth plan popup.");
            Wait.UntilElementClickable(UnpullGrowthPlanItemPopUpYesButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public void ClickClosePullDialog()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Click on Close pull dialog symbol");
            Wait.UntilElementClickable(PullDialogCloseButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public void UnpullGrowthItem(string giTitle)
        {
            Log.Step(nameof(GrowthItemGridViewWidget), $"Unpull growth item <{giTitle}>");
            Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            Wait.UntilElementClickable(GiUnpullButtonFromGrid(giTitle)).Click();
            AcceptDelete();
        }

        public void OpenColumnMenu()
        {
            SelectItem(PullUnpullDownArrow, ColumnTabForCheckboxes);
        }
        public void CloseColumnMenu()
        {
            Wait.UntilElementClickable(PullUnpullDownArrow).Click();
        }

        public void SelectAllColumnInPushPullDialogBox()
        {
            Log.Step(nameof(GrowthItemGridViewWidget), "Select all columns");
            OpenColumnMenu();
            var elements = Wait.UntilAllElementsLocated(ColumnCheckboxes);

            foreach (var element in elements.Where(element => element.Displayed))
            {
                element.Check();
                Wait.UntilJavaScriptReady(Timeout);
            }
            CloseColumnMenu();
        }

        public GrowthItem GetGiInfoFromPushPullDialogBox(string giTitle)
        {
            Wait.UntilJavaScriptReady();
            var targetDate = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Target Date"))
                .GetAttribute("textContent");
            DateTime? date = null;
            if (!string.IsNullOrEmpty(targetDate))
            {
                date = DateTime.Parse(targetDate);
            }
            var growthItem = new GrowthItem
            {
                Id = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Id")).GetAttribute("textContent"),
                Title = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Title")).GetAttribute("textContent"),
                Owner = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Owner")).GetAttribute("textContent"),
                Status = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Status")).GetAttribute("textContent"),
                Priority =
                    Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Priority")).GetAttribute("textContent"),
                Description = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Description"))
                    .GetAttribute("textContent"),
                Category =
                    Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Category")).GetAttribute("textContent"),
                Type = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Type")).GetAttribute("textContent"),
                TargetDate = date,
                Size = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Size")).GetAttribute("textContent"),
                AffectedTeams = Wait.UntilElementExists(GiValueByColumnInPushPullDialogBox(giTitle, "Affected Teams")).GetAttribute("textContent"),
            };

            return growthItem;
        }

        public int GetPullableItemCount()
        {
            Log.Info("Get pullable growth item count from 'Pull Items From Sub Teams' button");
            return Wait.UntilElementVisible(ClassicPullItemCountLabel).GetText().GetDigits();
        }

        public bool IsPullItemFromSubteamDisplayed() => Driver.IsElementDisplayed(PullGiFromPushPullDialogBox);

        public bool IsShowPullableItemDisplayed() => Driver.IsElementDisplayed(ShowPullableItemButton);

        public bool IsPullItemDisplayed(string title) => Driver.IsElementDisplayed(PullFromSubTeamButton(title));

        public bool IsUnPullItemDisplayed(string title) => Driver.IsElementDisplayed(UnPullFromSubTeamButton(title));

        public bool IsAddNewGrowthItemButtonDisplayed() => Driver.IsElementDisplayed(AddNewGrowthItemButton);
        public bool IsEditGrowthItemButtonDisplayed(string title) => Driver.IsElementDisplayed(GiEditButtonFromGrid(title));
        public bool IsCopyGrowthItemButtonDisplayed(string title) => Driver.IsElementDisplayed(GiCopyButtonFromGrid(title));
        public bool IsDeleteGrowthItemButtonDisplayed(string title) => Driver.IsElementDisplayed(GiDeleteButtonFromGrid(title));
        public void ClickCopyGrowthItemButton(string giTitle)
        {
            Log.Info($"Click on copy growth item {giTitle} button");
            Wait.UntilElementClickable(GiCopyButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public int GetCopiedGiCount(string title)
        {
            return Wait.UntilAllElementsLocated(GiRow(title)).Count;
        }

        public enum GrowthItemType
        {
            [Description("Team")]
            TeamGi,
            [Description("Individual")]
            IndividualGi,
            [Description("Organizational")]
            OrganizationalGi,
            [Description("Management")]
            ManagementGi,
        }
    }
}