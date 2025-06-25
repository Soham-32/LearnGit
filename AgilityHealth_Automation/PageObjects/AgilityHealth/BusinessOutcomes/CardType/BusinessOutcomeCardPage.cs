using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType
{
    public class BusinessOutcomeCardPage : BusinessOutcomeBasePage
    {
        public KeyResultsTabPage KeyResultsTab { get; set; }
        public InitiativesTabPage InitiativesTab { get; set; }
        public ChildOutcomeTabPage ChildOutcomeTab { get; set; }
        public ObstaclesTabPage ObstaclesTab { get; set; }
        public CheckListTabPage CheckListTab { get; set; }
        public CommentsTabPage CommentsTab { get; set; }
        
        public BusinessOutcomeCardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            KeyResultsTab = new KeyResultsTabPage(driver, log);
            InitiativesTab = new InitiativesTabPage(driver, log);
            ChildOutcomeTab = new ChildOutcomeTabPage(driver, log);
            ObstaclesTab = new ObstaclesTabPage(driver, log);
            CheckListTab = new CheckListTabPage(driver, log);
            CommentsTab = new CommentsTabPage(driver, log);
        }
        
        
        public void NavigateToBusinessOutcomesPageForProd(string env, int id)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/V2/outcomes/company/{id}/parents/0/team/0?cardTypeId=1&categoryLabelUid=1");
        }
        public void NavigateToBusinessOutcomesPageForSaDomain(string env, int id)
        {
            NavigateToUrl($"https://{env}.agilityinsights.sa/V2/outcomes/company/{id}/parents/0/team/0/tab/kanban?cardTypeId=1&categoryLabelUid=1");
        }
    }
}
