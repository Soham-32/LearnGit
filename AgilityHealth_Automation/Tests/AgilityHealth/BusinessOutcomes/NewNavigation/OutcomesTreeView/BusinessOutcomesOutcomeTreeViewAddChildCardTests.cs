using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.OutComeTreeView;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OutcomesTreeView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesOutcomeTreeViewAddChildCardTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicTheme);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OutcomeTreeView_AddChildCards()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesOutcomeTreeViewPage = new BusinessOutcomesOutcomeTreeViewPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var checklistTab = new CheckListTabPage(Driver, Log);

            Log.Info("Login to application, Go to Achieve outcomes, Navigate to the Outcome Tree View, and create child cards.");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.ClickOnAchieveOutcomeTab();
            businessOutcomeBasePage.ClickOnOutcomeTreeViewTab();
            businessOutcomesOutcomeTreeViewPage.SortByDescending();
            Log.Info($"Validating that the business outcome card '{_response.Title}' is displayed in Outcome Tree View");
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(_response.Title),
                 $"ERROR: '{_response.Title}' - Business outcome card is NOT displayed in Outcome Tree View.");

            Log.Info($"Add Key result into '{_response.Title}' card & validate that displayed on outcome tree view page");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddTabValue("Key Result", _response.Title);
            var keyResultDetails = BusinessOutcomesFactory.GetKeyResultRequest(Company.Id);
            addBusinessOutcomePage.KeyResultsTab.AddKeyResult(keyResultDetails);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesOutcomeTreeViewPage.ClickOnCardExpandCollapseIcon(_response.Title);
            businessOutcomesOutcomeTreeViewPage.ColumnResizer("Key Results");
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(keyResultDetails.Title), $"ERROR: '{keyResultDetails.Title}' - Key Result is NOT displayed in Outcome Tree View.");

            Log.Info($"Add initiatives into '{_response.Title}' card & validate that displayed on outcome tree view page");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddTabValue("Initiatives", _response.Title);
            var initiativesCardDetails = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, SwimlaneType.Initiatives);
            businessOutcomeBasePage.FillOutChildCardDetails(initiativesCardDetails);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesOutcomeTreeViewPage.ClickOnCardExpandCollapseIcon(_response.Title);
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(initiativesCardDetails.Title), $"ERROR: '{initiativesCardDetails.Title}' - Initiatives card is NOT displayed in Outcome Tree View.");

            Log.Info($"Add project into '{initiativesCardDetails.Title}' card & validate that displayed on outcome tree view page");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddTabValue("Projects", initiativesCardDetails.Title);
            var projectCardDetails = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, SwimlaneType.Projects);
            businessOutcomeBasePage.FillOutChildCardDetails(projectCardDetails);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesOutcomeTreeViewPage.ClickOnCardExpandCollapseIcon(initiativesCardDetails.Title);
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(projectCardDetails.Title), $"ERROR: '{projectCardDetails.Title}' - Project card is NOT displayed in Outcome Tree View.");

            Log.Info($"Add deliverables into '{projectCardDetails.Title}' card & validate that displayed on outcome tree view page");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddTabValue("Deliverables", projectCardDetails.Title);
            var deliverablesCardDetails = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, SwimlaneType.DeliveryColumn);
            businessOutcomeBasePage.FillOutChildCardDetails(deliverablesCardDetails);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesOutcomeTreeViewPage.ClickOnCardExpandCollapseIcon(projectCardDetails.Title);
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(deliverablesCardDetails.Title), $"ERROR: '{deliverablesCardDetails.Title}' - Deliverables card is NOT displayed in Outcome Tree View.");

            Log.Info($"Add stories into '{deliverablesCardDetails.Title}' card & validate that displayed on outcome tree view page");
            businessOutcomesOutcomeTreeViewPage.ColumnResizer("Card Title");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddTabValue("Stories", deliverablesCardDetails.Title);
            var storiesCardDetails = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, SwimlaneType.Stories);
            businessOutcomeBasePage.FillOutChildCardDetails(storiesCardDetails);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesOutcomeTreeViewPage.ClickOnCardExpandCollapseIcon(deliverablesCardDetails.Title);
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(storiesCardDetails.Title), $"ERROR: '{storiesCardDetails.Title}' - Stories card is NOT displayed in Outcome Tree View.");

            Log.Info($"Add checklist  into '{storiesCardDetails.Title}' card & validate that displayed on outcome tree view page");
            businessOutcomesOutcomeTreeViewPage.ColumnResizer("Card Title");
            businessOutcomesOutcomeTreeViewPage.ClickOnAddTabValue("Checklist", storiesCardDetails.Title);
            var checklistDetails = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });
            checklistTab.EnterCheckListItem(checklistDetails);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesOutcomeTreeViewPage.ClickOnCardExpandCollapseIcon(storiesCardDetails.Title);
            Assert.IsTrue(businessOutcomesOutcomeTreeViewPage.IsCardDisplayed(checklistDetails.ItemText), $"ERROR: '{checklistDetails.ItemText}' - Checklist is NOT displayed in Outcome Tree View.");
        }
    }
}

