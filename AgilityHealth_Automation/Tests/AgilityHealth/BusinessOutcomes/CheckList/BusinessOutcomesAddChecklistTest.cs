using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.CheckList
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesAddChecklistTest : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _businessOutcomeResponse, _deliverableCardResponse, _initiativeCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var deliverable = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription()).Tags;
            var initiatives = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;
            _businessOutcomeResponse = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, deliverable);
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Checklist_Add()
        {
            ChecklistValidator(_businessOutcomeResponse, BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), BusinessOutcomesCardTypeTags.Annually.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Checklist_Add()
        {
            ChecklistValidator(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives_Checklist_Add()
        {
            ChecklistValidator(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(),BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }

        private void ChecklistValidator(BusinessOutcomeResponse createdCardResponse, string cardType,string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var checklistTab = new CheckListTabPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the selected card type as {cardType} ");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(cardTypeTag);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());

            Log.Info($"Click on the created Card for the {cardType} and navigate to checklist");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Checklist");

            Log.Info("Add the checklists , Save and close the card");
            var checklistItem1 = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });
            checklistTab.EnterCheckListItem(checklistItem1);
            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info($"Click on the created Card for the {cardType} and navigate to checklist");
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Checklist");

            Log.Info("Verify the added checklist are displayed");
            var actualChecklist = checklistTab.GetCheckListResponse();
            Assert.AreEqual(1, actualChecklist.Count, "Check List count does not match");
            Assert.AreEqual(checklistItem1.ItemText, actualChecklist.First().ItemText,"Check list title does not match");
            Assert.AreEqual(checklistItem1.Owners.First(), actualChecklist.First().Owners.First().DisplayName,"Check list owners does not match");

            Log.Info("Create the new checklist and Update the Description , Save and close the card");
            var checklistItem2 = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });

            checklistTab.EnterCheckListItem(checklistItem2);
            businessOutcomeBasePage.EnterDescription("Updated Description");
            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info("Verify the created checklist and existing created checklist");
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Checklist");
            var expectedChecklists = new List<UpdateBusinessOutcomeChecklistItemRequest> { checklistItem1, checklistItem2 };
            actualChecklist = checklistTab.GetCheckListResponse();
            Assert.AreEqual(2, actualChecklist.Count, "Check List count does not match");
            Assert.That.ListsAreEqual(expectedChecklists.Select(text => text.ItemText).ToList(), actualChecklist.Select(text => text.ItemText).ToList(),"Check list item text does not match");
            Assert.That.ListsAreEqual(expectedChecklists.Select(own => own.Owners.First().ToString()).ToList(), actualChecklist.Select(own => own.Owners.First().DisplayName.ToString()).ToList(),"Check list owners does not match");

            Log.Info("Verify the overall Progress Percentage and Overall Progress Indication ");
            var expectedChecklistPercentage = GetChecklistProgressPercentage(new List<UpdateBusinessOutcomeChecklistItemRequest> { checklistItem1, checklistItem2 });

            Assert.AreEqual($"{expectedChecklistPercentage}%",
                checklistTab.GetChecklistProgressPercentage(),
                "Checklist Progress percentage doesn't match");

            //Verifying overall progress indicator
            var expectedChecklistProgress = $"transform: translateX({float.Parse(expectedChecklistPercentage) - 100}%);";
            Assert.AreEqual(expectedChecklistProgress,
                checklistTab.GetChecklistProgressIndication(),
                "Checklist progress indication doesn't match");
        }
    }
}
