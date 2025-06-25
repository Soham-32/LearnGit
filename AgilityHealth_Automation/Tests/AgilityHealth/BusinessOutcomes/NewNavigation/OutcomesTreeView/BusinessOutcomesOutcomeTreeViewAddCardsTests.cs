using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.OutComeTreeView;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OutcomesTreeView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesOutcomeTreeViewAddCardsTests : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 52632
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OutcomeTreeView_Create_1YearOutComeCard()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesOutcomeTreeViewPage = new BusinessOutcomesOutcomeTreeViewPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeCardOwner = "Automated Testing";
            var oneYearBusinessOutcomeCardDetails = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, SwimlaneType.StrategicTheme,
                new List<string> { businessOutcomeCardOwner });

            Log.Info("Login to application, Go to Achieve outcomes, Navigate to the Outcome Tree View, and create 1-Year Business Outcome Card");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.ClickOnAchieveOutcomeTab();
            businessOutcomeBasePage.ClickOnOutcomeTreeViewTab();
            businessOutcomesOutcomeTreeViewPage.SelectYearOutcomeType("1");

            Log.Info("Filling out and saving business outcome details.");
            businessOutcomeBasePage.FillOutBusinessOutcomeCardDetails(oneYearBusinessOutcomeCardDetails);

            if (!string.IsNullOrEmpty(businessOutcomeCardOwner))
                businessOutcomeBasePage.SetCardOwner(businessOutcomeCardOwner);

            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info($"Validating that the business outcome card '{oneYearBusinessOutcomeCardDetails.Title}' is displayed in Outcome Tree View and Card View.");
            businessOutcomesOutcomeTreeViewPage.SortByDescending();
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(oneYearBusinessOutcomeCardDetails.Title), $"ERROR: '{oneYearBusinessOutcomeCardDetails.Title}' - Business outcome card is NOT displayed in Outcome Tree View.");

            businessOutcomeBasePage.ClickOnCardViewTab();
            businessOutcomeBasePage.CardSearch(oneYearBusinessOutcomeCardDetails.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsCardPresentInSwimLane("1 Year Outcome", oneYearBusinessOutcomeCardDetails.Title), $"ERROR: '{oneYearBusinessOutcomeCardDetails.Title}' - Business outcome card is NOT found in '1 Year Outcomes' swimlane.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 52632
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OutcomeTreeView_Create_3YearOutComeCard()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesOutcomeTreeViewPage = new BusinessOutcomesOutcomeTreeViewPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeCardOwner = "Automated Testing";
            var threeYearBusinessOutcomeCardDetails = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, SwimlaneType.StrategicTheme,
                new List<string> { businessOutcomeCardOwner });

            Log.Info("Login to application, Go to Achieve outcomes, Navigate to the Outcome Tree View, and create 3-Year Business Outcome Card");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.ClickOnAchieveOutcomeTab();
            businessOutcomeBasePage.ClickOnOutcomeTreeViewTab();
            businessOutcomesOutcomeTreeViewPage.SelectYearOutcomeType("3");

            Log.Info("Filling out and saving business outcome details.");
            businessOutcomeBasePage.FillOutBusinessOutcomeCardDetails(threeYearBusinessOutcomeCardDetails);

            if (!string.IsNullOrEmpty(businessOutcomeCardOwner))
                businessOutcomeBasePage.SetCardOwner(businessOutcomeCardOwner);

            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info($"Validating that the business outcome card '{threeYearBusinessOutcomeCardDetails.Title}' is displayed in Outcome Tree View and Card View.");
            businessOutcomesOutcomeTreeViewPage.SelectOutcomeType("3");
            businessOutcomesOutcomeTreeViewPage.SortByDescending();
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(threeYearBusinessOutcomeCardDetails.Title), $"ERROR: '{threeYearBusinessOutcomeCardDetails.Title}' - Business outcome card is NOT displayed in Outcome Tree View.");

            businessOutcomeBasePage.ClickOnCardViewTab();
            businessOutcomeBasePage.CardSearch(threeYearBusinessOutcomeCardDetails.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsCardPresentInSwimLane("3 Year Outcomes", threeYearBusinessOutcomeCardDetails.Title), $"ERROR: '{threeYearBusinessOutcomeCardDetails.Title}' - Business outcome card is NOT found in '3 Year Outcomes' swimlane.");

        }
    }
}

