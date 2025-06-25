using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Create
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCreateTests7 : BaseTest
    {

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Add_CloseSaveChanges()
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

            addBusinessOutcomePage.EnterTitle(expected.Title);
            addBusinessOutcomePage.ClickOnCloseIcon();
            addBusinessOutcomePage.ConfirmPopUpClickOnSaveChangesButton();

            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(expected.Title), 
                $"New created business outcome with title {expected.Title} is not displayed on the dashboard.");
        }
    }
}