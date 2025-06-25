using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView
{
    public class BusinessOutcomesViewPage : BusinessOutcomeBasePage
    {
        public BusinessOutcomesViewPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        private readonly By ViewDropdown = By.XPath("//ul[@role='listbox']/li/a");
        private readonly By CardTitleFilterOptions = By.CssSelector(".k-columnmenu-item");
        private readonly By ColumnOptionList = By.XPath("//table[contains(@class,'k-table k-table-md')]//th[contains(@class,'k-header')]//span[@class='k-column-title']");
        private readonly By CardTypeDropdownOptions = By.XPath("//ul[@role='listbox']/li");
        private readonly By TimePeriodFilterOptions = By.XPath("//div[@class='k-button-group k-gantt-views']");
        private readonly By GridViewTableDetails = By.XPath("//tr[@class='k-table-row k-master-row']/td");
        private readonly By TableIdColumnText = By.XPath("//td[@field='prettyId']");
        private readonly By TableCardTitleText = By.XPath("//td[@field='title']");
        private readonly By GridViewTitle = By.XPath("//*[normalize-space(text())='Total Cards']");
        private readonly By GridViewFooter = By.XPath("//span[contains(text(),'items per page')]");
        private readonly By TimelineViewTimePeriodOptions = By.CssSelector(".k-button-group.k-gantt-views");
        private readonly By TimelineViewGraph = By.CssSelector(".k-task.k-task-single");
        private readonly By TimeLineViewHeaders = By.XPath("//div[@role='group']//span");
        private readonly By CardTitleColumnKebabButton = By.XPath("//th[.//span[text()='Card Title']]//span[contains(@class, 'k-grid-header-menu')]");
        private readonly By FilterOption = By.XPath("//div[contains(text(),'Filter')]");
        private readonly By GridViewCardTitleColumnsButton = By.XPath("//div[contains(text(),'Columns')]");
        private readonly By FilterOptionEnterText = By.XPath("//div[@class='k-filter-menu-container']/input[1]");
        private readonly By FilterButton = By.XPath("//button/span[contains(text(),'Filter')]");
        private static By GridViewDropdownValue(string dropdownValue) => By.XPath($"//li[@data-value='gridview']/parent::ul//*[contains(text(), '{dropdownValue}')]");
        private static By CardTypeDropdownItems(string cardTypeValue) => By.XPath($"//ul[@role='listbox']/li[contains(text(),'{cardTypeValue}')]");
        private readonly By GridViewLoader = By.XPath("//p[text()='Loading Grid View...']");
        private static By TimePeriodFilterItems(string timeFilterValue) => By.XPath($"//div[@class='k-button-group k-gantt-views']/button/span[contains(text(),'{timeFilterValue}')]");

        #endregion

        #region Methods

        public List<string> GetGridViewDropdownOptions() => Wait.UntilAllElementsLocated(ViewDropdown).Select(e => e.GetText()).ToList();

        public void ClickOnGridViewDropdownOptions(string gridViewValue)
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on GridView Dropdown");
            Wait.UntilElementClickable(GridViewDropdownValue(gridViewValue)).Click();
            Wait.UntilElementNotExist(GridViewLoader);
            Wait.HardWait(2000); //Wait till the grid view is loaded
        }

        public string GetGridViewTitleText()
        {
            return Wait.UntilElementVisible(GridViewTitle).Text.Replace(":", "").Trim();
        }

        public string GetGridViewFooterText()
        {
            return Wait.UntilElementVisible(GridViewFooter).Text;
        }
        public bool IsTimelineViewTimeOptionsDisplayed()
        {
            return Driver.IsElementDisplayed(TimelineViewTimePeriodOptions);
        }

        public bool IsTimelineViewGraphDetailsDisplayed()
        {
            return Driver.IsElementDisplayed(TimelineViewGraph);
        }


        public List<string> GetTimeLineViewHeadersText() => Wait.UntilAllElementsLocated(TimeLineViewHeaders).Select(e => e.GetText()).ToList();

        public List<string> GetCardTypeDropdownOptions()
        {
            Wait.UntilElementNotExist(GridViewLoader);
            return Wait.UntilAllElementsLocated(CardTypeDropdownOptions).Select(e => e.GetText()).ToList();
        }

        public List<string> GetTimePeriodFilterOptions() =>
      Wait.UntilAllElementsLocated(TimePeriodFilterOptions)
          .SelectMany(e => e.GetText().Trim().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
          .ToList();



        public void ClickOnCardTypeDropdownOptions(string cardTypeValue)
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on Card Type Dropdown");
            Wait.UntilElementClickable(CardTypeDropdownItems(cardTypeValue)).Click();
            Wait.UntilElementNotExist(GridViewLoader);
        }

        public void ClickOnTimePeriodFilter(string timeFilterValue)
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on Card Type Dropdown");
            Wait.UntilElementClickable(TimePeriodFilterItems(timeFilterValue)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnMoreMenuOfCardTitleColumn()
        {
            Wait.UntilJavaScriptReady();
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on Kebab button of Title Column");
            Wait.UntilElementClickable(CardTitleColumnKebabButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickColumnOptionFromMoreMenu()
        {
            Wait.UntilJavaScriptReady();
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on Kebab button of Id Column");
            Wait.UntilElementClickable(GridViewCardTitleColumnsButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public List<string> GetFilterOptionOfCardTitleColumn() => Wait.UntilAllElementsLocated(CardTitleFilterOptions).Select(e => e.GetText()).ToList();
        public List<string> GetColumnsValueFromTable()
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Get Column Values");
            return Driver.GetTextFromAllElements(ColumnOptionList).ToList();
        }
        public void ClickFilterOptions()
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on Filter Option");
            Wait.UntilElementClickable(FilterOption).Click();
        }

        public void EnterTextInFilterInput(string filterText)
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Enter the text for the Filter");
            Wait.UntilElementClickable(FilterOptionEnterText).Clear();
            Wait.UntilElementClickable(FilterOptionEnterText).SendKeys(filterText);
        }

        public void ClickOnFilterButton()
        {
            Log.Step(nameof(BusinessOutcomesViewPage), "Click on Filter Button");
            Driver.JavaScriptScrollToElement(FilterButton);
            Wait.UntilElementClickable(FilterButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetBusinessOutcomesGridViewDetails() => Wait.UntilAllElementsLocated(GridViewTableDetails).Select(e => e.GetText()).ToList();

        public BusinessOutcomesTimelineViewResponse GetBusinessOutcomeFromGrid()
        {
            var businessOutcomes = new BusinessOutcomesTimelineViewResponse
            {
                Id = Wait.UntilElementExists(TableIdColumnText).GetText(),
                Title = Wait.UntilElementExists(TableCardTitleText).GetText(),
            };

            return businessOutcomes;
        }


        public BusinessOutcomesTimelineViewResponse GetBusinessOutcomesTimelineViewDetails()
        {

            var businessOutcomes = new BusinessOutcomesTimelineViewResponse
            {
                Id = Wait.UntilElementExists(TableIdColumnText).GetText(),
                Title = Wait.UntilElementExists(TableCardTitleText).GetText(),
            };

            return businessOutcomes;
        }



        #endregion
    }
}