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

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.Dependency
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesAddDependencyTest : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _deliverableCardResponse, _storiesCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var deliverables = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription()).Tags;
            var stories = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.Monthly.GetDescription()).Tags;//
            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.DeliveryColumn, 0, deliverables);
            _storiesCardResponse = CreateBusinessOutcome(SwimlaneType.Stories, 0, stories);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Add_DependencyStoriesCardToDeliverables()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var deliverableCardPage = new DeliverableCardPage(Driver, Log);
            var storiesCardPage = new StoriesCardPage(Driver, Log);
            var businessOutcomeCardOwner = "Automated Testing";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);

            Log.Info("Navigate to Stories CardType and click on the card title");
            businessOutcomesDashboard.SelectCardType(BusinessOutcomesCardType.Monthly.GetDescription());
            businessOutcomesDashboard.SelectCardTypeFromDropdown(BusinessOutcomesCardTypeTags.Monthly.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_storiesCardResponse.Title);
            businessOutcomeBasePage.SetCardOwner(businessOutcomeCardOwner);

            Log.Info("Create Checklist Items and save the card");
            storiesCardPage.ClickOnTab("Checklist");
            var checklistItem1 = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });
            storiesCardPage.CheckListTab.EnterCheckListItem(checklistItem1);
            var checklistItem2 = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });
            storiesCardPage.CheckListTab.EnterCheckListItem(checklistItem2);
            storiesCardPage.CheckListTab.CompleteChecklistCheckBox(1);
            checklistItem1.IsComplete = true;
            var actualDependencyPercentage = storiesCardPage.StoriesTab.GetStoriesOverallProgressInfo();
            storiesCardPage.ClickOnSaveAndCloseButton();

            Log.Info("Navigate to Deliverables CardType and open the created card");
            businessOutcomesDashboard.SelectCardType(BusinessOutcomesCardType.DeliverablesTimeline.GetDescription());
            businessOutcomesDashboard.SelectCardTypeFromDropdown(BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
            businessOutcomeBasePage.CardSearch(_deliverableCardResponse.Title);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_deliverableCardResponse.Title);

            Log.Info("Add a dependency card");
            deliverableCardPage.ClickOnTab("Dependencies");
            deliverableCardPage.DependenciesTab.ClickOnDependencyAddButton();
            deliverableCardPage.DependenciesTab.AddDependencyCard(_storiesCardResponse.Title);

            deliverableCardPage.ClickOnSaveAndCloseButton();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_deliverableCardResponse.Title);
            deliverableCardPage.ClickOnTab("Dependencies");

            var actualDependencyCardInfo = deliverableCardPage.DependenciesTab.GetDependencyInfo();
            var expectedDependencyPercentage = deliverableCardPage.DependenciesTab.GetDependencyProgressPercentage();
            Assert.AreEqual($"{_storiesCardResponse.Title} - {businessOutcomeCardOwner}", actualDependencyCardInfo.First(),"Dependency added is incorrect");
            Assert.AreEqual($"{expectedDependencyPercentage}", actualDependencyPercentage,
                "Dependency Progress percentage doesn't match");
        }
    }
}
