using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Create
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCreateTests8 : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Add_MinimizeMaximize()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnPlusButton(SwimlaneType.StrategicIntent.GetDescription());

            Log.Info("Verify that Card is minimized by Default");
            Assert.IsFalse(addBusinessOutcomePage.MinimizeFormGetCssValue("margin").Equals("0px") || //in minimize margin is 32px and boarder radius is 4px
                           addBusinessOutcomePage.MinimizeFormGetCssValue("border-radius").Equals("0px"),"Card isn't minimized");

            addBusinessOutcomePage.ClickOnMaximizeIcon();

            Log.Info("Verify that pop up is maximized");
            Assert.IsTrue(addBusinessOutcomePage.MaximizeFormGetCssValue("margin").Equals("0px") && //in maximize margin is 0px and boarder radius is 0px
                          addBusinessOutcomePage.MaximizeFormGetCssValue("border-radius").Equals("0px"), "Card isn't maximized");

            addBusinessOutcomePage.ClickOnMinimizeIcon();

            Log.Info("Verify that pop up is minimized");
            Assert.IsFalse(addBusinessOutcomePage.MinimizeFormGetCssValue("margin").Equals("0px") ||
                           addBusinessOutcomePage.MinimizeFormGetCssValue("border-radius").Equals("0px"), "Card isn't minimized");
        }
    }
}