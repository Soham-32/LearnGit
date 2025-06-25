using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit
{
    public class MetricsPage : BasePage
    {
        public MetricsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Header
        private readonly By GetDataButton = By.XPath("//button[@onclick='getJiraData()']");

        //Jira Logo
        private readonly By JiraLogo = By.CssSelector("img[src='/images/jira_software.png']");

        //Header
        public bool IsGetDataButtonPresent()
        {
            return Driver.IsElementPresent(GetDataButton);
        }

        //Jira Logo
        public bool IsJiraLogoPresent()
        {
            return Driver.IsElementPresent(JiraLogo);
        }

        //Navigate
        public void NavigateToPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/metrics/{teamId}");
        }
    }
}