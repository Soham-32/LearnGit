using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesKeyResultsAddMultipleKeyResultsTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicTheme);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_KeyResults_Add_Multiple()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var keyResults = new List<KeyResultRequest>
                {BusinessOutcomesFactory.GetKeyResultRequest(Company.Id), BusinessOutcomesFactory.GetKeyResultRequest(Company.Id)};
            keyResults.ForEach(kr => addBusinessOutcomePage.KeyResultsTab.AddKeyResult(kr));

            //Overall progress info
            var expectedOverallPercentage = GetKeyResultsProgressPercentage(keyResults);
            Assert.AreEqual(expectedOverallPercentage, addBusinessOutcomePage.KeyResultsTab.GetOverallProgressInfo(),
                "Overall progress bar indication doesn't match");

            //Overall percentage info
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.GetOverallPercentageInfo().Contains(expectedOverallPercentage), "Overall progress percentage doesn't match");

            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            
            //Dashboard
            Assert.AreEqual($"{expectedOverallPercentage}%",
                businessOutcomesDashboard.GetOverallPercentageValue(_response.sourceCategoryName, _response.Title),
                "Overall percentage doesn't match");

            //Expanding Card
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_response.Title);

            //Key Result Names & Percentage
            keyResults.ForEach(kr => Assert.AreEqual($"{kr.Progress}%",
                businessOutcomesDashboard.GetKeyResultPercentageValue(_response.Title, kr.Title),
                $"Key Result - {kr.Title}  percentage doesn't match"));

        }
    }
}