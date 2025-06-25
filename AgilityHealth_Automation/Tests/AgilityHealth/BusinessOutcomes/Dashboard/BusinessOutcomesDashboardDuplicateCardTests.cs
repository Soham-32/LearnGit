using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardDuplicateCardTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;
        private static BusinessOutcomeRequest _request;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _request = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme, 2);
            _response = new SetupTeardownApi(TestEnvironment).CreateBusinessOutcome(_request);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_DuplicateCard()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Business outcomes and verify the created duplicate and original card on 'Quarterly' column");
            businessOutcomesDashboard.NavigateToPage(Company.Id);

            var expected = new BusinessOutcomeRequest
            {
                Title = $"Copy of {_response.Title}",
                SwimlaneType = SwimlaneType.QuarterlyObjective,
                CardColor = _response.CardColor,
            };

            businessOutcomesDashboard.TagsViewSelectTag(TimeFrameTags.Quarterly.GetDescription());
            businessOutcomesDashboard.ClickOnCopyButton(_response.Title);
            businessOutcomesDashboard.DuplicateCardCreate(User.CompanyName, expected.SwimlaneType.GetDescription());

            businessOutcomeBasePage.CardSearch(expected.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsCardPresentInSwimLane(expected.SwimlaneType.GetDescription(), expected.Title),
                $"Duplicate card with title <{expected.Title}> not found in column <{expected.SwimlaneType.GetDescription()}>.");

            var expectedPercentage = GetKeyResultsProgressPercentage(_request.KeyResults);
            Assert.AreEqual($"{expectedPercentage}%",
                businessOutcomesDashboard.GetOverallPercentageValue(expected.SwimlaneType.GetDescription(), expected.Title),
                "Overall percentage doesn't match");


            //Verifying Overall progress bar color
            var overallProgressColor = businessOutcomesDashboard.GetOverallProgressBarColor(expected.SwimlaneType.GetDescription(), expected.Title);
            Assert.AreEqual(expected.CardColor,
                CSharpHelpers.ConvertRgbToHex(overallProgressColor),
                $"Color doesn't match for '{expected.Title}' overall progress bar");

            _response.Tags.ForEach(tag => Assert.IsTrue(
                businessOutcomesDashboard.IsBusinessOutcomeCategoryPresent(_response.SwimlaneType.GetDescription(),
                    expected.Title, tag.Name),
                $"Business outcome- {expected.Title} doesn't have category - {tag}"));

            //Expanding Card
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(expected.Title);

            //Key Result Name & Percentage
            foreach (var kr in _request.KeyResults)
            {
                Assert.AreEqual($"{kr.Progress}%",
                    businessOutcomesDashboard.GetKeyResultPercentageValue(expected.Title, kr.Title),
                    $"Key Result - {kr.Title}  percentage doesn't match");

                //Key Result progress indication
                Assert.AreEqual($"transform: translateX({kr.Progress - 100}%);",
                    businessOutcomesDashboard.GetKeyResultProgressIndication(expected.Title, kr.Title),
                    $"Key Result - {kr.Title}  progress indication doesn't match");

                //Verifying key result progress bar color
                var keyResultProgressColor = businessOutcomesDashboard.GetKeyResultProgressBarColor(expected.Title, kr.Title);
                Assert.AreEqual(expected.CardColor, CSharpHelpers.ConvertRgbToHex(keyResultProgressColor),
                    $"Color doesn't match for '{kr.Title}' key result progress bar");
            }

            //Verifying color left vertical stripe
            Assert.AreEqual($"background: {CSharpHelpers.CovertHexToRgb(expected.CardColor)};",
                businessOutcomesDashboard.GetColorInfoForLeftVerticalStripe(expected.Title),
                $"Color doesn't match for '{expected.Title} left vertical stripe'");

        }
    }
}