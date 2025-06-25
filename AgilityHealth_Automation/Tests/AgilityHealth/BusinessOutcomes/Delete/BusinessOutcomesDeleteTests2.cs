using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Delete
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDeleteTests2 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Delete_Cancel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            addBusinessOutcomePage.ClickOnDeleteButton();
            addBusinessOutcomePage.DeletePopUp_ClickOnCancelButton();

            Assert.IsTrue(addBusinessOutcomePage.IsBusinessOutcomeFormDisplayed(), "Business Outcome form is not displayed.");

        }
    }
}