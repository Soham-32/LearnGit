using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems
{
    public class GiDashboardKanbanWidgetPage : GrowthItemsDashboardPage
    {
        public GiDashboardKanbanWidgetPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By CategoryDropdown = By.CssSelector("span[aria-owns='categorieskanban_listbox']");
        private readonly By SurveyTypeDropdown = By.CssSelector("span[aria-owns='surveytypekanban_listbox']");
        private readonly By ExportToExcelButton = By.ClassName("excelIconShow");
        private readonly By AllGrowthItemTitles = By.CssSelector("li.box-item p");
        
        private static By GiKanbanItemEditButton(string giTitle, string status) => By.XPath(
            $"//div[@status='{status}']//p[text()='{giTitle}']/preceding-sibling::div[@class='sec2']/g[@class='fas fa-edit'] |//div[@status='{status}']//p//font[text()='{giTitle}']/ancestor::p/preceding-sibling::div[@class='sec2']/g[@class='fas fa-edit']");

        private static By GiKanbanItem(string giTitle, string status) => By.XPath(
            $"//div[@status='{status}']//p[text()='{giTitle}']");

        private static By GiKanbanCategoryItem(string item) => By.XPath(
            $"//ul[@id='categorieskanban_listbox']/li[text()='{item}'] | //ul[@id='categorieskanban_listbox']/li//font[text()='{item}']");

        private static By GiKanbanSurveyTypeItem(string item) => By.XPath(
            $"//ul[@id='surveytypekanban_listbox']/li[text()='{item}'] | //ul[@id='surveytypekanban_listbox']/li//font[text()='{item}']");

        //Key Customer Verification
        #region GI tab KanbanView Elements

        private readonly By EditGrowthPlanItemTitle = By.Id("GIKanBanGrowthPlanEditor_wnd_title");
        private readonly By ColumnTitle = By.XPath("//div[@id='mainDiv']//span");
        private readonly By RadarTypeFilterText = By.XPath("//span[@aria-owns='surveytypekanban_listbox']//span//span");
        private readonly By CategoryFilterText = By.XPath("//span[@aria-owns='categorieskanban_listbox']//span//span");

        #endregion


        public void ClickEditKanbanGrowthItem(string title, string status)
        {
            Log.Step(nameof(GiDashboardKanbanWidgetPage), $"Click edit kanban growth item with title <{title}> and status <{status}>");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(GiKanbanItemEditButton(title, status));
            Wait.UntilElementClickable(GiKanbanItemEditButton(title, status)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesKanbanGiExist(string title, string status)
        {
            return Driver.IsElementPresent(GiKanbanItem(title, status));
        }

        public void FilterByCategory(string category)
        {
            Log.Step(nameof(GiDashboardKanbanWidgetPage), $"Filter by category <{category}>");
            SelectItem(CategoryDropdown, GiKanbanCategoryItem(category));
        }

        public void FilterBySurveyType(string surveyType)
        {
            Log.Step(nameof(GiDashboardKanbanWidgetPage), $"Filter by survey type <{surveyType}>");
            SelectItem(SurveyTypeDropdown, GiKanbanSurveyTypeItem(surveyType));
        }

        public void ClickExportToExcel()
        {
            Log.Step(nameof(GiDashboardKanbanWidgetPage), "Click Export to Excel button");
            Wait.UntilElementClickable(ExportToExcelButton).Click();

            if (Driver.IsInternetExplorer())
            {
                AutoIt.InternetExplorerDownloadClickOnSave(Driver.Title);
            }
        }

        public List<string> GetAllGrowthItemsTitles()
        {
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(KanbanBoardDiv);
            return Wait.UntilAllElementsLocated(AllGrowthItemTitles).Select(item => Driver.JavaScriptScrollToElement(item).GetText()).ToList();
        }

        //Key Customer Verification

        #region GI tab KanbanView Elements

        public bool IsEditGiPopupTitleDisplayed()
        {
            return Driver.IsElementDisplayed(EditGrowthPlanItemTitle);
        }
        public bool IsColumnTitleListDisplayed()
        {
            return Wait.UntilAllElementsLocated(ColumnTitle).Any(e=>e.Displayed);
        }
        public string GetRadarTypeFilterText()
        {
            Log.Step(GetType().Name, "Get selected radar type filter text");
            return Wait.UntilElementExists(RadarTypeFilterText).GetText();
        }
        public string GetCategoryFilterText()
        {
            Log.Step(GetType().Name, "Get selected category filter text");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementExists(CategoryFilterText).GetText();
        }
        #endregion
    }
}