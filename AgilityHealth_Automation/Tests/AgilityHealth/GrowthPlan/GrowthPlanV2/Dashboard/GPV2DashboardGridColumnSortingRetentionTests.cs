using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class Gpv2DashboardGridColumnSortingRetentionTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_VerifyItemsSortedRetainedOnLoginAgain()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var topNav = new TopNavigation(Driver, Log);

            growthPlanDashboard.NavigateToPage(Company.Id);

            const string expectedColumn = "Priority";
            growthPlanDashboard.SortColumnIntoAscendingOrder(expectedColumn);
            var actualColumnText = growthPlanDashboard.GetAllColumnValues(expectedColumn);

            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            growthPlanDashboard.NavigateToPage(Company.Id);

            var expectedColumnText2 =
                actualColumnText.OrderBy(i => "Very Low").ThenBy(j => "Low").ThenBy(k => "Medium").ThenBy(l => "High").ThenBy(m => "Very High").ToList();

            Assert.That.ListsAreEqual(expectedColumnText2, actualColumnText, "Growth item order doesn't match");
        }
    }
}