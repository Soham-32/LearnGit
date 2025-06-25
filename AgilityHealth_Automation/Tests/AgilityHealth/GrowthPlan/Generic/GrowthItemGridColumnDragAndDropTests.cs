using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.Generic
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("CompanyAdmin")]
    public class GrowthItemGridColumnDragAndDropTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamHierarchyResponse _team;
        private readonly GrowthItem GrowthItemInfo1 = GrowthPlanFactory.GetValidGrowthItem();
        private readonly GrowthItem GrowthEditedItemInfo1 = GrowthPlanFactory.GetValidUpdatedGrowthItem();


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                // Create Team
                _team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);

                // Create a Team assessment and Add Growth Item for the team
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Team_Assessment{Guid.NewGuid()}",
                    TeamMembers = new List<string>
                        { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() }
                };

                //Add Growth Item 
                setupUi.AddTeamAssessment(_team.TeamId, _teamAssessment);
            }

            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        public void GrowthItem_TeamAssessment_Radar_Gi_Grid_DragAndDropColumn_AddEditCopyDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var giGridViewPage = new GrowthItemGridView(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and navigate to the team assessment radar page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            Log.Info("Drag a 'Description' column and drop to 'group by that column section' and verify the 'Add New Item' button should be displayed");
            giGridViewPage.DragAndDropColumn("Description", 0, -10, true);
            Assert.IsTrue(growthItemGridView.IsAddNewGrowthItemButtonDisplayed(), "'Add New Item' button is not displayed");

            Log.Info("Add new growth item and verify that Gi should be added");
            growthItemGridView.ClickAddNewGrowthItem();
            GrowthItemInfo1.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthItemInfo1);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemInfo1.Title), $"Growth item {GrowthItemInfo1.Title} is not added");

            Log.Info("Verify that 'Edit', 'Copy', 'Delete' buttons should be displayed");
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsCopyGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsDeleteGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Delete' button is not displayed");

            Log.Info($"Edit {GrowthItemInfo1.Title} and verify that Gi should be edited");
            growthItemGridView.ClickGrowthItemEditButton(GrowthItemInfo1.Title);
            GrowthEditedItemInfo1.Owner = null;
            GrowthEditedItemInfo1.Category = "Organizational";
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthEditedItemInfo1);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthEditedItemInfo1.Title), $"Growth Item {GrowthEditedItemInfo1.Title} is not edited");

            /*bug id : 35461
            Log.Info($"Copy {GrowthEditedItemInfo1.Title} Gi and verify that Gi should be copied");
            growthItemGridView.ClickCopyGrowthItemButton(GrowthEditedItemInfo1.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(GrowthEditedItemInfo1.Title), $"Growth item {GrowthEditedItemInfo1.Title} is not copied");
            */

            Log.Info($"Delete {GrowthEditedItemInfo1.Title} and verify that Gi should be deleted");
            growthItemGridView.DeleteAllGIs();
            Assert.IsFalse(growthItemGridView.IsGiPresent(GrowthEditedItemInfo1.Title), $"Growth item {GrowthEditedItemInfo1.Title} is not deleted");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug: 35982
        public void GrowthItem_Tab_Gi_Grid_DragAndDropColumn_AddEditCopyDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemPage = new GrowthItemsPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var giGridViewPage = new GrowthItemGridView(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
           
            Log.Info($"Login as {User.FullName} and navigate to the growth item tab");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            growthItemPage.NavigateToPage(Company.Id, _team.TeamId);
            growthItemGridView.SwitchToGridView();

            Log.Info("Drag a 'Description' column and drop to 'group by that column section' and verify the 'Add New Item' button should be displayed");
            giGridViewPage.DragAndDropColumn("Description", 0, -10);
            Assert.IsTrue(growthItemGridView.IsAddNewGrowthItemButtonDisplayed(), "'Add New Item' button is not displayed");

            Log.Info("Add new growth item and verify that Gi should be added");
            growthItemPage.ClickAddNewItemButton();
            GrowthItemInfo1.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthItemInfo1);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemPage.DoesGiExist(GrowthItemInfo1.Title), $"Growth item {GrowthItemInfo1.Title} is not added");

            Log.Info("Verify that 'Edit', 'Copy', 'Delete' buttons should be displayed");
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsCopyGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsDeleteGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Delete' button is not displayed");

            Log.Info($"Edit the {GrowthItemInfo1.Title} and verify that Gi should be edited");
            growthItemPage.ClickGrowthItemEditButton(GrowthItemInfo1.Title);
            GrowthEditedItemInfo1.Owner = null;
            GrowthEditedItemInfo1.Category = "Organizational";
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthEditedItemInfo1);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemPage.DoesGiExist(GrowthEditedItemInfo1.Title), $"Growth Item {GrowthEditedItemInfo1.Title} is not edited");

            /*bug id : 35461
            Log.Info($"Copy {GrowthEditedItemInfo1.Title} GI and verify that Gi should be copied");
            growthItemPage.ClickCopyGrowthItemButton(GrowthEditedItemInfo1.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is not copied");*/

            Log.Info($"Delete {GrowthEditedItemInfo1.Title} and verify that Gi should be deleted");
            growthItemPage.DeleteGrowthItem(GrowthEditedItemInfo1.Title);
            //growthItemPage.DeleteGrowthItem(GrowthEditedItemInfo1.Title);
            Assert.IsFalse(growthItemPage.DoesGiExist(GrowthEditedItemInfo1.Title), $"Growth item {GrowthEditedItemInfo1.Title} is not deleted");
        }

        [TestMethod]
        public void GrowthItem_Dashboard_Gi_Grid_DragAndDropColumn_Edit()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var giGridViewPage = new GrowthItemGridView(Driver, Log);
            var growthItemDashboardPage = new GrowthItemsDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemPage = new GrowthItemsPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and navigate to the team assessment radar page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            Log.Info("Add new growth item and verify that Gi should be added");
            growthItemGridView.ClickAddNewGrowthItem();
            GrowthItemInfo1.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthItemInfo1);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(GrowthItemInfo1.Title), $"Growth item {GrowthItemInfo1.Title} is not added");

            Log.Info("Go to the growth item dashboard page");
            growthItemDashboardPage.NavigateToPage(Company.Id);
            growthItemGridView.SwitchToGridView();

            Log.Info("Drag a 'Description' column and drop to 'group by that column section' and verify the 'Edit' button should be displayed");
            giGridViewPage.DragAndDropColumn("Description", 0, -10);
            Assert.IsTrue(growthItemDashboardPage.IsEditGrowthItemButtonDisplayed(GrowthItemInfo1.Title), "'Edit' button is not displayed");

            Log.Info($"Edit the {GrowthItemInfo1.Title} and verify that Gi should be edited");
            growthItemDashboardPage.ClickOnEditGrowthItem(GrowthItemInfo1.Title);
            GrowthEditedItemInfo1.Owner = null;
            GrowthEditedItemInfo1.Category = "Organizational";
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthEditedItemInfo1);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemPage.DoesGiExist(GrowthEditedItemInfo1.Title), $"Growth Item {GrowthEditedItemInfo1.Title} is not edited");
        }
    }
}