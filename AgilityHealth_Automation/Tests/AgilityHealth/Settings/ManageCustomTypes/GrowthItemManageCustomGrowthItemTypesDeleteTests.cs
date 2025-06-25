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
    public class GrowthItemManageCustomGrowthItemTypesDeleteTests : BaseTest
    {
        private static SetupTeardownApi _setup;
        private static CompanyResponse _companyResponse;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            var companyRequest = CompanyFactory.GetValidPostCompany();
            _setup = new SetupTeardownApi(TestEnvironment);
            _companyResponse = _setup.CreateCompany(companyRequest).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_DeleteAllCustomTypes_And_ValidateInformationIcon()
        {
            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id, 3);
            _setup.CreateGrowthItemCustomType(newCustomGrowthItemTypeRequest);
            var customGrowthItemTexts =
                newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).ToList();

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);
            const string expectedCustomGrowthItemTypeInformationText = "Use this page to customize the available Growth Item Types used in Growth Items for your company. You can add new types to the default list and/or delete unnecessary types. All changes made on this page by any Company Admin takes effect immediately if the Custom Growth Item Type Feature is enabled in Manage Features.";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();

            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();

            manageCustomPage.ClickOnDeleteAllCustomGrowthItemTypesButton();
            manageCustomPage.ConfirmationDeletePopupClickOnDeleteButton();
            manageCustomPage.WaitUntilCustomGrowthItemTypeListIsLoaded();

            var actualCustomGrowthItemTypeList = manageCustomPage.GetAllCustomGrowthItemTypeList();
            var expectedDefaultGrowthItemTypeList = GrowthPlanFactory.GetNewGrowthPlanTypes();
            Assert.That.ListsAreEqual(expectedDefaultGrowthItemTypeList, actualCustomGrowthItemTypeList, "Custom growth item type list is not matched");
            foreach (var customText in customGrowthItemTexts)
            {
                Assert.That.ListNotContains(actualCustomGrowthItemTypeList, customText, $"Custom growth item type <{customText}> is present");
            }

            Log.Info("Hover on information icon Verify the information text");
            Assert.AreEqual(expectedCustomGrowthItemTypeInformationText, manageCustomPage.GetCustomGrowthItemTypeInformationText(), "Information text is not matched");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_Delete()
        {
            var newCustomGrowthItemTypeRequest = GrowthPlanFactory.CustomTypesCreateRequest(_companyResponse.Id);
            _setup.CreateGrowthItemCustomType(newCustomGrowthItemTypeRequest);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageCustomPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanPage = new CustomGrowthPlanSettingsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnCustomGrowthItemManageSettingsButton();

            customGrowthPlanPage.ClickOnManageGrowthItemTypesButton();
            var customGrowthItemTypeText = newCustomGrowthItemTypeRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).ToList().ListToString();

            manageCustomPage.ClickOnDeleteCustomGrowthItemTypeButton(customGrowthItemTypeText);
            manageCustomPage.ConfirmationDeletePopupClickOnDeleteButton();

            manageCustomPage.ClickOnSaveButton();

            Assert.IsFalse(manageCustomPage.IsCustomGrowthItemTypeDisplayed(customGrowthItemTypeText), $"Custom growth item type <{customGrowthItemTypeText}> is displayed.");
        }
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CustomGrowthItemTypes_Delete_Warning_Message_LastItemDelete()
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

            var customGrowthItemTypeList = manageCustomPage.GetAllCustomGrowthItemTypeList();
            for (var i = 1; i < customGrowthItemTypeList.Count(); i++)
            {
                manageCustomPage.ClickOnDeleteCustomGrowthItemTypeButton(customGrowthItemTypeList[i]);
                manageCustomPage.ConfirmationDeletePopupClickOnDeleteButton();
            }

            Log.Info("Deleting last custom growth item type and validating validation message");
            var lastCustomGrowthItemType = manageCustomPage.GetAllCustomGrowthItemTypeList();
            manageCustomPage.ClickOnDeleteCustomGrowthItemTypeButton(lastCustomGrowthItemType.First());

            var actualCustomGrowthItemTypeDeletePopupWarningMessage = manageCustomPage.GetConfirmationPopupDeleteWarningMessage().Replace("\r\n", " ");
            const string expectedCustomGrowthItemTypeDeletePopupWarningMessage = "The Growth Item Type List cannot be blank. Deleting this Growth Item Type will cause the Growth Item Type List to revert to the Default List. Are you sure you would like to do this?";
            Assert.AreEqual(expectedCustomGrowthItemTypeDeletePopupWarningMessage, actualCustomGrowthItemTypeDeletePopupWarningMessage, "Confirm delete popup warning message doesn't match");

            manageCustomPage.ConfirmationDeletePopupClickOnDeleteButton();
            manageCustomPage.WaitUntilCustomGrowthItemTypeListIsLoaded();
            var expectedDefaultCustomGrowthItemTypeList = manageCustomPage.GetAllCustomGrowthItemTypeList();
            var actualDefaultCustomGrowthItemTypeList = GrowthPlanFactory.GetNewGrowthPlanTypes();
            Assert.That.ListsAreEqual(expectedDefaultCustomGrowthItemTypeList, actualDefaultCustomGrowthItemTypeList, "Default custom growth item type list doesn't match");
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