using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Newtonsoft.Json;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraDataCenterIntegration;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraDataCenter
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraDataCenterGrowthItemSyncTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static bool _classInitFailed;
        private static readonly string JiraCenterInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraDataCenterIntegrationInfo.json");
        private static readonly JiraDataCenterIntegration JiraDataCenter = JsonConvert.DeserializeObject<JiraDataCenterIntegration>(JiraCenterInfo);

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("TestIntegration");
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamResponse.TeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyGrowthItemSyncToJiraBoardDataCenter()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var linkTeamPage = new LinkTeamBasePage(Driver, Log);
            var manageIntegrationsPage = new ManageIntegrationsPage(Driver, Log);
            var growthItemsPage = new GrowthItemsPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Team Assessment 'Growth Item Tab' page and verify 'Sync Committed Items' button is not displayed");
            growthItemsPage.NavigateToPage(Company.Id, _teamResponse.TeamId);
            Assert.IsFalse(growthItemsPage.IsSyncCommittedItemsButtonDisplayed(), "'Sync Committed Items' button is displayed");

            Log.Info("Add new growth item and verify 'Sync Jira' button is not displayed");
            growthItemsPage.ClickAddNewItemButton();
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.Category = "Team";
            growthItemInfo.Owner = null;
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsFalse(growthItemsPage.IsSyncJiraButtonDisplayed(growthItemInfo.Title), "'Sync Jira' button is displayed");

            Log.Info("Navigate to the 'Manage Integrations' page and Click on the 'Manage Button'.");
            manageIntegrationsPage.NavigateToPage(Company.Id);
            manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraDataCenter);

            Log.Info($"Unlink the already linked team and verify that the team - {_teamResponse.Name} is successfully linked to the Jira board - {JiraDataCenter.JiraBoard}.");
            linkTeamPage.UnlinkAlreadyLinkedTeam(JiraDataCenter.JiraBoard);
            linkTeamPage.LinKTeam(JiraDataCenter.JiraProjectName, JiraDataCenter.JiraBoard, _teamResponse.Name);

            Log.Info("Go to Team Assessment 'Growth Items' tab verify 'Sync Committed Items' button and 'Sync Jira' button is displayed");
            growthItemsPage.NavigateToPage(Company.Id, _teamResponse.TeamId);
            Assert.IsTrue(growthItemsPage.IsSyncCommittedItemsButtonDisplayed(), "'Sync Committed Items' button is not displayed");
            Assert.IsTrue(growthItemsPage.IsSyncJiraButtonDisplayed(growthItemInfo.Title), "'Sync Jira' button is not displayed");
            Assert.IsTrue(growthItemsPage.IsGiUpToDateToasterMessageDisplayed(), "Toaster message is not displayed");

            Log.Info("Click on 'Sync Jira' button, Verify toaster message and 'External Id' is present");
            growthItemsPage.ClickOnGrowthItemSyncJiraButton(growthItemInfo.Title);
            Assert.IsTrue(growthItemsPage.IsLinkSuccessfullyToasterMessageDisplayed(), "'Link Successfully' toaster message is not displayed");
            var actualGrowthItem = growthItemsPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.IsNotNull(actualGrowthItem.ExternalId, "'External Id' is null");

            Log.Info("Click on 'UnSync Jira' button, verify toaster message and add new GI with 'Committed' status");
            growthItemsPage.ClickOnGrowthItemUnSyncJiraButton(growthItemInfo.Title);
            Assert.IsTrue(growthItemsPage.IsUnlinkSuccessfullyToasterMessageDisplayed(), "'Unlink Successfully' toaster message is not displayed");

            growthItemsPage.ClickAddNewItemButton();
            growthItemInfo.Status = "Committed";
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
            addGrowthItemPopup.ClickSaveButton();

            Log.Info("Click on 'Sync Committed Items' button, verify 'External Id' is present and 'UnSync Jira' button is displayed");
            growthItemsPage.ClickOnSyncCommittedItemsButton();

            Assert.IsNotNull(actualGrowthItem.ExternalId, "'External Id' is null");
            Assert.IsTrue(growthItemsPage.IsUnSyncJiraButtonDisplayed(growthItemInfo.Title), "'UnSync Jira' button is not displayed");
        }
    }
}