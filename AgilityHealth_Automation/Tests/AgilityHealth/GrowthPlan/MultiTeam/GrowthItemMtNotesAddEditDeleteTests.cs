using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.Generic;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.MultiTeam
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
    public class GrowthItemMtNotesAddEditDeleteTests : GrowthItemNotesBaseTests
    {
        private static bool _classInitFailed;
        private static RadarResponse _radar;
        private static TeamHierarchyResponse _multiTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _multiTeam = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
                _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Smoke")]
        public void GrowthItem_MT_AddEditDeleteNotes()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

            Log.Info("Login as company admin");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to 'Radar' page, Add new GI ");
            radarPage.NavigateToPage(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
            growthItemGridView.SwitchToGridView();

            growthItemGridView.ClickAddNewGrowthItem();
            growthItemInfo.Owner = null;
            growthItemInfo.Type = GrowthPlanFactory.GetNewGrowthPlanTypes().Select(s => s).Last();
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info("Create/Update growth item note via 'Save' button of popup and verify");
            GrowthItemAddEditNotesViaSaveButtonOfPopup(growthItemInfo.Title);

            Driver.RefreshPage();

            Log.Info("Create/Update/Delete growth item note via 'Save' button of note section and verify");
            GrowthItemAddEditDeleteNotes(growthItemInfo.Title);

        }
    }
}