using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Generic;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems
{
    public class GiDashboardGridWidgetPage : GrowthItemGridView
    {
        public GiDashboardGridWidgetPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By CategoryDropdown = By.CssSelector("span[aria-owns='categories_listbox']");
        private readonly By SurveyTypeDropdown = By.CssSelector("span[aria-owns='surveytype_listbox']");
        private readonly By TagsDropdown = By.CssSelector("input[aria-owns='msTags_taglist msTags_listbox']");
        private readonly By ExportButton = By.Id("exportDropDown");
        private readonly By ExportExcelButton = By.CssSelector("#exportDropDown-content button.excelBtn");

        private static By GiCategoryItem(string item) => By.XPath($"//ul[@id='categories_listbox']/li[text()='{item}' and not(@id)] | //ul[@id='categories_listbox']/li//font[text()='{item}' and not(@id)]");

        private static By GiSurveyTypeItem(string item) => By.XPath(
            $"//ul[@id='surveytype_listbox']/li[text()='{item}'] | //ul[@id='surveytype_listbox']/li//font[text()='{item}']");

        private static By GiTagsItem(string item) => By.XPath(
            $"//ul[@id='msTags_listbox']/li[text()='{item}'] | //ul[@id='msTags_listbox']/li//font[text()='{item}']");

        private readonly By TeamCategoryList =
            By.XPath("//body//div[contains(@class,'k-animation-container')][1]//ul/li");
        private readonly By TalentDevelopmentCategoryList =
            By.XPath("//body//div[contains(@class,'k-animation-container')][2]//ul/li");

        private readonly By CategoryDropdownBox = By.XPath("//label[text()='Filter by category']//following-sibling::span | //font[text()='Filter by category']/ancestor::label//following-sibling::span");
        private readonly By AssessmentTypeToggleButton = By.Id("toggle-header-TeamInd");

        private static By GiDashboardEditButtonFromGrid(string giTitle) =>
            By.XPath($"//div[@id='GrowthPlan']//table/tbody/tr/td[text()='{giTitle}']/following-sibling::td//span[text()='Edit'] | //div[@id='GrowthPlan']//table/tbody/tr/td[text()='{giTitle}']/following-sibling::td//span//font[text()='Edit']");

        // Column drag and drop filter
        private static By GiHeaderDragArea(string header) => By.XPath($"//th[@role='columnheader'][(@style='')or(@style='color: rgb(0, 0, 0);')or not (@style)][@data-title='{header}']");
        private readonly By GiHeaderDropOutArea = By.XPath("//div[@id='GrowthPlan']/div[@data-role='droptarget']");
        private static readonly By GiGroupRowHeader = By.XPath("//tbody//tr[@class='k-grouping-row']//P");
        private static readonly By GiAllRows = By.XPath("//table[@role='treegrid']//tbody//tr");

        public void ClickGrowthItemDashboardEditButton(string giTitle)
        {
            Log.Step(nameof(GrowthItemsPage), $"Click on edit growth item {giTitle}");
            Wait.UntilElementClickable(GiDashboardEditButtonFromGrid(giTitle)).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ChangeAssessmentTypeView(AssessmentWidget widget)
        {
            Log.Step(nameof(GrowthItemsDashboardPage), $"Change widget view to <{widget:G}>");
            if (widget != AssessmentWidget.Individual) return;
            Wait.UntilElementVisible(AssessmentTypeToggleButton);
            Wait.UntilElementClickable(AssessmentTypeToggleButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickCategoryDropdown()
        {
            Log.Step(nameof(GrowthItemsPage), "Click Category Dropdown");
            Wait.UntilElementClickable(CategoryDropdownBox).Click();
        }

        public IList<string> GetTeamViewCategoryList()
        {
            Log.Step(nameof(TeamCategoryList), "Get Team view 'Category' dropdown list");

            ClickCategoryDropdown();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamCategoryList).Select(a => a.GetText()).ToList();
        }

        public IList<string> GetTalentDevelopmentViewCategoryList()
        {
            Log.Step(nameof(TalentDevelopmentCategoryList), "Get Talent Development view 'Category' dropdown list");
            ClickCategoryDropdown();
            //Wait.UntilJavaScriptReady();  
            return Wait.UntilAllElementsLocated(TalentDevelopmentCategoryList).Select(a => a.GetText()).ToList();
        }

        public void FilterByCategory(string category)
        {
            Wait.UntilJavaScriptReady();
            Log.Step(nameof(GiDashboardGridWidgetPage), $"Filter by category <{category}>");
            SelectItem(CategoryDropdown, GiCategoryItem(category));
            Wait.UntilJavaScriptReady();
            Wait.HardWait(2000); // Waiting for data to be loaded properly
        }

        public void FilterBySurveyType(string surveyType)
        {
            Log.Step(nameof(GiDashboardGridWidgetPage), $"Filter by survey type <{surveyType}>");
            SelectItem(SurveyTypeDropdown, GiSurveyTypeItem(surveyType));
        }

        public void FilterByTags(string tag)
        {
            Log.Step(nameof(GiDashboardGridWidgetPage), $"Filter by tags <{tag}>");
            SelectItem(TagsDropdown, GiTagsItem(tag), tag);
        }

        public void ClickExportToExcel()
        {
            Log.Step(nameof(GiDashboardGridWidgetPage), "Click on Export to Excel button");
            SelectItem(ExportButton, ExportExcelButton);

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
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

        // Column drag and drop filter
        public void DragAndDropColumn(string dimensionFrom, int xOffset, int yOffset)
        {
            Log.Step(nameof(GiDashboardGridWidgetPage), $"Drag {dimensionFrom} column and drop at 'Drag a column header and drop it here to group by that column' section");
            Wait.UntilElementClickable(GiHeaderDragArea(dimensionFrom));
            var giHeaderDragged = Driver.JavaScriptScrollToElement(GiHeaderDragArea(dimensionFrom));
            var giHeaderDropOut = Driver.JavaScriptScrollToElement(GiHeaderDropOutArea);
            Driver.DragElementToElement(giHeaderDragged, giHeaderDropOut, xOffset, yOffset);
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetGroupRowHeaderValues(string groupByColumnName)
        {
            Log.Step(nameof(GiDashboardGridWidgetPage), $"Get list of row header values and remove {groupByColumnName}");
            return Wait.UntilAllElementsLocated(GiGroupRowHeader).Select(d => Driver.JavaScriptScrollToElement(d).GetText().Replace($"{groupByColumnName}: ", "")).ToList();
        }

        public List<string> GetAllRawValues()
        {
            Log.Step(nameof(GiDashboardGridWidgetPage), "Get list of all row values");
            return Wait.UntilAllElementsLocated(GiAllRows).Select(a => a.GetText()).ToList();
        }

    }
}