using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Settings;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCampaign
{
    public class ManageCampaignsPage : BasePage
    {
        public ManageCampaignsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private By AddNewButton = By.CssSelector("a.k-grid-add");
        private By CampaignName = By.Id("Name");
        private By CampaignDescription = By.Id("Description");
        private By SaveButton = By.CssSelector("a.k-grid-update");
        private const string StartDateCalendarId = "StartDate_dateview";
        private const string EndDateCalendarId = "EndDate_dateview";

        private By CampaignEditButton(string campaignName) => By.XPath($"//td[text()='{campaignName}']/following-sibling::td/a[contains(@class, 'k-grid-edit')] | //font[text()='{campaignName}']/ancestor::td/following-sibling::td/a[contains(@class, 'k-grid-edit')]");
        private By CampaignDeleteButton(string campaignName) => By.XPath($"//td[text()='{campaignName}']/following-sibling::td/a[contains(@class, 'k-grid-delete')] | //font[text()='{campaignName}']/ancestor::td/following-sibling::td/a[contains(@class, 'k-grid-delete')]");
        private By CampaignItem(string name, string description, string startDate, string endDate) => By.XPath($"//td[text()='{name}']/following-sibling::td[text()='{description}']/following-sibling::td[text()='{startDate}']/following-sibling::td[text()='{endDate}'] | //font[text()='{name}']/ancestor::td/following-sibling::td//font[text()='{description}']/ancestor::td/following-sibling::td//font[text()='{startDate}']/ancestor::td/following-sibling::td//font[text()='{endDate}']");

        public void ClickAddNewCampaign()
        {
            Log.Step(nameof(ManageCampaignsPage), "Click Add New button");
            Wait.UntilElementVisible(AddNewButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void EnterCampaignDetails(Campaign campaign)
        {
            Log.Step(nameof(ManageCampaignsPage), "Enter campaign details");
            Wait.UntilElementVisible(CampaignName).SetText(campaign.Name);
            Wait.UntilElementVisible(CampaignDescription).SetText(campaign.Description);
            var startDateCal = new CalendarWidget(Driver, StartDateCalendarId);
            // set Start date
            startDateCal.SetDate(campaign.SatrtDate);
            var endDateCal = new CalendarWidget(Driver, EndDateCalendarId);
            // set End date
            endDateCal.SetDate(campaign.EndDate);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(CampaignName).Click();
        }

        public void ClickSaveCampaign()
        {
            Log.Step(nameof(ManageCampaignsPage), "Click on Save Campaign button");
            Wait.UntilElementVisible(SaveButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickEditCampaign(string name)
        {
            Log.Step(nameof(ManageCampaignsPage), $"Click on Edit button for campaign <{name}>");
            Wait.UntilElementVisible(CampaignEditButton(name)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void DeleteCampaign(string name)
        {
            Log.Step(nameof(ManageCampaignsPage), $"Delete campaign <{name}>");
            Wait.UntilElementVisible(CampaignDeleteButton(name)).Click();
            Driver.AcceptAlert();
        }

        public bool DoesCampaignExist(Campaign campaign)
        {
            return  Driver.IsElementDisplayed(CampaignItem(campaign.Name, campaign.Description, campaign.SatrtDate.ToString("MM/dd/yyyy"), campaign.EndDate.ToString("MM/dd/yyyy")),20);
        }
    }
}
