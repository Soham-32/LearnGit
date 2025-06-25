using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesKeyResultsAddTests1 : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_KeyResults_Add_Single()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var keyResult1 = BusinessOutcomesFactory.GetKeyResultRequest(Company.Id);

            addBusinessOutcomePage.KeyResultsTab.AddKeyResult(keyResult1);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomeBasePage.CardSearch(_response.Title);
            var expectedPercentage = GetKeyResultsProgressPercentage(new List<KeyResultRequest> { keyResult1 });
            Assert.AreEqual($"{expectedPercentage}%", 
                businessOutcomesDashboard.GetOverallPercentageValue(_response.SwimlaneType.GetDescription(), _response.Title),
                "Overall percentage doesn't match");

            //Verifying overall progress indicator
            var expectedOverallProgress = $"transform: translateX({float.Parse(expectedPercentage) - 100}%);";

            //Expanding Card
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_response.Title);

            //Key Result Name & Percentage
            Assert.AreEqual($"{expectedPercentage}%", 
                businessOutcomesDashboard.GetKeyResultPercentageValue(_response.Title, keyResult1.Title), 
                $"Key Result - {keyResult1.Title}  percentage doesn't match");

            //Key Result progress indication
            Assert.AreEqual(expectedOverallProgress, 
                businessOutcomesDashboard.GetKeyResultProgressIndication(_response.Title, keyResult1.Title), 
                $"Key Result - {keyResult1.Title}  progress indication doesn't match");
            
        }
    }
}