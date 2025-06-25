using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure
{
    public class CommonGrowthItemsDashboardBasePage : BasePage
    {
        public CommonGrowthItemsDashboardBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Headers
        private readonly By AddGrowthItemButton = By.Id("addgrowthteam");
        private readonly By GridViewButton = By.Id("aGrowthListView");
        private static readonly By CustomIframe = By.XPath("//iframe[contains(@src,'growthItems')]");
        #endregion

        #endregion

        #region Methods

        public void ClickOnAddGrowthItemButton()
        {
            Log.Step(nameof(CommonGrowthItemsDashboardBasePage), "Click on Add Growth Item button");
            Wait.UntilElementClickable(AddGrowthItemButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SwitchToGridView()
        {
            Log.Step(nameof(CommonGrowthItemsDashboardBasePage), "Switch to grid view");
            Driver.SwitchToFrame(CustomIframe);
            Wait.UntilElementClickable(GridViewButton).Click();
            Wait.UntilJavaScriptReady();
        }

        #endregion
    }
}
