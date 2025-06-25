using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.EnterpriseTeam
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
    public class GrowthItemEnterpriseTeamMemberAccessTests : BaseTest
    {
        private static bool _classInitFailed;
        private static int _enterpriseTeamId;
        private static RadarResponse _radar;
        private static TeamResponse _multiTeamResponse, _enterpriseTeamResponse;
        private static AddTeamWithMemberRequest _enterpriseTeam;
        private static TeamHierarchyResponse _multiTeam;
        private static readonly GrowthItem GrowthItemTeamInfo = GrowthPlanFactory.GetValidGrowthItem();
        private const string Password = SharedConstants.CommonPassword;
        private static User MemberUser => TestEnvironment.UserConfig.GetUserByDescription("member");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                //Get 'Automation Multi Team'
                _multiTeam = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeam");

                var firstName = $"{RandomDataUtil.GetFirstName()}ETMem";
                //Add new member in enterprise team
                _enterpriseTeam.Members.Add(new AddMemberRequest
                {
                    FirstName = "New_EnterpriseMember",
                    LastName = MemberUser.LastName,
                    Email = "ah_automation+" + CSharpHelpers.RandomNumber() + "@agiletransformation.com"
                });

                //Create new enterprise team
                _enterpriseTeamResponse = setup.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();
                _multiTeamResponse = setup.GetTeamResponse(_multiTeam.Name);

                //Add sub multi team and get enterpriseTeam id and radar id  
                setup.AddSubteams(_enterpriseTeamResponse.Uid, new List<Guid> { _multiTeamResponse.Uid }).GetAwaiter().GetResult();
                _enterpriseTeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_enterpriseTeamResponse.Name).TeamId;
                _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);

                //Give access to the newly created member
                setupUi.TeamMemberAccessAtTeamLevel(_enterpriseTeamId, (_enterpriseTeamResponse.Members.First().Email));

                //Add growth item 
                GrowthItemTeamInfo.Owner = null;
                GrowthItemTeamInfo.Category = null;
                setupUi.AddGiForETeam(_enterpriseTeamId, GrowthItemTeamInfo);

                //create account for member and set password
                setupUi.SetUserPassword(_enterpriseTeamResponse.Members.First().Email, Password, "Inbox");
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        public void GrowthItem_ET_Member_GridView_AddEditDeleteCopy_Buttons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Log.Info("Login as enterprise member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_enterpriseTeamResponse.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, verify that growth item is present in gridView and 'Add/Edit/Delete/Copy' buttons are displayed");
            radarPage.NavigateToPage(_enterpriseTeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.SwitchToGridView();

            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemTeamInfo.Title), $"Growth Item {GrowthItemTeamInfo.Title} is not present");
            Assert.IsTrue(growthItemGridView.IsAddNewGrowthItemButtonDisplayed(), "'Add New Growth Item' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemTeamInfo.Title), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsCopyGrowthItemButtonDisplayed(GrowthItemTeamInfo.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsDeleteGrowthItemButtonDisplayed(GrowthItemTeamInfo.Title), "'Delete' button is not displayed");

        }

        [TestMethod]
        public void GrowthItem_ET_Member_GridView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as enterprise member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_enterpriseTeamResponse.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            radarPage.NavigateToPage(_enterpriseTeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth Item {GrowthItemTeamInfo.Title} is not added successfully");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not present");

            Log.Info($"Delete {growthEditedItemInfo.Title} GI and verify");
            growthItemGridView.DeleteGrowthItem(growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is not deleted successfully");

        }

        [TestMethod]
        public void GrowthItem_ET_Member_GridView_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as enterprise member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_enterpriseTeamResponse.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and create a new GI");
            radarPage.NavigateToPage(_enterpriseTeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"Copy {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickCopyGrowthItemButton(growthItemInfo.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} isn't copied successfully");
        }

        [TestMethod]
        [TestCategory("Critical")]
        public void GrowthItem_ET_Member_KanbanView_AddEditDelete_Buttons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Log.Info("Login as enterprise member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_enterpriseTeamResponse.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and verify that growth item is present in kanban View");
            radarPage.NavigateToPage(_enterpriseTeamId, _radar.Id, TeamType.EnterpriseTeam);

            growthItemKanbanView.SwitchToKanbanView();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(GrowthItemTeamInfo.Status, 1, GrowthItemTeamInfo.Title), "Growth Item is not exist");
            Assert.IsTrue(growthItemKanbanView.DoesAddNewGrowthItemButtonDisplayed(), "'Add New Growth Item' button is not Displayed");
            Assert.IsTrue(growthItemKanbanView.DoesEditButtonDisplayed(GrowthItemTeamInfo.Title, GrowthItemTeamInfo.Status), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemKanbanView.DoesCopyButtonDisplayed(1, GrowthItemTeamInfo.Status, GrowthItemTeamInfo.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemKanbanView.DoesDeleteButtonDisplayed(1, GrowthItemTeamInfo.Status, GrowthItemTeamInfo.Title), "'Delete' button is not displayed");

        }

        [TestMethod]
        public void GrowthItem_ET_Member_KanbanView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as enterprise member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_enterpriseTeamResponse.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            radarPage.NavigateToPage(_enterpriseTeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be added successfully");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Status = "Done";
            growthEditedItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not exist");

            Log.Info($"'Delete'{growthEditedItemInfo.Title} GI and Verify ");
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} should be deleted successfully");
        }

        [TestMethod]
        public void GrowthItem_ET_Member_KanbanView_DragDrop_And_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as enterprise member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_enterpriseTeamResponse.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI");
            radarPage.NavigateToPage(_enterpriseTeamId, _radar.Id, TeamType.EnterpriseTeam);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Category = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be added successfully");

            Log.Info($" drag {growthItemInfo.Title} GI and drop in 'Done' status");
            growthItemKanbanView.DragDropGi(1, growthItemInfo.Status, growthItemInfo.Title, "Done");
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be dragged and dropped successfully");

            Log.Info($"Copy {growthItemInfo.Title}GI and verify");
            growthItemKanbanView.ClickKanbanGiCopyButton("Done", 1, growthItemInfo.Title);
            Assert.AreEqual(2, growthItemKanbanView.GetGiCount("Done", growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be copied successfully");
        }
    }
}