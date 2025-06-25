using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.OutComeTreeView;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OutcomesTreeView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesOutcomeTreeViewHeaderFooterTests : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug Id : 52632
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OutcomeTreeView_Headers()
        {
            var loginPage = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesOutcomeTreeViewPage = new BusinessOutcomesOutcomeTreeViewPage(Driver, Log);

            Log.Info("Login to application, Go to Achieve outcomes, and navigate to the Outcome Tree View.");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.ClickOnAchieveOutcomeTab();
            businessOutcomeBasePage.ClickOnOutcomeTreeViewTab();

            Log.Info("Verifying URL, card count, and year outcome selection.");
            Assert.IsTrue(Driver.Url.Contains("tree"), "URL does not contain 'tree'");
            var totalCards = businessOutcomesOutcomeTreeViewPage.GetTotalCards().ToInt();
            Assert.IsTrue(totalCards >= 0, "Card count is inappropriate");

            businessOutcomesOutcomeTreeViewPage.SelectOutcomeType("3");
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.GetSelectedYearView().Contains("3"), "Selected year does not match 3-year view");

            businessOutcomesOutcomeTreeViewPage.SelectOutcomeType("1");
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.GetSelectedYearView().Contains("1"), "Selected year does not match 1-year view");

            Log.Info("Verify add buttons and Tree folder icon are visible.");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddAnOutcomeButton();
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.DoesAddYearOutcomeButtonExist("1"), "Add 1-year outcome button not displayed");
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.DoesAddYearOutcomeButtonExist("3"), "Add 3-year outcome button not displayed");
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsTreeFolderIconDisplayed(), "Tree folder icon not displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OutcomeTreeView_Pagination()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesOutcomeTreeViewPage = new BusinessOutcomesOutcomeTreeViewPage(Driver, Log);

            Log.Info("Login to application, Go to Achieve outcomes, and navigate to the Outcome Tree View.");

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.ClickOnAchieveOutcomeTab();
            businessOutcomeBasePage.ClickOnOutcomeTreeViewTab();
            Assert.IsTrue(Driver.Url.Contains("tree"), "URL does not contain 'tree'");

            Log.Info("Verifying pagination: navigating buttons on page and checking page sizes.");
            businessOutcomesOutcomeTreeViewPage.IsGoToNextPageButtonDisplayed();
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsGoToNextPageButtonDisplayed(), "Go to next button is not displayed.");

            businessOutcomesOutcomeTreeViewPage.IsGoToPreviousPageButtonDisplayed();
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsGoToPreviousPageButtonDisplayed(), "Go to previous button is not displayed.");

            businessOutcomesOutcomeTreeViewPage.IsGoToLastPageButtonDisplayed();
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsGoToLastPageButtonDisplayed(), "Go to last page button is not displayed.");

            businessOutcomesOutcomeTreeViewPage.ChooseCardsPerPage("20");
            Assert.AreEqual(20, businessOutcomesOutcomeTreeViewPage.GetCurrentCardsPerPage(), "Page size is incorrect");

            businessOutcomesOutcomeTreeViewPage.ChooseCardsPerPage("30");
            Assert.AreEqual(30, businessOutcomesOutcomeTreeViewPage.GetCurrentCardsPerPage(), "Page size is incorrect");
        }
    }
}

