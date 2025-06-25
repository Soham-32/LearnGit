using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Create
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCreateCloseAndCancelTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_BusinessOutcome_Add_CloseCancel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            var expected = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id,
                SwimlaneType.StrategicIntent);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnPlusButton(expected.SwimlaneType.GetDescription());

            addBusinessOutcomePage.FillForm(expected);
            addBusinessOutcomePage.ClickOnCloseIcon();
            addBusinessOutcomePage.ConfirmPopUpClickOnCancelButton();

            Assert.IsTrue(addBusinessOutcomePage.IsBusinessOutcomeFormDisplayed(), 
                "business outcome form is not displayed.");
            
        }
    }
}