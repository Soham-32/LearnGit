using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
    public class GrowthItemTeamMemberAccessTests : BaseTest
    {
        private static bool _classInitFailed;
        private static int _teamId;
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamResponse _createTeamResponseForTeam;
        private static AddTeamWithMemberRequest _team;
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

                //Create team
                _team = TeamFactory.GetNormalTeam("Team");

                //Add new member
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = RandomDataUtil.GetFirstName(),
                    LastName = MemberUser.LastName,
                    Email = "ah_automation+" + _team.Members.FirstOrDefault().FirstName + "@agiletransformation.com"
                });

                _createTeamResponseForTeam = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_createTeamResponseForTeam.Name).TeamId;

                // Create a team assessment and add growth item for the team
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    TeamMembers = _team.Members.Select(a => a.FullName()).ToList()
                };

                //Give access to the newly created member
                setupUi.TeamMemberAccessAtTeamLevel(_teamId, (_createTeamResponseForTeam.Members.First().Email));

                //Add Growth Item 
                GrowthItemTeamInfo.Owner = null;
                setupUi.AddTeamAssessmentAndGi(_teamId, _teamAssessment, new List<GrowthItem> { GrowthItemTeamInfo });

                //Give access to sharing assessment to the member
                setupUi.StartSharingAssessment(_teamId, _teamAssessment.AssessmentName);

                //Create account for member and set password
                setupUi.SetUserPassword(_createTeamResponseForTeam.Members.First().Email, Password, "Inbox");
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51053
        [TestCategory("Critical")]
        public void GrowthItem_Team_Member_GridView_AddEditDeleteCopy_Buttons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Log.Info("Login as team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createTeamResponseForTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and verify that growth item is present in gridView and verify 'Add/Edit/Delete/Copy' buttons are displayed");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemTeamInfo.Title), $"Growth Item {GrowthItemTeamInfo.Title} is not present");
            Assert.IsTrue(growthItemGridView.IsAddNewGrowthItemButtonDisplayed(), "'Add New Growth Item' Button is not displayed");
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemTeamInfo.Title), "'Edit' Button is not displayed");
            Assert.IsTrue(growthItemGridView.IsCopyGrowthItemButtonDisplayed(GrowthItemTeamInfo.Title), "'Copy' Button is not displayed");
            Assert.IsTrue(growthItemGridView.IsDeleteGrowthItemButtonDisplayed(GrowthItemTeamInfo.Title), "'Delete' Button is not displayed");
        }

        [TestMethod]
        public void GrowthItem_Team_Member_GridView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Log.Info("Login as team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createTeamResponseForTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} is not added successfully");

            Log.Info($"'Edit' {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            growthEditedItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not present");

            Log.Info($"'Delete' {growthEditedItemInfo.Title} and verify");
            growthItemGridView.DeleteGrowthItem(growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is not deleted successfully");

        }
        
        [TestMethod]
        public void GrowthItem_Team_Member_GridView_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Log.Info("Login as team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createTeamResponseForTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and create a new GI");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            growthItemGridView.SwitchToGridView();
            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info($"Copy {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickCopyGrowthItemButton(growthItemInfo.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} isn't copied successfully");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51053
        [TestCategory("Critical")]
        public void GrowthItem_Team_Member_KanbanView_AddEditDelete_Buttons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);

            Log.Info("Login as team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createTeamResponseForTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page and verify that growth item is present in kanban and 'Add/Edit/Delete/Copy' buttons are displayed");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            growthItemKanbanView.SwitchToKanbanView();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(GrowthItemTeamInfo.Status, 1, GrowthItemTeamInfo.Title), "Growth Item is not exist");
            Assert.IsTrue(growthItemKanbanView.DoesAddNewGrowthItemButtonDisplayed(), "'Add New Growth Item' button is not Displayed");
            Assert.IsTrue(growthItemKanbanView.DoesEditButtonDisplayed(GrowthItemTeamInfo.Title, GrowthItemTeamInfo.Status), "'Edit' Button is not displayed");
            Assert.IsTrue(growthItemKanbanView.DoesCopyButtonDisplayed(1, GrowthItemTeamInfo.Status, GrowthItemTeamInfo.Title), "'Copy' Button is not displayed");
            Assert.IsTrue(growthItemKanbanView.DoesDeleteButtonDisplayed(1, GrowthItemTeamInfo.Status, GrowthItemTeamInfo.Title), "'Delete' Button is not displayed");
        }

        [TestMethod]
        public void GrowthItem_Team_Member_KanbanView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as  team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createTeamResponseForTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be added successfully");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Status = "Done";
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not exist");

            Log.Info($"'Delete'{growthEditedItemInfo.Title} GI and Verify ");
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} should be deleted successfully");

            // Below method is used to clean up all the GIs
            growthItemKanbanView.DeleteAllKanbanGi();
        }

        [TestMethod]
        public void GrowthItem_Team_Member_KanbanView_DragDrop_And_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Take Login as team member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_createTeamResponseForTeam.Members.First().Email, Password);

            Log.Info("Go to 'Radar' page, Add new GI");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), "Growth Item should be added successfully");

            Log.Info($" drag {growthItemInfo.Title} GI and drop in 'Done' status");
            growthItemKanbanView.DragDropGi(1, growthItemInfo.Status, growthItemInfo.Title, "Done");
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} isn't dragged and dropped successfully");

            Log.Info($"Copy {growthItemInfo.Title}GI and verify");
            growthItemKanbanView.ClickKanbanGiCopyButton("Done", 1, growthItemInfo.Title);
            Assert.AreEqual(2, growthItemKanbanView.GetGiCount("Done", growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be copied successfully");
        }
    }
}