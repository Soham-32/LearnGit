using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCampaign;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageCampaign
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageCampaigns")]
    public class ManageCampaignTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")]//Bug Id:52425
        public void ManageCampaigns_AddEditDeleteCampaigns()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageCampaigns = new ManageCampaignsPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnSettingsLink();

            settingsPage.SelectSettingsOption("View Campaigns");

            manageCampaigns.ClickAddNewCampaign();

            var campaignsInfo = ManageCampaignsFactory.GetCompaignData();
            manageCampaigns.EnterCampaignDetails(campaignsInfo);

            manageCampaigns.ClickSaveCampaign();

            Assert.IsTrue(manageCampaigns.DoesCampaignExist(campaignsInfo), $"New Campaign {campaignsInfo.Name} isn't present");

            manageCampaigns.ClickEditCampaign(campaignsInfo.Name);

            var campaignsUpdatedInfo = ManageCampaignsFactory.GetCompaignData();
            manageCampaigns.EnterCampaignDetails(campaignsUpdatedInfo);

            manageCampaigns.ClickSaveCampaign();

            Assert.IsTrue(manageCampaigns.DoesCampaignExist(campaignsUpdatedInfo), $"Edited Campaign {campaignsUpdatedInfo.Name} isn't present");

            manageCampaigns.DeleteCampaign(campaignsUpdatedInfo.Name);

            Assert.IsFalse(manageCampaigns.DoesCampaignExist(campaignsUpdatedInfo), "New Campaign should be deleted properly");
        }
    }
}
