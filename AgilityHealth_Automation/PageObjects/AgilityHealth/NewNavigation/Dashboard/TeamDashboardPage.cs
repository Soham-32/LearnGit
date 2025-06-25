using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System;
using AgilityHealth_Automation.Enum.NewNavigation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard
{
    public class TeamDashboardPage : BasePage
    {
        public TeamBasePage TeamBase { get; set; }
        public TeamDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            TeamBase = new TeamBasePage(driver, log);
        }

        #region Locators

        #region Header
        private readonly By GainInsightsTab = By.XPath("//span[text()='Gain Insights']");
        private readonly By MeasureTab = By.XPath("//span[contains(text(),'Measure')]");
        private readonly By ActiveTab = By.CssSelector("#mainTab .nav-link.active");
        private readonly By GrowthItemTab = By.XPath("//a[contains(text(),'Growth Items')]");
        private readonly By LearnTab = By.XPath("//span[text()='Learn']");
        private readonly By AchieveOutcomeTab = By.XPath("//button[normalize-space(@class)='nav-link']//span[text()='Achieve Outcomes']");
        #endregion

        #region Team Dashboard Header
        private readonly By CreateNewTeamButton = By.Id("createnewteam");
        private readonly By ActiveArchiveButton = By.Id("btnMenuActiveArchive");
        private readonly By ActiveMenuItem = By.CssSelector("li#active");
        private readonly By ArchivedMenuItem = By.CssSelector("li#archived");
        private readonly By BulkMgmtButton = By.Id("ddMenuBulkOperations");
        private static By BulkMgmtItem(string item) => By.XPath($"//div[@id='btnMenuBulkOperations']//li[./a[text()='{item}']]");
        #endregion

        #region Filters
        private readonly By SearchTextbox = By.Id("teamFilterBox");
        private readonly By GridViewIcon = By.Id("aGridView");
        private readonly By TeamLevelTextbox = By.CssSelector(".team-filter-input [aria-owns='TeamVsMultiTeam_listbox']");
        private static By TeamLevelItem(string item) => By.XPath($"//ul[@id='TeamVsMultiTeam_listbox']/li/div[text()='{item}']");
        private static readonly By ExportButton = By.Id("exportDropDown");
        private static readonly By ExcelButton = By.CssSelector("#exportDropDown .excelBtn");
        #endregion

        #region Table Grid
        private readonly By FirstOnlyTag = By.XPath("//div[@class='firstonlytag']");  //Rename Rwquired
        private readonly By FirstTeamTag = By.XPath("//div[@class='firstwithmoretag']/div/div");
        private readonly By MoreTags = By.XPath("//div[@class='extratags']/preceding-sibling::span");
        private readonly By ExtraTeamTags = By.XPath("//div[@class='extratags']/div");
        private static By TeamEditButton(string teamName) => By.XPath($"//table[@role='grid']//tbody//tr//td//a[text()='{teamName}']/../following-sibling::td/div[@class='gridMenuButton']//li[contains(@id, 'menu_Edit')]");
        private static By TeamGridCellValue(int columnIndex, string columnHeader) => By.XPath($"//div[@id='Teams']//tbody/tr[{columnIndex}]/td[count(//div[@id='Teams']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]");
        private static By TeamName(string teamName) => By.XPath($"//table[@role='grid']//tbody//tr//td//a[text()='{teamName}']");
        private static By GridColumnHeader(string columnName) => By.CssSelector($"th[data-title='{columnName}'] a:last-child");
        private readonly By SelectFilterSymbol = By.CssSelector("th[data-title='Team Name'] span[class='k-icon k-i-arrowhead-s']");
        private readonly By ColumnFilterOption =
            By.XPath("//ul[@role='menubar']/li[contains(@class,'k-columns-item k-state-default')]/span");
        private readonly By VisibleColumns = By.CssSelector("ul[style*='display: block'] span.k-link");
        private static By ColumnValue(string columnHeader) => By.XPath($"//div[@id='Teams']//tbody/tr/td[count(//div[@id='Teams']//thead//th[@data-title='{columnHeader}']/preceding-sibling::th)+1]");
        private readonly By TeamAvatarSelectors = By.CssSelector("#team_list table img");
        private readonly By GridColumnHeaders = By.CssSelector("#Teams .k-header>a.k-link");
        private readonly By GridViewRows = By.CssSelector("#Teams tbody tr");
        private readonly By NoTeamMessage = By.Id("noteam-regularMessage");
        private readonly By TeamActiveMenu = By.CssSelector("li[id$='mn_active']");
        private readonly By TeamArchiveMenuItem = By.CssSelector("li[id^='menu_Archive']");
        private readonly By ConfirmArchiveButton = By.Id("delete_confirm");
        private readonly By ConfirmRestoreButton = By.Id("restore_confirm");
        private readonly By RestoreButton = By.CssSelector(".gridActiveButton [id^='menu_Active']");
        private readonly By ArchiveReasonList = By.CssSelector("span[aria-owns='archiveOption_listbox']");
        private static By ArchiveReasonListItem(string reason) => By.XPath($"//ul[@id='archiveOption_listbox']/li[text()='{reason}']");
        private readonly By TeamNameLink = By.XPath("//td[contains(@id,'teamName-')]//a");
        private static By TeamAvatarLink(string teamName) => By.XPath($"//td[contains(@id,'teamName-')]//a[text()='{teamName}']/../preceding-sibling::td//a");

        #endregion

        #region Create a new team popup
        private readonly By SelectTeamTypePopupTeamQuickLaunchRadioButton = By.Id("quick_team_btn");
        private readonly By SelectTeamTypePopupTeamFullWizardRadioButton = By.Id("team_btn");
        private readonly By SelectTeamTypePopupMultiTeamRadioButton = By.Id("multiteam_btn");
        private readonly By SelectTeamTypePopupPortfolioTeamRadioButton = By.Id("enterprise_btn");
        private readonly By SelectTeamTypePopupEnterpriseTeamRadioButton = By.Id("ntier_btn");
        private readonly By SelectTeamTypePopupNextButton = By.Id("NextCreateTeam");
        private readonly By SelectTeamTypePopupXIcon = By.XPath("//span[text()='Close']//parent::a");
        #endregion




        #endregion


        #region Methods

        #region Header

        public void ClickOnGainInsightsTab()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on Gain Insights tab");
            Wait.UntilElementVisible(GainInsightsTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnMeasureTab()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on Measure tab");
            Wait.UntilElementClickable(MeasureTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnGrowthItemTab()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on GrowthItem tab");
            Wait.UntilElementClickable(GrowthItemTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnAchieveOutcomeTab()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on Achieve Outcome tab");
            Wait.UntilElementClickable(AchieveOutcomeTab).Click();
            Wait.HardWait(2000); //Wait until learn tab loads successfully
        }

        public void ClickOnLearnTab()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on Learn tab");
            Wait.UntilElementClickable(LearnTab).Click();
            Wait.HardWait(20000); //Wait until learn tab loads successfully
        }

        public string GetActiveMainTab()
        {
            return Wait.UntilElementVisible(ActiveTab).GetText();
        }

        public int GetTeamIdFromLink(string teamName)
        {
            return int.Parse(Regex.Match(Wait.UntilElementExists(TeamName(teamName)).GetElementAttribute("href"), @"/editv2/(\d+)\?").Groups[1].Value);
        }
        #endregion

        #region Team Dashboard Header
        public void ClickOnCreateNewTeamButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Create a Add New Team' button");
            Wait.UntilElementVisible(CreateNewTeamButton);
            Wait.UntilElementClickable(CreateNewTeamButton).Click();
        }

        public void FilterArchivedTeam()
        {
            Log.Step(nameof(TeamDashboardPage), "Filter Archived team");
            Wait.UntilElementVisible(ActiveArchiveButton).Click();
            Wait.UntilElementClickable(ArchivedMenuItem).Click();
        }

        public void FilterActiveTeam()
        {
            Log.Step(nameof(TeamDashboardPage), "Filter Active team");
            Wait.UntilElementVisible(ActiveArchiveButton).Click();
            Wait.UntilElementClickable(ActiveMenuItem).Click();
        }

        public void SelectBulkMgmt(string item)
        {
            Log.Step(nameof(TeamDashboardPage), $"Select Bulk Mgmt item {item}");
            Wait.HardWait(5000);  // Wait until team dashboard load
            SwitchToIframeForNewNav();
            SelectItem(BulkMgmtButton, BulkMgmtItem(item));
        }
        #endregion

        #region Filters
        public void SearchTeam(string search)
        {
            Log.Step(nameof(TeamDashboardPage), $"On Team Dashboard , searching for {search}");
            Thread.Sleep(5000);//Wait for team dashboard load 
            SwitchToIframeForNewNav();
            Wait.UntilElementVisible(SearchTextbox).SetText(search);
            Wait.UntilJavaScriptReady();
        }
        public void SwitchToGridView()
        {
            Log.Step(nameof(TeamDashboardPage), "Switch team dashboard to grid view");
            Thread.Sleep(2000);//Wait for team dashboard load 
            SwitchToIframeForNewNav();
            if (!Wait.UntilElementVisible(GridViewIcon).GetAttribute("class").Contains("active"))
            {
                Wait.UntilElementClickable(GridViewIcon).Click();
            }
            Wait.UntilJavaScriptReady();
        }

        public void FilterTeamType(string teamType)
        {
            Log.Step(nameof(TeamDashboardPage), $"Filter by team type {teamType}");
            SelectItem(TeamLevelTextbox, TeamLevelItem(teamType));
        }

        public void ExportToExcel()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on Export to Excel button");
            Wait.UntilElementVisible(ExportButton).Click();
            Wait.UntilElementVisible(ExcelButton).Click();
        }
        #endregion

        #region Table Grid
        public string GetTeamGridCellValue(int columnIndex, string columnHeader)
        {
            Wait.UntilJavaScriptReady();

            // Check if the column header is visible, if not, add it
            if (!GetVisibleColumnNamesFromGrid().Contains(columnHeader))
            {
                AddColumns(new List<string> { columnHeader });
                Wait.UntilJavaScriptReady();
            }

            // Get the cell value
            var baseSelector = TeamGridCellValue(columnIndex, columnHeader);

            var teamNameSelector = columnHeader == "Team Name"
                ? By.XPath($"{baseSelector.ToString().Replace("By.XPath: ", "")}//a")
                : baseSelector;

            var selector = Wait.UntilElementExists(teamNameSelector);

            var cellValue = columnHeader == "Team Tags" ? selector.GetAttribute("textContent") : selector.Text;

            // Handle special cases
            if (string.IsNullOrEmpty(cellValue) && new List<string> { "No of Assessments", "No of Team Assessments", "No of Sub-Teams", "No of Team Members" }.Contains(columnHeader))
            {
                cellValue = "0";
            }
            else if (columnHeader.Equals("Last Date of Assessment") && !string.IsNullOrEmpty(cellValue))
            {
                cellValue = DateTime.Parse(cellValue).ToString("MM/dd/yyyy");
            }

            return cellValue;
        }

        public bool IsTeamDisplayed(string teamName)
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(TeamName(teamName));
        }

        public List<string> GetTeamTagsValue()
        {
            // Condition for 'Single' tag and multiple tag which contains under '+More'
            var elementTexts = new List<string>();
            if (Driver.IsElementPresent(FirstOnlyTag))
            {

                var firstOnlyTag = Wait.UntilElementVisible(FirstOnlyTag).GetText();
                elementTexts.Add(firstOnlyTag);
                return elementTexts;
            }
            else
            {
                var firstTeamTagsValue = Wait.UntilElementVisible(FirstTeamTag).GetText();
                Driver.MoveToElement(MoreTags);
                elementTexts = Driver.GetTextFromAllElements(ExtraTeamTags).ToList();
                elementTexts.Add(firstTeamTagsValue);
                return elementTexts;
            }
        }
        public void ClickOnGridColumn(string columnName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Sort grid column <{columnName}>");
            Wait.UntilElementClickable(GridColumnHeader(columnName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void AddColumns(List<string> columns)
        {
            Log.Step(nameof(TeamDashboardPage), $"On team dashboard, add columns <{string.Join(",", columns)}>");
            SelectItem(SelectFilterSymbol, ColumnFilterOption);

            foreach (var ele in Wait.UntilAllElementsLocated(VisibleColumns))
            {
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

        public IList<string> FormatColumnDates(IEnumerable<string> actualColumnText) =>
            actualColumnText.Select(t => string.IsNullOrWhiteSpace(t)
                ? t : DateTime.Parse(t).ToString("yyyy/MM/dd")).ToList();

        private static IList<string> FormatColumnNumbers(IEnumerable<string> actualColumnText) =>
            actualColumnText.Select(t => string.IsNullOrWhiteSpace(t)
                ? t : int.Parse(t).ToString("D2")).ToList();


        private static IList<string> FormatColumnTags(IEnumerable<string> actualColumnText) =>
            actualColumnText.Select(t => string.IsNullOrWhiteSpace(t)
                ? t : t.Split(',')[0].Trim()).ToList();

        public bool CheckTeamFilterCorrectly(string teamType)
        {
            Wait.UntilJavaScriptReady();
            var teamAvatars = Wait.UntilAllElementsLocated(TeamAvatarSelectors);
            var borderColorMap = new Dictionary<string, string>
            {
                {"Team", "rgb(51, 169, 236)"},
                {"Multi-Team", "rgb(254, 154, 46)"},
                {"Portfolio Team", "rgb(56, 179, 72)"},
                {"Enterprise Team", "rgb(0, 0, 255)"}
            };

            foreach (var element in teamAvatars)
            {
                var css = element.GetCssValue("border-color");
                var expectedBorderColor = borderColorMap[teamType];

                if (!css.Contains(expectedBorderColor))
                {
                    return false;
                }
            }

            return true;
        }

        public List<string> GetVisibleColumnNamesFromGrid()
        {
            Log.Step(nameof(TeamDashboardPage), "Column Header");
            return Wait.UntilAllElementsLocated(GridColumnHeaders).Where(e => e.Displayed).Select(e => e.GetAttribute("textContent")).ToList();
        }

        public int GetNumberOfGridRows() => Wait.UntilAllElementsLocated(GridViewRows).Count;

        public void ArchiveTeam(string reason)
        {
            Log.Step(nameof(TeamDashboardPage), $"Archive a team with reason ${reason}");
            Wait.UntilElementVisible(TeamActiveMenu).Click();
            Wait.UntilElementVisible(TeamArchiveMenuItem).Click();
            SelectItem(ArchiveReasonList, ArchiveReasonListItem(reason));
            Wait.UntilElementVisible(ConfirmArchiveButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void RestoreTeam()
        {
            Log.Step(nameof(TeamDashboardPage), "Restore a team");
            Wait.UntilElementVisible(RestoreButton).Click();
            Wait.UntilElementVisible(ConfirmRestoreButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsNoTeamMessageDisplayed()
        {
            return Driver.IsElementDisplayed(NoTeamMessage);
        }

        public void ClickOnEditTeamButton(string teamName)
        {
            Log.Step(nameof(TeamDashboardPage), $"Click on edit team button with team name {teamName}");
            Wait.UntilElementVisible(TeamEditButton(teamName)).Click();
        }
        #endregion

        #region Create a new team popup
        public void ClickOnTeamQuickLaunchRadioButtonOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Select 'Team Quick Launch' option on 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupTeamQuickLaunchRadioButton);
            Wait.UntilElementClickable(SelectTeamTypePopupTeamQuickLaunchRadioButton).Click();
        }
        public void ClickOnTeamFullWizardRadioButtonOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Select 'Team Full Wizard' option on 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupTeamFullWizardRadioButton);
            Wait.UntilElementClickable(SelectTeamTypePopupTeamFullWizardRadioButton).Click();
        }
        public void ClickOnMultiTeamRadioButtonOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Select 'Multi Team' option on 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupMultiTeamRadioButton);
            Wait.UntilElementClickable(SelectTeamTypePopupMultiTeamRadioButton).Click();
        }
        public void ClickOnPortfolioTeamRadioButtonOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Select 'portfolio' option on 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupPortfolioTeamRadioButton);
            Wait.UntilElementClickable(SelectTeamTypePopupPortfolioTeamRadioButton).Click();
        }
        public void ClickOnEnterpriseTeamRadioButtonOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Select 'Enterprise' option on 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupEnterpriseTeamRadioButton);
            Wait.UntilElementClickable(SelectTeamTypePopupEnterpriseTeamRadioButton).Click();
        }
        public void ClickOnNextButtonOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'NEXT' button of 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupNextButton);
            Wait.UntilElementClickable(SelectTeamTypePopupNextButton).Click();
        }
        public void ClickOnXIconOfSelectTeamTypePopup()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'X' icon of 'Select a Team Type' popup");
            Wait.UntilElementVisible(SelectTeamTypePopupXIcon);
            Wait.UntilElementClickable(SelectTeamTypePopupXIcon).Click();
        }


        public void SelectTeamType(TeamType teamType)
        {
            Log.Step(nameof(TeamDashboardPage), $"On Select Team popup , select {teamType:G} option");
            switch (teamType)
            {
                case TeamType.QuickLaunchTeam:
                    ClickOnTeamQuickLaunchRadioButtonOfSelectTeamTypePopup();
                    break;
                case TeamType.FullWizardTeam:
                    ClickOnTeamFullWizardRadioButtonOfSelectTeamTypePopup();
                    break;
                case TeamType.MultiTeam:
                    ClickOnMultiTeamRadioButtonOfSelectTeamTypePopup();
                    break;
                case TeamType.PortfolioTeam:
                    ClickOnPortfolioTeamRadioButtonOfSelectTeamTypePopup();
                    break;
                case TeamType.EnterpriseTeam:
                    ClickOnEnterpriseTeamRadioButtonOfSelectTeamTypePopup();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamType), teamType, null);
            }
        }

        public void ClickOnAddNewTeamButtonAndSelectTeamType(TeamType teamType)
        {
            Log.Step(nameof(TeamDashboardPage), $"Click on 'Add a New Team' button and select '{teamType}' option the click on 'Next' button");
            ClickOnCreateNewTeamButton();
            SelectTeamType(teamType);
            ClickOnNextButtonOfSelectTeamTypePopup();
        }
        #endregion


        public string NavigateToPage(int companyId)
        {
            var newNavUrl = GetTeamDashboardUrl(companyId);
            NavigateToUrl(newNavUrl);
            Wait.UntilJavaScriptReady();
            return newNavUrl;
        }

        public string GetTeamDashboardUrl(int companyId)
        {
            var newNavUrl = $"{BaseTest.ApplicationUrl}/company/{companyId}/teams/{companyId}?parentIds=[{companyId}]";
            return newNavUrl;
        }

        public void NavigateToNewNavTeamDashboard(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.sa/V2/manage-teams/company/{companyId}/parents/0/team/0/tab/teams-dashboard");
        }

        public void ClickOnTeamName(string teamName)
        {
            var teamNameElements = Driver.GetTextFromAllElements(TeamNameLink).ToList();

            foreach (var name in teamNameElements)
            {
                if (name.Equals(teamName))
                {
                    Wait.UntilElementClickable(TeamName(teamName)).Click();
                }
            }
        }


        public void ClickOnTeamAvatarIcon(string teamName)
        {
            var teamNameElements = Driver.GetTextFromAllElements(TeamNameLink).ToList();
            foreach (var name in teamNameElements)
            {
                if (name.Equals(teamName))
                {
                    Wait.UntilElementClickable(TeamAvatarLink(teamName)).Click();
                }
            }
        }

        #endregion
    }
}
