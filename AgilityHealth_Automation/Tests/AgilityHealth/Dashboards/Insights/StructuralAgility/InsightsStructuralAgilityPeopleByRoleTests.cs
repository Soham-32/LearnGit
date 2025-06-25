using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard")]
    public class InsightsStructuralAgilityPeopleByRoleTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_PeopleByRole_Filter()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);
            
            teamAgility.ClickOnStructuralAgilityTab();

            structuralAgility.SelectPeopleFilter("Software Delivery");

            Log.Info("Verify the widgets updated");
            var expectedLegends = new List<string> { "Product Owner", "Designer", "QA Tester", "Technical Lead" };
            var actualPeopleByRoleLeLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.PeopleByRole);
            Assert.IsFalse(expectedLegends.Except(actualPeopleByRoleLeLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.PeopleByRole.Title} and filter 'Software Delivery'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualPeopleByRoleLeLegends)}>");
            var actualPeopleDistributionLeLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.PeopleByRole);
            Assert.IsFalse(expectedLegends.Except(actualPeopleDistributionLeLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.PeopleDistribution.Title} and filter 'Software Delivery'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualPeopleDistributionLeLegends)}>");

            structuralAgility.SelectPeopleFilter("Business Operations");

            Log.Info("Verify the widgets updated");
            expectedLegends = new List<string> { "Product Owner", "Designer", "Sales", "Technical Lead" };
            actualPeopleByRoleLeLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.PeopleByRole);
            Assert.IsFalse(expectedLegends.Except(actualPeopleByRoleLeLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.PeopleByRole.Title} and filter 'Business Operations'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualPeopleByRoleLeLegends)}>");
            actualPeopleDistributionLeLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.PeopleByRole);
            Assert.IsFalse(expectedLegends.Except(actualPeopleDistributionLeLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.PeopleDistribution.Title} and filter 'Business Operations'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualPeopleDistributionLeLegends)}>");

            structuralAgility.SelectPeopleFilter("Service and Support");

            Log.Info("Verify the widgets updated");
            expectedLegends = new List<string> { "Product Owner", "Designer", "QA Tester", "Technical Lead" };
            actualPeopleByRoleLeLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.PeopleByRole);
            Assert.IsFalse(expectedLegends.Except(actualPeopleByRoleLeLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.PeopleByRole.Title} and filter 'Service and Support'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualPeopleByRoleLeLegends)}>");
            actualPeopleDistributionLeLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.PeopleByRole);
            Assert.IsFalse(expectedLegends.Except(actualPeopleDistributionLeLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.PeopleDistribution.Title} and filter 'Service and Support'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualPeopleDistributionLeLegends)}>");
        }
    }
}
