using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit
{
    public class EditTeamBasePage : BasePage
    {

        public EditTeamBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }


        private readonly By ReturnToDashboardButton = By.XPath("//a[text()='Return to Dashboard']");
        private readonly By SubTeamsTab = By.XPath("//div[contains(@class,'tabs')]/a[text()='Sub-Teams']");
        private readonly By GrowthTeamTab = By.XPath("//div[contains(@class,'tabs')]/a[text()='Growth Team']");
        private readonly By StakeHoldersTab = By.XPath("//div[contains(@class,'tabs')]/a[text()='Stakeholders']");
        private readonly By TeamMembersTab = By.XPath("//div[contains(@class,'tabs')]/a[text()='Team Members']");
        private readonly By TeamProfileTab = By.XPath("//div[contains(@class,'tabs')]/a[text()='Team Profile']");
        private readonly By MetricsTab = By.XPath("//div[contains(@class,'tabs')]/a[text()='Metrics']");
        protected By PageHeaderTitle = By.XPath("//div[@class='txt fl-lt']//h1");


        public void GoToDashboard()
        {
            Log.Step(GetType().Name, "On Edit Team page, click Return To Dashboard button");
            Wait.UntilElementClickable(ReturnToDashboardButton).ClickOn(Driver);
        }

        // Tabs
        public void GoToTeamMembersTab()
        {
            Log.Step(GetType().Name, "On Edit Team page, select Team Members tab");
            Wait.UntilElementClickable(TeamMembersTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void GoToSubTeamsTab()
        {
            Log.Step(GetType().Name, "On Edit Team page, select Sub-Teams tab");
            Wait.UntilElementClickable(SubTeamsTab).Click();
        }

        public void GoToStakeHoldersTab()
        {
            Log.Step(GetType().Name, "On Edit Team page, select Stakeholders tab");
            Wait.UntilElementClickable(StakeHoldersTab).Click();
        }

        public void GoToGrowthTeamTab()
        {
            Log.Step(GetType().Name, "On Edit Team page, select Growth Team tab");
            Wait.UntilElementClickable(GrowthTeamTab).Click();
        }

        public void GoToMetricsTab()
        {
            Log.Step(GetType().Name, "On Edit Team page, select Metrics tab");
            Wait.UntilElementClickable(MetricsTab).Click();
        }

        public void GoToTeamProfileTab()
        {
            Log.Step(GetType().Name, "On Edit Team page, select Team Profile tab");
            Wait.UntilElementClickable(TeamProfileTab).Click();
        }

        public bool IsMetricsTabPresent()
        {
            return Driver.IsElementPresent(MetricsTab);
        }
        public string GetPageHeaderTitle()
        {
            Log.Step(GetType().Name, "On Edit Team page, get page header title");
            return Wait.UntilElementVisible(PageHeaderTitle).GetText();
        }

        public void NavigateToPage(string teamId)
        {
            Log.Step(GetType().Name, $"Navigate to team edit page, team id {teamId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/edit/{teamId}");
        }
    }

}
