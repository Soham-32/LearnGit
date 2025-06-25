using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesKeyResultsViewDetails : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _businessOutcome;

        [ClassInitialize]
        public static void ClassSetUp(TestContext context)
        {
            _businessOutcome = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_KeyResults_Add_SubTarget()
        {
            var loginPage = new LoginPage(Driver, Log);
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var cardPage = new BusinessOutcomeCardPage(Driver, Log);
            var detailsTabPage = new KeyResultsDetailsTabPage(Driver, Log);

            Log.Info($"Login to application as {User.Username} user");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to Business Outcomes card");
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.ClickOnBusinessOutcomeLink(_businessOutcome.Title);
            var keyResult = BusinessOutcomesFactory.GetKeyResultRequest(Company.Id);

            Log.Info($"Adding Key Result: {keyResult.Title}");
            cardPage.KeyResultsTab.AddKeyResult(keyResult);
            cardPage.ClickOnSaveButton();

            Log.Info("Viewing Key Result Details");
            cardPage.ClickOnKeyResultActionButton(keyResult.Title);
            cardPage.SelectAction("View Details");
            var keyResultDetails = BusinessOutcomesFactory.CreateDefaultKeyResultDetails();

            // Add first SubTarget
            var subTarget1Value = $"{CSharpHelpers.RandomNumberBetween2Numbers(Convert.ToInt32(keyResult.Progress), Convert.ToInt32(keyResult.Target))}%";
            keyResultDetails.SubTargets.Add(new SubTargetDto
            {
                SubTargetValue = subTarget1Value,
                ByWhenDate = DateTime.Today.AddMonths(1).ToString("MM/dd/yyyy")
            });

            // Add second SubTarget
            var subTarget2Value = $"{CSharpHelpers.RandomNumberBetween2Numbers(Convert.ToInt32(subTarget1Value.Replace("%", "")), Convert.ToInt32(keyResult.Target))}%";
            keyResultDetails.SubTargets.Add(new SubTargetDto
            {
                SubTargetValue = subTarget2Value,
                ByWhenDate = DateTime.Today.AddMonths(2).ToString("MM/dd/yyyy")
            });

            // Add third SubTarget
            keyResultDetails.SubTargets.Add(new SubTargetDto
            {
                SubTargetValue = $"{keyResult.Target}%",
                ByWhenDate = DateTime.Today.AddMonths(3).ToString("MM/dd/yyyy")
            });

            Log.Info("Filling Key Result Details");
            detailsTabPage.FillKeyResultDetails(keyResultDetails);
            detailsTabPage.ClickOnDetailSaveButton();

            Log.Info("Re-opening Key Result Details to verify graph data");
            cardPage.ClickOnKeyResultActionButton(keyResult.Title);
            cardPage.SelectAction("View Details");
            detailsTabPage.ClickGraphViewTab();

            Log.Info("Fetching SubTarget tooltip data from graph");
            var actualGraphData = detailsTabPage.GetSubTargetTooltipData();
            var expectedData = keyResultDetails.SubTargets.ToDictionary(
                subTarget => subTarget.ByWhenDate.Replace("-","/"),
                subTarget => subTarget.SubTargetValue.Replace("%", "")
            );

            Log.Info("Asserting that expected and actual SubTarget values match");
            CollectionAssert.AreEquivalent(expectedData, actualGraphData, "Mismatch between expected and actual SubTarget tooltip data");
        }
    }
}
