using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard")]
    public class InsightsStructuralAllocationByRoleTests : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_AllocationByRole_Validation()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);

            teamAgility.ClickOnStructuralAgilityTab();
            structuralAgility.WaitUntilWidgetsLoaded();

            structuralAgility.SelectAllocationByRoleWorkType("Software Delivery");

            var roleAllocationYAxisValues = structuralAgility.GetWidgetYAxisValues(StructuralAgilityWidgets.AllocationByRole).ToList();
            var expectedRoleAllocationYAxisValues = new List<string>
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5+"
            };

            Assert.That.ListsAreEqual(expectedRoleAllocationYAxisValues, roleAllocationYAxisValues,
                $"Y Axis values do not match for widget {StructuralAgilityWidgets.AllocationByRole.Title}.");
        }
    }
}
