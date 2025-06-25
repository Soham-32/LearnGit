using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class ManageCampaignsTabPage : AssessmentDashboardBasePage
    {
        public ManageCampaignsTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By PageHeaderTitleText = By.XPath("//div[@id='headerScheduler']/h1");
        private readonly By CreateNewCampaignsButton = By.XPath("//a[contains(@aria-label, 'Create New Campaign')]");
        private readonly By IncludeArchivedCheckbox = By.XPath("//input[@id='showArchivedCheckbox']");
        private readonly By SearchBox = By.XPath("//input[@id='search']");

        private readonly By AllColumnHeaderText = By.XPath("//table[@role]/thead//th[not(@style)]");
        private readonly By CampaignNameList = By.XPath("//div[@id='Campaigns']//table//tbody//td[contains(@id,'campaignName')]");
        private static By ListByColumnName(string columnName) => By.XPath($"//div[@id='campaignsGrid']//table//td[count(//div[@id='campaignsGrid']//table//th/a[contains(text(),'{columnName}')]//parent::th//preceding-sibling::th)+1]");
        private static By EditCampaignButton(string campaignName) => By.XPath($"//tbody/tr/td[@id]/a[text()='{campaignName}']//parent::td/following-sibling::td/a[@actionid='edit']");
        private static By EditCampaignButtonByIndex(int index) => By.XPath($"//tbody/tr[{index}]/td/a[@actionid='edit']");
        private static By DeleteCampaignButton(string campaignName) => By.XPath($"//tbody/tr/td[@id]/a[text()='{campaignName}']//parent::td/following-sibling::td/a[@actionid='delete']");
        private static By DeleteCampaignButtonByIndex(int index) => By.XPath($"//tbody/tr[{index}]/td/a[text()='Delete']");
        private static By ViewCampaignButton(string campaignName) => By.XPath($"//tbody/tr/td[@id]/a[text()='{campaignName}']//parent::td/following-sibling::td/a[@actionid='view']");
        private static By ViewCampaignButtonByIndex(int index) => By.XPath($"//tbody/tr[{index}]/td/a[text()='View']");


        //Methods
        public string GetManageCampaignHeaderTitleText()
        {
            return Wait.UntilElementVisible(PageHeaderTitleText).GetText();
        }

        public string GetCreateNewCampaignsButtonText()
        {
            Log.Step(nameof(ManageCampaignsTabPage), "Get the 'Create New Campaign' button text");
            return Wait.UntilElementVisible(CreateNewCampaignsButton).GetText();
        }

        public void ClickOnIncludeArchivedCheckbox()
        {
            Log.Step(nameof(ManageCampaignsTabPage), "Click on 'Include Archived' Checkbox");
            Wait.UntilElementClickable(IncludeArchivedCheckbox).Click();
        }

        public void ClickOnCreateNewCampaignsButton()
        {
            Log.Step(nameof(ManageCampaignsTabPage), "Click on 'Create new campaigns' button");
            Wait.UntilElementClickable(CreateNewCampaignsButton).Click();
        }

        public void SearchCampaignDetails(string text)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Search {text} from Search box.");
            Wait.UntilElementVisible(SearchBox).SendKeys(text);
        }

        public List<string> GetAllColumnHeaderTextList()
        {
            Log.Step(nameof(ManageCampaignsTabPage), "Get manage campaign column header text");
            Wait.HardWait(2000); // It takes time to appear the header column
            return Driver.GetTextFromAllElements(AllColumnHeaderText).ToList();
        }

        public List<string> GetCampaignNameList()
        {
            Log.Step(nameof(ManageCampaignsTabPage), "Get Campaign Name List");
            return Driver.GetTextFromAllElements(CampaignNameList).ToList();
        }

        public List<string> GetListByColumnName(string columnName)
        {
            Log.Step(nameof(ManageCampaignsTabPage), "Get list by column name");
            return Driver.GetTextFromAllElements(ListByColumnName(columnName)).ToList();
        }
        public void ClickOnEditCampaignsButton(string campaignName)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Click on {campaignName} campaign 'Edit'  button");
            Wait.UntilElementClickable(EditCampaignButton(campaignName)).Click();
        }

        public void ClickOnEditCampaignsButtonByIndex(int index)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Click on {index} no. campaign 'Edit'  button");
            Wait.UntilElementClickable(EditCampaignButtonByIndex(index)).Click();
        }

        public void ClickOnDeleteCampaignsButton(string campaignName)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Click on {campaignName} campaign 'Delete'  button");
            Wait.UntilElementClickable(DeleteCampaignButton(campaignName)).Click();
            Driver.AcceptAlert();
        }

        public void ClickOnDeleteCampaignsButtonByIndex(int index)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Click on {index} no. campaign 'Delete'  button");
            Wait.UntilElementClickable(DeleteCampaignButtonByIndex(index)).Click();
        }

        public void ClickOnViewCampaignsButton(string campaignName)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Click on {campaignName} campaign 'View'  button");
            Wait.UntilElementClickable(ViewCampaignButton(campaignName)).Click();
        }

        public void ClickOnViewCampaignsButtonByIndex(int index)
        {
            Log.Step(nameof(ManageCampaignsTabPage), $"Click on {index} no. campaign 'View'  button");
            Wait.UntilElementClickable(ViewCampaignButtonByIndex(index)).Click();
        }
        public void NavigateToManageCampaignsTabForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/company/{companyId}/ManageCampaignsDashboard");
        }
    }
}