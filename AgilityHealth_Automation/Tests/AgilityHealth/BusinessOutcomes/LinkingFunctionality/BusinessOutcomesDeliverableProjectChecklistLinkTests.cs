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

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.LinkingFunctionality
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDeliverableProjectChecklistLinkTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _deliverableCardResponse, _projectsCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var deliverable = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription()).Tags;
            var projects = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.ProjectsTimeline.GetDescription()).Tags;
            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.DeliveryColumn, 0, deliverable);
            _projectsCardResponse = CreateBusinessOutcome(SwimlaneType.Projects, 0, projects);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_Project_Deliverable_Checklist_LinkTests()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var deliverableCardPage = new DeliverableCardPage(Driver, Log);
            var projectCardPage = new ProjectsCardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            Log.Info("Navigate to Deliverables CardType");
            businessOutcomesDashboard.SelectCardType("Deliverables");
            businessOutcomesDashboard.SelectCardTypeFromDropdown("Deliverables Timeline");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_deliverableCardResponse.Title);

            Log.Info("Create Checklist Items and verify checklist items are added successfully");
            deliverableCardPage.ClickOnTab("Checklist");
            var checklistItem1 = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });
            deliverableCardPage.CheckListTab.EnterCheckListItem(checklistItem1);
            var checklistItem2 = BusinessOutcomesFactory.GetChecklistItemRequest(new List<string>() { User.FullName });
            deliverableCardPage.CheckListTab.EnterCheckListItem(checklistItem2);
            deliverableCardPage.ClickOnSaveAndCloseButton();
            businessOutcomeBasePage.CardSearch(_deliverableCardResponse.Title);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_deliverableCardResponse.Title);
            deliverableCardPage.ClickOnTab("Checklist");

            var expectedChecklists = new List<UpdateBusinessOutcomeChecklistItemRequest> { checklistItem1, checklistItem2 };
            var actualChecklist = deliverableCardPage.CheckListTab.GetCheckListResponse();
            Assert.AreEqual(2, actualChecklist.Count, "Check List count does not match");

            for (var i = 0; i < expectedChecklists.Count; i++)
            {
                Assert.AreEqual(checklistItem1.ItemText, actualChecklist.First().ItemText, "Checklist Title doesn't match");
                Assert.AreEqual(checklistItem1.Owners.First(), actualChecklist.First().Owners.First().DisplayName, "Checklist owner doesn't match");
            }

            var expectedChecklistPercentage = GetChecklistProgressPercentage(expectedChecklists);
            Assert.AreEqual($"{expectedChecklistPercentage}%", deliverableCardPage.CheckListTab.GetChecklistProgressPercentage(),
                "Checklist Progress percentage doesn't match");

            //Verifying overall progress indicator
            var expectedChecklistProgress = $"transform: translateX({float.Parse(expectedChecklistPercentage) - 100}%);";
            Assert.AreEqual(expectedChecklistProgress, deliverableCardPage.CheckListTab.GetChecklistProgressIndication(), "Checklist progress indication doesn't match");

            deliverableCardPage.ClickOnCloseIcon();

            Log.Info("Navigate to Initiatives CardType");
            businessOutcomesDashboard.SelectCardType("Projects");
            businessOutcomesDashboard.SelectCardTypeFromDropdown("Projects Timeline");
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely("Q1 (Jan-March)");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_projectsCardResponse.Title);

            Log.Info("Add Child Card for the deliverables tab and verify that the child card is linked successfully");
            projectCardPage.ClickOnTab("Deliverables");
            projectCardPage.DeliverableTab.ClickOnSelectChildCardButton();
            projectCardPage.DeliverableTab.SelectChildCards(new List<string> { _deliverableCardResponse.Title });

            var actualDeliverableChildCard = projectCardPage.DeliverableTab.GetDeliverableChildCardResponse();
            Assert.AreEqual(_deliverableCardResponse.Title, actualDeliverableChildCard.First().Title, "Child Outcome does not match");
            Assert.AreEqual(expectedChecklistPercentage, $"{actualDeliverableChildCard.First().Progress}",
                "Linked Items percentage does not match");
            Assert.AreEqual(1, actualDeliverableChildCard.Count,
                "Linked Items does not match");

            var expectedDeliverableProgressPercentage = GetDeliverablesProgressPercentage(projectCardPage.DeliverableTab.GetDeliverableChildCardResponse());
            Assert.AreEqual($"{expectedDeliverableProgressPercentage}%",
                projectCardPage.DeliverableTab.GetDeliverableProgressPercentage(),
                "Deliverable Progress percentage doesn't match");

            deliverableCardPage.ClickOnSaveButton();

            Log.Info("Select the added child card and check one of the checklist item for the Deliverables card , Verify the Overall checklist Percentage and Indication");
            projectCardPage.DeliverableTab.ClickOnChildCardTitle();
            deliverableCardPage.ClickOnTab("Checklist");
            deliverableCardPage.CheckListTab.CompleteChecklistCheckBox(1);
            checklistItem1.IsComplete = true;

            var expectedCompleteChecklistPercentage =
                GetChecklistProgressPercentage(expectedChecklists);
            Assert.AreEqual($"{expectedCompleteChecklistPercentage}%",
                deliverableCardPage.CheckListTab.GetChecklistProgressPercentage(),
                "Checklist Progress percentage doesn't match");

            deliverableCardPage.ClickOnSaveAndCloseButton();

            Log.Info("Verify the Initiatives updated Overall Progress percentage ");
            projectCardPage.ClickOnCloseIcon();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_projectsCardResponse.Title);
            projectCardPage.ClickOnTab("Deliverables");

            expectedDeliverableProgressPercentage = GetDeliverablesProgressPercentage(projectCardPage.DeliverableTab.GetDeliverableChildCardResponse());
            Assert.AreEqual($"{expectedDeliverableProgressPercentage}%",
                projectCardPage.DeliverableTab.GetDeliverableProgressPercentage(),
                "Deliverable Progress percentage doesn't match");
        }
    }
}