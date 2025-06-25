using System;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Add
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class GrowthPlanAddItemTests4 : BaseTest
    {
        private static bool _classInitFailed;
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static AddTeamWithMemberRequest _team;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

        [ClassInitialize]
        public static void GetTeamDetails(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                // Create a team
                _team = TeamFactory.GetNormalTeam("Team");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember1.FirstName,
                    LastName = SharedConstants.TeamMember1.LastName,
                    Email = SharedConstants.TeamMember1.Email
                });
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember2.FirstName,
                    LastName = SharedConstants.TeamMember2.LastName,
                    Email = SharedConstants.TeamMember2.Email
                });
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_Add_CloseIcon_ConfirmationPopUp_Works()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _teamId);
            growthPlanDashboard.ClickOnAddGrowthItemButton();

            growthPlanAddItemPage.FillForm(GrowthItemInfo);
            growthPlanAddItemPage.ClickOnCloseIconButton();
            growthPlanAddItemPage.ConfirmationPopUpClickOnCancelButton();
            Assert.IsTrue(growthPlanAddItemPage.IsGrowthPlanTitleDisplayed(), "'Add Growth Item' popup is closed");

            growthPlanAddItemPage.ClickOnCloseIconButton();
            growthPlanAddItemPage.ConfirmationPopUpClickOnDiscardChangesButton();
            Assert.IsFalse(growthPlanAddItemPage.IsGrowthPlanTitleDisplayed(), "'Add Growth Item' popup is still open");

            var titleName = GrowthItemInfo.Title;
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            growthPlanAddItemPage.FillForm(GrowthItemInfo);
            growthPlanAddItemPage.ClickOnCloseIconButton();
            growthPlanAddItemPage.ConfirmationPopUpClickOnSaveChangesButton();

            var allColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();
            growthPlanDashboard.AddColumns(allColumn);

            var actualGiTitleNamesList = growthPlanDashboard.GetAllColumnValues("Title");
            Assert.That.ListContains(actualGiTitleNamesList, titleName, $"Growth Item {titleName} is not available");
            Assert.AreEqual(GrowthItemInfo.Title, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Title"), "'Title' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Owner, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Owner(s)"), "'Owner(s)' doesn't match");
            Assert.AreEqual(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Target Date"), "'Target Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Status, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Status"), "'Status' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Priority, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Priority"), "'Priority' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Category, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Category"), "'Category' doesn't match");
            Assert.AreEqual(GrowthItemInfo.RadarType, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Radar Type"), "'Radar Type' doesn't match");
            Assert.AreEqual(GrowthItemInfo.CompetencyTargets[0], growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Competency Target"), "'Competency Target' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Type, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Type"), "'Type' doesn't match");
            Assert.AreEqual(User.FullName, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Updated By"), "'Updated By' doesn't match");
            Assert.AreEqual(_team.Name, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Origination"), "'Origination' doesn't match");
            Assert.AreEqual(_team.Name, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Team"), "'Team' doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void GPV2_Add_Delete_Button_Disabled_While_CreateGi()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _teamId);
            growthPlanDashboard.ClickOnAddGrowthItemButton();

            Assert.IsFalse(growthPlanAddItemPage.IsDeleteButtonEnabled(), "'Delete' button is enabled");
            growthPlanAddItemPage.FillForm(GrowthItemInfo);
            Assert.IsFalse(growthPlanAddItemPage.IsDeleteButtonEnabled(), "'Delete' button is enabled");
        }
    }
}
