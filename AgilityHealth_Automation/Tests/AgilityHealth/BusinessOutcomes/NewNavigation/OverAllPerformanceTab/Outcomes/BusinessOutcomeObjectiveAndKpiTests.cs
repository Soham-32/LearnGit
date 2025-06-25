using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.OverAllPerformanceTab.Outcomes
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomeObjectiveAndKpiTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_OverallPerformance_Outcomes_ObjectiveAndKpis_GridViewHeadersAndData()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomesOverallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var businessOutcomeObjectivesPage = new BusinessOutcomeObjectivesPage(Driver, Log);

            Log.Info("Logging in to the application.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigating to Business Outcomes Dashboard and add key result to created card {_response.Title}.");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);
            var keyResult1 = BusinessOutcomesFactory.GetKeyResultRequest(Company.Id);
            addBusinessOutcomePage.KeyResultsTab.AddKeyResult(keyResult1);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            Log.Info("Click on 'Overall Performance' tab, Go to 'Objectives' and verify all the details.");
            addBusinessOutcomePage.ClickOnOverallPerformanceTab();
            businessOutcomesOverallPerformancePage.ClickOnObjectiveProgressBar();

            Assert.IsTrue(businessOutcomeObjectivesPage.IsHeaderDisplayed("Objectives"), "Objectives popup header is not displayed");
            Assert.IsTrue(businessOutcomeObjectivesPage.IsOverallProgressTextDisplayed(), "'Overall Progress' bar is not displayed");

            var expectedColumnHeaderList = new List<string> { "ID", "Title","Owners", "Status", "Progress" };
            var actualColumnHeaderList = businessOutcomeObjectivesPage.GetPopupHeaderData("ID");
            Assert.That.ListsAreEqual(expectedColumnHeaderList, actualColumnHeaderList);

            businessOutcomeObjectivesPage.ObjectivesSortByDescending("ID");
            Assert.IsTrue(businessOutcomeObjectivesPage.IsTitleDisplayed(_response.Title), "BO Card title is not displayed");
            Assert.AreEqual("On Track", businessOutcomeObjectivesPage.GetCardStatus(_response.Title), "Bo card status is not matched");
            businessOutcomeObjectivesPage.ClickOnObjectiveCloseButton();

            Log.Info("Click 'KPIs' and verify all the details.");
            businessOutcomesOverallPerformancePage.ClickOnOutcomesObjectiveProgressBar("KPIs");
            Assert.IsTrue(businessOutcomeObjectivesPage.IsHeaderDisplayed("Key Results"), "'Key Results' popup header is not displayed");
            Assert.IsTrue(businessOutcomeObjectivesPage.IsOverallProgressTextDisplayed());

            var expectedKpisColumnHeaderList = new List<string> { "Card ID", "Key Result Title", "Card Title", "Metric", "Progress", "Owner", "Baseline", "Target", "Stretch", "Actual" };
            var actualKpisColumnHeaderList = businessOutcomeObjectivesPage.GetPopupHeaderData("Card ID");
            Assert.That.ListsAreEqual(expectedKpisColumnHeaderList, actualKpisColumnHeaderList);

            Assert.IsTrue(businessOutcomesOverallPerformancePage.IsTitleDisplayed(_response.Title), "Card title is not displayed");
            Assert.IsTrue(businessOutcomeObjectivesPage.IsKeyResultTitleDisplayed(keyResult1.Title), "Key result title is not displayed");

        }
    }
}