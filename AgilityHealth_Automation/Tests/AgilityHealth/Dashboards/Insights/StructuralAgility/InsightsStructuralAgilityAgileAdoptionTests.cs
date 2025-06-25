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
    public class InsightsStructuralAgilityAgileAdoptionTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_AgileAdoption_Filter()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);

            teamAgility.ClickOnStructuralAgilityTab();

            structuralAgility.SelectTeamCategories("Agile Adoption");

            Log.Info("Verify the widgets updated");
            var expectedLegends = new List<string> { "In-Progress", "Activated", "Unassigned" };
            var actualAgileAdoptionLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.AgileAdoption);
            Assert.IsFalse(expectedLegends.Except(actualAgileAdoptionLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.AgileAdoption.Title} and filter 'Agile Adoption'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualAgileAdoptionLegends)}>");
            var actualAgileAdoptionContributionLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.AgileAdoption);
            Assert.IsFalse(expectedLegends.Except(actualAgileAdoptionContributionLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.AgileAdoptionDistribution.Title} and filter 'Agile Adoption'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualAgileAdoptionContributionLegends)}>");

            structuralAgility.SelectTeamCategories("Team Formation");

            Log.Info("Verify the widgets updated");
            expectedLegends = new List<string> { "Formed", "Forming", "Not Formed", "Unassigned" };
            actualAgileAdoptionLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.AgileAdoption);
            Assert.IsFalse(expectedLegends.Except(actualAgileAdoptionLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.AgileAdoption.Title} and filter 'Team Formation'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualAgileAdoptionLegends)}>");
            actualAgileAdoptionContributionLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.AgileAdoption);
            Assert.IsFalse(expectedLegends.Except(actualAgileAdoptionContributionLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.AgileAdoptionDistribution.Title} and filter 'Team Formation'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualAgileAdoptionContributionLegends)}>");

            structuralAgility.SelectTeamCategories("Work Type");

            Log.Info("Verify the widgets updated");
            expectedLegends = new List<string> { "Software Delivery", "Business Operations", "Service and Support" };
            actualAgileAdoptionLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.AgileAdoption);
            Assert.IsFalse(expectedLegends.Except(actualAgileAdoptionLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.AgileAdoption.Title} and filter 'Work Type'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualAgileAdoptionLegends)}>");
            actualAgileAdoptionContributionLegends = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.AgileAdoption);
            Assert.IsFalse(expectedLegends.Except(actualAgileAdoptionContributionLegends).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.AgileAdoptionDistribution.Title} and filter 'Work Type'. Expected: <{string.Join(",", expectedLegends)}>. Actual: <{string.Join(",", actualAgileAdoptionContributionLegends)}>");
        }
    }
}
