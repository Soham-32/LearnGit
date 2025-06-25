using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams
{
    internal class TeamDashboardPage : BasePage
    {
        public TeamDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By TeamLoadIcon = By.XPath("//div[@class='k-loading-image']");
        private readonly By DashboardList = By.XPath("//div[@class='ah-teams-banner']//a");
        private readonly By AddTeamBtn = By.CssSelector(".toggleBar span.add_team_btn");
        private readonly By TeamRadioButton = By.Id("team_btn");
        private readonly By MultiTeamRadioButton = By.Id("multiteam_btn");
        private readonly By EnterpriseTeamRadioButton = By.Id("enterprise_btn");
        private readonly By NTierTeamRadioButton = By.Id("ntier_btn");
        private readonly By SelectTeamPopupAddTeam = By.Id("go");
        private readonly By TeamFilterBox = By.Id("teamFilterBox");
        private readonly By ConfirmDeleteButton = By.Id("delete_confirm");
        private readonly By RemoveReasonMark = By.CssSelector("span[aria-owns='archiveOption_listbox']");
        private readonly By StateButton = By.XPath("//button[@id='btnState'][normalize-space(text())='State']");
        private readonly By ActiveOption = By.Id("showActive");
        private readonly By ArchivedOption = By.Id("showArchived");
        private readonly By BulkDataMgmtButton = By.XPath("//*[@id = 'btnState'][text() = 'Bulk Data Mgmt.']");
        private readonly By BulkDataMgmtMenu = By.XPath("//div[@id='bulkMenu']//span[@role='menuitem']");
        private readonly By DownloadTemplateListItem = By.Id("blkTemplate");
        private readonly By ImportListItem = By.Id("blkImport");
        private readonly By ExportListItem = By.Id("blkExport");
        private readonly By GenerateExtIdentifiersListItem = By.Id("blkIdentifiers");
        private readonly By LogoImg = By.ClassName("navbar-brand");
        private readonly By TeamTypeDropbox = By.CssSelector("span[aria-owns='TeamVsMultiTeam_listbox']");
        private readonly By TeamStartDate = By.Id("teamStartDate");
        private readonly By TeamEndDate = By.Id("teamEndDate");
        private readonly By FilterButton = By.CssSelector("#Teams .submit.btn.green-btn");
        private static By GridColumnHeader(string columnName) => By.CssSelector($"th[data-title='{columnName}'] a:last-child");
        private readonly By ActiveResetFilterButton = By.CssSelector(".active_class #clear");
        private readonly By ExportButton = By.Id("exportDropDown");
        private readonly By ExportExcelButton = By.CssSelector("#exportDropDown-content button.excelBtn");
        private readonly By PendoCloseButton = By.ClassName("_pendo-close-guide");
        private readonly By OpenFilterButton = By.CssSelector(".open-filters");
        private readonly By ClearAllLink = By.Id("dashboardClearAll");
        private readonly By SelectAllLink = By.Id("dashboardShowAll");
        private readonly By ViewButton = By.XPath("//button[contains(text(),'View')]");
        private readonly By GridLink = By.XPath("//span[contains(text(),'Grid')]");
        private readonly By SwimLaneView = By.CssSelector(".team-circles.row");
        private readonly By GridView = By.CssSelector(".team-list");
        private readonly By SwimLaneLink = By.CssSelector(".act-swim-lanes");

        private readonly By TeamsWithZeroMembers = By.XPath("//td[@role='gridcell'][9][@style=''][normalize-space() = '']"); //It will count the cells with no value
        private readonly By GridColumnHeaders = By.XPath("//div[@id='Teams']//th/a[2]");
        private readonly By GridViewRows = By.CssSelector("#Teams tbody tr");
        private readonly By TotalTeamCount = By.CssSelector("div.pagerTop span.k-pager-info.k-label");
        private readonly By TeamAvatarSelectors = By.XPath("//div[@id='Teams']//tbody/tr/td[count(//div[@id='Teams']//thead//th[@data-title='Avatar']/preceding-sibling::th)+1]//img");
        private readonly By TeamLinksSelectors = By.XPath("//div[@id='Teams']//tbody/tr/td[count(//div[@id='Teams']//thead//th[@data-title='Team Name']/preceding-sibling::th)+1]/a[not(@href='#')]");
        private readonly By SwimlaneItemSelectors = By.CssSelector(".swimlane");
        private readonly By EnterpriseTeamLane = By.CssSelector("h4.Enterprise");
        private readonly By MultiTeamLane = By.CssSelector("h4.MultiTeam");
        private readonly By TeamLane = By.CssSelector("h4.Team");
        private readonly By ExpandAllButton = By.CssSelector("div.spn-row-state");
        private readonly By CollapseAllButton = By.CssSelector("div.spn-row-state.on");
        private readonly By ExpandCollapseArrow = By.CssSelector("div.swimlane div.arrow>i");
        private readonly By ResourceCenterButton = By.XPath("//button[@aria-label='Open Resource Center']");
        private readonly By ZenDeskLink = By.XPath("//div//footer//div//a[@data-testid='Icon--zendesk']");
        private readonly By NoItemText = By.XPath("//span[normalize-space()='No items to display']");
        private readonly By FirstTeam = By.XPath("(//tbody[@role='rowgroup']//td[3]//a)[1]");
        private static By TeamName(int index) => By.XPath($"(//td[contains(@data-id, 'teamTitle')])[{index}]");
        private static By TeamName(string teamName) => By.XPath($"//*[@id='Teams']//a[contains(text(), '{teamName}')]");
        private static By TeamEditButton(string teamName) =>
            By.XPath($"//*[@id='Teams']//*[contains(text(), '{teamName}')]/../..//a[contains(@class, 'edit')] | //*[@id='Teams']//*[contains(normalize-space(text()), '{teamName}')]/../..//a[contains(@class, 'edit')]");
        private static By TeamArchiveButton(string teamName) =>
            By.XPath($"//*[@id='Teams']//a[text()='{teamName}']/../..//span[@title = 'Archive']");
        private static By TeamRestoreButton(string teamName) =>
            By.XPath($"//*[@id='Teams']//a[text()='{teamName}']/../..//span[@title = 'Restore']");
        private static By TeamAvatarImage(int rowIndex) => By.XPath(
            $"//div[@id='Teams']//tbody/tr[{rowIndex}]/td[count(//div[@id='Teams']//thead//th[@data-title='Avatar']/preceding-sibling::th)+1]//img");

        //Archive
        private static By SelectArchiveTeamCheckBox(string team) => By.XPath($"//td//a[text()='{team}']//ancestor::tr//td//input[contains(@class,'chkRestore')]");
        private readonly By ArchiveRestoreButton = By.XPath("//div[@id='mainRowOrg']//div//div//span[text()='Restore']");
        private readonly By ArchivedResetFilterButton = By.CssSelector(".archived_class #clearArchived");
        private readonly By ArchivedExportButton = By.Id("exportDropDownArchived");
        private readonly By ArchivedExportExcelButton = By.CssSelector("#exportDropDownArchived-content button.excelBtn");

        //Restore Team confirmation popup
        private readonly By RestoreTeamConfirmationPopupRestoreButton = By.Id("restore_confirm");
        private readonly By RestoreTeamConfirmationPopupCancelButton = By.Id("cancel_restore");

        //Tabs
        private readonly By TeamDashboard = By.CssSelector("div.ah-teams-banner a[href$='teams']");
        private readonly By GrowthItemDashBoard = By.CssSelector("a[href$='growthitems']");
        private readonly By AssessmentDashBoard = By.CssSelector("a[href$='AssessmentManagementDashboard']");
        private readonly By FacilitatorDashboard = By.CssSelector("a[href$='FacilitatorDashboard']");


        private static By SwimLaneItem(string item) =>
            By.XPath($"//div[contains(@class,' swimlane')]//span[contains(@class,'title')][text()='{item}']");

        private static By FilterItem(string item) => By.XPath($"//label[@class='light'][normalize-space(text()) = '{item}']");
        private static By TeamTypeItem(string item) => By.XPath($"//ul[@id='TeamVsMultiTeam_listbox']/li[text()='{item}'] | //ul[@id='TeamVsMultiTeam_listbox']/li//font[text()='{item}']");

        private static By ViewItem(string item) =>
            By.XPath(
                $"//button[@id='btnView']/following-sibling::div[@class='tdropdown-content']/span[text()='{item}']");

        private static By StateItem(string item) =>
            By.XPath(
                $"//button[@id='btnState']/following-sibling::div[@class='tdropdown-content']/span[text()='{item}']");
        private static By ReasonListItem(string reason) => By.XPath($"//ul[@id='archiveOption_listbox']/li[text()='{reason}'] | //ul[@id='archiveOption_listbox']/li//font[text()='{reason}']");
        private static By CellValue(int index, string columnHeader) => By.XPath($"//div[@id='Teams']//tbody/tr[{index}]/td[count(//div[@id='Teams']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]");
        private static By ColumnValue(string columnHeader) => By.XPath($"//div[@id='Teams']//tbody/tr/td[count(//div[@id='Teams']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]");
        private readonly By SelectFilterSymbol = By.CssSelector("th[data-title='Team Name'] span[class='k-icon k-i-arrowhead-s']");
        private readonly By ColumnFilterOption =
            By.XPath("//ul[@role='menubar']/li[contains(@class,'k-columns-item k-state-default')]/span");
        private readonly By ColumnFilterAllColumns = By.XPath("//ul[@role='menubar']/li[@role='menuitem']//ul//li//span");
        private readonly By VisibleColumns = By.CssSelector("ul[style*='display: block'] span.k-link");
        private readonly By BulkProcessingPopup = By.Id("bulk_operation_processing");
        private readonly By ColumnCheckboxes = By.XPath("//ul[@role='menubar']//li[contains(@class,'k-columns-item k-state-default')]/div//input");

        //Key-cutomers verification
        #region
        private readonly By NTierTeamSelectors = By.XPath("//div[@id='Teams']//tbody/tr/td[count(//div[@id='Teams']//thead//th[@data-title='Team Name']/preceding-sibling::th)+1]/span");
        #endregion

        public List<string> GetDashboardsList()
        {
            Log.Step(nameof(TeamDashboardPage), "Get All dashboards list");
            return Driver.GetTextFromAllElements(DashboardList).ToList();
        }

        public string GetAddATeamButtonText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get 'Add A Team' button text");
            return Wait.UntilElementVisible(AddTeamBtn).GetText();
        }

        public void WaitForTeamDashboardToLoad()
        {
            Wait.UntilElementNotExist(TeamLoadIcon);
        }
        public void SelectAllColumns()
        {
            Log.Step(nameof(TeamDashboardPage), "Select all columns");
            SelectItem(SelectFilterSymbol, ColumnFilterOption);
            var elements = Wait.UntilAllElementsLocated(ColumnCheckboxes);
            foreach (var element in elements.Where(element => element.Displayed))
            {
                element.Check();
            }
        }
        public void GridTeamView()
        {
            Log.Step(nameof(TeamDashboardPage), "Switching to Grid View");
            Wait.UntilJavaScriptReady();
            if (!Wait.UntilElementExists(GridView).Displayed)
            {
                MoveToViewButton();
                Wait.UntilElementVisible(GridLink).Click();
                MoveToLogo();
            }
            // wait for the grid to load
            Wait.UntilJavaScriptReady();
            // reset any filters
            CloseWelcomePopup();
            ResetGridView();
        }

        public void SortGridColumn(string columnName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Sort grid column <{columnName}>");
            Wait.UntilElementClickable(GridColumnHeader(columnName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SearchTeam(string search)
        {
            Log.Step(nameof(TeamDashboardPage), $"On Team Dashboard , searching for {search}");
            Wait.UntilJavaScriptReady();

            GridTeamView();
            if (Driver.IsElementDisplayed(ArchivedResetFilterButton))
            {
                Wait.UntilElementClickable(ArchivedResetFilterButton).Click();
                Wait.UntilJavaScriptReady();
            }
            Wait.UntilElementClickable(TeamFilterBox).Clear();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(TeamFilterBox).SetText(search, false);
            Wait.UntilJavaScriptReady();
            Wait.HardWait(2000);
        }
        public void SelectFirstTeamFromDashboard()
        {
            Log.Step(nameof(TeamDashboardPage), "On Team Dashboard , Click on first team.");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(FirstTeam).Click();
        }

        public void GoToTeamAssessmentDashboard(int i)
        {
            Log.Step(nameof(TeamDashboardPage), $"Go to team assessment dashboard at index <{i}>");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamName(i)).Click();
            Wait.UntilJavaScriptReady();
        }


        public void CloseWelcomePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Close Welcome popup");
            if (Driver.IsElementDisplayed(PendoCloseButton))
            {
                Wait.UntilElementClickable(PendoCloseButton).Click();
            }
            Wait.UntilJavaScriptReady();
        }

        public void ClickAddTeamButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click Add Team button");
            Wait.UntilElementVisible(SelectTeamPopupAddTeam);
            Wait.UntilElementClickable(SelectTeamPopupAddTeam).Click();
        }

        public void ClickAddATeamButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click Add A Team button");
            Wait.HardWait(500); // Wait until 'Add Team' button is visible
            Wait.UntilElementVisible(AddTeamBtn);
            Wait.UntilElementClickable(AddTeamBtn).Click();
            Wait.HardWait(2000);// As it takes time to open the popup
        }

        public bool IsAddTeamButtonDisplayed()
        {
            return Driver.IsElementDisplayed(AddTeamBtn);
        }
        public bool IsTeamDisplayed()
        {
            return Driver.IsElementDisplayed(FirstTeam);
        }

        public void SelectTeamType(TeamType teamType)
        {
            Log.Step(nameof(TeamDashboardPage), $"On Select Team popup , select {teamType:G} option");
            switch (teamType)
            {
                case TeamType.Team:
                    ClickTeamType();
                    break;
                case TeamType.MultiTeam:
                    ClickMultiteamType();
                    break;
                case TeamType.EnterpriseTeam:
                    ClickEnterpriseTeamType();
                    break;
                case TeamType.NTier:
                    ClickNTierTeamType();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }
        }

        public void ClickTeamType()
        {
            Wait.UntilElementVisible(TeamRadioButton);
            Wait.UntilElementClickable(TeamRadioButton).Click();
        }

        public void ClickMultiteamType()
        {
            Wait.UntilElementVisible(MultiTeamRadioButton);
            Wait.UntilElementClickable(MultiTeamRadioButton).Click();
        }

        public void ClickEnterpriseTeamType()
        {
            Wait.UntilElementVisible(EnterpriseTeamRadioButton);
            Wait.UntilElementClickable(EnterpriseTeamRadioButton).Click();
        }

        public void ClickNTierTeamType()
        {
            Wait.UntilElementVisible(NTierTeamRadioButton);
            Wait.UntilElementClickable(NTierTeamRadioButton).Click();
        }

        public bool AddTeam_DoesRadioButtonExist(TeamType type)
        {
            return Driver.IsElementDisplayed(type switch
            {
                TeamType.Team => TeamRadioButton,
                TeamType.MultiTeam => MultiTeamRadioButton,
                TeamType.EnterpriseTeam => EnterpriseTeamRadioButton,
                TeamType.NTier => NTierTeamRadioButton,
                _ => null
            });
        }

        public void ClickTeamEditButton(string teamName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Click edit button on team <{teamName}>");
            Wait.HardWait(2000);//Team dashboard taking time to load
            Wait.UntilElementClickable(TeamEditButton(teamName)).Click();
        }

        public bool IsTeamEditButtonDisplayed(string teamName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Is Edit button for team {teamName} displayed ?");
            return Driver.IsElementDisplayed(TeamEditButton(teamName));
        }

        public void DeleteTeam(string teamName, RemoveTeamReason reason)
        {
            Log.Step(nameof(TeamDashboardPage), $"Delete team {teamName} with reason {reason}");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamArchiveButton(teamName)).Click();
            Wait.UntilJavaScriptReady();
            SelectItem(RemoveReasonMark, ReasonListItem(reason.GetDescription()));
            Wait.UntilElementClickable(ConfirmDeleteButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public int GetNumberOfGridRows() => Wait.UntilAllElementsLocated(GridViewRows).Count;

        public string GetAvatarSourceFromGrid(int rowIndex) => Wait.UntilElementVisible(TeamAvatarImage(rowIndex)).GetAttribute("src");

        internal List<string> GetVisibleColumnNamesFromGrid()
        {
            var displayedElements = Wait.UntilAllElementsLocated(GridColumnHeaders).Where(e => e.Displayed);

            return displayedElements.Select(e => e.GetText()).ToList();
        }

        public string GetCellValue(int index, string columnHeader)
        {
            Wait.UntilJavaScriptReady();
            if (!GetVisibleColumnNamesFromGrid().Contains(columnHeader))
            {
                if (columnHeader.Equals("Team Tags"))
                {
                    AddColumns(new List<string> { "Team Tags" });
                }
                else if (columnHeader.Equals("Number of Sub Teams"))
                {
                    AddColumns(new List<string> { "Number of Sub Teams" });
                }
                else if (columnHeader.Equals("Sub Teams"))
                {
                    AddColumns(new List<string> { "Sub Teams" });
                }
                else if (columnHeader.Equals("Multi Teams"))
                {
                    AddColumns(new List<string> { "Multi Teams" });
                }
            }

            Wait.UntilJavaScriptReady();
            var selector = Wait.UntilElementVisible(CellValue(index, columnHeader));
            var cellValue = selector.Text;
            if (new List<string> { "Number of Assessments", "Number of Team Assessments", "Number of Sub Teams", "Number of Team Members" }.Contains(columnHeader)
                && string.IsNullOrEmpty(cellValue))
                cellValue = "0";

            if (columnHeader.Equals("Last Date of Assessment") && !string.IsNullOrEmpty(cellValue))
                cellValue = DateTime.Parse(cellValue).ToString("MM/dd/yyyy");

            return cellValue;
        }

        //Key-customers verification
        #region
        public List<string> GetAllTeamsNames(string filterName = "All")
        {
            //Switching to grid view & selecting All teams
            GridTeamView();
            FilterTeamType(filterName);

            var teamSelector = filterName == "N-Tier Teams" ? NTierTeamSelectors : TeamLinksSelectors;

            return Driver.GetTextFromAllElements(teamSelector).ToList();
        }
        #endregion

        public bool DoesTeamDisplay(string teamName) => Driver.IsElementDisplayed(TeamName(teamName));

        public bool DoesAnyTeamDisplay() => Driver.IsElementDisplayed(NoItemText);

        public void FilterTeamStatus(string status)
        {
            Log.Step(nameof(TeamDashboardPage), $"Filter team status <{status}>");
            MoveToStateButton();

            if (status.Equals("Active"))
            {
                Wait.UntilElementClickable(ActiveOption).Click();
            }
            else
            {
                Wait.UntilElementClickable(ArchivedOption).Click();
            }

            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamFilterBox).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnTeamName(string teamName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Click on {teamName} team");
            Wait.UntilElementVisible(TeamName(teamName));
            Wait.HardWait(3000); // Wait until team dashboard load
            Wait.UntilElementClickable(TeamName(teamName)).Click();
        }
        //Archive
        public void RestoreTeam(string teamName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Restore team <{teamName}>");
            Wait.UntilElementVisible(TeamRestoreButton(teamName));
            Wait.UntilElementClickable(TeamRestoreButton(teamName)).Click();
            RestoreTeamConfirmationPopupClickOnRestoreButton();
        }

        public void SelectArchiveTeam(string teamName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Select archive team {teamName}");
            Wait.UntilElementVisible(SelectArchiveTeamCheckBox(teamName)).Check();
        }

        public void ClickOnArchiveRestoreButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Archive Restore' button");
            Wait.UntilElementClickable(ArchiveRestoreButton).Click();
        }
        public void ClickOnArchivedResetFilterButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Archive Reset' button");
            Wait.UntilElementClickable(ArchivedResetFilterButton).Click();
        }
        public void ArchivedClickOnExportToExcelButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Export to Excel' button");
            SelectItem(ArchivedExportButton, ArchivedExportExcelButton);
            Wait.UntilJavaScriptReady();

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
        }
        public bool IsArchiveResetButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ArchiveRestoreButton);
        }

        //Restore Team confirmation popup
        public void RestoreTeamConfirmationPopupClickOnRestoreButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Restore' button from 'Restore Team' confirmation popup");
            Wait.UntilElementVisible(RestoreTeamConfirmationPopupRestoreButton);
            Wait.UntilElementClickable(RestoreTeamConfirmationPopupRestoreButton).Click();
            Wait.HardWait(3000); //Wait till team will restore
        }
        public void RestoreTeamConfirmationPopupClickOnCancelButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Cancel' button from 'Restore Team' confirmation popup");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(RestoreTeamConfirmationPopupCancelButton).Click();
        }

        public int TotalTeam()
        {
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(GridViewRows);

            return Driver.GetElementCount(GridViewRows);
        }

        public bool VerifyTeamTagsDisplay(string tag)
        {
            var elements = Wait.UntilAllElementsLocated(ColumnValue("Team Tags"));

            return elements.All(element => element.Text.Contains(tag));
        }

        public void MoveToLogo()
        {
            Driver.MoveToElement(Wait.UntilElementVisible(LogoImg));
        }

        public void FilterTeamType(string teamType)
        {
            Log.Step(nameof(TeamDashboardPage), $"Filter team type <{teamType}>");
            SelectItem(TeamTypeDropbox, TeamTypeItem(teamType));
        }

        public bool CheckTeamFilterCorrectly(string teamType)
        {
            Wait.UntilJavaScriptReady();
            var teamAvatars = Wait.UntilAllElementsLocated(TeamAvatarSelectors);
            var teamLinks = Wait.UntilAllElementsLocated(TeamLinksSelectors);
            var avatarFlag = false;
            var linkFlag = false;

            foreach (var element in teamAvatars)
            {
                if (teamType.Equals("Team"))
                {
                    var teamBorderColor = (Driver.IsInternetExplorer()) ? "#fff" : "rgb(0, 174, 239)";
                    var css = element.GetCssValue("border-color");

                    if (!css.Contains(teamBorderColor))
                    {
                        avatarFlag = false;
                        break;
                    }
                }
                else if (teamType.Equals("Multi-Team"))
                {
                    var multiTeamBorderColor = (Driver.IsInternetExplorer()) ? "#fe9a2e" : "rgb(247, 148, 29)";
                    var css = element.GetCssValue("border-color");

                    if (!css.Contains(multiTeamBorderColor))
                    {
                        avatarFlag = false;
                        break;
                    }
                }
                else if (teamType.Equals("Enterprise Team"))
                {
                    var enterpriseTeamBorderColor =
                        (Driver.IsInternetExplorer()) ? "#38b348" : "rgb(57, 181, 74)";
                    var css = element.GetCssValue("border-color");

                    if (!css.Contains(enterpriseTeamBorderColor))
                    {
                        avatarFlag = false;
                        break;
                    }
                }
                avatarFlag = true;
            }

            foreach (var element in teamLinks)
            {
                if (teamType.Equals("Team"))
                {
                    if (!element.GetAttribute("href").Contains("/teams/"))
                    {
                        linkFlag = false;
                        break;
                    }
                }
                else if (teamType.Equals("Multi-Team"))
                {
                    if (!element.GetAttribute("href").Contains("/multiteam/"))
                    {
                        linkFlag = false;
                        break;
                    }
                }
                else if (teamType.Equals("Enterprise Team"))
                {
                    if (!element.GetAttribute("href").Contains("/enterprise/"))
                    {
                        linkFlag = false;
                        break;
                    }
                }
                linkFlag = true;
            }

            return avatarFlag && linkFlag;
        }

        public void FilterTeamDate(string startDate, string endDate)
        {
            Log.Step(nameof(TeamDashboardPage), $"Filter team date with start date <{startDate}> and en date <{endDate}>");
            ResetGridView();

            Wait.UntilElementVisible(TeamStartDate).Clear();
            Wait.UntilElementVisible(TeamEndDate).Clear();

            if (!string.IsNullOrEmpty(startDate))
            {
                Wait.UntilElementVisible(TeamStartDate).SetText(startDate);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                Wait.UntilElementVisible(TeamEndDate).SetText(endDate);
            }

            Wait.UntilElementVisible(FilterButton).Click();

            Wait.UntilJavaScriptReady();
        }

        public bool CheckDateFilterCorrectly(string startDate, string endDate)
        {
            var elements = Wait.UntilAllElementsLocated(ColumnValue("Last Date of Assessment"));
            var flag = true;

            foreach (var element in elements)
            {
                if (string.IsNullOrEmpty(element.Text)) continue;
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    if (CSharpHelpers.CompareTwoDate(element.Text, startDate) >= 0 ||
                        CSharpHelpers.CompareTwoDate(element.Text, endDate) <= 0) continue;
                    flag = false;
                    break;
                }
                if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    if (CSharpHelpers.CompareTwoDate(element.Text, startDate) >= 0) continue;
                    flag = false;
                    break;
                }

                if (CSharpHelpers.CompareTwoDate(element.Text, endDate) <= 0) continue;
                flag = false;
                break;

            }

            return flag;
        }

        public IList<string> GetColumnValues(string columnName)
        {
            Wait.UntilJavaScriptReady();
            var originalValues = Driver.GetTextFromAllElements(ColumnValue(columnName));
            return FormatColumn(columnName, originalValues);
        }
        public IList<string> GetTeamsColumnValues(string columnName)
        {
            Log.Step(nameof(TeamDashboardPage), "Get all teams column value");
            Wait.UntilJavaScriptReady();
            var originalValues = Driver.GetTextFromAllElements(ColumnValue(columnName));
            var getColumnValueList = originalValues.Select(a => a.Split(',')).Select(q => q.FirstOrDefault()).ToList();
            return FormatColumn(columnName, getColumnValueList);
        }
        public void ClickExportToExcel()
        {
            Log.Step(nameof(TeamDashboardPage), "Click Export to Excel button");
            SelectItem(ExportButton, ExportExcelButton);
            Wait.UntilJavaScriptReady();

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
        }

        public void ClickFilter()
        {
            Log.Step(nameof(TeamDashboardPage), "Click Filter");
            Wait.UntilElementClickable(OpenFilterButton).Click();
        }

        public void ClickClearAllLink()
        {
            Log.Step(nameof(TeamDashboardPage), "Click Clear All");
            Wait.UntilElementClickable(ClearAllLink).Click();
        }

        public void ClickSelectAllLink()
        {
            Log.Step(nameof(TeamDashboardPage), "Click Select All");
            Wait.UntilElementClickable(SelectAllLink).Click();
        }

        public int TotalSwimlane() => Wait.UntilAllElementsLocated(SwimlaneItemSelectors).Count(ele => ele.Displayed);

        public void SelectFilterItem(string item)
        {
            Log.Step(nameof(TeamDashboardPage), $"Select filter item <{item}>");
            Wait.UntilElementClickable(FilterItem(item)).Click();
        }

        public bool DoesSwimlaneItemDisplay(string item) => Driver.IsElementDisplayed(SwimLaneItem(item));

        internal void GoToSwimLaneView()
        {
            Log.Step(nameof(TeamDashboardPage), "Go to Swim Lane");
            if (Driver.IsElementDisplayed(SwimLaneView)) return;
            Wait.UntilElementClickable(ViewButton).Click();
            Wait.UntilElementClickable(SwimLaneLink).Click();
        }

        internal void ResetGridView()
        {
            Log.Step(nameof(TeamDashboardPage), "Reset grid view");
            if (Driver.IsElementDisplayed(ActiveResetFilterButton))
            {
                Wait.UntilElementClickable(ActiveResetFilterButton).Click();
            }
            else
            {
                Wait.UntilElementClickable(ArchivedResetFilterButton).Click();
            }

            Wait.UntilJavaScriptReady();
        }

        public string GetFilterText() => Wait.UntilElementExists(TeamFilterBox).GetText();

        public void MoveToViewButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Move to View button");
            Driver.MoveToElement(Wait.UntilElementExists(ViewButton));
        }

        public bool DoesViewItemExist(string item) => Driver.IsElementDisplayed(ViewItem(item));

        public void MoveToStateButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Move to State button");
            Driver.MoveToElement(Wait.UntilElementExists(StateButton));
        }

        public bool DoesStateItemExist(string item) => Driver.IsElementDisplayed(StateItem(item));

        public void MoveToBulkDataMgmtButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Move to Bulk Data Mgmt. button");
            Driver.MoveToElement(Wait.UntilElementExists(BulkDataMgmtButton));
        }

        public List<string> GetBulkDataMgmtMenuList()
        {
            
            Log.Step(nameof(TeamDashboardPage), "Get All Bulk Data Management Menu List");

            var options = Driver.FindElements(BulkDataMgmtMenu);

            // Extract text from web elements
            var actualValues = options.Select(option => option.Text.Trim()).ToList();

            return actualValues;
        }

        public void ClickOnBulkDownloadTemplate()
        {
            MoveToBulkDataMgmtButton();
            Log.Step(nameof(TeamDashboardPage), "Select 'Download Template' on the Bulk Data Mgmt dropdown");
            Wait.UntilElementVisible(DownloadTemplateListItem).Click();
        }

        public void ClickOnBulkImport()
        {
            MoveToBulkDataMgmtButton();
            Log.Step(nameof(TeamDashboardPage), "Select 'Import' on the Bulk Data Mgmt dropdown");
            Wait.UntilElementVisible(ImportListItem).Click();
        }

        public void ClickOnBulkExport()
        {
            MoveToBulkDataMgmtButton();
            Log.Step(nameof(TeamDashboardPage), "Select 'Export' on the Bulk Data Mgmt dropdown");
            Wait.UntilElementVisible(ExportListItem).Click();
        }

        public void ClickOnBulkGenerateExtIdentifiers()
        {
            MoveToBulkDataMgmtButton();
            Log.Step(nameof(TeamDashboardPage), "Select 'Generate Ext. Identifiers' on the Bulk Data Mgmt dropdown");
            Wait.UntilElementVisible(GenerateExtIdentifiersListItem).Click();
            Wait.UntilElementVisible(BulkProcessingPopup);
            Wait.UntilElementInvisible(BulkProcessingPopup);
        }

        public List<string> EtLaneColorsList() =>
            Wait.UntilAllElementsLocated(EnterpriseTeamLane).Select(ele =>
                CSharpHelpers.ConvertRgbToHex(ele.GetCssValue("background-color")).ToUpper()).ToList();

        public List<string> MtLaneColorsList() =>
            Wait.UntilAllElementsLocated(MultiTeamLane).Select(ele =>
                CSharpHelpers.ConvertRgbToHex(ele.GetCssValue("background-color")).ToUpper()).ToList();

        public List<string> TeamLaneColorsList() =>
            Wait.UntilAllElementsLocated(TeamLane).Select(ele =>
                CSharpHelpers.ConvertRgbToHex(ele.GetCssValue("background-color")).ToUpper()).ToList();

        public void ClickGrowthItemDashBoard()
        {
            Log.Step(nameof(TeamDashboardPage), "On team dashboard, click on Growth Item dashboard");
            Wait.UntilElementVisible(GrowthItemDashBoard);
            Wait.UntilElementClickable(GrowthItemDashBoard).Click();
            Wait.UntilJavaScriptReady();
        }

        //Key-customers-verification
        #region
        public bool DoesExpandButtonExist() => Driver.IsElementDisplayed(ExpandAllButton);
        #endregion

        public void ExpandAllSwimLane()
        {
            Log.Step(nameof(TeamDashboardPage), "On team dashboard, Expand All Swim Lane");
            Wait.UntilElementClickable(ExpandAllButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void CollapseAllSwimLane()
        {
            Log.Step(nameof(TeamDashboardPage), "On team dashboard, Collapse All Swim Lane");
            Wait.UntilElementClickable(CollapseAllButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsExpandAllWorkingProperly() =>
            Wait.UntilAllElementsLocated(ExpandCollapseArrow).All(ele => ele.GetElementAttribute("class").Equals("fa fa-chevron-down rotate") || ele.GetElementAttribute("class").Equals("rotate"));

        public bool IsCollapseAllWorkingProperly() =>
            Wait.UntilAllElementsLocated(ExpandCollapseArrow).All(ele => string.IsNullOrEmpty(ele.GetElementAttribute("class")) || ele.GetElementAttribute("class").Equals("fa fa-chevron-down"));

        //Tabs
        public void ClickOnTeamDashboard()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on Team Dashboard");
            Wait.UntilElementClickable(TeamDashboard).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickAssessmentDashBoard()
        {
            Log.Step(nameof(TeamDashboardPage), "On team dashboard, click on Assessment Dashboard");
            Wait.UntilElementVisible(AssessmentDashBoard);
            Wait.UntilElementClickable(AssessmentDashBoard).Click();
            Wait.UntilJavaScriptReady();
        }


        public void ClickFacilitatorDashboard()
        {
            Log.Step(nameof(TeamDashboardPage), "On team dashboard, click on Facilitator Dashboard");
            Wait.UntilElementClickable(FacilitatorDashboard).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickResourceCenterButton()
        {
            Log.Step(nameof(TeamDashboardPage), "On team dashboard, click on 'Resource Center' Button");
            Wait.UntilElementClickable(ResourceCenterButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsFacilitatorDashboardDisplayed() => Driver.IsElementDisplayed(FacilitatorDashboard);


        public bool IsResourceCenterButtonDisplayed()
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(ResourceCenterButton, 60);
        }

        public bool IsZenDeskLinkDisplayed()
        {
            ClickResourceCenterButton();
            Driver.SwitchToFrame(By.Id("webWidget"));
            return Driver.IsElementDisplayed(ZenDeskLink, 60);
        }
        public bool IsGrowthItemsDashboardDisplayed() => Driver.IsElementDisplayed(GrowthItemDashBoard);

        public void AddColumns(List<string> columns)
        {
            Log.Step(nameof(TeamDashboardPage), $"On team dashboard, add columns <{string.Join(",", columns)}>");
            SelectItem(SelectFilterSymbol, ColumnFilterOption);

            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
                Wait.UntilJavaScriptReady();
                var elementText = Driver.MoveToElement(ele).GetAttribute("textContent");
                var checkbox = Wait.ForSubElement(ele, By.TagName("input"));

                if (columns.Contains(elementText))
                {
                    checkbox.Check();
                }
                Wait.UntilJavaScriptReady();
            }
            Wait.UntilElementClickable(SelectFilterSymbol).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetAllColumnNames()
        {
            Log.Step(nameof(TeamDashboardPage), "Get all columns form 'column' filter");
            SelectItem(SelectFilterSymbol, ColumnFilterOption);
            var columnList = Wait.UntilAllElementsLocated(ColumnFilterAllColumns).Select(a => a.GetText()).ToList();
            return columnList;
        }

        public bool DoesTeamsListExist() => Driver.IsElementDisplayed(GridView) || Driver.IsElementDisplayed(SwimLaneView);

        public bool IsBulkManagementButtonDisplayed() => Driver.IsElementDisplayed(BulkDataMgmtButton);

        public IList<string> FormatColumnDates(IEnumerable<string> actualColumnText) =>
            actualColumnText.Select(t => string.IsNullOrWhiteSpace(t)
                ? t : DateTime.Parse(t).ToString("yyyy/MM/dd")).ToList();

        private static IList<string> FormatColumnNumbers(IEnumerable<string> actualColumnText) =>
            actualColumnText.Select(t => string.IsNullOrWhiteSpace(t)
                ? t : int.Parse(t).ToString("D2")).ToList();


        private static IList<string> FormatColumnTags(IEnumerable<string> actualColumnText) =>
            actualColumnText.Select(t => string.IsNullOrWhiteSpace(t)
                ? t : t.Split(',')[0].Trim()).ToList();

        public IList<string> FormatColumn(string columnName, IList<string> actualColumnText) =>
            columnName switch
            {
                // format the values so they can be sorted
                "Last Date of Assessment" => FormatColumnDates(actualColumnText),
                "Number of Sub Teams" => FormatColumnNumbers(actualColumnText),
                "Number of Team Assessments" => FormatColumnNumbers(actualColumnText),
                "Number of Assessments" => FormatColumnNumbers(actualColumnText),
                "Number of Individual Assessments" => FormatColumnNumbers(actualColumnText),
                "Team Tags" => FormatColumnTags(actualColumnText),
                _ => actualColumnText
            };

        public string GetTeamAssessmentDashboardUrl(string teamName)
        {
            Wait.UntilJavaScriptReady();
            var element = Wait.InCase(TeamName(teamName));
            if (element == null)
            {
                GridTeamView();
                Wait.HardWait(2000); //due to take time to load team dashboard
                SearchTeam(teamName);
                element = Wait.UntilElementExists(By.LinkText(teamName));
            }

            return element.GetElementAttribute("href");

        }

        public string GetTeamIdFromLink(string teamName)
        {
            var url = GetTeamAssessmentDashboardUrl(teamName);
            return url.Split('/')[4];
        }

        public int GetCountOfTeamsWithZeroMembers()
        {
            Log.Step(nameof(TeamDashboardPage), "Get count of teams with zero members");
            return Driver.FindElements(TeamsWithZeroMembers).Count;
        }
        public int GetCountofTeamsByWorkType(string workType)
        {
            Log.Step(nameof(TeamDashboardPage), $"Search team by {workType}, then filter it by teams.");
            SearchTeam(workType);
            FilterTeamType("Team");
            var text = Driver.FindElement(TotalTeamCount).Text;
            if (text == "No items to display") { return 0; }
            var match = Regex.Match(text, @"of (\d+) items");
            return int.Parse(match.Groups[1].Value);
        }

        public int GetCountOfAllActiveTeams()
        {
            Log.Step(nameof(TeamDashboardPage), $"Calculate the all active teams count");
            var totalTeams = GetCountofTeamsByWorkType("");
            SortGridColumn("Number of Team Members");
            int teamsWithZeroMembers = GetCountOfTeamsWithZeroMembers();
            int groupOfIndividualsTeamCount = GetCountofTeamsByWorkType("Group Of Individuals");
            var activeTeamsCount = totalTeams - (teamsWithZeroMembers + groupOfIndividualsTeamCount);
            return activeTeamsCount;
        }

        //Navigation

        public void NavigateToPage(int companyId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/company/{companyId}/teams");
            Wait.UntilJavaScriptReady();
        }

        public void NavigateToTeamDashboardPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/teams");
        }
    }

    public enum TeamType
    {
        Team,
        MultiTeam,
        EnterpriseTeam,
        NTier
    }
    public enum RemoveTeamReason
    {
        [Description("Archive - Project Completed")]
        ArchiveProjectCompleted,
        [Description("Archive - Team Disbanded")]
        ArchiveTeamDisbanded,
        [Description("Archive - No Budget")]
        ArchiveNoBudget,
        [Description("Archive - No Longer Using AgilityHealth")]
        ArchiveNoLongerUsingAgilityHealth,
        [Description("Archive - Other")]
        ArchiveOther,
        [Description("Permanently Delete")]
        PermanentlyDelete
    }
}