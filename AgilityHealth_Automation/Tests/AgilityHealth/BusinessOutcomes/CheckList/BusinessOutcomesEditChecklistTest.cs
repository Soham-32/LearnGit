using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
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
    public class BusinessOutcomesEditChecklistTest : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _businessOutcomeResponse, _deliverableCardResponse, _initiativeCardResponse;
        private static List<UpdateBusinessOutcomeChecklistItemRequest> _businessOutcomeChecklist, _deliverableChecklist, _initiativeChecklist;


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

            _businessOutcomeChecklist = setupApi.CreateChecklistItemRequest(Company.Id, _businessOutcomeResponse.Uid, 2);

            _deliverableChecklist = setupApi.CreateChecklistItemRequest(Company.Id, _deliverableCardResponse.Uid, 2);

            _initiativeChecklist = setupApi.CreateChecklistItemRequest(Company.Id, _initiativeCardResponse.Uid, 2);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Checklist_Edit()
        {
            ChecklistValidator(_businessOutcomeResponse, BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), _businessOutcomeChecklist, BusinessOutcomesCardTypeTags.Annually.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Checklist_Edit()
        {
            ChecklistValidator(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), _deliverableChecklist, BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives_Checklist_Edit()
        {
            ChecklistValidator(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(), _initiativeChecklist, BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }

        private void ChecklistValidator(BusinessOutcomeResponse createdCardResponse, string cardType, List<UpdateBusinessOutcomeChecklistItemRequest> checklist, string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var checklistTab = new CheckListTabPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            Log.Info($"Navigate to selected {cardType}");
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(cardTypeTag);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);

            Log.Info($"Create the Checklist and verify checklist items are saved successfully");
            businessOutcomeBasePage.ClickOnTab("Checklist");

            //add one more check list item
            for (var row = 1; row <= checklist.Count; row++)
            {
                checklistTab.SelectOwner(new List<string>() { User.FullName }, row);
            }

            businessOutcomeBasePage.ClickOnSaveAndCloseButton();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Checklist");
            var actualChecklist = checklistTab.GetCheckListResponse();

            Assert.That.ListsAreEqual(checklist.Select(text => text.ItemText).ToList(), actualChecklist.Select(text => text.ItemText).ToList(),"Check List items are not equal");
            Assert.IsTrue(actualChecklist.Any(n => n.Owners.Any(m => m.DisplayName == User.FullName)),"Owners does not display");

            var expectedChecklistPercentage = GetChecklistProgressPercentage(checklist);
            Assert.AreEqual($"{expectedChecklistPercentage}%",
                checklistTab.GetChecklistProgressPercentage(),
                "Checklist Progress percentage doesn't match");

            //Verifying overall progress indicator
            var expectedChecklistProgress = $"transform: translateX({float.Parse(expectedChecklistPercentage) - 100}%);";
            Assert.AreEqual(expectedChecklistProgress,
                checklistTab.GetChecklistProgressIndication(),
                "Checklist progress indication doesn't match");

            Log.Info("Complete the checklist and verify the progress percentage and Indication");
            checklistTab.CompleteChecklistCheckBox(1);
            checklist.First().IsComplete = true;

            var expectedCompleteChecklistPercentage =
                GetChecklistProgressPercentage(checklist);
            Assert.AreEqual(2, actualChecklist.Count,"Check List count does not match");
            Assert.That.ListsAreEqual(checklist.Select(text => text.ItemText).ToList(), actualChecklist.Select(text => text.ItemText).ToList(), "Check List items are not equal");
            Assert.IsTrue(actualChecklist.Any(n => n.Owners.Any(m => m.DisplayName == User.FullName)), "Owners does not display");

            Assert.AreEqual($"{expectedCompleteChecklistPercentage}%",
                checklistTab.GetChecklistProgressPercentage(),
                "Checklist Progress percentage doesn't match");

            businessOutcomeBasePage.ClickOnSaveAndCloseButton();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Checklist");
            Assert.AreEqual(2, actualChecklist.Count, "Check List count does not match");
            Assert.That.ListsAreEqual(checklist.Select(text => text.ItemText).ToList(), actualChecklist.Select(text => text.ItemText).ToList(), "Check List items are not equal");
            Assert.AreEqual($"{expectedCompleteChecklistPercentage}%",
                checklistTab.GetChecklistProgressPercentage(),
                "Checklist Progress percentage doesn't match");
        }
    }
}
