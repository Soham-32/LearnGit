﻿using AgilityHealth_Automation.ObjectFactories.NewNavigation.GrowthPlan;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("GrowthItem"), TestCategory("NewNavigation")]
    public class GrowthItemTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 46547
        public void GrowthItem_MultiTeam_GridView_AddEditDelete()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.NavigateToPage(Company.Id);
            SwitchToNewNav();

            Log.Info("Click on Measure tab");
            teamDashboardPage.ClickOnMeasureTab();

            Log.Info("Expand Enterprise, Portfolio");
            leftNavPage.ExpandTeam(SharedConstants.EnterpriseTeam);
            leftNavPage.ExpandTeam(SharedConstants.PortfolioTeam);

            Log.Info("Click on a multi team");
            leftNavPage.ClickOnATeam(SharedConstants.MultiTeam);

            Log.Info("Switch to Grid view");
            growthItemGridViewPage.SwitchToGridView();

            Log.Info("Click on Add Growth Item button");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();

            Log.Info("Add a new growth item");
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            growthEditedItemInfo.Category = "Enterprise";

            growthItemGridViewPage.EnterGrowthItemInfo(growthItemInfo);
            growthItemGridViewPage.ClickOnSaveButton();

            Log.Info("Verify that the added growth showing correctly in the grid");
            var actualGrowthItem = growthItemGridViewPage.GetGrowthItemFromGrid(growthItemInfo.Title);
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthItemInfo.TargetDate?.Date, actualGrowthItem.TargetDate?.Date,
                "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");

            Log.Info($"'Edit' {growthItemInfo.Title} GI");
            growthItemGridViewPage.ClickGrowthItemEditButton(growthItemInfo.Title);

            growthItemGridViewPage.EnterGrowthItemInfo(growthEditedItemInfo);
            growthItemGridViewPage.ClickOnSaveButton();

            Log.Info("Verify that the edited growth showing correctly in the grid");
            var editedGrowthItem = growthItemGridViewPage.GetGrowthItemFromGrid(growthEditedItemInfo.Title);
            Assert.AreEqual(growthEditedItemInfo.Category, editedGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Type, editedGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Title, editedGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Status, editedGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Priority, editedGrowthItem.Priority, "Priority doesn't match");
            Assert.AreEqual(growthEditedItemInfo.TargetDate?.Date, editedGrowthItem.TargetDate?.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Size, editedGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Description, editedGrowthItem.Description,
                "Description doesn't match");
            Assert.AreEqual(growthEditedItemInfo.Color, editedGrowthItem.Color, "Color doesn't match");

            //Delete growth item is not available yet in New Nav
        }
    }
}
