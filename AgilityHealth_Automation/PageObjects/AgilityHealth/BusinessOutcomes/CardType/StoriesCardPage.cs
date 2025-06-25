using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType
{
    public class StoriesCardPage : BusinessOutcomeBasePage
    {
        public KeyResultsTabPage KeyResultsTab { get; set; }
        public DeliverablesTabPage DeliverableTab { get; set; }
        public DependenciesTabPage DependenciesTab { get; set; }
        public ObstaclesTabPage ObstaclesTab { get; set; }
        public CheckListTabPage CheckListTab { get; set; }
        public CommentsTabPage CommentsTab { get; set; }
        public AdditionalDetailsPage AdditionalDetails { get; set; }
        public StoriesTabPage StoriesTab { get; set; }

        public StoriesCardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            KeyResultsTab = new KeyResultsTabPage(driver, log);
            DeliverableTab = new DeliverablesTabPage(driver, log);
            DependenciesTab = new DependenciesTabPage(driver, log);
            ObstaclesTab = new ObstaclesTabPage(driver, log);
            CheckListTab = new CheckListTabPage(driver, log);
            CommentsTab = new CommentsTabPage(driver, log);
            AdditionalDetails = new AdditionalDetailsPage(driver, log);
            StoriesTab = new StoriesTabPage(driver, log);
        }
    }
}