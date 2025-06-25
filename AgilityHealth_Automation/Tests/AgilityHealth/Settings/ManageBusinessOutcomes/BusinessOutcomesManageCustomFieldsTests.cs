using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageBusinessOutcomes
{

    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageBusinessOutcomes")]
    public class BusinessOutcomesManageCustomFieldsTests : BaseTest
    {
        private static bool _classInitFailed;
        private static string _classInitFailedMessage;
        private static User _user;
        private static AtCommon.Dtos.Company _settingsCompany;

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            try
            {
                var siteAdminUserConfig = new UserConfig("SA");
                _settingsCompany = siteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);
                var siteAdmin = User.IsSiteAdmin() ? User : siteAdminUserConfig.GetUserByDescription("user 1");
                _user = User.IsSiteAdmin() ? siteAdmin : TestEnvironment.UserConfig.GetUserByDescription("user 3");
                new SetUpMethods(testContext, TestEnvironment).TurnOnBusinessOutcomesFeature(siteAdmin, _settingsCompany.Id);
            }
            catch (Exception e)
            {
                _classInitFailed = true;
                _classInitFailedMessage = e.ToLogString(e.StackTrace);
                throw;
            }

        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_CustomFields_Add()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var newCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();

            manageCustomFieldsPage.ClickOnAddCustomFieldLink();
            manageCustomFieldsPage.EnterCustomFieldName("", newCustomField.Name);
            manageCustomFieldsPage.ClickOnSaveButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            Assert.IsTrue(manageCustomFieldsPage.IsCustomFieldDisplayed(newCustomField.Name), 
                $"Custom Field <{newCustomField}> is not displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_CustomFields_Edit()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);
            var newCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();
            SetCustomFields(new List<CustomFieldRequest> {newCustomField});

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var updatedCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();

            manageCustomFieldsPage.EnterCustomFieldName(newCustomField.Name, updatedCustomField.Name);
            manageCustomFieldsPage.ClickOnSaveButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            Assert.IsTrue(manageCustomFieldsPage.IsCustomFieldDisplayed(updatedCustomField.Name), 
                $"Custom Field <{updatedCustomField.Name}> is not displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_CustomFields_Delete()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);
            var newCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();
            SetCustomFields(new List<CustomFieldRequest> {newCustomField});

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var updatedCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();

            manageCustomFieldsPage.ClickOnCustomFieldDeleteButton(newCustomField.Name);
            manageCustomFieldsPage.ClickOnSaveButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            Assert.IsFalse(manageCustomFieldsPage.IsCustomFieldDisplayed(updatedCustomField.Name), 
                $"Custom Field <{updatedCustomField.Name}> is still displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_CustomFields_ConfirmPopup_Cancel()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var newCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();

            manageCustomFieldsPage.ClickOnAddCustomFieldLink();
            manageCustomFieldsPage.EnterCustomFieldName("", newCustomField.Name);
            manageCustomFieldsPage.ClickOnCancelButton();
            manageCustomFieldsPage.ConfirmPopUpClickOnCancelButton();

            Assert.IsTrue(manageCustomFieldsPage.IsCustomFieldDisplayed(newCustomField.Name), 
                $"Custom Field <{newCustomField}> is not displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_CustomFields_ConfirmPopup_DiscardChanges()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var newCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();

            manageCustomFieldsPage.ClickOnAddCustomFieldLink();
            manageCustomFieldsPage.EnterCustomFieldName("", newCustomField.Name);
            manageCustomFieldsPage.ClickOnCancelButton();
            manageCustomFieldsPage.ConfirmPopUpClickOnDiscardChangesButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            Assert.IsFalse(manageCustomFieldsPage.IsCustomFieldDisplayed(newCustomField.Name), 
                $"Custom Field <{newCustomField}> is still displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_CustomFields_ConfirmPopup_SaveChanges()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var newCustomField = BusinessOutcomesFactory.GetCustomFieldRequest();

            manageCustomFieldsPage.ClickOnAddCustomFieldLink();
            manageCustomFieldsPage.EnterCustomFieldName("", newCustomField.Name);
            manageCustomFieldsPage.ClickOnCancelButton();
            manageCustomFieldsPage.ConfirmPopUpClickOnSaveChangesButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            Assert.IsTrue(manageCustomFieldsPage.IsCustomFieldDisplayed(newCustomField.Name), 
                $"Custom Field <{newCustomField}> is not displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")] 
        public void BusinessOutcomes_Settings_CustomFields_Reorder()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);
            BusinessOutcomesFactory.GetCustomFieldRequest();
            var customFields = SetCustomFields(
                new List<CustomFieldRequest> 
                {
                    BusinessOutcomesFactory.GetCustomFieldRequest(),
                    BusinessOutcomesFactory.GetCustomFieldRequest()
                });

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomFieldsPage = new ManageCustomFieldsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            manageCustomFieldsPage.DragCustomFieldToCustomField(customFields[1].Name, customFields[0].Name);
            manageCustomFieldsPage.ClickOnSaveButton();

            manageCustomFieldsPage.ClickOnManageCustomFieldsButton();
            var expected = customFields.Select(c => c.Name).Reverse().ToList();
            var actual = manageCustomFieldsPage.GetAllCustomFieldTexts();

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], $"The custom field at index <{i}> does not match.");
            }
        }

        private static List<CustomFieldResponse> SetCustomFields(List<CustomFieldRequest> request)
        {
            return new SetupTeardownApi(TestEnvironment).AddBusinessOutcomesCustomFields(_settingsCompany.Id, request, _user);
        }
    }
}