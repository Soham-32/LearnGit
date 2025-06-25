using System;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.Documents
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesAddLinkTests : BusinessOutcomesBaseTest
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
            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, deliverables);
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives);
            _storiesCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, stories);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_AddLink()
        {
            AddLinkValidator(_businessOutcomeResponse, BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), BusinessOutcomesCardTypeTags.Annually.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_AddLink()
        {
            AddLinkValidator(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives__AddLink()
        {
            AddLinkValidator(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(), BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Stories__AddLink()
        {
            AddLinkValidator(_storiesCardResponse, BusinessOutcomesCardType.Monthly.GetDescription(), BusinessOutcomesCardTypeTags.Monthly.GetDescription());
        }

        private void AddLinkValidator(BusinessOutcomeResponse createdCardResponse, string cardType, string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var documentsTabPage = new DocumentsTabPage(Driver, Log);
            var cardTabName = "Documents";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the selected card type as {cardType} ");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(cardTypeTag);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());

            Log.Info($"Click on the created Card for the {cardType} and navigate to Obstacles");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab(cardTabName);

            Log.Info("Click on 'Add Document' dropdown and add the link from 'Add A Link' dropdown");
            documentsTabPage.ClickOnUploadALink();
            Assert.IsTrue(documentsTabPage.IsAddLinkPopupDisplayed(),"Add A Link popup is not displayed");

            Log.Info($"Verify the title and Url are added ");
            documentsTabPage.AddLinkPopUp(createdCardResponse.Title, Driver.GetCurrentUrl());
            businessOutcomeBasePage.ClickOnSaveButton();

           var actualDocumentsResponse = documentsTabPage.GetDocumentsResponse().FirstOrDefault();
           Assert.AreEqual(1, documentsTabPage.GetDocumentsResponse().Count, "Added Documents count does not match");

           Assert.AreEqual(createdCardResponse.Title, actualDocumentsResponse.Title, "Documents title does not match");
           Assert.IsNotNull(actualDocumentsResponse.Size, "Document Size does not match");
           Assert.AreEqual(User.FullName, actualDocumentsResponse.AddedBy, "Documents Added By Name doesn't match");
           Assert.AreEqual(DateTime.Today.ToString("dd - MMM - yyyy"), actualDocumentsResponse.Date, "Document date does not match");
           Assert.AreEqual("-", actualDocumentsResponse.Parent, "Document parent is not empty");
           Assert.AreEqual("-", actualDocumentsResponse.CardType, "Document CardType is not empty");
           Assert.AreEqual("-", actualDocumentsResponse.CardTitle, "Document Title is not empty");
           Assert.AreEqual("Draft", actualDocumentsResponse.Status, "Document status does not match");
           Assert.AreEqual("-", actualDocumentsResponse.ApprovedBy, "Document Approved By does not match");
           Assert.AreEqual("-", actualDocumentsResponse.ApprovedDate, "Document Approved Date does not match");

            documentsTabPage.ClickAddedLink(createdCardResponse.Title);
            Driver.SwitchToLastWindow();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());
            Assert.IsTrue(Driver.GetCurrentUrl().Contains($"kanban?cardTypeId={createdCardResponse.BusinessOutcomeCardTypeId}"),
            $"Not navigated to Assessment Review page i.e. Assessment is NOT saved as Draft only. Navigated to {Driver.GetCurrentUrl()}");
        }

        

    }
}
