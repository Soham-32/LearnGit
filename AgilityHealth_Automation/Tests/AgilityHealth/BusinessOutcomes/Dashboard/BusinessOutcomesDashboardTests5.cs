using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardTests5 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response1;
        private static BusinessOutcomeResponse _response2;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var request1 = GetBusinessOutcomeRequest(SwimlaneType.StrategicIntent);
            request1.CardColor = "#34B2ED";
            _response1 = setup.CreateBusinessOutcome(request1);
            
            var request2 = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
            request1.CardColor = "#B70000";
            _response2 = setup.CreateBusinessOutcome(request2);
            
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_Filter_ByColor()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ColorFilterClickOnFilterButton();
            businessOutcomesDashboard.ColorFilterSelectColor(_response1.CardColor);
            businessOutcomesDashboard.ColorFilterClose();

            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response1.Title),
                $"Business outcome- {_response1.Title} isn't present");
            Assert.IsFalse(businessOutcomesDashboard.IsBusinessOutcomePresent(_response2.Title),
                $"Business outcome- {_response2.Title} is present");

            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomesDashboard.ColorFilterClickOnFilterButton();
            Assert.IsTrue(businessOutcomesDashboard.IsColorFilterColorSelected(_response1.CardColor), $"Business outcome- {_response1.CardColor} isn't selected");

            businessOutcomesDashboard.ColorFilterClickOnClearButton();
            businessOutcomesDashboard.ColorFilterClose();

            businessOutcomeBasePage.CardSearch(_response1.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response1.Title),
                $"Business outcome- {_response1.Title} isn't present");

            businessOutcomeBasePage.CardSearch(_response2.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response2.Title),
                $"Business outcome- {_response2.Title} isn't present");
        }
    }
}