using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Edit
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesEditAndCancelTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_BusinessOutcome_Edit_Cancel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var expected = BusinessOutcomesFactory.GetBusinessOutcomeForUpdate(Company.Id, _response.SwimlaneType);
            addBusinessOutcomePage.FillForm(expected);
            addBusinessOutcomePage.ClickOnCloseIcon();
            addBusinessOutcomePage.ConfirmPopUpClickOnCancelButton();

            Assert.IsTrue(addBusinessOutcomePage.IsBusinessOutcomeFormDisplayed(), "Business outcome form is not displayed.");
            
        }
    }
}