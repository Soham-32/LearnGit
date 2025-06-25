using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesKeyResultsAddTests3 : BusinessOutcomesBaseTest
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
        public void BusinessOutcomes_KeyResults_Add_NewMetricType()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var keyResult1 = BusinessOutcomesFactory.GetKeyResultRequest(Company.Id);
            addBusinessOutcomePage.KeyResultsTab.ClickOnKeyResultAddButton();
            addBusinessOutcomePage.KeyResultsTab.EnterKeyResultTitle(1, keyResult1.Title);

            var newMetric = BusinessOutcomesFactory.GetValidBusinessOutcomeAddMetricBody(Company.Id);
            addBusinessOutcomePage.KeyResultsTab.AddNewMetric(keyResult1.Title, newMetric);
            addBusinessOutcomePage.KeyResultsTab.SelectMetric(1, newMetric.Name);

            Log.Info("Verify that new Metric type can be selected while adding new key result and new metric type pop up not displayed");
            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsMetricTypePopUpPresent(), "Metric Type pop up is displayed.");
        }
    }
}