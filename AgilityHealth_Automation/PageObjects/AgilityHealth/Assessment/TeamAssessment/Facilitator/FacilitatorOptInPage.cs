using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Facilitator
{
    public class FacilitatorOptInPage : BasePage
    {
        public FacilitatorOptInPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By DateAndTime = By.XPath("//div[@id = 'firstsignup']//tbody/tr[1]/td");
        private readonly By Team = By.XPath("//div[@id = 'firstsignup']//tbody/tr[2]/td");
        private readonly By Assessment = By.XPath("//div[@id = 'firstsignup']//tbody/tr[3]/td");
        private readonly By Location = By.XPath("//div[@id = 'firstsignup']//tbody/tr[4]/td");

        public string GetDateAndTime()
        {
            return Wait.UntilElementVisible(DateAndTime).GetText();
        }

        public string GetTeam()
        {
            return Wait.UntilElementVisible(Team).GetText();
        }

        public string GetAssessment()
        {
            return Wait.UntilElementVisible(Assessment).GetText();
        }

        public string GetLocation()
        {
            return Wait.UntilElementVisible(Location).GetText();
        }
    }
}
