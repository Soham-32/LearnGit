using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
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
    public class BusinessOutcomesKeyResultsEditTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent, 1);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]      
        public void BusinessOutcomes_KeyResults_Edit()
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
            keyResult1.IsImpact = true;
            addBusinessOutcomePage.KeyResultsTab.EditKeyResult(_response.KeyResults.First().Title, keyResult1);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            var expectedPercentage = GetKeyResultsProgressPercentage(new List<KeyResultRequest> { keyResult1 });
            Assert.AreEqual($"{expectedPercentage}%",
                businessOutcomesDashboard.GetOverallPercentageValue(_response.sourceCategoryName, _response.Title),
                "Overall percentage doesn't match");

            //Verifying overall progress indicator
            var expectedOverallProgress = $"transform: translateX({float.Parse(expectedPercentage) - 100}%);";
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_response.Title);

            Assert.IsTrue(businessOutcomesDashboard.IsKeyResultImpactIconVisible(_response.Title, keyResult1.Title),
                $"Impact icon not present for {keyResult1.Title}");
            Assert.AreEqual($"{expectedPercentage}%",
                businessOutcomesDashboard.GetKeyResultPercentageValue(_response.Title, keyResult1.Title),
                $"Key Result - {keyResult1.Title}  percentage doesn't match");
            Assert.AreEqual(expectedOverallProgress,
                businessOutcomesDashboard.GetKeyResultProgressIndication(_response.Title, keyResult1.Title),
                $"Key Result - {keyResult1.Title}  progress indication doesn't match");
        }
    }
}