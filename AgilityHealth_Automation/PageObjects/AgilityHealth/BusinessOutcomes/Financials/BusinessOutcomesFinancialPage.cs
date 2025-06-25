using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Financials
{
    public class BusinessOutcomesFinancialPage : BusinessOutcomeBasePage
    {
        public  BusinessOutcomesFinancialPage(IWebDriver driver, ILogger log) : base(driver, log)
        {

        }
        #region Locators

        private readonly By FinancialPageTitle = By.XPath("//p[contains(normalize-space(),'Current Financial Progress')]");

        #endregion

        #region Methods
        public bool IsFinancialPageTitleDisplayed()
        { 
        return Driver.IsElementDisplayed(FinancialPageTitle);
        }

        #endregion
    }
}