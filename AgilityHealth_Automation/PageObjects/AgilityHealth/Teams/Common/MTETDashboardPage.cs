using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common
{
    public class MtEtDashboardPage : BasePage
    {
        public MtEtDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddAPulseCheckButton = By.XPath("//input[@value='Add A Pulse Check']");

        private By DynamicRadar(string radarName) =>
           By.XPath($"//div[@id='mtListView']//h5[text()='{radarName}']/preceding-sibling::div /a");

        private readonly By AssessmentTypeListView = By.Id("mtListView");
        private readonly By TeamName = By.TagName("h1");


        public void ClickOnAddAPulseCheckButton()
        {
            Log.Step(nameof(MtEtDashboardPage), $"On Assessment page, click on radar 'Add A Pulse Check' Button");
            Wait.UntilElementClickable(AddAPulseCheckButton).Click();
        }

        public void ClickOnRadar(string radarName)
        {
            Log.Step(nameof(MtEtDashboardPage), $"On Assessment page, click on radar ${radarName}");
            Wait.UntilJavaScriptReady();
            if (Driver.IsElementPresent(AssessmentTypeListView))
            {
                Wait.UntilElementClickable(DynamicRadar(radarName)).Click();
            }

        }

        public string GetTeamName()
        {
            return Wait.UntilElementVisible(TeamName).GetText();
        }

        public void NavigateToRadarDetailsPage(string url, int teamId, bool isEt = false, int radarId = SharedConstants.TeamSurveyId)
        {
            if (url.Contains("multiteam") || url.Contains("enterprise"))
            {
                NavigateToUrl(!isEt
                ? $"{BaseTest.ApplicationUrl}/multiteam/{teamId}/radar/{radarId}"
                : $"{BaseTest.ApplicationUrl}/enterprise/{teamId}/radar/{radarId}");
            }
        }

        public void NavigateToPage(int teamId, bool isEt = false, bool isNtier = false)
        {
            Log.Step(nameof(MtEtDashboardPage), $"Navigate to team assessment, team id {teamId}");

            string url = isNtier
                ? $"{BaseTest.ApplicationUrl}/ntier/dashboard?teamId={teamId}"
                : isEt
                    ? $"{BaseTest.ApplicationUrl}/enterprise/{teamId}/dashboard?growthItemId=0"
                    : $"{BaseTest.ApplicationUrl}/multiteam/{teamId}/dashboard?growthItemId=0";

            Driver.Navigate().GoToUrl(url);
            Wait.HardWait(3000); // Wait till page navigate to the dashboard.
        }
    }
}