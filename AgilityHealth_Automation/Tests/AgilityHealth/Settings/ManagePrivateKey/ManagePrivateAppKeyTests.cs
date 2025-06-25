using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManagePrivateKey;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManagePrivateKey
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManagePrivateAppKey")]
    public class ManagePrivateAppKeyTests : BaseTest
    {
        public string PrivateAppName = "PrivateApp" + RandomDataUtil.GetCompanyCity();
        private const string PrivateAppUserAccess = "Feature Admin";

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManagePrivateAppKey_AddAndDelete()
        {
            var login = new LoginPage(Driver, Log);
            var managePrivateAppKeyPage = new ManagePrivateAppKeyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Verify that user is able to create PrivateAppKey using 'Manage App keys'");
            managePrivateAppKeyPage.NavigateToPage(Company.Id);

            Log.Info("Verify that if any PrivateAppkey is present then will be deleted.'");
            managePrivateAppKeyPage.DeleteAllPrivateAppKeys();

            managePrivateAppKeyPage.ClickOnPrivateAddAppKeyButton();
            managePrivateAppKeyPage.EnterPrivateAppKeyName(PrivateAppName);
            managePrivateAppKeyPage.ClickOnCreatePrivateAppCloseButton();

            Assert.IsFalse(managePrivateAppKeyPage.IsPrivateAppDisplayed(), $"Created App{PrivateAppName} is displayed");

            managePrivateAppKeyPage.ClickOnPrivateAddAppKeyButton();
            managePrivateAppKeyPage.EnterPrivateAppKeyName(PrivateAppName);
            managePrivateAppKeyPage.ClickOnCreatePrivateAppCreateButton();
            Assert.IsTrue(managePrivateAppKeyPage.IsPrivateAppKeyDisplayed(), "Created app key is not displayed");

            var app1 = managePrivateAppKeyPage.GetPrivateAppKey();
            managePrivateAppKeyPage.ClickOnCreatePrivateAppCloseButton();

            var expiryAt = DateTime.Now.Date.AddDays(365).ToString("MM/dd/yyyy");

            Assert.IsTrue(managePrivateAppKeyPage.IsPrivateAppDisplayed(), $"Created App{PrivateAppName} is not displayed");
            Assert.AreEqual(User.Username, managePrivateAppKeyPage.GetPrivateAppCreatedBy(PrivateAppName), "CreatedBy is not matched");
            Assert.AreEqual(PrivateAppUserAccess, managePrivateAppKeyPage.GetPrivateAppUserAccess(PrivateAppName), "User Access is not matched");

            Assert.AreEqual(expiryAt, managePrivateAppKeyPage.GetPrivateAppExpiryAt(PrivateAppName), "Expiry At is not matched");

            Log.Info("Verify that 'App Key' Should not be the same if it's created second time with Same Primary app name");

            managePrivateAppKeyPage.ClickOnPrivateAddAppKeyButton();
            managePrivateAppKeyPage.EnterPrivateAppKeyName(PrivateAppName);
            managePrivateAppKeyPage.ClickOnCreatePrivateAppCreateButton();

            var app2 = managePrivateAppKeyPage.GetPrivateAppKey();

            Assert.AreNotEqual(app1, app2, "Both Private app Keys are matched");
            managePrivateAppKeyPage.ClickOnCreatePrivateAppCloseButton();

            Assert.IsTrue(managePrivateAppKeyPage.IsPrivateAppDisplayed(), $"Created App{PrivateAppName} is not displayed");
            Assert.AreEqual(User.Username, managePrivateAppKeyPage.GetPrivateAppCreatedBy(PrivateAppName), "CreatedBy is not matched");
            Assert.AreEqual(PrivateAppUserAccess, managePrivateAppKeyPage.GetPrivateAppUserAccess(PrivateAppName), "User Access is not matched");
            Assert.AreEqual(expiryAt, managePrivateAppKeyPage.GetPrivateAppExpiryAt(PrivateAppName), "Expiry At is not matched");

            Log.Info("Verify the 'App Key' after deleting it");
            managePrivateAppKeyPage.ClickOnPrivateAppDeleteButton(PrivateAppName);
            Assert.IsFalse(managePrivateAppKeyPage.IsPrivateAppDisplayed(), $"Created App{PrivateAppName} is displayed");
        }
    }
}