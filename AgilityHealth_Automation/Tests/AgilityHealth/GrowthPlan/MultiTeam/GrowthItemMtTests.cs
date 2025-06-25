using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments.Team.Custom;


namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.MultiTeam
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
    public class GrowthItemMtTests : BaseTest
    {
        private static bool _classInitFailed;
        private static RadarResponse _radar;
        private static TeamHierarchyResponse _multiTeam, _team;
        private static TeamAssessmentInfo _teamAssessment;
        private const string RadarType = "Radar Type";

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                _team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _multiTeam = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
                _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);

                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Team_Assessment{Guid.NewGuid()}",
                    TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() }
                };
                setupUi.AddTeamAssessment(_team.TeamId, _teamAssessment);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("Sanity")]
        public void GrowthItem_MT_GridView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            growthItemInfo.Owner = null;
            growthItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} is not present");

            var actualGrowthItem = growthItemGridView.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} is not added successfully");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            growthEditedItemInfo.Category = GrowthPlanFactory.GetEnterpriseTeamGrowthPlanCategories().ListToString();
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not present");

            var editedGrowthItem = growthItemGridView.GetGrowthItemFromGrid(growthEditedItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not present");
            Assert.AreEqual(growthEditedItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthEditedItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Description, editedGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            Log.Info($"Delete {growthEditedItemInfo.Title} GI and verify");
            growthItemGridView.DeleteGrowthItem(growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is still present");
        }

        [TestMethod]
        public void GrowthItem_MT_GridView_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, verify that growth item is present in gridView");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} is not present");

            Log.Info($"Copy {growthItemInfo.Title} GI and verify");
            growthItemGridView.ClickCopyGrowthItemButton(growthItemInfo.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} isn't copied");
            // Below method is used to clean up all the GIs
            growthItemGridView.DeleteAllGIs();
        }

        [TestMethod]
        public void GrowthItem_MT_KanbanView_AddEditDelete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var giDashboardKanbanView = new GiDashboardKanbanWidgetPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI and verify");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            
            growthItemInfo.Owner = null;
            growthItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist(growthItemInfo.Status, 1, growthItemInfo.Title), $"Growth Item {growthItemInfo.Title} is not exist");

            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            Assert.AreEqual(growthItemInfo.Title, addGrowthItemPopup.GetTitleValue(), "Title doesn't matched");
            Assert.AreEqual(growthItemInfo.Category, addGrowthItemPopup.GetCategoryValue(), "Category doesn't matched");
            Assert.AreEqual(growthItemInfo.Type, addGrowthItemPopup.GetTypeValue(), "Types doesn't matched");
            Assert.AreEqual(growthItemInfo.Status, addGrowthItemPopup.GetStatusValue(), "Status doesn't matched");
            Assert.AreEqual(growthItemInfo.Priority, addGrowthItemPopup.GetPriorityValue(), "Priority doesn't matched");
            Assert.AreEqual(growthItemInfo.Size, addGrowthItemPopup.GetSizeValue(), "Size doesn't matched");
            Assert.AreEqual(growthItemInfo.Description, addGrowthItemPopup.GetDescription(), "Description doesn't matched");
            Assert.AreEqual(growthItemInfo.Color, addGrowthItemPopup.GetColorValue(), "Color doesn't match");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify ");
            addGrowthItemPopup.ClickCancelButton();
            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthItemInfo.Title, growthItemInfo.Status);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(_teamAssessment.AssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            growthEditedItemInfo.Category = GrowthPlanFactory.GetEnterpriseTeamGrowthPlanCategories().ListToString();
            growthEditedItemInfo.Status = GrowthItemDetailStatusType.Finished.AsString();
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not exist");

            giDashboardKanbanView.ClickEditKanbanGrowthItem(growthEditedItemInfo.Title, growthEditedItemInfo.Status);
            Assert.AreEqual(growthEditedItemInfo.Title, addGrowthItemPopup.GetTitleValue(), "Title doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Category, addGrowthItemPopup.GetCategoryValue(), "Category doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Type, addGrowthItemPopup.GetTypeValue(), "Type doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Status, addGrowthItemPopup.GetStatusValue(), "Status doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Priority, addGrowthItemPopup.GetPriorityValue(), "Priority doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Size, addGrowthItemPopup.GetSizeValue(), "Size doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Description, addGrowthItemPopup.GetDescription(), "Description doesn't matched");
            Assert.AreEqual(growthEditedItemInfo.Color, addGrowthItemPopup.GetColorValue(), "Color doesn't match");

            Log.Info($"'Delete'{growthEditedItemInfo.Title} GI and Verify ");
            addGrowthItemPopup.ClickCancelButton();
            growthItemKanbanView.DeleteKanbanGi("Done", 1, growthEditedItemInfo.Title);
            Assert.IsFalse(growthItemKanbanView.DoesKanbanGiExist("Done", 1, growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is still exist");

            // Below method is used to clean up all the GIs
            growthItemKanbanView.DeleteAllKanbanGi();
        }

        [TestMethod]
        public void GrowthItem_MT_KanbanView_DragDrop_And_Copy()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemKanbanView = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemKanbanView.SwitchToKanbanView();

            growthItemKanbanView.ClickKanbanAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
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

        [TestMethod]
        public void GrowthItem_MT_ExportGrowthItemToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);

            const string teamName = SharedConstants.Team;
            var downloadFileName = $"{_multiTeam.Name}- Items in growth plan.xlsx";

            FileUtil.DeleteFilesInDownloadFolder(downloadFileName);

            Driver.NavigateToPage(ApplicationUrl);
            var user = (User.IsMember()) ?
                TestEnvironment.UserConfig.GetUserByDescription("user 3") : User;

            login.LoginToApplication(user.Username, user.Password);

            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);

            mtEtDashboardPage.NavigateToRadarDetailsPage(Driver.GetCurrentUrl(), _multiTeam.TeamId);
            growthItemGridView.DeleteAllGIs();

            var growthItemInfo1 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo1.RadarType = null;
            growthItemInfo1.Comments = null;
            growthItemInfo1.Owner = "";
            growthItemInfo1.Rank = "1";
            growthItemInfo1.DateCreated = DateTime.Now;
            growthItemInfo1.AffectedTeams = teamName;

            var growthItemInfo2 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo2.Category = GrowthPlanFactory.GetEnterpriseTeamGrowthPlanCategories().ListToString();
            growthItemInfo2.RadarType = null;
            growthItemInfo2.Comments = null;
            growthItemInfo2.Owner = "";
            growthItemInfo2.Rank = "2";
            growthItemInfo2.DateCreated = DateTime.Now;
            growthItemInfo2.CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember.FirstOrDefault() };
            growthItemInfo2.AffectedTeams = teamName;

            var growthItemInfo3 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo3.RadarType = null;
            growthItemInfo3.Rank = "8";
            growthItemInfo3.Color = "#c7aed6";
            growthItemInfo3.Comments = null;
            growthItemInfo3.Owner = "";
            growthItemInfo3.Rank = "3";
            growthItemInfo3.DateCreated = DateTime.Now;
            growthItemInfo3.CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember.Last() };
            growthItemInfo3.AffectedTeams = teamName;

            var growthItems = new List<GrowthItem> { growthItemInfo1, growthItemInfo2, growthItemInfo3 };

            foreach (var gi in growthItems)
            {
                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopup.EnterGrowthItemInfo(gi);
                addGrowthItemPopup.ClickSaveButton();
            }

            Driver.RefreshPage();

            if (User.IsMember())
            {
                topNav.LogOut();

                login.LoginToApplication(User.Username, User.Password);

                mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            }

            var expectedColumn = new List<string>()
            {
                "Id",
                "Rank",
                "Title",
                "Description",
                "Category",
                "Competency Target",
                "Status",
                "Target Date",
                "Solution",
                "Affected Teams"
            };
            growthItemGridView.AddColumns(expectedColumn);

            growthItemGridView.ClickExportToExcel();

            Log.Info($"Exported file name = {downloadFileName}");
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(downloadFileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedColumn.Count, tbl.Columns.Count, "Column count doesn't match");

            var actualColumn = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn.Count; i++)
            {
                Log.Info($"Column Heading {i} - Expected='{expectedColumn[i]}' Actual='{actualColumn[i]}'");
                Assert.AreEqual(expectedColumn[i], actualColumn[i], $"column index {i} value doesn't match");
            }

            for (var i = 0; i < growthItems.Count; i++)
            {
                var expectedRow = new List<string>()
                {
                    growthItems[i].Rank,
                    growthItems[i].Title,
                    growthItems[i].Description,
                    growthItems[i].Category,
                    growthItems[i].CompetencyTargets.ListToString(),
                    growthItems[i].Status,
                    growthItems[i].TargetDate?.ToUniversalTime().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    "",
                    growthItems[i].AffectedTeams
                };
                var actualRow = tbl.Rows[i].ItemArray.Select(j => j.ToString()).ToList();
                actualRow.RemoveAt(0);
                for (var k = 0; k < expectedRow.Count; k++)
                {
                    Log.Info($"Row {i} Column {k} - Expected='{expectedRow[k]}' Actual='{actualRow[k]}'");
                    Assert.AreEqual(expectedRow[k], actualRow[k], $"Row {i} Column {k} value doesn't match");
                }
            }

            expectedColumn = new List<string>()
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
                "Date Created",
                "Target Date",
                "Solution",
                "Size",
                "Affected Teams"
            };
            growthItemGridView.AddColumns(expectedColumn);

            FileUtil.DeleteFilesInDownloadFolder(downloadFileName);
            growthItemGridView.ClickExportToExcel();

            spreadsheet = FileUtil.WaitUntilFileDownloaded(downloadFileName);
            tbl = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedColumn.Count, tbl.Columns.Count, "Column count doesn't match");

            actualColumn = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn.Count; i++)
            {
                Log.Info($"Column Heading {i} - Expected='{expectedColumn[i]}' Actual='{actualColumn[i]}'");
                Assert.AreEqual(expectedColumn[i], actualColumn[i], $"column name {i} value doesn't match");
            }

            for (var i = 0; i < growthItems.Count; i++)
            {
                var expectedRow = new List<string>()
                {
                    growthItems[i].Rank,
                    growthItems[i].Title,
                    growthItems[i].Description,
                    growthItems[i].Priority,
                    growthItems[i].Owner,
                    growthItems[i].Type,
                    growthItems[i].Category,
                    growthItems[i].CompetencyTargets.ListToString(),
                    growthItems[i].Status,
                    growthItems[i].DateCreated?.ToUniversalTime().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    growthItems[i].TargetDate?.ToUniversalTime().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    "",
                    growthItems[i].Size,
                    growthItems[i].AffectedTeams
                };
                var actualRow = tbl.Rows[i].ItemArray.Select(j => j.ToString()).ToList();
                actualRow.RemoveAt(0);
                for (var k = 0; k < expectedRow.Count; k++)
                {
                    Log.Info($"Row {i} Column {k} - Expected='{expectedRow[k]}' Actual='{actualRow[k]}'");
                    Assert.AreEqual(expectedRow[k], actualRow[k], $"Row {i} Column {k} value doesn't match");
                }
            }
            // Below method is used to clean up all the GIs
            growthItemGridView.DeleteAllGIs();

        }

        [TestMethod]
        public void GrowthItem_MT_PullEditUnPullToMT_Classic()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team level Radar' page, Add new GI and verify");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} is not added successfully");

            Log.Info("Go to 'Multi level Radar' Page and delete all Growth Items present on that page");
            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtEtDashboardPage.ClickOnRadar(_teamAssessment.AssessmentType);
            growthItemGridView.DeleteAllGIs();
            growthItemGridView.SwitchToGridView();

            Log.Info($"Pull {growthItemInfo.Title} growth item and verify is it present on 'Multi Level Radar' page");
            growthItemGridView.ClickPullItemFromSubTeam();
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(growthItemInfo.Title), "Pull able item should be displayed");
            growthItemGridView.PullItemFromSubTeam(growthItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsUnPullItemDisplayed(growthItemInfo.Title), "UnPull item should be displayed");
            growthItemGridView.ClickClosePullDialog();
            growthItemGridView.SelectAllColumn();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Pull item is not present");

            Log.Info($"Edit {growthItemInfo.Title} growth item and verify {growthEditedItemInfo.Title} growth item is present on 'Multi level Radar' Page");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            growthEditedItemInfo.Owner = "";
            growthEditedItemInfo.Category = "Organizational";
            addGrowthItemPopup.EnterGrowthItemInfo(growthEditedItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} isn't present");

            Log.Info($"UnPull {growthEditedItemInfo.Title} growth item and verify it is not present on 'Multi Level Radar' page");
            growthItemGridView.ClickPullItemFromSubTeam();
            growthItemGridView.UnPullItemFromSubTeam(growthEditedItemInfo.Title);
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(growthEditedItemInfo.Title), "Pull able item should be displayed");
            growthItemGridView.ClickClosePullDialog();
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), "Unpull item should not present");

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            // Below method is used to clean up all the GIs
            growthItemGridView.DeleteAllGIs();
        }

        [TestMethod]
        public void GrowthItem_MT_Pull_VerifyGIBasedOnStatus()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopupPage = new AddGrowthItemPopupPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var notStartedGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            var committedGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            var inProgressGrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var cancelledGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            var doneGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            var onHoldGrowthItem = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Team level Radar' page, Add new GI and verify");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            growthItemGridView.SwitchToGridView();
            growthItemGridView.DeleteAllGIs();

            committedGrowthItem.Status = GrowthItemStatusType.Committed.AsString();
            inProgressGrowthItemInfo.Status = GrowthItemStatusType.StartedWithAge.AsString();
            cancelledGrowthItem.Status = GrowthItemStatusType.Cancelled.AsString();
            doneGrowthItem.Status = GrowthItemStatusType.Finished.AsString();

            var growthItems = new List<GrowthItem> { notStartedGrowthItem, committedGrowthItem, inProgressGrowthItemInfo, cancelledGrowthItem, doneGrowthItem, onHoldGrowthItem };

            foreach (var gi in growthItems)
            {
                gi.Owner = "";
                gi.Category = GrowthPlanFactory.GetMultiTeamGrowthPlanCategories().LastOrDefault();
                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopupPage.EnterGrowthItemInfo(gi);
                addGrowthItemPopupPage.ClickSaveButton();
                Assert.IsTrue(growthItemGridView.IsGiPresent(gi.Title), $"Growth item {gi.Title} isn't present");
            }

            Log.Info("Go to 'multi level Radar' page and delete all GI");
            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtEtDashboardPage.ClickOnRadar(_teamAssessment.AssessmentType);
            growthItemGridView.DeleteAllGIs();

            Log.Info($"Verify that GI with 'On Hold','Not Started','Committed' and 'In Progress' status is present on 'Multi Level Radar' page");
            growthItemGridView.ClickPullItemFromSubTeam();
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(onHoldGrowthItem.Title), "Growth item with 'On Hold' status isn't display");
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(notStartedGrowthItem.Title), "Growth item with 'Not Started' status isn't display");
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(committedGrowthItem.Title), "Growth item with 'Committed' status isn't display");
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(inProgressGrowthItemInfo.Title), "Growth item with 'In Progress' status isn't display");

            Log.Info($"Verify that GI with 'Cancelled' and 'Done' status is not present on 'Multi Level Radar' page");
            Assert.IsFalse(growthItemGridView.IsPullItemDisplayed(cancelledGrowthItem.Title), "Growth item with 'Cancelled' status is display");
            Assert.IsFalse(growthItemGridView.IsPullItemDisplayed(doneGrowthItem.Title), "Growth item with 'Done' status is display");

            try
            {
                Log.Info($"Navigate back to {_teamAssessment.AssessmentName} assessment and delete all GI's ");
                teamAssessmentDashboard.NavigateToPage(_team.TeamId);
                teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
                // Below method is used to clean up all the GIs
                growthItemGridView.DeleteAllGIs();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}