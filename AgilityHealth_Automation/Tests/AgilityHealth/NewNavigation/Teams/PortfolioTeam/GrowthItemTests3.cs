using AgilityHealth_Automation.ObjectFactories.NewNavigation.GrowthPlan;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("GrowthItem"), TestCategory("NewNavigation")]
    public class GrowthItemTests3 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GrowthItem_PortfolioTeam_AddEditDeleteComment()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewPage(Driver, Log);
            var addGrowthItemCommentPopupPage = new AddGrowthItemCommentPopupPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.NavigateToPage(Company.Id);
            SwitchToNewNav();

            Log.Info("Click on Measure tab");
            teamDashboardPage.ClickOnMeasureTab();

            Log.Info("Click on Growth Items Tab");
            teamDashboardPage.ClickOnGrowthItemTab();

            Log.Info("Expand Enterprise");
            leftNavPage.ExpandTeam(SharedConstants.EnterpriseTeam);

            Log.Info("Click on a portfolio team");
            leftNavPage.ClickOnATeam(SharedConstants.PortfolioTeam);

            Log.Info("Switch to Grid view");
            growthItemGridViewPage.SwitchToGridView();

            Log.Info("Click on Add Growth Item button");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            growthItemInfo.Category = "";
            growthItemGridViewPage.EnterGrowthItemInfo(growthItemInfo);

            Log.Info("Save growth item");
            growthItemGridViewPage.ClickOnSaveButton();

            Log.Info($"'Edit' {growthItemInfo.Title} GI");
            growthItemGridViewPage.ClickGrowthItemEditButton(growthItemInfo.Title);

            Log.Info("Click on Comment tab");
            addGrowthItemCommentPopupPage.ClickOnCommentTab();

            Log.Info("Add a new comment");
            var commentContent = "This is a test comment";
            addGrowthItemCommentPopupPage.AddGrowthItemComment(commentContent);
            addGrowthItemCommentPopupPage.ClickOnCommentSaveButton();

            Log.Info("Verify that comment is added properly");
            Assert.IsTrue(addGrowthItemCommentPopupPage.IsCommentPresent(commentContent), $"Comment - {commentContent} is not present");

            Log.Info("Update the comment");
            var updatedCommentContent = "This is an updated test comment";
            addGrowthItemCommentPopupPage.UpdateGrowthItemComment(commentContent, updatedCommentContent);
            addGrowthItemCommentPopupPage.ClickOnCommentUpdateButton();

            Log.Info("Verify that comment is updated");
            Assert.IsTrue(addGrowthItemCommentPopupPage.IsCommentPresent(updatedCommentContent), $"Comment - {updatedCommentContent} is not present");
            Assert.IsFalse(addGrowthItemCommentPopupPage.IsCommentPresent(commentContent), $"Comment- {commentContent} is Present");

            Log.Info("Save growth item");
            growthItemGridViewPage.ClickOnSaveButton();

            Log.Info($"'Edit' {growthItemInfo.Title} GI");
            growthItemGridViewPage.ClickGrowthItemEditButton(growthItemInfo.Title);

            Log.Info("Click on Comment tab");
            addGrowthItemCommentPopupPage.ClickOnCommentTab();

            Log.Info("Verify that updated comment showing");
            Assert.IsTrue(addGrowthItemCommentPopupPage.IsCommentPresent(updatedCommentContent), $"Comment - {updatedCommentContent} is not present");

            Log.Info("Delete the comment");
            addGrowthItemCommentPopupPage.DeleteGrowthItemComment(updatedCommentContent);

            Assert.IsFalse(addGrowthItemCommentPopupPage.IsCommentPresent(updatedCommentContent), $"Comment- {commentContent} is Present");
        }
    }
}
