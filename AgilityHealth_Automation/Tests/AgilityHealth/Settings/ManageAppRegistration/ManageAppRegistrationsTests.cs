using System;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageAppRegistration;
using AtCommon.Api;
using AtCommon.Dtos.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageAppRegistration
{
    [TestClass]
    [TestCategory("ManageAppRegistrations"), TestCategory("ManageAppRegistrations")]
    public class ManageAppRegistrationsTests : BaseTest
    {

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageAppRegistrations_AddApp()
        {
            var login = new LoginPage(Driver, Log);
            var settings = new SettingsPage(Driver, Log);
            var manageAppRegistrationsPage = new ManageAppRegistrationsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            settings.NavigateToPage();
            settings.SelectSettingsOption("View Apps");
            var newAppName = $"temp{Guid.NewGuid():N}";

            manageAppRegistrationsPage.AddNewAppRegistration(newAppName);
            var actual = manageAppRegistrationsPage.GetNewAppRegistrationInfoFromPopup();
            Assert.AreEqual(newAppName, actual.AppName, "AppName doesn't match.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.ClientId), "ClientId is empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Secret), "Secret is empty.");

            manageAppRegistrationsPage.ClickCloseButtonOnCreatePopup();

            var allRegistrations = manageAppRegistrationsPage.GetAppRegistrations();
            Assert.IsTrue(allRegistrations.Any(r => r.AppName == newAppName), 
                $"New app {newAppName} not displayed in grid.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageAppRegistrations_DeleteApp()
        {
            // create an app registration
            var client = ClientFactory
                .GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName).GetAwaiter()
                .GetResult();
            var response =
                client.PostAsync<AddAppRegistrationResponse>(RequestUris.OauthRegister(), new AddAppRegistrationRequest { AppName = $"temp{Guid.NewGuid():N}"}).GetAwaiter().GetResult();
            response.EnsureSuccess();
            
            var login = new LoginPage(Driver, Log);
            var manageAppRegistrationsPage = new ManageAppRegistrationsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            manageAppRegistrationsPage.NavigateToPage(Company.Id);
            manageAppRegistrationsPage.ClickDeleteButton(response.Dto.AppName);

            var allRegistrations = manageAppRegistrationsPage.GetAppRegistrations();
            Assert.IsFalse(allRegistrations.Any(r => r.AppName == response.Dto.AppName), 
                $"Deleted app {response.Dto.AppName} is still displayed in grid.");
        }
    }
}