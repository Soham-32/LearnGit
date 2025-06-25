using System.Threading;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.LeftNav
{
    [TestClass]
    [TestCategory("LeftNav"), TestCategory("NewNavigation")]
    public class SwitchCompanyTests : NewNavBaseTest
    {
        private static readonly UserConfig UserConfig = new UserConfig("SA", true);

        [TestMethod]
        [TestCategory("Search")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void LeftNav_SwitchCompany()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);

            SwitchToNewNav();
            Thread.Sleep(3000); //Added wait till team dashboard loading

            if (User.IsPartnerAdmin() || User.IsSiteAdmin())
            {
                var caUser = UserConfig.GetUserByDescription("user 2");
                teamDashboardPage.NavigateToPage(Company.Id);

                leftNavPage.SelectCompany(caUser.CompanyName);
                Assert.IsTrue(leftNavPage.DoesTeamDisplay(caUser.CompanyName), "Switching company is not working properly");
            }
            else
            {
                Assert.IsTrue(leftNavPage.IsCompanyDisabled(), "Company field is not disabled");
            }
        }
    }
}
