using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Add
{
    internal class AddRadarDetailsPage : RadarDetailsBasePage
    {
        public AddRadarDetailsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Radar Details
        private readonly By SaveAndContinueButton = By.XPath("//input[@value='Save and Continue']");
         
        public void ClickOnSaveAndContinueButton()  
        {
            Log.Step(nameof(AddRadarDetailsPage), "Click on 'Save and Continue' Button");
            Wait.UntilElementClickable(SaveAndContinueButton).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}