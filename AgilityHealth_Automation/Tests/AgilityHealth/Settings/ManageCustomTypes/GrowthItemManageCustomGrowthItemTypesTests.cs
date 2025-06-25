using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageCustomTypes
{

    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageCustomTypes")]
    public class GrowthItemManageCustomGrowthItemTypesTests : BaseTest
    {
        private static SetupTeardownApi _setUp;
        private static CompanyResponse _companyResponse;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            var companyRequest = CompanyFactory.GetValidPostCompany();
            _setUp = new SetupTeardownApi(TestEnvironment);
            _companyResponse = _setUp.CreateCompany(companyRequest).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_Add()
        {
            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();

            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();

            var newCustomGrowthItemTypeRequest= GrowthPlanFactory.CustomTypesCreateRequest(3);
            var customGrowthItemTypeTexts = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).ToList();
            foreach (var customGrowthItemText in customGrowthItemTypeTexts)
            {
                manageCustomPage.ClickOnAddCustomGrowthItemTypeButton();
                manageCustomPage.EnterCustomGrowthItemTypeName("", customGrowthItemText);
            }
            manageCustomPage.ClickOnSaveButton();

            var actualCustomGrowthItemTypeList = manageCustomPage.GetAllCustomGrowthItemTypeList();
            foreach (var type in customGrowthItemTypeTexts)
            {
                Assert.That.ListContains(actualCustomGrowthItemTypeList, type, $"Custom growth item type <{type}> is not displayed.");
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_Edit()
        {
            var newCustomGrowthItemTypeRequest= GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id, 3);
            _setUp.CreateGrowthItemCustomType(newCustomGrowthItemTypeRequest);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();

            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();

            var customGrowthItemTypeTexts = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).ToList();

            var updatedCustomGrowthItemType = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id, 3);
            var updatedCustomGrowthItemTypeTexts = updatedCustomGrowthItemType.CustomGrowthPlanTypes.Select(a => a.CustomText).ToList();

            for (var item = 0; item < customGrowthItemTypeTexts.Count; item++)
            {
                Log.Info($"Updating custom growth item type- {customGrowthItemTypeTexts[item]} to {updatedCustomGrowthItemTypeTexts[item]} in the list");
                manageCustomPage.EnterCustomGrowthItemTypeName(customGrowthItemTypeTexts[item], updatedCustomGrowthItemTypeTexts[item]);
            }
            manageCustomPage.ClickOnSaveButton();
            manageCustomPage.WaitUntilCustomGrowthItemTypeListIsLoaded();

            var actualCustomGrowthItemTypeList = manageCustomPage.GetAllCustomGrowthItemTypeList();
            foreach (var type in updatedCustomGrowthItemTypeTexts)
            {
                Assert.That.ListContains(actualCustomGrowthItemTypeList, type, $"Custom growth item type <{type}> is not displayed.");
            }
            foreach (var type in customGrowthItemTypeTexts)
            {
                Assert.That.ListNotContains(actualCustomGrowthItemTypeList, type, $"Custom growth item type <{type}> is displayed.");
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_ConfirmCancelPopup_Cancel()
        {
            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();

            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            var customGrowthItemTypeText = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();

            Log.Info($"Adding custom field- {customGrowthItemTypeText}  in the custom growth item type list , and not click on 'Save' button");
            manageCustomPage.ClickOnAddCustomGrowthItemTypeButton();
            manageCustomPage.EnterCustomGrowthItemTypeName("", customGrowthItemTypeText);

            Log.Info($"Click on 'Cancel' button then clicking on 'Cancel' button on confirmation pop up. Verify that added item should be present there.");
            manageCustomPage.ClickOnCancelButton();
            manageCustomPage.ConfirmationCancelPopupClickOnCancelButton();
            Assert.IsTrue(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is not displayed.");

            Log.Info("Click on 'Save' button and update existing custom growth item type.");
            manageCustomPage.ClickOnSaveButton();
            var updatedCustomGrowthItemType = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            var updatedCustomGrowthItemTypeText = updatedCustomGrowthItemType.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();
            manageCustomPage.EnterCustomGrowthItemTypeName(customGrowthItemTypeText, updatedCustomGrowthItemTypeText);

            Log.Info($"Verify that updated custom growth item type {updatedCustomGrowthItemTypeText} is saved when user clicks on 'Cancel' button from cancel pop up");
            manageCustomPage.ClickOnCancelButton();
            manageCustomPage.ConfirmationCancelPopupClickOnCancelButton();
            Assert.IsTrue(manageCustomPage.IsCustomGrowthItemTypeDisplayed(updatedCustomGrowthItemTypeText), $"Custom growth item type <{updatedCustomGrowthItemTypeText}> is not displayed.");
            Assert.IsFalse(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is displayed.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_ConfirmCancelPopup_DiscardChanges()
        {
            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();

            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            var customGrowthItemTypeText = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();

            Log.Info($"Adding custom field- {customGrowthItemTypeText}  in the custom growth item type list ,and not clicking on 'Save' button");
            manageCustomPage.ClickOnAddCustomGrowthItemTypeButton();
            manageCustomPage.EnterCustomGrowthItemTypeName("", customGrowthItemTypeText);

            Log.Info("Click on 'Cancel' button then clicking on 'Discard Changes' button on confirmation. Verify that added item should not be present there.");
            manageCustomPage.ClickOnCancelButton();
            manageCustomPage.ConfirmationCancelPopupClickOnDiscardChangesButton();
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();
            Assert.IsFalse(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is displayed.");

            Log.Info($"Adding custom field- {customGrowthItemTypeText} in the custom growth item type list");
            manageCustomPage.ClickOnAddCustomGrowthItemTypeButton();
            manageCustomPage.EnterCustomGrowthItemTypeName("", customGrowthItemTypeText);
            manageCustomPage.ClickOnSaveButton();

            Log.Info("Update existing custom growth item type ,and not clicking on 'Save' button");
            var updatedCustomGrowthItemType = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            var updatedCustomGrowthItemTypeText = updatedCustomGrowthItemType.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();
            manageCustomPage.EnterCustomGrowthItemTypeName(customGrowthItemTypeText, updatedCustomGrowthItemTypeText);
            
            Log.Info("Click on 'Cancel' button then clicking on 'Discard Changes' button on confirmation.");
            manageCustomPage.ClickOnCancelButton();
            manageCustomPage.ConfirmationCancelPopupClickOnDiscardChangesButton();
            Assert.IsTrue(customGrowthPlanPage.IsManageGrowthItemTypesButtonPresent(), "Custom growth item type changes are not discarded");

            Log.Info($"Verify that custom growth item type {customGrowthItemTypeText} is present and {updatedCustomGrowthItemTypeText} is not present");
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();
            Assert.IsTrue(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is not displayed.");
            Assert.IsFalse(manageCustomPage.IsCustomGrowthItemTypeDisplayed(updatedCustomGrowthItemTypeText), $"Custom growth item type <{updatedCustomGrowthItemTypeText}> is displayed.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_ConfirmCancelPopup_SaveChanges()
        {
            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();

            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            var customGrowthItemTypeText = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();

            Log.Info($"Adding custom field- {customGrowthItemTypeText}  in the custom growth item type list ,and not clicking on 'Save' button");
            manageCustomPage.ClickOnAddCustomGrowthItemTypeButton();
            manageCustomPage.EnterCustomGrowthItemTypeName("", customGrowthItemTypeText);

            Log.Info("Click on 'Cancel' button then clicking on 'Save Changes' button on confirmation. Verify that added item should be present there.");
            manageCustomPage.ClickOnCancelButton();
            manageCustomPage.ConfirmationCancelPopupClickOnSaveChangesButton();
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();
            Assert.IsTrue(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is not displayed.");

            Log.Info("Update existing custom growth item type and not clicking on 'Save' button");
            var updatedCustomGrowthItemType = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            var updatedCustomGrowthItemTypeText = updatedCustomGrowthItemType.CustomGrowthPlanTypes.Select(a => a.CustomText).FirstOrDefault();
            manageCustomPage.EnterCustomGrowthItemTypeName(customGrowthItemTypeText, updatedCustomGrowthItemTypeText);

            Log.Info("Click on 'Cancel' button then clicking on 'Save Changes' button on confirmation.");
            manageCustomPage.ClickOnCancelButton();
            manageCustomPage.ConfirmationCancelPopupClickOnSaveChangesButton();
            Assert.IsTrue(customGrowthPlanPage.IsManageGrowthItemTypesButtonPresent(), "Custom growth item type changes are not saved");

            Log.Info($"Verify that custom growth item type {updatedCustomGrowthItemTypeText} is present and {customGrowthItemTypeText} is not present");
            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();
            Assert.IsTrue(manageCustomPage.IsCustomGrowthItemTypeDisplayed(updatedCustomGrowthItemTypeText), $"Custom growth item type <{updatedCustomGrowthItemTypeText}> is not displayed.");
            Assert.IsFalse(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is displayed.");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (User.Type != UserType.SiteAdmin && User.Type != UserType.PartnerAdmin) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyResponse.Name).GetAwaiter().GetResult();
        }
    }
}