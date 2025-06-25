using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings
{
    [TestClass]
    [TestCategory("Settings")]
    public class SettingsNavigationTests : BaseTest
    {

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        public void Settings_V2_VerifyNavigation()
        {
            Log.Info("Test  : Verify that user can navigate to respective pages from new settings page");
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var v2Settings = new SettingsV2Page(Driver, Log);
            var demandCategory = new ManageBusinessOutcomeTagsPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var customFields = new ManageCustomFieldsPage(Driver, Log);
            var boDashboardSettingsPage = new BusinessOutcomesDashboardSettingsPage(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            login.NavigateToPage();

            if (User.IsSiteAdmin())
            {
                login.LoginToApplication(User.Username, User.Password);
                companyDashboardPage.WaitUntilLoaded();
                v2Header.ClickOnBusinessOutComeLink();
            }
            else
            {
                var user4 = TestEnvironment.UserConfig.GetUserByDescription("user 4");
                login.LoginToApplication(user4.Username, user4.Password);
                topNav.ClickOnBusinessOutComeLink();
            }
            Assert.IsTrue(boDashboard.IsPageLoadedCompletely(), "Business Outcome page isn't loaded successfully.Stopping execution");

            if (User.IsSiteAdmin())
            {
                leftNav.SelectCompany(User.CompanyName);
            }

            v2Header.ClickOnSettingsLink();
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/V2/settings/company/"), $"User isn't navigated to Settings page, instead navigated to {Driver.GetCurrentUrl()}");

            v2Settings.ClickOnBusinessOutcomesManageSettingsButton();
            var buttonName = "Manage Business Outcomes Tags";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(demandCategory.IsManageBusinessOutcomeTagHeaderTextPresent(), $"User isn't navigated to '{buttonName}' page");
            demandCategory.ClickOnCancelButton();

            buttonName = "Manage Custom Fields";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(customFields.IsManageCustomFieldsHeaderTextPresent(), $"User isn't navigated to '{buttonName}' page");
            customFields.ClickOnCancelButton();

            buttonName = "Manage Dashboard Settings";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(boDashboardSettingsPage.IsDashboardSettingsHeadingDisplayed(), $"User isn't navigated to '{buttonName}' page");
            boDashboardSettingsPage.ClickOnCancelButton();

            boDashboardSettingsPage.ClickOnCloseButton();

            buttonName = "Manage Financials";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/companycost"), $"User isn't navigated to '{buttonName}' page, instead navigated to {Driver.GetCurrentUrl()}");
            Driver.BackAndWait();
            v2Settings.WaitUntilPageLoaded(buttonName);

            buttonName = "Manage Model & Checklist";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(Driver.GetCurrentUrl().Equals(ApplicationUrl + "/teammaturity/company/" + Company.Id), $"User isn't navigated to '{buttonName}' page, instead navigated to {Driver.GetCurrentUrl()}");
            Driver.BackAndWait();
            v2Settings.WaitUntilPageLoaded(buttonName);

            buttonName = "Manage Profile";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(Driver.GetCurrentUrl().StartsWith(ApplicationUrl + "/account?companyId="), $"User isn't navigated to '{buttonName}' page, instead navigated to {Driver.GetCurrentUrl()}");
            Driver.BackAndWait();
            v2Settings.WaitUntilPageLoaded(buttonName);

            buttonName = "Manage Radars";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/surveys"), $"User isn't navigated to '{buttonName}' page, instead navigated to {Driver.GetCurrentUrl()}");
            Driver.BackAndWait();
            v2Settings.WaitUntilPageLoaded(buttonName);

            buttonName = "Manage Team Tags";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(Driver.GetCurrentUrl().Equals(ApplicationUrl + "/tags/company/" + Company.Id), $"User isn't navigated to '{buttonName}' page, instead navigated to {Driver.GetCurrentUrl()}");
            Driver.BackAndWait();
            v2Settings.WaitUntilPageLoaded(buttonName);

            buttonName = "Manage Users";
            Log.Info($"Verify that '{buttonName}' button present and it's working as expected");
            Assert.IsTrue(v2Settings.IsSettingOptionPresent(buttonName), $"'{buttonName}' button isn't present");
            v2Settings.SelectSettingsOption(buttonName);
            Assert.IsTrue(Driver.GetCurrentUrl().Equals(ApplicationUrl + "/user/" + Company.Id), $"User isn't navigated to '{buttonName}' page, instead navigated to {Driver.GetCurrentUrl()}");

        }
    }
}