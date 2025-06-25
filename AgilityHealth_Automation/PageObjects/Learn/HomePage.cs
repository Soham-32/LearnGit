using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.Learn
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators
        private readonly By AllCoursesTab = By.XPath("//header[@id='masthead']//span[normalize-space()='All Courses']");
        private readonly By MyDashboardTab = By.XPath("//header[@id='masthead']//span[normalize-space()='My Dashboard']");
        private readonly By WelcomeTitle = By.XPath("//h2[text()='Welcome to Agility University']");

        #endregion

        #region Methods

        public bool IsAllCoursesTabDisplayed()
        {
            return Driver.IsElementDisplayed(AllCoursesTab);
        }

        public bool IsMyDashboardTabDisplayed()
        {
            return Driver.IsElementDisplayed(MyDashboardTab);
        }

        public bool IsWelcomeTitleDisplayed()
        {
            return Driver.IsElementDisplayed(WelcomeTitle);
        }

        #endregion
    }
}
