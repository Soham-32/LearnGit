using System;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.Documents
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesDeleteDocumentsTest : BusinessOutcomesBaseTest
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
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Documents_Delete()
        {
            DocumentsValidator(_businessOutcomeResponse, BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), BusinessOutcomesCardTypeTags.Annually.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Documents_Delete()
        {
            DocumentsValidator(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives_Documents_Delete()
        {
            DocumentsValidator(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(), BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Stories_Documents_Delete()
        {
            DocumentsValidator(_storiesCardResponse, BusinessOutcomesCardType.Monthly.GetDescription(), BusinessOutcomesCardTypeTags.Monthly.GetDescription());
        }

        private void DocumentsValidator(BusinessOutcomeResponse createdCardResponse, string cardType, string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var documentsTabPage = new DocumentsTabPage(Driver, Log);

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
            businessOutcomeBasePage.ClickOnTab("Documents");

            Log.Info("Click on 'Upload Stakeholders' button and upload the excel file which contains fresh stakeholders");
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\StakeholdersImportData.xlsx");

            documentsTabPage.UploadDocument(filePath);

            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info($"Click on the created Card for the {cardType} and navigate to Documents ");
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.Tags.FirstOrDefault()?.Name ?? SwimlaneType.StrategicIntent.GetDescription());
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Documents");

            Log.Info("Verify the added obstacles are displayed");
            var actualDocumentsResponse = documentsTabPage.GetDocumentsResponse().FirstOrDefault();
            Assert.AreEqual(1, documentsTabPage.GetDocumentsResponse().Count, "Added Documents count does not match");
            Assert.AreEqual("StakeholdersImportData.xlsx", actualDocumentsResponse.Title, "Documents title does not match");

            Log.Info("Verify the deleted Obstacles are not displayed");
            documentsTabPage.DeleteDocument("StakeholdersImportData.xlsx");
            businessOutcomeBasePage.ClickOnSaveButton();
            Assert.IsFalse(documentsTabPage.IsDocumentDisplayed("StakeholdersImportData.xlsx"), "Added Document is not displayed");//

        }
    }
}
