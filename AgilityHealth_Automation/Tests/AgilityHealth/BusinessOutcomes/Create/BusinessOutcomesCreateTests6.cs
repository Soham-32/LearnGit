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
    public class BusinessOutcomesCreateTests6 : BaseTest
    {
        
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Add_MandatoryFields()
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

            Assert.IsFalse(addBusinessOutcomePage.IsSaveButtonEnabled(),
                "'Save And Close' button is enabled and can be clicked without adding Title");
            Assert.IsFalse(addBusinessOutcomePage.IsSaveDropdownButtonEnabled(),
                "'Save Dropdown' button is enabled and can be clicked without adding Title");

            addBusinessOutcomePage.EnterTitle(expected.Title);

            Assert.IsTrue(addBusinessOutcomePage.IsSaveButtonEnabled(),
                "'Save And Close' button is not enabled.");
            Assert.IsTrue(addBusinessOutcomePage.IsSaveDropdownButtonEnabled(),
                "'Save Dropdown' button is not enabled.");


        }
    }
}