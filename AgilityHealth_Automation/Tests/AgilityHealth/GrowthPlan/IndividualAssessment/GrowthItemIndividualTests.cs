using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.IndividualAssessment
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GrowthItemIndividualTests : BaseTest
    {
        private static bool _classInitFailed;
        private static CreateIndividualAssessmentRequest _assessment;
        private static AddTeamWithMemberRequest _team;
        private static User _member;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem UpdatedGrowthItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
        private const string RadarType="Radar Type";

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _member = TestEnvironment.UserConfig.GetUserByDescription("member");
                var adminUser = (User.IsMember()) ?
                        TestEnvironment.UserConfig.GetUserByDescription("user 3") : User;
                // add new GOI team with members
                _team = TeamFactory.GetGoiTeam("IndividualGI");
                _team.Members.Add(
                    new AddMemberRequest
                    {
                        FirstName = _member.FirstName,
                        LastName = _member.LastName,
                        Email = _member.Username
                    }
                );
                var setup = new SetupTeardownApi(TestEnvironment);
                var teamResponse = setup.CreateTeam(_team, adminUser).GetAwaiter().GetResult();

                // add new Individual assessment
                _assessment = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                    Company.Id, User.CompanyName, teamResponse.Uid, $"IndividualGITests{Guid.NewGuid()}");

                foreach (var member in teamResponse.Members)
                {
                    _assessment.Members.Add(new IndividualAssessmentMemberRequest
                    {
                        Uid = member.Uid,
                        FirstName = member.FirstName,
                        LastName = member.LastName,
                        Email = member.Email
                    });
                }

                setup.CreateIndividualAssessment(_assessment, SharedConstants.IndividualAssessmentType, adminUser)
                    .GetAwaiter().GetResult();

                // complete survey
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                foreach (var member in _assessment.Members)
                {
                    setupUi.CompleteIndividualSurvey(member.Email, _assessment.PointOfContactEmail);
                }
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        
        [TestMethod]
        [TestCategory("KnownDefect")]//Bug Id:52900
        [TestCategory("Sanity")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_Kanban_AddDragDeleteGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanViewWidget = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopupPage = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemTabKanbanWidgetPage = new GiDashboardKanbanWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            iAssessmentDashboardPage.ClickOnRadar(
                $"{_assessment.AssessmentName} - {_assessment.Members.First().FirstName} {_assessment.Members.First().LastName}");

            growthItemKanbanViewWidget.ClickKanbanAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopupPage.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            GrowthItemInfo.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            GrowthItemInfo.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            GrowthItemInfo.Owner = null;
            GrowthItemInfo.RadarType = null;
            GrowthItemInfo.Comments = null;

            addGrowthItemPopupPage.EnterGrowthItemInfo(GrowthItemInfo);
            addGrowthItemPopupPage.ClickSaveButton();
            growthItemKanbanViewWidget.ShowAllStatusPanels();

            Assert.IsTrue(growthItemKanbanViewWidget.DoesKanbanGiExist(GrowthItemInfo.Status, 1, GrowthItemInfo.Title),
                $"Kanban item with Title '{GrowthItemInfo.Title}' and Status '{GrowthItemInfo.Status}' not found");

            Log.Info($"Edit {GrowthItemInfo.Title} GI and verify 'Radar Type' tooltip icon is not displayed ");
            growthItemTabKanbanWidgetPage.ClickEditKanbanGrowthItem(GrowthItemInfo.Title, GrowthItemInfo.Status);
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopupPage.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            addGrowthItemPopupPage.ClickCancelButton();

            growthItemKanbanViewWidget.DragDropGi(1, GrowthItemInfo.Status, GrowthItemInfo.Title, "Done");

            Assert.IsTrue(growthItemKanbanViewWidget.DoesKanbanGiExist("Done", 1, GrowthItemInfo.Title),
                $"Kanban item with Title '{GrowthItemInfo.Title}' and Status 'Done' not found");

            growthItemKanbanViewWidget.DeleteKanbanGi("Done", 1, GrowthItemInfo.Title);

            Assert.IsFalse(growthItemKanbanViewWidget.DoesKanbanGiExist("Done", 1, GrowthItemInfo.Title),
                $"Kanban item with Title '{GrowthItemInfo.Title}' and Status 'Done' not deleted properly");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_RollUp_Kanban_AddDragDeleteGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var growthItemKanbanViewWidget = new GrowthItemKanbanViewWidget(Driver, Log);
            var addGrowthItemPopupPage = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemTabKanbanWidgetPage = new GiDashboardKanbanWidgetPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            iAssessmentDashboardPage.ClickOnRadar(_assessment.AssessmentName + " - Roll up");

            growthItemKanbanViewWidget.ClickKanbanAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopupPage.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            GrowthItemInfo.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            GrowthItemInfo.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            GrowthItemInfo.Owner = null;
            GrowthItemInfo.RadarType = null;
            GrowthItemInfo.Comments = null;
            addGrowthItemPopupPage.EnterGrowthItemInfo(GrowthItemInfo);
            addGrowthItemPopupPage.ClickSaveButton();
            Assert.IsTrue(growthItemKanbanViewWidget.DoesKanbanGiExist(GrowthItemInfo.Status, 1, GrowthItemInfo.Title),"Growth Item should be added properly");

            Log.Info($"Edit {GrowthItemInfo.Title} GI and verify 'Radar Type' tooltip icon is not displayed ");
            growthItemTabKanbanWidgetPage.ClickEditKanbanGrowthItem(GrowthItemInfo.Title, GrowthItemInfo.Status);
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopupPage.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopupPage.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");
            addGrowthItemPopupPage.ClickCancelButton();
            growthItemKanbanViewWidget.ShowAllStatusPanels();

            growthItemKanbanViewWidget.DragDropGi(1, GrowthItemInfo.Status, GrowthItemInfo.Title, "Done");

            Assert.IsTrue(growthItemKanbanViewWidget.DoesKanbanGiExist("Done", 1, GrowthItemInfo.Title),
                "Growth Item should be edited properly");

            growthItemKanbanViewWidget.DeleteKanbanGi("Done", 1, GrowthItemInfo.Title);

            Assert.IsFalse(growthItemKanbanViewWidget.DoesKanbanGiExist("Done", 1, GrowthItemInfo.Title),
                "Growth Item should be deleted properly");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_Member_Grid_AddEditDeleteGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            iAssessmentDashboardPage.ClickOnRadar(
                $"{_assessment.AssessmentName} - {_assessment.Members.First().FirstName} {_assessment.Members.First().LastName}");

            growthItemGridView.SwitchToGridView();
            growthItemGridView.DeleteAllGIs();
            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            GrowthItemInfo.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            GrowthItemInfo.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            GrowthItemInfo.Owner = null;
            GrowthItemInfo.RadarType = null;
            GrowthItemInfo.Comments = null;
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            var actualGrowthItem = growthItemGridView.GetGrowthItemFromGrid(1);
            Assert.AreEqual(GrowthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(GrowthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(GrowthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(GrowthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(GrowthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(GrowthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(GrowthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(GrowthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            growthItemGridView.ClickGrowthItemEditButton(GrowthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            UpdatedGrowthItemInfo.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().LastOrDefault();
            UpdatedGrowthItemInfo.CompetencyTargets = new List<string> { SharedConstants.DimensionFoundation };
            UpdatedGrowthItemInfo.Owner = null;
            UpdatedGrowthItemInfo.RadarType = null;
            UpdatedGrowthItemInfo.Comments = null;
            addGrowthItemPopup.EnterGrowthItemInfo(UpdatedGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            var editedGrowthItem = growthItemGridView.GetGrowthItemFromGrid(1);
            Assert.AreEqual(UpdatedGrowthItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Description, editedGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            growthItemGridView.DeleteGrowthItem(1);
            Assert.AreEqual(0, growthItemGridView.GetGrowthItemCount(), "Growth isn't deleted successfully");
        }


        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 41416
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_RollUp_Grid_AddEditDeleteGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            iAssessmentDashboardPage.ClickOnRadar(
                _assessment.AssessmentName + " - Roll up");

            growthItemGridView.SwitchToGridView();
            growthItemGridView.DeleteAllGIs();
            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            GrowthItemInfo.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            GrowthItemInfo.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            GrowthItemInfo.Owner = null;
            GrowthItemInfo.RadarType = null;
            GrowthItemInfo.Comments = null;
            addGrowthItemPopup.EnterGrowthItemInfo(GrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            growthItemGridView.SelectAllColumn();
            var actualGrowthItem = growthItemGridView.GetGrowthItemFromGrid(1);
            Assert.AreEqual(GrowthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(GrowthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(GrowthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(GrowthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(GrowthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(GrowthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(GrowthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(GrowthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");


            growthItemGridView.ClickGrowthItemEditButton(GrowthItemInfo.Title);
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled");
            Assert.AreEqual(SharedConstants.IndividualAssessmentType, addGrowthItemPopup.GetRadarTypeValue(), "Radar Type doesn't match");
            Assert.IsFalse(addGrowthItemPopup.IsRadarTypeTooltipIconDisplayed(), $"{RadarType} tooltip icon is displayed");

            UpdatedGrowthItemInfo.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().LastOrDefault();
            UpdatedGrowthItemInfo.CompetencyTargets = new List<string> { SharedConstants.DimensionFoundation };
            UpdatedGrowthItemInfo.Owner = null;
            UpdatedGrowthItemInfo.RadarType = null;
            UpdatedGrowthItemInfo.Comments = null;
            addGrowthItemPopup.EnterGrowthItemInfo(UpdatedGrowthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            var editedGrowthItem = growthItemGridView.GetGrowthItemFromGrid(1);
            Assert.AreEqual(UpdatedGrowthItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Description, editedGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual(UpdatedGrowthItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            growthItemGridView.DeleteGrowthItem(1);
            Assert.AreEqual(0, growthItemGridView.GetGrowthItemCount(), "Growth isn't deleted successfully");
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_Member_Grid_ExportGrowthItemToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            iAssessmentDashboardPage.ClickOnRadar(
                $"{_assessment.AssessmentName} - {_assessment.Members.First().FirstName} {_assessment.Members.First().LastName}");

            growthItemGridView.DeleteAllGIs();
            var growthItemInfo1 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo1.Rank = "1";
            growthItemInfo1.Color = "#c3d69b";
            growthItemInfo1.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            growthItemInfo1.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            growthItemInfo1.Owner = null;
            growthItemInfo1.RadarType = null;
            growthItemInfo1.Comments = null;
            growthItemInfo1.TargetDate = null;
            growthItemInfo1.DateCreated = DateTime.Now;

            var growthItemInfo2 = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            growthItemInfo2.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().LastOrDefault();
            growthItemInfo2.CompetencyTargets = new List<string> { SharedConstants.DimensionFoundation };
            growthItemInfo2.Owner = null;
            growthItemInfo2.Rank = "2";
            growthItemInfo2.RadarType = null;
            growthItemInfo2.Comments = null;
            growthItemInfo2.TargetDate = null;
            growthItemInfo2.DateCreated = DateTime.Now;

            var growthItemInfo3 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo3.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            growthItemInfo3.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            growthItemInfo3.Owner = null;
            growthItemInfo3.RadarType = null;
            growthItemInfo3.Comments = null;
            growthItemInfo3.TargetDate = null;
            growthItemInfo3.DateCreated = DateTime.Now;

            var growthItems = new List<GrowthItem> { growthItemInfo1, growthItemInfo2, growthItemInfo3 };

            foreach (var gi in growthItems)
            {
                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopup.EnterGrowthItemInfo(gi);
                addGrowthItemPopup.ClickSaveButton();

            }

            var filename = $"GrowthPlanFor{_team.Name}Assessment{_assessment.AssessmentName} - {_assessment.Members.First().FirstName} {_assessment.Members.First().LastName}.xlsx";
            Log.Info($"Expected Filename = '{filename}'");
            FileUtil.DeleteFilesInDownloadFolder(filename);

            var expectedColumn = new List<string>()
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
                "Solution"

            };
            growthItemGridView.AddColumns(expectedColumn);

            growthItemGridView.ClickExportToExcel();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(filename);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedColumn.Count, tbl.Columns.Count, "Column count doesn't match");

            var actualColumn = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn.Count; i++)
            {
                Log.Info($"Checking column {i} - Expected='{expectedColumn[i]}', Actual='{actualColumn[i]}'");
                Assert.AreEqual(expectedColumn[i], actualColumn[i], $"{i}th column value doesn't match");
            }
            for (var i = 0; i < growthItems.Count; i++)
            {
                var expectedRow = new List<string>()
                {
                    growthItems[i].Rank,
                    growthItems[i].Title,
                    growthItems[i].Description,
                    growthItems[i].Priority,
                    "",
                    growthItems[i].Type,
                    growthItems[i].Category,
                    string.Join(", ", growthItems[i].CompetencyTargets),
                    growthItems[i].Status,
                    "",
                    growthItems[i].Size,
                    ""
                };
                var actualRow = tbl.Rows[i].ItemArray.Select(j => j.ToString()).ToList();
                actualRow.RemoveAt(0);
                for (var k = 0; k < expectedRow.Count; k++)
                {
                    Log.Info($"Row {i} Column {k} - Expected='{expectedRow[k]}' Actual='{actualRow[k]}'");
                    Assert.AreEqual(expectedRow[k], actualRow[k], $"Row {i} Column {k}");
                }
            }

            var expectedColumn2 = new List<string>()
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

            FileUtil.DeleteFilesInDownloadFolder(filename);
            growthItemGridView.ClickExportToExcel();
            var spreadsheet2 = FileUtil.WaitUntilFileDownloaded(filename);

            var tbl2 = ExcelUtil.GetExcelData(spreadsheet2);

            Assert.AreEqual(expectedColumn2.Count, tbl2.Columns.Count, "Column count doesn't match");

            var actualColumn2 = (from DataColumn item in tbl2.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn2.Count; i++)
            {
                Log.Info($"Checking column {i} - Expected='{expectedColumn2[i]}', Actual='{actualColumn2[i]}'");
                Assert.AreEqual(expectedColumn2[i], actualColumn2[i], $"{i}th column value doesn't match");
            }
            for (var i = 0; i < growthItems.Count; i++)
            {
                var dateCreated = growthItems[i].DateCreated;
                if (dateCreated != null)
                {
                    var expectedRow = new List<string>()
                    {
                        growthItems[i].Rank,
                        growthItems[i].Title,
                        growthItems[i].Description,
                        growthItems[i].Priority,
                        "",
                        growthItems[i].Type,
                        growthItems[i].Category,
                        string.Join(", ", growthItems[i].CompetencyTargets),
                        growthItems[i].Status,
                        "",
                        growthItems[i].Size,
                        "",
                        dateCreated.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                    };
                    var actualRow = tbl2.Rows[i].ItemArray.Select(j => j.ToString()).ToList();
                    actualRow.RemoveAt(0);
                    for (var k = 0; k < expectedRow.Count; k++)
                    {
                        Log.Info($"Row {i} Column {k} - Expected='{expectedRow[k]}' Actual='{actualRow[k]}'");
                        Assert.AreEqual(expectedRow[k], actualRow[k], $"Row {i} Column {k}");
                    }
                }
            }

            growthItemGridView.DeleteAllGIs();
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_PullItemsToRollUpView()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            var committedGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            committedGrowthItem.Status = GrowthItemStatusType.Committed.AsString();

            var inProgressGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            inProgressGrowthItem.Status = GrowthItemStatusType.StartedWithAge.AsString();

            var cancelledGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            cancelledGrowthItem.Status = GrowthItemStatusType.Cancelled.AsString();

            var doneGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            doneGrowthItem.Status = GrowthItemStatusType.Finished.AsString();

            var notStartedGrowthItem = GrowthPlanFactory.GetValidGrowthItem();
            var onHoldGrowthItem = GrowthPlanFactory.GetValidUpdatedGrowthItem();

            Driver.NavigateToPage(ApplicationUrl);
            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            Log.Info($"Search {_team.Name} team and go to the Assessment dashboard.");
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);

            Log.Info("Click on 'Participant' radar and delete all the GIs");
            iAssessmentDashboardPage.ClickOnRadar($"{_assessment.AssessmentName} - {_assessment.Members.First().FirstName} {_assessment.Members.First().LastName}");
            growthItemGridView.SwitchToGridView();
            growthItemGridView.DeleteAllGIs();

            Log.Info("Click on 'Add New Growth Item' button, fill the Growth Item information with different status then save it.");
            var growthItems = new List<GrowthItem> { notStartedGrowthItem, committedGrowthItem, inProgressGrowthItem, cancelledGrowthItem, doneGrowthItem, onHoldGrowthItem };
            foreach (var gi in growthItems)
            {
                gi.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().LastOrDefault();
                gi.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
                gi.Owner = null;
                gi.RadarType = null;
                gi.Comments = null;
                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopup.EnterGrowthItemInfo(gi);
                addGrowthItemPopup.ClickSaveButton();
                Assert.IsTrue(growthItemGridView.IsGiPresent(gi.Title), $"Growth item {gi.Title} isn't present.");
            }

            Log.Info("Go back to Assessment dashboard and click on 'Roll up' radar then delete all the GIs.");
            Driver.Back();
            iAssessmentDashboardPage.ClickOnRadar(_assessment.AssessmentName + " - Roll up");
            growthItemGridView.SwitchToGridView();
            growthItemGridView.DeleteAllGIs();

            Log.Info("Verify Pullable item counts");
            Assert.AreEqual(4, growthItemGridView.GetPullableItemCount(), "Pullable item count doesn't match.");

            Log.Info("Pull both the Growth items and verify 'Pull Items From Sub Teams' count and 'Pulled Growth Item' count.");
            growthItemGridView.ClickPullItemFromSubTeam();
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(onHoldGrowthItem.Title), "Growth item with 'On Hold' status isn't displayed.");
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(notStartedGrowthItem.Title), "Growth item with 'Not Started' status isn't displayed.");
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(committedGrowthItem.Title), "Growth item with 'Committed' status isn't displayed.");
            Assert.IsTrue(growthItemGridView.IsPullItemDisplayed(inProgressGrowthItem.Title), "Growth item with 'In Progress' status isn't displayed.");

            Log.Info("Verify that GI with 'Cancelled' and 'Done' status is not present on 'Roll up' Radar page.");
            Assert.IsFalse(growthItemGridView.IsPullItemDisplayed(cancelledGrowthItem.Title), "Growth item with 'Cancelled' status is displayed.");
            Assert.IsFalse(growthItemGridView.IsPullItemDisplayed(doneGrowthItem.Title), "Growth item with 'Done' status is displayed.");

            growthItemGridView.PullItemFromSubTeam(onHoldGrowthItem.Title);
            growthItemGridView.PullItemFromSubTeam(notStartedGrowthItem.Title);
            growthItemGridView.ClickClosePullDialog();
            Assert.AreEqual(2, growthItemGridView.GetPullableItemCount(), "Pullable item count doesn't match");
            Assert.AreEqual(2, growthItemGridView.GetGrowthItemCount(), "Growth item count doesn't match");

            Log.Info("On 'Pull Growth Plan Items From Sub-Teams' Popup UnPull Growth item and verify 'Pull Items From Sub Teams' count and 'Pulled Growth Item' count.");
            growthItemGridView.ClickPullItemFromSubTeam();
            growthItemGridView.UnPullItemFromSubTeam(onHoldGrowthItem.Title);
            growthItemGridView.ClickClosePullDialog();
            Assert.AreEqual(3, growthItemGridView.GetPullableItemCount(), "Pullable item count doesn't match");
            Assert.AreEqual(1, growthItemGridView.GetGrowthItemCount(), "Growth item count doesn't match");

            Log.Info($"Verify that Edit, Copy and Unpull buttons are displayed for {notStartedGrowthItem.Title} growth item");
            Assert.IsTrue(growthItemGridView.IsEditGrowthItemButtonDisplayed(notStartedGrowthItem.Title), "'Edit' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsCopyGrowthItemButtonDisplayed(notStartedGrowthItem.Title), "'Copy' button is not displayed");
            Assert.IsTrue(growthItemGridView.IsUnpullGrowthItemButtonDisplayed(notStartedGrowthItem.Title), "'Unpull' button is not displayed");

            Log.Info("On 'Growth Item Grid' UnPull Growth item and verify 'Pull Items From Sub Teams' count and 'Pulled Growth Item' count.");
            growthItemGridView.ClickOnUnpullItemButtonFromGrid(notStartedGrowthItem.Title);
            growthItemGridView.ClickOnUnpullGrowthPlanPopUpYesButton();
            Assert.AreEqual(4, growthItemGridView.GetPullableItemCount(), "Pullable item count doesn't match");
            Assert.AreEqual(0, growthItemGridView.GetGrowthItemCount(), "Growth item count doesn't match");
        }


        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 41416
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthItem_IA_RollUp_ExportGrowthItemToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            var rollUpName = _assessment.AssessmentName + " - Roll up";
            var href = iAssessmentDashboardPage.EditBatch_GetLinkHref(_assessment.AssessmentName);
            var batchid = href.Split('/')[6];
            iAssessmentDashboardPage.ClickOnRadar(rollUpName);

            growthItemGridView.SwitchToGridView();
            growthItemGridView.DeleteAllGIs();

            var growthItemInfo1 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo1.Color = "#c3d69b";
            growthItemInfo1.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            growthItemInfo1.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            growthItemInfo1.Owner = null;
            growthItemInfo1.RadarType = null;
            growthItemInfo1.Comments = null;
            growthItemInfo1.TargetDate = null;
            growthItemInfo1.DateCreated = DateTime.Now;

            var growthItemInfo2 = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            growthItemInfo2.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().LastOrDefault();
            growthItemInfo2.CompetencyTargets = new List<string> { SharedConstants.DimensionFoundation };
            growthItemInfo2.Owner = null;
            growthItemInfo2.Rank = "2";
            growthItemInfo2.RadarType = null;
            growthItemInfo2.Comments = null;
            growthItemInfo2.TargetDate = null;
            growthItemInfo2.DateCreated = DateTime.Now;

            var growthItemInfo3 = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo3.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().FirstOrDefault();
            growthItemInfo3.CompetencyTargets = new List<string> { SharedConstants.DimensionCulture };
            growthItemInfo3.Rank = "1";
            growthItemInfo3.Owner = null;
            growthItemInfo3.RadarType = null;
            growthItemInfo3.Comments = null;
            growthItemInfo3.TargetDate = null;
            growthItemInfo3.DateCreated = DateTime.Now;

            var growthItems = new List<GrowthItem> { growthItemInfo1, growthItemInfo2, growthItemInfo3 };

            foreach (var gi in growthItems)
            {
                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopup.EnterGrowthItemInfo(gi);
                addGrowthItemPopup.ClickSaveButton();
            }

            var filename = $"IndividualAssessments_{batchid}- Items in growth plan.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(filename);

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
                "Solution"
            };
            growthItemGridView.AddColumns(expectedColumn);

            growthItemGridView.ClickExportToExcel();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(filename);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedColumn.Count, tbl.Columns.Count, "Column count doesn't match");

            var actualColumn = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedColumn[i], actualColumn[i], $"{i}th column value doesn't match");
            }
            var expectedRow1 = new List<string>()
            {
                growthItemInfo1.Rank,
                growthItemInfo1.Title,
                growthItemInfo1.Description,
                growthItemInfo1.Category,
                string.Join(", ", growthItemInfo1.CompetencyTargets),
                growthItemInfo1.Status,
                "",
                ""
            };
            var actualRow1 = tbl.Rows[2].ItemArray.Select(i => i.ToString()).ToList();
            actualRow1.RemoveAt(0);

            for (var i = 0; i < expectedRow1.Count; i++)
            {
                Log.Info($"Row 1, Column {i} - Expected='{expectedRow1[i]}' Actual='{actualRow1[i]}'");
                Assert.AreEqual(expectedRow1[i], actualRow1[i], $"{i}th row value doesn't match");
            }

            var expectedRow2 = new List<string>()
            {
                growthItemInfo2.Rank,
                growthItemInfo2.Title,
                growthItemInfo2.Description,
                growthItemInfo2.Category,
                string.Join(", ", growthItemInfo2.CompetencyTargets),
                growthItemInfo2.Status,
                "",
                ""
            };
            var actualRow2 = tbl.Rows[1].ItemArray.Select(i => i.ToString()).ToList();
            actualRow2.RemoveAt(0);

            for (var i = 0; i < expectedRow2.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow2[i], actualRow2[i], $"{i}th row value doesn't match");
            }

            var expectedRow3 = new List<string>()
            {
                growthItemInfo3.Rank,
                growthItemInfo3.Title,
                growthItemInfo3.Description,
                growthItemInfo3.Category,
                string.Join(", ", growthItemInfo3.CompetencyTargets),
                growthItemInfo3.Status,
                "",
                ""
            };
            var actualRow3 = tbl.Rows[0].ItemArray.Select(i => i.ToString()).ToList();
            actualRow3.RemoveAt(0);

            for (var i = 0; i < expectedRow3.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow3[i], actualRow3[i], $"{i}th row value doesn't match");
            }

            var expectedColumn2 = new List<string>()
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
                "Size"
            };
            growthItemGridView.AddColumns(expectedColumn2);

            FileUtil.DeleteFilesInDownloadFolder(filename);
            growthItemGridView.ClickExportToExcel();
            spreadsheet = FileUtil.WaitUntilFileDownloaded(filename);

            var tbl2 = ExcelUtil.GetExcelData(spreadsheet);

            Assert.AreEqual(expectedColumn2.Count, tbl2.Columns.Count, "Column count doesn't match");

            var actualColumn2 = (from DataColumn item in tbl2.Columns select item.ColumnName).ToList();
            for (var i = 0; i < expectedColumn2.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedColumn2[i], actualColumn2[i], $"column name {i} value doesn't match");
            }

            var expectedRow4 = new List<string>()
            {
                growthItemInfo1.Rank,
                growthItemInfo1.Title,
                growthItemInfo1.Description,
                growthItemInfo1.Priority,
                "", // owner
                growthItemInfo1.Type,
                growthItemInfo1.Category,
                string.Join(", ", growthItemInfo1.CompetencyTargets),
                growthItemInfo1.Status,
                growthItemInfo1.DateCreated.Value.ToUniversalTime().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                "", // target date
                "", // solution
                growthItemInfo1.Size
            };
            var actualRow4 = tbl2.Rows[2].ItemArray.Select(i => i.ToString()).ToList();
            actualRow4.RemoveAt(0);

            for (var i = 0; i < expectedRow4.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow4[i], actualRow4[i], $"Row 1 Column {i} value doesn't match");
            }

            var expectedRow5 = new List<string>()
            {
                growthItemInfo2.Rank,
                growthItemInfo2.Title,
                growthItemInfo2.Description,
                growthItemInfo2.Priority,
                "",
                growthItemInfo2.Type,
                growthItemInfo2.Category,
                string.Join(", ", growthItemInfo2.CompetencyTargets),
                growthItemInfo2.Status,
                growthItemInfo2.DateCreated.Value.ToUniversalTime().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                "",
                "",
                growthItemInfo2.Size,
            };
            var actualRow5 = tbl2.Rows[1].ItemArray.Select(i => i.ToString()).ToList();
            actualRow5.RemoveAt(0);

            for (var i = 0; i < expectedRow5.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow5[i], actualRow5[i], $"Row 2 Column {i} value doesn't match");
            }

            var expectedRow6 = new List<string>()
            {
                growthItemInfo3.Rank,
                growthItemInfo3.Title,
                growthItemInfo3.Description,
                growthItemInfo3.Priority,
                "",
                growthItemInfo3.Type,
                growthItemInfo3.Category,
                string.Join(", ", growthItemInfo3.CompetencyTargets),
                growthItemInfo3.Status,
                growthItemInfo3.DateCreated.Value.ToUniversalTime().ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                "",
                "",
                growthItemInfo3.Size
            };
            var actualRow6 = tbl2.Rows[0].ItemArray.Select(i => i.ToString()).ToList();
            actualRow6.RemoveAt(0);

            for (var i = 0; i < expectedRow6.Count; i++)
            {
                Log.Info($"Checking column {i}");
                Assert.AreEqual(expectedRow6[i], actualRow6[i], $"Row 3 Column {i} value doesn't match");
            }

            growthItemGridView.DeleteAllGIs();
        }
    }
}