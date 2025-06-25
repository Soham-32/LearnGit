using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes
{
    public class CustomGrowthPlanSettingsPage : BasePage
    {
        public CustomGrowthPlanSettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Locators
        private readonly By ManageGrowthItemTypesButton = AutomationId.Equals("Growth Item Types_btn");

        // Methods
        public void ClickOnManageGrowthItemTypesButton()
        {
            Log.Step(nameof(CustomGrowthPlanSettingsPage), "Click on 'Manage Growth Item Type' button");
            Wait.UntilElementClickable(ManageGrowthItemTypesButton).Click();
            new ManageCustomTypesPage(Driver,Log).WaitUntilCustomGrowthItemTypeListIsLoaded();
        }
        public bool IsManageGrowthItemTypesButtonPresent()
        {
            return Driver.IsElementDisplayed(ManageGrowthItemTypesButton);
        }
    }
}