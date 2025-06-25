using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomeKeyResultActionStrategicAndAnnualTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _responseStrategicTheme, _responseStrategicIntent;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _responseStrategicTheme = CreateBusinessOutcome(SwimlaneType.StrategicTheme);
            _responseStrategicIntent = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_KeyResults_1yrOutcome_Action()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_responseStrategicTheme.Title);

            var keyResults = new List<KeyResultRequest>
                {BusinessOutcomesFactory.GetKeyResultRequest(Company.Id)};
            keyResults.ForEach(kr => addBusinessOutcomePage.KeyResultsTab.AddKeyResult(kr));
            var keyResultTitle = addBusinessOutcomePage.KeyResultsTab.GetKeyResultTitleText(1);

            addBusinessOutcomePage.KeyResultsTab.ClickOnKeyResultsMoreButton(keyResultTitle);

            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsKeyResultMoreLinkToOutcomeDisplayed());
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsKeyResultMoreViewDetailsDisplayed());
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsKeyResultMoreDeleteDisplayed());
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_KeyResults_3yrOutcome_Action()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_responseStrategicIntent.Title);

            var keyResults = new List<KeyResultRequest>
                {BusinessOutcomesFactory.GetKeyResultRequest(Company.Id)};
            keyResults.ForEach(kr => addBusinessOutcomePage.KeyResultsTab.AddKeyResult(kr));
            var keyResultTitle = addBusinessOutcomePage.KeyResultsTab.GetKeyResultTitleText(1);

            addBusinessOutcomePage.KeyResultsTab.ClickOnKeyResultsMoreButton(keyResultTitle);

            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsKeyResultMoreLinkToOutcomeDisplayed());
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsKeyResultMoreViewDetailsDisplayed());
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsKeyResultMoreDeleteDisplayed());
        }
    }
}
