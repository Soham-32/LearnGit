using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardTests7 : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicTheme, 1);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_ExpandCollapseCard()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);

            Log.Info("Verify that by default Key Result section is collapsed and not visible");
            businessOutcomeBasePage.CardSearch(_response.Title);
            Assert.IsFalse(businessOutcomesDashboard.IsKeyResultVisible(_response.Title, _response.KeyResults[0].Title), 
                "Business Outcome card is expanded by default");

            businessOutcomeBasePage.CardSearch(_response.Title);
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_response.Title);
            Log.Info("verify that Key Result section is expanded and visible");
            Assert.IsTrue(businessOutcomesDashboard.IsKeyResultVisible(_response.Title, _response.KeyResults[0].Title),
            "Business Outcome card isn't expanded");

            
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_response.Title);
            Log.Info("verify that Key Result section is collapsed and not visible");
            Assert.IsFalse(businessOutcomesDashboard.IsKeyResultVisible(_response.Title, _response.KeyResults[0].Title), 
                "Business Outcome card is expanded by default");
        }
    }
}