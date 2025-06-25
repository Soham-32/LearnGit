using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.Obstacles
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesObstaclesEditTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _businessOutcomeResponse, _deliverableCardResponse, _initiativeCardResponse, _storiesCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            // Create BusinessOutcome with Obstacles

            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var deliverable = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription()).Tags;
            var initiatives = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;
            var stories = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.Monthly.GetDescription()).Tags;

            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.DeliveryColumn, 0, deliverable, null, new BusinessOutcomeRequest()
            {
                Obstacles = new List<BusinessOutcomeObstaclesRequest>
                {
                    BusinessOutcomesFactory.GetBusinessOutcomeObstacleRequest(),
                }
            });
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives, null, new BusinessOutcomeRequest()
            {
                Obstacles = new List<BusinessOutcomeObstaclesRequest>
                {
                    BusinessOutcomesFactory.GetBusinessOutcomeObstacleRequest(),
                }
            });
            _storiesCardResponse = CreateBusinessOutcome(SwimlaneType.Stories, 0, stories, null, new BusinessOutcomeRequest()
            {
                Obstacles = new List<BusinessOutcomeObstaclesRequest>
                {
                    BusinessOutcomesFactory.GetBusinessOutcomeObstacleRequest(),
                }
            });
            _businessOutcomeResponse = CreateBusinessOutcome(
                SwimlaneType.StrategicIntent,
                0,
                null,
                null,
                new BusinessOutcomeRequest()
                {
                    Obstacles = new List<BusinessOutcomeObstaclesRequest>
                    {
                        BusinessOutcomesFactory.GetBusinessOutcomeObstacleRequest(),
                    }
                }
            );
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Obstacles_Edit()
        {
            EditObstacles(_businessOutcomeResponse, BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), BusinessOutcomesCardTypeTags.Annually.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Obstacles_Edit()
        {
            EditObstacles(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives_Obstacles_Edit()
        {
            EditObstacles(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(), BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Stories_Obstacles_Edit()
        {
            EditObstacles(_storiesCardResponse, BusinessOutcomesCardType.Monthly.GetDescription(), BusinessOutcomesCardTypeTags.Monthly.GetDescription());
        }

        private void EditObstacles(BusinessOutcomeResponse createdCardResponse, string cardType, string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var obstaclesTabPage = new ObstaclesTabPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the selected card type as {cardType} ");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(cardTypeTag);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());

            Log.Info($"Click on the created Card for the {cardType} and navigate to Obstacles");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Obstacles");
            var obstacles = new List<BusinessOutcomeObstaclesRequest>
                { BusinessOutcomesFactory.GetBusinessOutcomeObstacleRequest()};
            obstacles.ForEach(obstacle => addBusinessOutcomePage.ObstaclesTab.AddObstacle(obstacle, new List<string>() { User.FullName }, true));
            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info($"Click on the created Card for the {cardType} and navigate to Obstacles");
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Obstacles");

            Log.Info("Verify the added obstacles are displayed");
            var actualObstacles = obstaclesTabPage.GetObstaclesResponse();

            Assert.AreEqual(1, actualObstacles.Count, "Obstacles count does not match");

            for (var i = 0; i < actualObstacles.Count; i++)
            {
                Assert.AreEqual(obstacles[i].Title, actualObstacles[i].Title, "Obstacles title does not match");
                Assert.AreEqual(CSharpHelpers.GetNormalizedText(obstacles[i].Description), CSharpHelpers.GetNormalizedText(actualObstacles[i].Description), "Obstacles Description does not match");
                Assert.AreEqual(((BusinessOutcomesObstacles)obstacles[i].ObstacleType).ToString(), ((BusinessOutcomesObstacles)actualObstacles[i].ObstacleType).ToString(), "Obstacles Type does not match");
                var roam = obstacles[i].ObstacleType;
                if (roam == 2)
                    Assert.AreEqual(((RoamType)obstacles[i].Roam).ToString(),
                        ((RoamType)actualObstacles[i].Roam).ToString(), "Obstacles ROAM does not match");
                //Bug 52427
                //  Assert.AreEqual(obstacles[i].Impact, actualObstacles[i].Impact, "Obstacles Impact does not match");
                // Assert.AreEqual(obstacles[i].ObstacleOwners.First().UserId, actualObstacles.First().ObstacleOwners.First().DisplayName, "Obstacle owners does not match");
                Assert.AreEqual(((StatusType)obstacles[i].Status).ToString(), ((StatusType)actualObstacles[i].Status).ToString(), "Obstacles ROAM does not match");
                Assert.AreEqual(obstacles[i].EndDate, actualObstacles[i].EndDate, "Obstacles End Dates does not match");
            }

            addBusinessOutcomePage.ClickOnCloseIcon();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Obstacles");
            Assert.AreEqual(1, actualObstacles.Count, "Obstacles count does not match");

        }
    }
}