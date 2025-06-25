using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesKeyResultsAddMultipleKeyResultsWeightDistributionTests : BusinessOutcomesBaseTest
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
        public void BusinessOutcomes_KeyResults_Weight()
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

            var actualKeyResults = addBusinessOutcomePage.KeyResultsTab.GetKeyResult();

            Assert.AreEqual(actualKeyResults.Count,expectedKeyResults.Count, "Added Key Results are not present");

            for (var i = 0; i < expectedKeyResults.Count; i++)
            {
                Assert.AreEqual(actualKeyResults[i].Title, expectedKeyResults[i].Title, $"Key result title doesn't match for {i+1}th row");
                Assert.AreEqual(actualKeyResults[i].Weight,50, $"Key result Weight doesn't match for {i + 1}th row");
            }

            addBusinessOutcomePage.KeyResultsTab.ClickOnWeightButton();

            var expectedWeightHeaderValues = new List<string>() { "Key Results", "Weights" };
            Assert.That.ListsAreEqual(expectedWeightHeaderValues,addBusinessOutcomePage.KeyResultsTab.GetWeightTableHeader());
            Assert.AreEqual("Sum of Key Results percentage should be equal to 100",
                addBusinessOutcomePage.KeyResultsTab.GetWeightTooltipValue(), "Weight tooltip Value doesn't match");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsWeightTableCancelButtonDisplayed(),"Cancel Button is not displayed");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsWeightTableApplyButtonDisplayed(), "Apply Button is not displayed");
            var actualKeyResultValues = addBusinessOutcomePage.KeyResultsTab.GetKeyResultsWeightTableRow();
           var expectedDistributedWeights = DistributedWeights(actualKeyResultValues.Count);
            for (var i=0;i < actualKeyResultValues.Count;i++)
            {
                Assert.AreEqual(expectedKeyResults[i].Title, actualKeyResultValues[i].Title,"Key Result Title doesn't match");
                Assert.AreEqual(expectedDistributedWeights[i], actualKeyResultValues[i].Weight,"Key Result Weight doesn't match");
                Assert.IsTrue(actualKeyResultValues[i].IsImpact, "Impact Icon is not displayed");
            }

            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsTotalWeightTextDisplayed(), "Total Weight text is not displayed");
            Assert.AreEqual(addBusinessOutcomePage.KeyResultsTab.GetTotalWeightValue(), "100","Total weight value is incorrect");

            addBusinessOutcomePage.KeyResultsTab.ClickOnWeightTablePopupCancelButton();
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            Driver.RefreshPage();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            for (var i = 0; i < expectedKeyResults.Count; i++)
            {
                Assert.AreEqual(actualKeyResults[i].Title, expectedKeyResults[i].Title, $"Key result title doesn't match for {i + 1}th row");
                Assert.AreEqual(actualKeyResults[i].Weight, 50, $"Key result Weight doesn't match for {i + 1}th row");
            }
        }
    }
}
