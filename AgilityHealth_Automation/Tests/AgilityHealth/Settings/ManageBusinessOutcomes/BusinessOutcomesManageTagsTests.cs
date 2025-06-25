using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes;
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
    public class BusinessOutcomesManageTagsTests : BaseTest
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
        public void BusinessOutcomes_Settings_Tags_Label_Add()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var newLabelName = RandomDataUtil.GetBusinessOutcomeSourceCategoryName();
            manageBoTagsPage.AddNewLabel(newLabelName, "Text");
            manageBoTagsPage.ClickOnSaveButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            var actualLabels = manageBoTagsPage.GetAllLabelNames();
            Assert.IsTrue(actualLabels.Any(l => l == newLabelName), 
                $"New Label <{newLabelName}> is not displayed.");
            Assert.IsTrue(actualLabels.Count(l => l == newLabelName) == 1, 
                $"There are duplicate labels created for <{newLabelName}>.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Label_Edit()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id,1);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var updatedName = $"Updated_{RandomDataUtil.GetBusinessOutcomeSourceCategoryName()}";
            manageBoTagsPage.EnterLabelName(newLabel.Name, updatedName);
            if (_user.IsSiteAdmin())
            {
                manageBoTagsPage.ToggleKanbanMode(updatedName, true); 
            }
            manageBoTagsPage.ClickOnSaveButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsTrue(manageBoTagsPage.IsLabelDisplayed(updatedName), 
                $"Updated Label <{updatedName}> is not displayed.");
            if (_user.IsSiteAdmin())
            {
                Assert.IsTrue(manageBoTagsPage.IsKanbanModeOn(updatedName),
                        $"Kanban toggle is not on for label <{updatedName}>."); 
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Label_Delete()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            manageBoTagsPage.ClickLabelDeleteButton(newLabel.Name);
            manageBoTagsPage.ClickOnDeleteLabelPopupDeleteLabelButton();
            manageBoTagsPage.ClickOnSaveButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsFalse(manageBoTagsPage.IsLabelDisplayed(newLabel.Name), 
                $"Label <{newLabel.Name}> is not deleted.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Label_Delete_Cancel()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            manageBoTagsPage.ClickLabelDeleteButton(newLabel.Name);
            manageBoTagsPage.ClickOnDeleteLabelCancelButton();
            manageBoTagsPage.ClickOnCancelButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsTrue(manageBoTagsPage.IsLabelDisplayed(newLabel.Name), 
                $"Label <{newLabel.Name}> is deleted after clicking cancel.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Tag_Add()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var newTag = BusinessOutcomesFactory.GetBusinessOutcomeTagRequest();
            manageBoTagsPage.ClickOnAddBusinessOutcomeTagButton(newLabel.Name);
            manageBoTagsPage.EnterTagName(newLabel.Name, "", newTag.Name);
            manageBoTagsPage.ClickOnSaveButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsTrue(manageBoTagsPage.IsTagDisplayed(newLabel.Name, newTag.Name), 
                $"Could not find tag <{newTag.Name}> under label <{newLabel.Name}>.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Tag_Edit()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id, 1);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var editedTag = BusinessOutcomesFactory.GetBusinessOutcomeTagRequest();
            manageBoTagsPage.EnterTagName(newLabel.Name, newLabel.Tags[0].Name, editedTag.Name);
            manageBoTagsPage.ClickOnSaveButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsTrue(manageBoTagsPage.IsTagDisplayed(newLabel.Name, editedTag.Name), 
                $"Could not find tag <{editedTag.Name}> under label <{newLabel.Name}>.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Tag_Delete()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id, 1);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            manageBoTagsPage.ClickOnDeleteTagButton(newLabel.Name, newLabel.Tags[0].Name);

            manageBoTagsPage.ClickOnSaveButton();

            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsFalse(manageBoTagsPage.IsTagDisplayed(newLabel.Name, newLabel.Tags[0].Name), 
                $"Tag <{newLabel.Tags[0].Name}> still exists under label <{newLabel.Name}> after delete.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_Tag_Reorder()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            // add a new label
            var newLabel = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(_settingsCompany.Id, 2);
            AddNewLabel(newLabel);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            manageBoTagsPage.DragTagToTag(newLabel.Name, 
                newLabel.Tags[1].Name, newLabel.Tags[0].Name);

            manageBoTagsPage.ClickOnSaveButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            newLabel.Tags.Reverse();
            var actualTags = manageBoTagsPage.GetTagNames(newLabel.Name);
            for (var i = 0; i < newLabel.Tags.Count; i++)
            {
                Assert.AreEqual(newLabel.Tags[i].Name, actualTags[i], $"The Tag at index <{i}> doesn't match.");
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_ConfirmPopup_Cancel()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var newLabelName = RandomDataUtil.GetBusinessOutcomeSourceCategoryName();
            manageBoTagsPage.AddNewLabel(newLabelName, "Text");
            manageBoTagsPage.ClickOnCancelButton();

            manageBoTagsPage.ConfirmPopUpClickOnCancelButton();
            Assert.IsTrue(manageBoTagsPage.IsLabelDisplayed(newLabelName), $"New label <{newLabelName}> is not displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_ConfirmPopup_DiscardChanges()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var newLabelName = RandomDataUtil.GetBusinessOutcomeSourceCategoryName();
            manageBoTagsPage.AddNewLabel(newLabelName, "Text");
            manageBoTagsPage.ClickOnCancelButton();

            manageBoTagsPage.ConfirmPopUpClickOnDiscardChangesButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsFalse(manageBoTagsPage.IsLabelDisplayed(newLabelName), $"Discarded new label <{newLabelName}> is displayed.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Tags_ConfirmPopup_SaveChanges()
        {
            VerifySetup(_classInitFailed, _classInitFailedMessage);

            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var manageBoTagsPage = new ManageBusinessOutcomeTagsPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(_user.Username, _user.Password);

            v2SettingsPage.NavigateToPage(_settingsCompany.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();

            var newLabelName = RandomDataUtil.GetBusinessOutcomeSourceCategoryName();
            manageBoTagsPage.AddNewLabel(newLabelName, "Text");
            manageBoTagsPage.ClickOnCancelButton();

            manageBoTagsPage.ConfirmPopUpClickOnSaveChangesButton();
            manageBoTagsPage.ClickOnManageBusinessOutcomesTagsButton();
            Assert.IsTrue(manageBoTagsPage.IsLabelDisplayed(newLabelName), $"New label <{newLabelName}> is not displayed.");
        }

        private static void AddNewLabel(BusinessOutcomeCategoryLabelRequest request)
        {
            new SetupTeardownApi(TestEnvironment)
                .AddBusinessOutcomesLabels(new List<BusinessOutcomeCategoryLabelRequest> {request}, _user);
        }
    }
}