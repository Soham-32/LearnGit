using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardFilterByTimeFrameTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _threeYearBoCard;
        private static BusinessOutcomeResponse _oneYearBoCard;
        private static BusinessOutcomeResponse _quarterlyBoCard;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var request1 = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
            _oneYearBoCard = setup.CreateBusinessOutcome(request1);

            var request2 = GetBusinessOutcomeRequest(SwimlaneType.QuarterlyObjective);
            _quarterlyBoCard = setup.CreateBusinessOutcome(request2);

            var request3 = GetBusinessOutcomeRequest(SwimlaneType.StrategicIntent);
            _threeYearBoCard = setup.CreateBusinessOutcome(request3);

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void BusinessOutcomes_Dashboard_Filter_ByTimeFrame()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Business OutComes and verify the Created cards and column as per selected time frame");
            businessOutcomesDashboard.NavigateToPage(Company.Id);

            var listColumnNames = businessOutcomesDashboard.GetAllColumnNames();
            var timeFrame = businessOutcomesDashboard.GetTagFromDropDown();
            Assert.AreEqual(TimeFrameTags.Annually.GetDescription(), timeFrame, $"TimeFrame{timeFrame} does not match in List");

            Assert.That.ListContains(listColumnNames, SwimlaneType.StrategicIntent.GetDescription().ToUpper(), $"Column {SwimlaneType.StrategicIntent.GetDescription()} does not match");
            Assert.That.ListContains(listColumnNames, SwimlaneType.StrategicTheme.GetDescription().ToUpper(), $"Column {SwimlaneType.StrategicTheme.GetDescription()} does not match");
            Assert.That.ListNotContains(listColumnNames, SwimlaneType.QuarterlyObjective.GetDescription().ToUpper(), $"Column {SwimlaneType.QuarterlyObjective.GetDescription()} does match");

            businessOutcomeBasePage.CardSearch(_threeYearBoCard.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_threeYearBoCard.Title),
                $"Business outcome- {_threeYearBoCard.Title} isn't present");

            businessOutcomeBasePage.CardSearch(_oneYearBoCard.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_oneYearBoCard.Title),
                $"Business outcome- {_oneYearBoCard.Title} isn't present");

            businessOutcomeBasePage.CardSearch(_quarterlyBoCard.Title);
            Assert.IsFalse(businessOutcomesDashboard.IsBusinessOutcomePresent(_quarterlyBoCard.Title),
                $"Business outcome- {_quarterlyBoCard.Title} is present");

            businessOutcomesDashboard.TagsViewSelectTag(TimeFrameTags.Quarterly.GetDescription());

            listColumnNames = businessOutcomesDashboard.GetAllColumnNames();
            timeFrame = businessOutcomesDashboard.GetTagFromDropDown();
            Assert.AreEqual(TimeFrameTags.Quarterly.GetDescription(), timeFrame, $"TimeFrame{timeFrame} does not match in List");

            Assert.That.ListNotContains(listColumnNames, SwimlaneType.StrategicIntent.GetDescription().ToUpper(), $"Column {SwimlaneType.StrategicIntent.GetDescription()} does not match");
            Assert.That.ListContains(listColumnNames, SwimlaneType.QuarterlyObjective.GetDescription().ToUpper(), $"Column {SwimlaneType.QuarterlyObjective.GetDescription()} does not match");
            Assert.That.ListContains(listColumnNames, SwimlaneType.StrategicTheme.GetDescription().ToUpper(), $"Column {SwimlaneType.StrategicTheme.GetDescription()} does match");


            businessOutcomeBasePage.CardSearch(_threeYearBoCard.Title);
            Assert.IsFalse(businessOutcomesDashboard.IsBusinessOutcomePresent(_threeYearBoCard.Title),
                $"Business outcome- {_threeYearBoCard.Title} isn't present");

            businessOutcomeBasePage.CardSearch(_oneYearBoCard.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_oneYearBoCard.Title),
                $"Business outcome- {_oneYearBoCard.Title} is present");

            businessOutcomeBasePage.CardSearch(_quarterlyBoCard.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_quarterlyBoCard.Title),
                $"Business outcome- {_quarterlyBoCard.Title} isn't present");

            timeFrame = businessOutcomesDashboard.GetTagFromDropDown();
            Assert.AreEqual(TimeFrameTags.Quarterly.GetDescription(), timeFrame, $"TimeFrame{timeFrame} does not match in List");

        }
    }
}