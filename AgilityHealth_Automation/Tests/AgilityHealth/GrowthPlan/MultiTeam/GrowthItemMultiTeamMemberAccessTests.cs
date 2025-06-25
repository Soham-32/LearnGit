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
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.MultiTeam
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
    public class GrowthItemMultiTeamMemberAccessTests : BaseTest
    {
        private static bool _classInitFailed;
        private static int _teamId;
        private static int _multiTeamId;
        private static TeamAssessmentInfo _teamAssessment;
        private static RadarResponse _radar;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _createTeamResponseForTeam, _createResponseForMultiTeam;
        private static AddTeamWithMemberRequest _multiTeam;
        private static readonly GrowthItem GrowthItemMultiTeamInfo = GrowthPlanFactory.GetValidGrowthItem();
        private const string Password = SharedConstants.CommonPassword;
        private static User MemberUser => TestEnvironment.UserConfig.GetUserByDescription("member");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                // Get team details
                _team = TeamFactory.GetNormalTeam("Team");

                // Add member
                _team.Members.Add(SharedConstants.TeamMember1);

                // Create a team and get radar details
                _createTeamResponseForTeam = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_createTeamResponseForTeam.Name).TeamId;
                _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);

                // Create a team assessment
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Team_Assessment{Guid.NewGuid()}",
                    TeamMembers = _team.Members.Select(a => a.FullName()).ToList()
                };
                setupUi.AddTeamAssessment(_teamId, _teamAssessment);

                // Get multi-team details
                _multiTeam = TeamFactory.GetMultiTeam("MTSubTeam");

                var firstName = RandomDataUtil.GetFirstName();
                // Add new multi member
                _multiTeam.Members.Add(new AddMemberRequest
                {
                    FirstName = "New_Member",
                    LastName = MemberUser.LastName,
                    Email = "ah_automation+" + CSharpHelpers.RandomNumber() + "@agiletransformation.com"
                });

                // Create multi team and get team id
                _createResponseForMultiTeam = setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                _multiTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_createResponseForMultiTeam.Name).TeamId;

                // Add sub team
                setup.AddSubteams(_createResponseForMultiTeam.Uid, new List<Guid> { _createTeamResponseForTeam.Uid }).GetAwaiter().GetResult();

                // Give member access 
                setupUi.TeamMemberAccessAtTeamLevel(_multiTeamId, (_createResponseForMultiTeam.Members.First().Email));

                // Add growth item 
                GrowthItemMultiTeamInfo.Owner = null;
                GrowthItemMultiTeamInfo.Type = "Other";
                setupUi.AddGiForMTeam(_multiTeamId, GrowthItemMultiTeamInfo);

                // Set password for the multi member
                setupUi.SetUserPassword(_createResponseForMultiTeam.Members.First().Email, Password, "Inbox");
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        public void GrowthItem_MT_Member_GridView_AddEditDeleteCopy_Buttons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Log.Info("Login as multi team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createResponseForMultiTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, verify that growth item is present in gridView and 'Add/Edit/Delete/Copy' buttons are displayed");
            radarPage.NavigateToPage(_multiTeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.SwitchToGridView();

            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemMultiTeamInfo.Title), $"Growth item {GrowthItemMultiTeamInfo.Title} is not present");
            Assert.IsTrue(growthItemGridView.IsAddNewGrowthItemButtonDisplayed(), "'Add New Growth Item' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemMultiTeamInfo.Title), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsCopyGrowthItemButtonDisplayed(GrowthItemMultiTeamInfo.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsDeleteGrowthItemButtonDisplayed(GrowthItemMultiTeamInfo.Title), "'Delete' button is not displayed");
        }

        [TestMethod]
        public void GrowthItem_MT_Member_GridView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as multi team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createResponseForMultiTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            radarPage.NavigateToPage(_multiTeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = "Other";
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} is not present");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Type = "Other";
            growthEditedItemInfo.Category = "Enterprise";
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not present");

            Log.Info($"Delete {growthEditedItemInfo.Title} GI and verify");
            growthItemGridView.DeleteGrowthItem(growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is still present");
        }

        [TestMethod]
        public void GrowthItem_MT_Member_GridView_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as multi team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createResponseForMultiTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and create a new GI");
            radarPage.NavigateToPage(_multiTeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"Copy {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickCopyGrowthItemButton(growthItemInfo.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} isn't copied");
        }

        [TestMethod]
        [TestCategory("Critical")]
        public void GrowthItem_MT_Member_KanbanView_AddEditDelete_Buttons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);

            Log.Info("Login as multi team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createResponseForMultiTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and verify that growth item is present in kanban and 'Add/Edit/Delete/Copy' buttons are displayed");
            radarPage.NavigateToPage(_multiTeamId, _radar.Id, TeamType.MultiTeam);
            growthItemKanbanView.SwitchToKanbanView();

            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(GrowthItemMultiTeamInfo.Status, 1, GrowthItemMultiTeamInfo.Title), $"Growth Item {GrowthItemMultiTeamInfo.Title} is not exist");
            Assert.IsTrue(growthItemKanbanView.DoesAddNewGrowthItemButtonDisplayed(), "'Add New Growth Item' button is not Displayed");
            Assert.IsTrue(growthItemKanbanView.DoesEditButtonDisplayed(GrowthItemMultiTeamInfo.Title, GrowthItemMultiTeamInfo.Status), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemKanbanView.DoesCopyButtonDisplayed(1, GrowthItemMultiTeamInfo.Status, GrowthItemMultiTeamInfo.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemKanbanView.DoesDeleteButtonDisplayed(1, GrowthItemMultiTeamInfo.Status, GrowthItemMultiTeamInfo.Title), "'Delete' button is not displayed");
        }

        [TestMethod]
        public void GrowthItem_MT_Member_KanbanView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as multi team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createResponseForMultiTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            radarPage.NavigateToPage(_multiTeamId, _radar.Id, TeamType.MultiTeam);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = "Other";
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} is not exist");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Type = "Other";
            growthEditedItemInfo.Category = "Enterprise";
            growthEditedItemInfo.Status = "Done";
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not exist");

            Log.Info($"'Delete'{growthEditedItemInfo.Title} GI and Verify ");
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is still exist");

            // Below method is used to clean up all the GIs
            growthItemKanbanView.DeleteAllKanbanGi();
        }

        [TestMethod]
        public void GrowthItem_MT_Member_KanbanView_DragDrop_And_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as multi team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createResponseForMultiTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI");
            radarPage.NavigateToPage(_multiTeamId, _radar.Id, TeamType.MultiTeam);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = "Other";
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($" drag {growthItemInfo.Title} GI and drop in 'Done' status");
            growthItemKanbanView.DragDropGi(1, growthItemInfo.Status, growthItemInfo.Title, "Done");
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} isn't dragged and dropped successfully");

            Log.Info($"Copy {growthItemInfo.Title}GI and verify");
            growthItemKanbanView.ClickKanbanGiCopyButton("Done", 1, growthItemInfo.Title);
            Assert.AreEqual(2, growthItemKanbanView.GetGiCount("Done", growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} isn't copied");

            // Below method is used to clean up all the GIs
            growthItemKanbanView.DeleteAllKanbanGi();
        }
    }
}