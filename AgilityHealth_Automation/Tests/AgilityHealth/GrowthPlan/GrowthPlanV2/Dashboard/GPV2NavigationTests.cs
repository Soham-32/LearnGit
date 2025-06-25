using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class Gpv2NavigationTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_VerifyGrowthPlanHeaderLink()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Assert.IsTrue(topNav.IsGrowthPlanLinkDisplayed(), "Growth Plan link is not present");
           
            topNav.ClickOnGrowthPlanLink();
            var expectedUrl = ApplicationUrl + "/V2/growthplan";
            var actualUrl = Driver.GetCurrentUrl();
            Assert.IsTrue(actualUrl.StartsWith(expectedUrl), $"Growth Plan url doesn't match. Expected : {expectedUrl}, Actual : {actualUrl}");
        }
    }
}