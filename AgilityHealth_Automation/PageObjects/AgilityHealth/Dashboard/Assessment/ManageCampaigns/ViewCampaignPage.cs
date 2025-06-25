using OpenQA.Selenium;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class ViewCampaignPage : ReviewAndSubmitPage
    {
        public ViewCampaignPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //locators
        private readonly By CampaignNameText = By.CssSelector("h1.MuiTypography-h1");
        private readonly By SystemAutomationsDeleteButton = By.XPath("//span[text()='Delete']//parent::button");

        //methods
        public string GetCampaignName()
        {
            Log.Step(GetType().Name, "Get 'Campaign Name' value");
            WaitTillSpinnerNotExist();
            return Wait.UntilElementVisible(CampaignNameText).GetText();
        }

        public void MoveToPageDown()
        {
            Driver.MoveToElement(Wait.UntilElementVisible(SystemAutomationsDeleteButton));
            Wait.HardWait(2000); //wait till scroll
        }
    }
}