using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar
{
    internal class GrowthItemKanbanViewWidget : AssessmentDetailsCommonPage
    {
        public GrowthItemKanbanViewWidget(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private TimeSpan Timeout => Driver.Manage().Timeouts().AsynchronousJavaScript;

        private readonly By KanbanNewGrowthItemButton = By.CssSelector(".fas.fa-plus-square");
        private readonly By MainDiv = By.Id("mainDiv");
        private readonly By KanbanViewHeader = By.Id("kanbanSummary");
        private readonly By KanbanCardSetting = By.ClassName("gearIcon");

        //Customize Growth Plan Cards
        private readonly By SelectAllPanelsButton = By.XPath("//div[@id='divStatusPanels']//th[@onclick='clickAndCheckAllPanels()']");
        private readonly By OkayButton = By.Id("do_Filter");

        private static By GiSelectors(string status, string giTitle) =>
            By.XPath($"//div[@status='{status}']//p[text()='{giTitle}']");
        private static By GiSelector(int index, string status, string giTitle) =>
            By.XPath($"(//div[@status='{status}']//p[text()='{giTitle}'])[{index}]");
        private static By GiDeleteButton(int index, string status, string giTitle) =>
            By.XPath($"(//div[@status='{status}']//p[text()='{giTitle}']/preceding-sibling::div[@class='sec2']/g[@class='fas fa-trash-alt'])[{index}]");
        private static By GiCopyButton(int index, string status, string giTitle) =>
            By.XPath($"(//div[@status='{status}']//p[text()='{giTitle}']/preceding-sibling::div[@class='sec2']/g[@class='fas fa-copy'])[{index}]");
        private static By GiContainer(string status) => By.XPath($"//div[@status='{status}']");
        private static By GiKanbanItemEditButton(string giTitle, string status) => By.XPath($"//div[@status='{status}']//p[text()='{giTitle}']/preceding-sibling::div[@class='sec2']/g[@class='fas fa-edit']");
        private readonly By GiKanbanEditIcon = By.XPath("//g[@title='Edit Growth Plan item']");
        private readonly By AllDeleteGiButtons = By.XPath("//div[@id ='growthPlanKannanView']//g[@title = 'Delete Growth Plan item']");
        private readonly By GiKanbanHistoryIcon = By.XPath("//g[@title='History']");
        private readonly By GiKanbanHistoryPopupHeaderText = By.XPath("//span[@id='historyDialog_wnd_title' and (text()='History Growth Plan Item' or .//*[text()='History Growth Plan Item'])]");
        private readonly By HistoryPopupCloseIcon = By.XPath("//span[@id='historyDialog_wnd_title']//following-sibling::div//a//span[@class='k-icon k-i-close']");
        private readonly By CustomizeCardPopupHeaderText = By.XPath("//span[@id='showFilterKanbanView_wnd_title' and (text()='Customize Growth Plan Cards' or .//*[text()='Customize Growth Plan Cards'])]");
        public void ClickKanbanAddNewGrowthItem()
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), "In Kanban view, click on Add New Growth Item button");
            SwitchToKanbanView();
            Wait.UntilElementClickable(KanbanNewGrowthItemButton).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public bool DoesKanbanGiExist(string status, int index, string giTitle)
        {
            return Driver.IsElementDisplayed(GiSelector(index, status, giTitle), 15);
        }

        public void ClickOnKanbanGiEditIcon()
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), "In Kanban view, click on edit growth item icon.");
            Wait.UntilElementClickable(GiKanbanEditIcon).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public bool IsKanbanGiEditIconDisplayed()
        {
            return Driver.IsElementDisplayed(GiKanbanEditIcon);
        }
        public void ClickOnKanbanGiHistoryIcon()
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), "In Kanban view, click on history icon.");
            Wait.UntilElementClickable(GiKanbanHistoryIcon).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public bool IsKanbanGiHistoryPopupDisplayed()
        {
            return Driver.IsElementDisplayed(GiKanbanHistoryPopupHeaderText);
        }
        public void ClickOnHistoryPopupCloseIcon()
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), "In Kanban view, click on close icon for history popup.");
            Wait.UntilElementClickable(HistoryPopupCloseIcon).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public void ClickOnCustomizeGrowthPlanCardsIcon()
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), "In Kanban view, click on 'Customize Growth Plan cards' icon.");
            Wait.UntilElementClickable(KanbanCardSetting).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }
        public bool IsCustomizeCardPopupDisplayed()
        {
            return Driver.IsElementDisplayed(CustomizeCardPopupHeaderText);
        }

        public void ClickKanbanGiCopyButton(string status, int index, string giTitle)
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), $"In Kanban view, growth item <{giTitle}>, click on Copy button");
            Wait.UntilElementClickable(GiCopyButton(index, status, giTitle)).Click();
            Wait.UntilJavaScriptReady(Timeout);
        }

        public void DeleteKanbanGi(string status, int index, string giTitle)
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), $"In Kanban view, delete <{giTitle}>");
            Wait.UntilJavaScriptReady(Timeout);
            Wait.UntilElementClickable(GiDeleteButton(index, status, giTitle)).Click();
            try
            {
                Driver.AcceptAlert();
            }
            catch
            {
                AcceptDelete();
            }

            //Waiting till Delete button not visible
            Wait.UntilElementNotExist(GiDeleteButton(index, status, giTitle));
        }

        public void DeleteAllKanbanGi()
        {

            Log.Step(nameof(GrowthItemGridViewWidget), "Delete all growth items from kanban view");
            try
            {
                Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
                ShowAllStatusPanels();
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

        public void DragDropGi(int index, string sourceStatus, string sourceItem, string destinationStatus)
        {
            Log.Step(nameof(GrowthItemKanbanViewWidget), $"In Kanban view, drag growth item <{sourceItem}> from <{sourceStatus}> to <{destinationStatus}>");
            if (Driver.IsElementPresent(GrowthPlanSectionMarker))
            {
                Driver.JavaScriptScrollToElement(GrowthPlanSectionMarker);
            }
            var selector = GiSelector(index, sourceStatus, sourceItem);
            Wait.UntilElementVisible(selector);
            var destinationContainer = GiContainer(destinationStatus);
            Driver.DragElementToElement(Wait.UntilElementVisible(selector),
                Wait.UntilElementVisible(destinationContainer));
            Wait.UntilCssValueEquals(MainDiv, "Opacity", "1");
            Wait.UntilElementClickable(KanbanViewHeader);
        }

        public int GetGiCount(string status, string title)
        {
            return Wait.UntilAllElementsLocated(GiSelectors(status, title)).Count;
        }

        public void ShowAllStatusPanels()
        {
            Wait.UntilElementClickable(KanbanCardSetting).Click();
            Wait.HardWait(2000); // Wait until 'Select All Panels' button is clickable
            Wait.UntilElementClickable(SelectAllPanelsButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(OkayButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesAddNewGrowthItemButtonDisplayed() => Driver.IsElementDisplayed(KanbanNewGrowthItemButton);
        public bool DoesEditButtonDisplayed(string title, string status) => Driver.IsElementDisplayed(GiKanbanItemEditButton(title, status));
        public bool DoesCopyButtonDisplayed(int index, string title, string status) => Driver.IsElementDisplayed(GiCopyButton(index, title, status));
        public bool DoesDeleteButtonDisplayed(int index, string title, string status) => Driver.IsElementDisplayed(GiDeleteButton(index, title, status));

    }
}