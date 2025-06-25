using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using OpenQA.Selenium;
using AgilityHealth_Automation.Utilities;
using System;
using System.Linq;


namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType
{
    public class InitiativeCardPage : BusinessOutcomeBasePage
    {
        public KeyResultsTabPage KeyResultsTab { get; set; }
        public DeliverablesTabPage DeliverableTab { get; set; }
        public DependenciesTabPage DependenciesTab { get; set; }
        public ObstaclesTabPage ObstaclesTab { get; set; }
        public CheckListTabPage CheckListTab { get; set; }
        public CommentsTabPage CommentsTab { get; set; }
        public AdditionalDetailsPage AdditionalDetails { get; set; }
        public FinancialsTabPage FinancialTab { get; set; }


        public InitiativeCardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            KeyResultsTab = new KeyResultsTabPage(driver, log);
            DeliverableTab = new DeliverablesTabPage(driver, log);
            DependenciesTab = new DependenciesTabPage(driver, log);
            ObstaclesTab = new ObstaclesTabPage(driver, log);
            CheckListTab = new CheckListTabPage(driver, log);
            CommentsTab = new CommentsTabPage(driver, log);
            AdditionalDetails = new AdditionalDetailsPage(driver, log);
            FinancialTab = new FinancialsTabPage(driver, log);

        }

        //Locators
        #region Additional Details

        private readonly By AdditionalDetailsExpand = By.XPath("//*[text()='Additional Details']/span/*");
        private readonly By AdditionalDetailsCollapse = By.XPath("//*[text()='Additional Details']/parent::div/*[local-name()='svg'] | //*[text()='Additional Details']/ancestor::p/parent::div/*[local-name()='svg']");

        #endregion

        public readonly By ParentOutcomeLink = By.XPath("//tbody[@data-rbd-droppable-id='BO-Key-Results']/tr[1]//td/a");
        public readonly By ConfirmButton = By.XPath("//div[@class='k-window k-dialog']//button/span[text()='Confirm']");






        //Methods
        #region Additional Details

        public void ClickOnAdditionalDetail(bool open = true)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Click on additional details");
            if (open)
            {
                Wait.UntilElementClickable(AdditionalDetailsExpand).Click();
            }
            else
            {
                Wait.UntilElementClickable(AdditionalDetailsCollapse).Click();
            }
        }
        #endregion

        public string GetParentOutcomeLinkName()
        {
            return Wait.UntilElementExists(ParentOutcomeLink).GetAttribute("aria-label").Split(new[] { " - " }, StringSplitOptions.None).Last().Trim();
        }

        public void ClickOnParentOutcomeLink()
        {
            Log.Step(nameof(KeyResultsTabPage), "Click on 'Parent Outcome Link' ");
            Wait.UntilElementExists(ParentOutcomeLink).Click();
        }

        public void ClickOnConfirmButton()
        {
            Log.Step(nameof(InitiativeCardPage), "Click on 'Confirm' button ");
            Wait.UntilElementClickable(ConfirmButton).Click();
        }

    }
}
