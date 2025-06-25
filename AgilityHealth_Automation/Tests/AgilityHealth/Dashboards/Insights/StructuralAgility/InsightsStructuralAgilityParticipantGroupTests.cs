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
    public class InsightsStructuralAgilityParticipantGroupTests : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_ParticipantGroup_Filter()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);
            
            teamAgility.ClickOnStructuralAgilityTab();


            Log.Info("Verify the widgets displayed");
            var expectedItems3 = new List<string> { "FTE", "Contractor" };
            var actualItems3 = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroup);
            Assert.IsFalse(expectedItems3.Except(actualItems3).Any(),
                $"Legend Items do not match for widget {StructuralAgilityWidgets.ParticipantGroup.Title} and filter 'FTE vs Contractor'. Expected: <{string.Join(",", expectedItems3)}>. Actual: <{string.Join(",", actualItems3)}>");

            structuralAgility.SelectParticipantGroupFilter("Distributed");
            structuralAgility.SelectParticipantGroupFilter("Collocated");

            Log.Info("Verify the widgets updated");
            var expectedItems1 = new List<string> {"Distributed", "Collocated" , "FTE", "Contractor" };
            var actualItems1 = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroup);
            Assert.IsFalse(expectedItems1.Except(actualItems1).Any(), 
                $"Legend Items do not match for widget {StructuralAgilityWidgets.ParticipantGroup.Title} and 'FTE', 'Contractor'.'Distributed',' Collocated' are selected in filter. Expected: <{string.Join(",", expectedItems1)}>. Actual: <{string.Join(",", actualItems1)}>");

            structuralAgility.SelectParticipantGroupFilter("Unassigned");
            var expectedItems2 = new List<string> { "Distributed", "Collocated", "Unassigned", "FTE", "Contractor" };
            var actualItems2 = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroupDistribution);
            Assert.IsFalse(expectedItems1.Except(actualItems2).Any(), 
                $"Legend Items do not match for widget {StructuralAgilityWidgets.ParticipantGroupDistribution.Title} and all values selected in filter. Expected: <{string.Join(",", expectedItems1)}>. Actual: <{string.Join(",", actualItems2)}>");
            
            var actualItems4 = structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroup);
            Assert.IsFalse(expectedItems2.Except(actualItems4).Any(), 
                $"Legend Items do not match for widget {StructuralAgilityWidgets.ParticipantGroup.Title} and all values selected in filter. Expected: <{string.Join(",", expectedItems2)}>. Actual: <{string.Join(",", actualItems4)}>");
        }
    }
}