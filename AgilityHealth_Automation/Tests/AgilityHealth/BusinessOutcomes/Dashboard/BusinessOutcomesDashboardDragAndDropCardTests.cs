using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardDragAndDropCardTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response1;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response1 = CreateBusinessOutcome(SwimlaneType.QuarterlyObjective);
            CreateBusinessOutcome(SwimlaneType.QuarterlyObjective);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_DragAndDrop()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Business Outcomes and Drag and Drop the card");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.TagsViewSelectTag(TimeFrameTags.Quarterly.GetDescription());
            var initialOrder =
                businessOutcomesDashboard.GetAllBusinessOutcomeNamesByColumn(_response1.SwimlaneType.GetDescription());

            businessOutcomesDashboard.DragCardToCard(_response1.SwimlaneType.GetDescription(), initialOrder[0], initialOrder[1], 0, 100);

            var revisedOrder = businessOutcomesDashboard.GetAllBusinessOutcomeNamesByColumn(_response1.SwimlaneType.GetDescription());

            Log.Info("Verifying whether positions are swapped correctly.");
            Assert.AreEqual(initialOrder.IndexOf(initialOrder[0]), revisedOrder.IndexOf(initialOrder[1]), $"{initialOrder[0]} index doesn't match.");
            Assert.AreEqual(initialOrder.IndexOf(initialOrder[1]), revisedOrder.IndexOf(initialOrder[0]), $"{initialOrder[1]} index doesn't match.");
        }
    }
}