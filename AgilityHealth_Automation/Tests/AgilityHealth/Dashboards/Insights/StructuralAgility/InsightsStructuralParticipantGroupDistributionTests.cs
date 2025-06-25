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
    public class InsightsStructuralParticipantGroupDistributionTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_ParticipantGroupDistribution_Validation()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);
            
            teamAgility.ClickOnStructuralAgilityTab();

            var participantGroupDistributionYAxisValues = structuralAgility.GetWidgetYAxisValues(StructuralAgilityWidgets.ParticipantGroupDistribution);
            var expectedParticipantGroupDistributionYAxisValues = new List<string>()
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6"
            };

            Assert.IsFalse(expectedParticipantGroupDistributionYAxisValues.Except(participantGroupDistributionYAxisValues).Any(),
                $"X Axis values do not match for widget {StructuralAgilityWidgets.ParticipantGroupDistribution.Title}. Expected: <{string.Join(",", expectedParticipantGroupDistributionYAxisValues)}>. Actual: <{string.Join(",", participantGroupDistributionYAxisValues)}>");

            var participantGroupDistributionTitle = structuralAgility.GetWidgetTitle(StructuralAgilityWidgets.ParticipantGroupDistribution);
            Assert.AreEqual(StructuralAgilityWidgets.ParticipantGroupDistribution.Title, participantGroupDistributionTitle, "Participant Group Distribution widget title displays incorrectly");

            var participantGroupDistributionLegends =
                structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroupDistribution);
            var expectedParticipantGroupDistributionLegends = new List<string>()
            {
                "FTE",
                "Contractor",
            };

            Assert.IsFalse(expectedParticipantGroupDistributionLegends.Except(participantGroupDistributionLegends).Any(),
                $"Legends do not match for widget {StructuralAgilityWidgets.ParticipantGroupDistribution.Title}. Expected: <{string.Join(",", expectedParticipantGroupDistributionLegends)}>. Actual: <{string.Join(",", participantGroupDistributionLegends)}>");

            structuralAgility.SelectParticipantGroupFilter("Distributed");
            structuralAgility.SelectParticipantGroupFilter("Collocated");

            participantGroupDistributionLegends =
                structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroupDistribution);
            expectedParticipantGroupDistributionLegends = new List<string>()
            {
                "Distributed",
                "Collocated",
                "FTE",
                "Contractor",
            };

            Assert.IsFalse(
                expectedParticipantGroupDistributionLegends.Except(participantGroupDistributionLegends).Any(),
                $"Legends do not match for widget {StructuralAgilityWidgets.ParticipantGroupDistribution.Title}. Expected: <{string.Join(",", expectedParticipantGroupDistributionLegends)}>. Actual: <{string.Join(",", participantGroupDistributionLegends)}>");
        }
    }
}
