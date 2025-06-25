using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit
{
    internal class EditRadarDetailsPage : RadarDetailsBasePage
    {
        public EditRadarDetailsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By UpdateButton = By.Id("updateBtn");

        public void ClickOnUpdateButton()  
        {
            Log.Step(nameof(EditRadarDetailsPage), "Click on 'Update' Button");
            Wait.UntilElementClickable(UpdateButton).Click();
        }

        public string GetHeaderCompanyName()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get Header Radar Company Name from Header 'Company' dropdown");
            return Wait.UntilElementExists(HeaderCompanyDropdown).GetText().Replace("\r\nselect", "");
        }

    }
}