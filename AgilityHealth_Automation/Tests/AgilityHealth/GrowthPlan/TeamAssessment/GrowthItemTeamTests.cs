using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
    public class GrowthItemTeamTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamHierarchyResponse _team;
        private const string RadarType = "Radar Type";


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                // Create Team
                _team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);

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
        public void GrowthItem_Team_GridView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Radar Page");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            var actualGrowthItem = growthItemGridView.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} is not added successfully");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date,
                "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            Log.Info($"'Edit' {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Radar page");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(),"Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            growthEditedItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            var editedGrowthItem = growthItemGridView.GetGrowthItemFromGrid(growthEditedItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not present");
            Assert.AreEqual(growthEditedItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthEditedItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date,
                "TargetDate doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Description, editedGrowthItem.Description,
                "Description doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            Log.Info($"'Delete' {growthEditedItemInfo.Title} and verify");
            growthItemGridView.DeleteGrowthItem(growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is not deleted successfully");
        }

        [TestMethod]
        public void GrowthItem_Team_GridView_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Take Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page and verify that growth item is present in gridView");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} is not added successfully");

            Log.Info($"Copy {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickCopyGrowthItemButton(growthItemInfo.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthItemInfo.Title),
                $"Growth item {growthItemInfo.Title} isn't copied successfully");
            // Below method is used to clean up all the GIs
            growthItemGridView.DeleteAllGIs();
        }

        [TestMethod]
        public void GrowthItem_Team_KanbanView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} should be added successfully");
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Radar page 'Kanban view'");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            Assert.AreEqual(growthItemInfo.Title, addGrowthItemPopup.GetTitleValue(), "Title doesn't matched");
            Assert.AreEqual(growthItemInfo.Category, addGrowthItemPopup.GetCategoryValue(), "Category doesn't matched");
            Assert.AreEqual(growthItemInfo.Type, addGrowthItemPopup.GetTypeValue(), "Type doesn't matched");
            Assert.AreEqual(growthItemInfo.Status, addGrowthItemPopup.GetStatusValue(), "Status doesn't matched");
            Assert.AreEqual(growthItemInfo.Priority, addGrowthItemPopup.GetPriorityValue(), "Priority doesn't matched");
            Assert.AreEqual(growthItemInfo.Size, addGrowthItemPopup.GetSizeValue(), "Size doesn't matched");
            Assert.AreEqual(growthItemInfo.Description, addGrowthItemPopup.GetDescription(), "Description doesn't matched");
            Assert.AreEqual(growthItemInfo.Color, addGrowthItemPopup.GetColorValue(), "Color doesn't match");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            addGrowthItemPopup.ClickCancelButton();
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Status = "Done";
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title),
                $"Growth Item {growthEditedItemInfo.Title} is not exist");
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthEditedItemInfo.Title, growthEditedItemInfo.Status);
            Assert.AreEqual(growthEditedItemInfo.Title, addGrowthItemPopup.GetTitleValue(), "Title doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Category, addGrowthItemPopup.GetCategoryValue(),
                "Category doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Type, addGrowthItemPopup.GetTypeValue(), "Type doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Status, addGrowthItemPopup.GetStatusValue(), "Status doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Priority, addGrowthItemPopup.GetPriorityValue(),
                "Priority doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Size, addGrowthItemPopup.GetSizeValue(), "Size doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Description, addGrowthItemPopup.GetDescription(),
                "Description doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Color, addGrowthItemPopup.GetColorValue(), "Color doesn't match");

            Log.Info($"'Delete'{growthEditedItemInfo.Title} GI and Verify ");
            addGrowthItemPopup.ClickCancelButton();
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title),
                $"Growth Item {growthEditedItemInfo.Title} should be deleted successfully");

            // Below method is used to clean up all the GIs
            growthItemKanbanView.DeleteAllKanbanGi();
        }

        [TestMethod]
        public void GrowthItem_Team_KanbanView_DragDrop_And_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Take Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title),
                "Growth Item should be added successfully");

            Log.Info($" drag {growthItemInfo.Title} GI and drop in 'Done' status");
            growthItemKanbanView.DragDropGi(1, growthItemInfo.Status, growthItemInfo.Title, "Done");
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthItemInfo.Title),
                $"Growth Item {growthItemInfo.Title} isn't dragged and dropped successfully");

            Log.Info($"Copy {growthItemInfo.Title}GI and verify");
            growthItemKanbanView.ClickKanbanGiCopyButton("Done", 1, growthItemInfo.Title);
            Assert.AreEqual(2, growthItemKanbanView.GetGiCount("Done", growthItemInfo.Title),
                $"Growth Item {growthItemInfo.Title} should be copied successfully");
            // Below method is used to clean up all the GIs
            growthItemKanbanView.DeleteAllKanbanGi();

        }

        [TestMethod]
        public void GrowthItem_Team_ExportGrowthItemToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthItemInfo1 = GrowthPlanFactory.GetValidGrowthItem();

            var fileName = $"GrowthPlanFor{_team.Name}Assessment{_teamAssessment.AssessmentName}.xlsx";

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            radarPage.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();

            growthItemInfo.Category = "Team";
            growthItemInfo.TargetDate = null;
            growthItemInfo.Rank = "1";
            growthItemInfo.DateCreated = DateTime.Now;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo1.TargetDate = null;
            growthItemInfo1.Category = "Organizational";
            growthItemInfo1.Rank = "2";
            growthItemInfo1.DateCreated = DateTime.Now;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo1);
            addGrowthItemPopup.ClickSaveButton();

            var expectedColumn = new List<string>
            {
                "Id",
                "Rank",
                "Title",
                "Description",
                "Priority",
                "Owner",
                "Type",
                "Category",
                "Competency Target",
                "Status",
                "Target Date",
                "Size",
                "Solution",
            };
            growthItemGridView.AddColumns(expectedColumn);

            growthItemGridView.ClickExportToExcel();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);
            Assert.AreEqual(expectedColumn.Count, tbl.Columns.Count, "Column count doesn't match");

            var actualColumn = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedColumn[i], actualColumn[i], $"column name {i} value doesn't match");
            }

            var expectedRow1 = new List<string>
            {
                growthItemInfo.Rank,
                growthItemInfo.Title,
                growthItemInfo.Description,
                growthItemInfo.Priority,
                growthItemInfo.Owner,
                growthItemInfo.Type,
                growthItemInfo.Category,
                string.Join(", ", growthItemInfo.CompetencyTargets),
                growthItemInfo.Status,
                "",
                growthItemInfo.Size,
                ""
            };
            var actualRow1 = tbl.Rows[0].ItemArray.Select(i => i.ToString()).ToList();
            actualRow1.RemoveAt(0);

            for (var i = 0; i < expectedRow1.Count; i++)
            {
                Log.Info($"Row 1, Column {i} - Expected='{expectedRow1[i]}' Actual='{actualRow1[i]}'");
                Assert.AreEqual(expectedRow1[i], actualRow1[i], $"Row 1, Column {i} value doesn't match");
            }

            var expectedRow2 = new List<string>()
            {
                growthItemInfo1.Rank,
                growthItemInfo1.Title,
                growthItemInfo1.Description,
                growthItemInfo1.Priority,
                growthItemInfo1.Owner,
                growthItemInfo1.Type,
                growthItemInfo1.Category,
                string.Join(", ", growthItemInfo1.CompetencyTargets),
                growthItemInfo1.Status,
                "",
                growthItemInfo1.Size,
                ""
            };
            var actualRow2 = tbl.Rows[1].ItemArray.Select(i => i.ToString()).ToList();
            actualRow2.RemoveAt(0);

            for (var i = 0; i < expectedRow2.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow2[i], actualRow2[i], $"Row 2, Column {i} value doesn't match");
            }

            var expectedColumn2 = new List<string>
            {
                "Id",
                "Rank",
                "Title",
                "Description",
                "Priority",
                "Owner",
                "Type",
                "Category",
                "Competency Target",
                "Status",
                "Target Date",
                "Size",
                "Solution",
                "Date Created"
            };
            growthItemGridView.AddColumns(expectedColumn2);

            FileUtil.DeleteFilesInDownloadFolder(fileName);
            growthItemGridView.ClickExportToExcel();
            spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl2 = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedColumn2.Count, tbl2.Columns.Count, "Column count doesn't match");

            var actualColumn2 = (from DataColumn item in tbl2.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn2.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedColumn2[i], actualColumn2[i], $"column name {i} value doesn't match");
            }

            var expectedRow4 = new List<string>
            {
                growthItemInfo.Rank,
                growthItemInfo.Title,
                growthItemInfo.Description,
                growthItemInfo.Priority,
                growthItemInfo.Owner,
                growthItemInfo.Type,
                growthItemInfo.Category,
                string.Join(", ", growthItemInfo.CompetencyTargets),
                growthItemInfo.Status,
                "",
                growthItemInfo.Size,
                "",
                growthItemInfo.DateCreated.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            };
            var actualRow4 = tbl2.Rows[0].ItemArray.Select(i => i.ToString()).ToList();
            actualRow4.RemoveAt(0);

            for (var i = 0; i < expectedRow4.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow4[i], actualRow4[i], $"Row 1, Column {i} value doesn't match");
            }

            var expectedRow5 = new List<string>
            {
                growthItemInfo1.Rank,
                growthItemInfo1.Title,
                growthItemInfo1.Description,
                growthItemInfo1.Priority,
                growthItemInfo1.Owner,
                growthItemInfo1.Type,
                growthItemInfo1.Category,
                string.Join(", ", growthItemInfo1.CompetencyTargets),
                growthItemInfo1.Status,
                "",
                growthItemInfo1.Size,
                "",
                growthItemInfo1.DateCreated.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            };
            var actualRow5 = tbl2.Rows[1].ItemArray.Select(i => i.ToString()).ToList();
            actualRow5.RemoveAt(0);

            for (var i = 0; i < expectedRow5.Count; i++)
            {
                Assert.AreEqual(expectedRow5[i], actualRow5[i], $"Row 2, Column {i} value doesn't match");
            }

            growthItemGridView.DeleteAllGIs();
            Assert.AreEqual(0, growthItemGridView.GetGrowthItemCount(), "Growth item is still present");
        }
    }
}