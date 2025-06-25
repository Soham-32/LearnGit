using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using AtCommon.Utilities;
using System.Linq;
using System.Threading;
using System;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.OutComeTreeView
{
    public class BusinessOutcomesOutcomeTreeViewPage : BusinessOutcomeBasePage
    {
        public BusinessOutcomesOutcomeTreeViewPage(IWebDriver driver, ILogger log = null) : base(driver, log)
        {
        }
        //Locators
        private readonly By AddAnOutcomeButton = By.Id("add-outcome-button");
        private By ColumnResize(string childrenColumnName) => By.XPath($"(//span[text()='{childrenColumnName}']/ancestor::span/following-sibling::span[@class='k-column-resizer'])[last()]");
        private readonly By TotalCards = By.XPath("//p/span[contains(text(), 'Total Cards')]/following-sibling::span");
        private readonly By OutcomeTypeDropdown = By.XPath("//button[@id='add-outcome-button']//parent::div//*[@aria-haspopup='listbox']");
        private readonly By SelectedYearView = By.XPath("//div[contains(@class, 'MuiInputBase-root') and contains(@class, 'notranslate')]");
        private readonly By TreeFolderIcon = By.XPath("//div[@aria-label='Tree View Visual is Coming Soon']/preceding-sibling::button");
        private readonly By ColumnMenuIcon = By.XPath("//a[@aria-label ='ID Column menu']");
        private readonly By SortToDescending = By.XPath("//div[@class='k-columnmenu-item-wrapper']//div[text()='Sort Descending']");
        private By GetYearOutcomeType(string year)
        {
            return By.XPath($"//li[text()='{year} Year Outcome{(year == "3" ? "s" : "")}']");
        }
        private static By PlusIcon(string cardTitle) => By.XPath($"//div[text()='{cardTitle}']/following-sibling::button[@id='add-child-button']");

        private static By AddTabValue(string tabName) => By.XPath($"//ul[@aria-labelledby='add-child-button']//li[contains(., 'Add') and contains(., '{tabName}')]");
        private static By CardTitle(string cardTitle) => By.XPath($"//*[text()='{cardTitle}']");

        private readonly By AddKeyResult = By.XPath("//ul[@aria-labelledby='add-child-button']//*[text()='Add Key Result']");
        private readonly By AddProjects = By.XPath("//ul[@aria-labelledby='add-child-button']//*[text()='Projects']");
        private readonly By AddInitiatives = By.XPath("//ul[@aria-labelledby='add-child-button']//*[text()='Initiatives']");
        private readonly By AddDeliverables = By.XPath("//ul[@aria-labelledby='add-child-button']//*[text()='Deliverables']");
        private readonly By AddStories = By.XPath("//ul[@aria-labelledby='add-child-button']//*[text()='Stories']");
        private readonly By AddChecklist = By.XPath("//ul[@aria-labelledby='add-child-button']//*[text()='Add Checklist']");

        private static By CardExpandCollapseIcon(string cardName) =>
             By.XPath($"//*[text()='{cardName}']/parent::div//*[@data-icon='chevron-circle-down']");
        //Pagination
        private readonly By FirstPage = By.XPath("//span[@class='k-pager-sizes']//parent::div//button[@title='Go to the first page']");
        private readonly By LastPage = By.XPath("(//div[@class='k-pager-numbers-wrap']//button[@title='Go to the last page'])[2]");
        private readonly By PreviousPage = By.XPath("//span[@class='k-pager-sizes']//parent::div//button[@title='Go to the previous page']");
        private readonly By NextPage = By.XPath("//span[@class='k-pager-sizes']//parent::div//button[@title='Go to the next page']");
        private readonly By PageDropdown = By.XPath("//span[@class='k-pager-sizes']//button[@aria-label='select']");

        private readonly By CurrentCardsPerPage = By.XPath("//span[@class='k-dropdownlist k-picker k-picker-md k-rounded-md k-picker-solid']/span/span");
        private By ChooseCardsPerPageOptions(string pages) => By.XPath($"//div[@class='k-list-content']/ul/li/span[contains(text(), '{pages}')]");
        private readonly By CurrentPage = By.XPath("//div/button[@aria-current='true']/span");

        private By ViewYear(string year)
        {
            return By.XPath($"//ul[@class='MuiList-root MuiList-padding MuiMenu-list css-r8u8y9']/li[.//text()='{year} Year Outcome{(year == "3" ? "s" : "")}']");
        }


        //Methods

        public void SelectYearOutcomeType(string year)
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), $"Selecting {year} Year Outcome type.");
            Wait.UntilElementClickable(AddAnOutcomeButton).Click();
            Wait.UntilElementVisible(GetYearOutcomeType(year)).Click();
        }

        public bool IsCardDisplayed(string cardTitle)
        {
            Wait.HardWait(3000);
            Driver.JavaScriptScrollToElement(CardTitle(cardTitle));
            return Driver.IsElementDisplayed(CardTitle(cardTitle));
        }

        public void ColumnResizer(string columnName)
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), $"Drag the Column Name {columnName}");
            var columnResizerElement = Driver.FindElement(ColumnResize(columnName)); // Fix: Store the result of FindElement in a variable
            Driver.Moveslider(columnResizerElement, 150, 0); // Pass the correct IWebElement to Moveslider
        }

        public bool IsCardDFixisplayed(string cardTitle)

        {
            return Driver.IsElementDisplayed(CardTitle(cardTitle));
        }

        public void ClickOnAddTabValue(string tabName, string cardTitle)

        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), $"Clicking on Add Key Result for card '{cardTitle}'.");
            var cardElement = Wait.UntilElementVisible(CardTitle(cardTitle));
            Driver.JavaScriptScrollToElementCenter(CardTitle(cardTitle));
            for (var retryAttempt = 0; retryAttempt < 2; retryAttempt++)
            {
                Driver.MoveToElement(cardElement);
                var plusIcon = PlusIcon(cardTitle);
                if (Driver.IsElementDisplayed(plusIcon))
                {
                    Wait.UntilElementClickable(PlusIcon(cardTitle)).Click();
                    break; // Success, exit loop
                }
                Wait.HardWait(2000); // Wait for the element to be displayed
            }
            Wait.UntilElementClickable(AddTabValue(tabName)).Click();
            Wait.HardWait(2000);
        }
        public void ClickOnCardExpandCollapseIcon(string cardName)
        {
            var attempt = 0;
            while (attempt < 3)
            {
                Wait.UntilJavaScriptReady();
                // Re-fetch icon on every attempt to avoid stale reference
                var iconElements = Driver.FindElements(CardExpandCollapseIcon(cardName));
                if (!iconElements.Any())
                {
                    Wait.HardWait(300);
                    attempt++;
                    continue;
                }
                var icon = iconElements.First();
                var style = icon.GetAttribute("style");
                // If already expanded, exit
                if (style != null && style.Contains("rotate(180deg)"))
                    return;
                if (icon.Displayed && icon.Enabled)
                {
                    icon.Click();
                    // Confirm it actually expanded
                    var updatedIcon = Driver.FindElement(CardExpandCollapseIcon(cardName));
                    var updatedStyle = updatedIcon.GetAttribute("style");
                    if (updatedStyle != null && updatedStyle.Contains("rotate(180deg)"))
                        return;
                }

                Wait.HardWait(3000);
                attempt++;
            }

        }

        public void SortByDescending()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Sorting by descending.");
            Wait.UntilElementClickable(ColumnMenuIcon).Click();
            Wait.UntilJavaScriptReady();
            var sortDescending = Wait.UntilElementClickable(SortToDescending);
            sortDescending.Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetSelectedYearView()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Getting selected view year.");
            return Wait.UntilElementVisible(SelectedYearView).Text;
        }
        public void SelectOutcomeType(string year)
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), $"Viewing {year} Year Outcome type.");
            Wait.UntilElementClickable(OutcomeTypeDropdown).Click();
            Wait.UntilElementVisible(ViewYear(year)).Click();
            Wait.UntilElementClickable(TreeFolderIcon).Click();
        }
        public void ClickOnAddAnOutcomeButton()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Clicking on Add An Outcome button.");
            Wait.UntilElementVisible(AddAnOutcomeButton).Click();
        }
        public bool DoesAddYearOutcomeButtonExist(string year)
        {
            return Wait.UntilElementVisible(GetYearOutcomeType($"{year}")).Displayed;
        }

        public void ClickOnTreeFolderIcon()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Clicking on Tree Folder Icon.");
            Wait.UntilElementClickable(TreeFolderIcon).Click();
        }

        public bool IsTreeFolderIconDisplayed()
        {
            return Wait.UntilElementVisible(TreeFolderIcon).Displayed;
        }

        public string GetTotalCards()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Getting Total Cards.");
            var totalCards = Wait.UntilElementVisible(TotalCards).Text;
            return totalCards;
        }

        //Pagination
        public bool IsGoToLastPageButtonDisplayed()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Going to last page.");
            for (var attempt = 1; attempt <= 2; attempt++)
            {
                if (Driver.IsElementDisplayed(LastPage))
                    return true;
            }
            return false;
        }

        public bool IsGoToPreviousPageButtonDisplayed()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Going to previous page.");
            for (var attempt = 1; attempt <= 2; attempt++)
            {
                if (Driver.IsElementDisplayed(PreviousPage))
                    return true;
            }
            return false;
        }

        public bool IsGoToNextPageButtonDisplayed()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Going to next page.");
            for (var attempt = 1; attempt <= 2; attempt++)
            {
                if (Driver.IsElementDisplayed(NextPage))
                    return true;
            }
            return false;
        }

        public int GetCurrentCardsPerPage()
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), "Getting current cards per page.");
            var cardsPerPage = Wait.UntilElementVisible(CurrentCardsPerPage).Text;
            return int.Parse(cardsPerPage);
        }

        public void ChooseCardsPerPage(string pageSize)
        {
            Log.Step(nameof(BusinessOutcomesOutcomeTreeViewPage), $"Choosing cards per page: {pageSize}.");
            Wait.HardWait(2000); // Wait for outcome tree view element loaded
            Wait.UntilElementClickable(PageDropdown).Click();
            var optionElement = Wait.UntilElementVisible(ChooseCardsPerPageOptions(pageSize));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", optionElement);
            optionElement.Click();
        }
    }
}