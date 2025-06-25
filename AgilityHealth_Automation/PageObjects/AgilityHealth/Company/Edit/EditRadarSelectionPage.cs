using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit
{
    public class EditRadarSelectionPage : RadarSelectionBase
    {
        public EditRadarSelectionPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditCompanyHeaderWidget(driver, log);
        }

        public EditCompanyHeaderWidget Header { get; set; }

        private readonly By RadarSelectionDiv = By.XPath("//div[starts-with(@automation-id, 'editcompany-tabpanel')][2]");

        //Key-customer verification
        #region Elements
        private readonly By RadarSelectionHeaderTitle = By.XPath("//form//*[contains(text(),'Radar Selection')]");
        #endregion

        public new void WaitUntilLoaded()
        {
            Wait.UntilElementVisible(RadarSelectionDiv);
        }

        //Key-customer verification
        #region Elements
        public string GetRadarSelectionHeaderText()
        {
            Log.Step(nameof(EditRadarSelectionPage), "Get the 'RadarSelection' header text");
            Wait.HardWait(3000);
            return Wait.UntilElementVisible(RadarSelectionHeaderTitle).GetText();
        }
        #endregion
    }
}
