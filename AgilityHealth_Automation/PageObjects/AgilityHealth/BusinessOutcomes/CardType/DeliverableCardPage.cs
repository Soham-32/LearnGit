using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType
{
    public class DeliverableCardPage : BusinessOutcomeBasePage
    {
        public KeyResultsTabPage KeyResultsTab { get; set; }
        public DependenciesTabPage DependenciesTab { get; set; }
        public ObstaclesTabPage ObstaclesTab { get; set; }
        public CheckListTabPage CheckListTab { get; set; }
        public CommentsTabPage CommentsTab { get; set; }
        public AdditionalDetailsPage AdditionalDetails { get; set; }
        public FinancialsTabPage FinancialTab { get; set; }

        public DeliverableCardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            KeyResultsTab = new KeyResultsTabPage(driver, log);
            DependenciesTab = new DependenciesTabPage(driver, log);
            ObstaclesTab = new ObstaclesTabPage(driver, log);
            CheckListTab = new CheckListTabPage(driver, log);
            CommentsTab = new CommentsTabPage(driver, log);
            AdditionalDetails = new AdditionalDetailsPage(driver, log);
        }

        #region SelectDeliverableParentOutcomeDropdown
        private readonly By SelectDeliverableParentOutcomeDropdown = By.XPath("//input[@id='selectedCard']");
        #endregion

        #region SelectParentOutcomeDropdown
        private readonly By SelectParentOutcomeDropdown = By.XPath("//span[(text()='Select Parent' or font//font[text()='Select Parent'])]//following-sibling::div//input");
        public static By SelectParentOutcomeDropdownItem(string parentOutcome) => By.XPath($"//ul[@id='selectedCard-listbox']//li[contains(text(),'{parentOutcome}') or //font[contains(text(),'{parentOutcome}')]");
        public readonly By ParentOutcomeLink = By.XPath("//form[@id='businessOutcomeForm']//div/p[text()='Parent']/../following-sibling::div/p[2] | //form[@id='businessOutcomeForm']//div//font[text()='Parent']/../../../following-sibling::div//p[2]//font");
        #endregion



        public void SelectDeliverableParentOutcome(string parentOutcome)
        {
            Log.Step(nameof(DeliverableCardPage), "Click on 'Select Parent Outcome' Dropdown and select parent card");
            Wait.UntilElementExists(SelectDeliverableParentOutcomeDropdown).SetText(parentOutcome);
            Wait.HardWait(2000);// need to wait till load the data
            SelectItem(SelectParentOutcomeDropdown, SelectParentOutcomeDropdownItem(parentOutcome));
        }

    }
}
