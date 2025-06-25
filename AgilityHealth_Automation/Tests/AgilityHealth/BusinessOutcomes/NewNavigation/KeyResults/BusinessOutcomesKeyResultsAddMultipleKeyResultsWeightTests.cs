using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesKeyResultsAddMultipleKeyResultsWeightTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicTheme);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_KeyResults_Add_Multiple()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            Log.Info($"Verify the Overall Progress For Created Key Result");
            var expectedKeyResults = new List<KeyResultRequest>
                {BusinessOutcomesFactory.GetKeyResultRequest(Company.Id), BusinessOutcomesFactory.GetKeyResultRequest(Company.Id)};
            expectedKeyResults.ForEach(kr => addBusinessOutcomePage.KeyResultsTab.AddKeyResult(kr));

            //Overall progress info
            var expectedOverallPercentage = GetKeyResultsProgressPercentage(expectedKeyResults);
            Assert.AreEqual(expectedOverallPercentage, addBusinessOutcomePage.KeyResultsTab.GetOverallProgressInfo(),
                "Overall progress bar indication doesn't match");

            //Overall percentage info
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.GetOverallPercentageInfo().Contains(expectedOverallPercentage), "Overall progress percentage doesn't match");

            Log.Info($"Verify the Created Card Key Results after Save");
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            Driver.RefreshPage();

            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var actualKeyResults = addBusinessOutcomePage.KeyResultsTab.GetKeyResult();

            Assert.AreEqual(actualKeyResults.Count,expectedKeyResults.Count, "Added Key Results are not present");

            for (var i = 0; i < expectedKeyResults.Count; i++)
            {
                Assert.AreEqual(actualKeyResults[i].Title, expectedKeyResults[i].Title, $"Key result title doesn't match for {i+1}th row");
                Assert.AreEqual(actualKeyResults[i].Weight,50, $"Key result Weight doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].Metric.Name, $"{expectedKeyResults[i].Metric.Name} (#)", $"Key result Metric doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].Start, expectedKeyResults[i].Start, $"Key result Start doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].Target, expectedKeyResults[i].Target, $"Key result Target doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].Stretch,"0", $"Key result Target doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].Progress, expectedKeyResults[i].Progress, $"Key result Progress doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].ProgressBar, CustomGetKeyResultsProgressPercentage(expectedKeyResults[i]), $"Key result Progress bar doesn't match for {i + 1}th row");
            }
            
            addBusinessOutcomePage.ClickOnCloseIcon();

            Log.Info($"Verify the Overall Progress and Created Key Result on Dashboard View");
            Assert.AreEqual($"{expectedOverallPercentage}%",
                businessOutcomesDashboard.GetOverallPercentageValue(_response.sourceCategoryName, _response.Title),
                "Overall percentage doesn't match");

            //Expanding Card
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_response.Title);

            //Key Result Names & Percentage
            expectedKeyResults.ForEach(kr => Assert.AreEqual($"{kr.Progress}%",
                businessOutcomesDashboard.GetKeyResultPercentageValue(_response.Title, kr.Title),
                $"Key Result - {kr.Title}  percentage doesn't match"));

        }
    }
}
