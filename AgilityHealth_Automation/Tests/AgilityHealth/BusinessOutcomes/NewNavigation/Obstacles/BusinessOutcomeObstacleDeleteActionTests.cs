using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.Obstacles
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomeObstacleDeleteActionTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _businessOutcomeResponse, _deliverableCardResponse, _initiativeCardResponse, _storiesCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var deliverables = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription()).Tags;
            var initiatives = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;
            var stories = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.Monthly.GetDescription()).Tags;
            _businessOutcomeResponse = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.DeliveryColumn, 0, deliverables);
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives);
            _storiesCardResponse = CreateBusinessOutcome(SwimlaneType.Stories, 0, stories);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Obstacles_Delete()
        {
            DeleteObstacles(_businessOutcomeResponse, BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), BusinessOutcomesCardTypeTags.Annually.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Obstacles_Delete()
        {
            DeleteObstacles(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives_Obstacles_Delete()
        {
            DeleteObstacles(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(), BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Stories_Obstacles_Delete()
        {
            DeleteObstacles(_storiesCardResponse, BusinessOutcomesCardType.Monthly.GetDescription(), BusinessOutcomesCardTypeTags.Monthly.GetDescription());
        }

        private void DeleteObstacles(BusinessOutcomeResponse createdCardResponse, string cardType, string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the selected card type as {cardType} ");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(cardTypeTag);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);

            Log.Info($"Click on the created Card for the {cardType} and navigate to Obstacles");
            addBusinessOutcomePage.ClickOnTab("Obstacles");
            var obstacles = new List<BusinessOutcomeObstaclesRequest>
                {BusinessOutcomesFactory.GetBusinessOutcomeObstacleRequest(new List<string>() { User.FullName })};
            obstacles.ForEach(obstacle => addBusinessOutcomePage.ObstaclesTab.AddObstacle(obstacle, new List<string>() { User.FullName }));
            var obstacleResponse = addBusinessOutcomePage.ObstaclesTab.GetObstaclesResponse();
            businessOutcomeBasePage.ClickOnSaveButton();

            Log.Info($"Verify the deleted obstacle is not displayed");
            addBusinessOutcomePage.ClickOnTab("Obstacles");
            addBusinessOutcomePage.ObstaclesTab.DeleteObstacle(obstacles.FirstOrDefault()?.Title);
            Assert.IsFalse(addBusinessOutcomePage.ObstaclesTab.IsObstacleTitleDisplayed(obstacleResponse.FirstOrDefault()?.Title), "Obstacle Title is displayed");

            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            addBusinessOutcomePage.ClickOnTab("Obstacles");
            Assert.IsFalse(addBusinessOutcomePage.ObstaclesTab.IsObstacleTitleDisplayed(obstacleResponse.FirstOrDefault()?.Title), "Obstacle Title is displayed");

        }
    }
}
