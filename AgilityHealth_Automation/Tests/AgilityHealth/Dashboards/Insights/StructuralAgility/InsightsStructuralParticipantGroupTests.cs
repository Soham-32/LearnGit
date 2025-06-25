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
    public class InsightsStructuralParticipantGroupTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void Insights_ParticipantGroup_Validation()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgility.NavigateToPage(Company.InsightsId);

            teamAgility.ClickOnStructuralAgilityTab();
            teamAgility.WaitUntilWidgetsLoaded();
            
            var participantGroupTitle = structuralAgility.GetWidgetTitle(StructuralAgilityWidgets.ParticipantGroup);
            Assert.AreEqual(StructuralAgilityWidgets.ParticipantGroup.Title, participantGroupTitle, $"Participant Group widget title displays incorrectly");

            var participantGroupSubtitle = structuralAgility.GetWidgetSubtitle(StructuralAgilityWidgets.ParticipantGroup);
            Assert.AreEqual("What is the distribution of tags across my organization?", participantGroupSubtitle, $"{StructuralAgilityWidgets.ParticipantGroup.Title} widget subtitle displays incorrectly");

            var participantGroupLegends =
                structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroup);
            IList<string> expectedParticipantGroupLegends = new List<string>()
            {
                "FTE",
                "Contractor"
            };

            Assert.IsFalse(expectedParticipantGroupLegends.Except(participantGroupLegends).Any(),
                $"Legends do not match for widget {StructuralAgilityWidgets.ParticipantGroup.Title}. Expected: <{string.Join(",", expectedParticipantGroupLegends)}>. Actual: <{string.Join(",", participantGroupLegends)}>");

            structuralAgility.SelectParticipantGroupFilter("Distributed");
            structuralAgility.SelectParticipantGroupFilter("Collocated");

            participantGroupLegends =
                structuralAgility.GetWidgetLegends(StructuralAgilityWidgets.ParticipantGroup);
            expectedParticipantGroupLegends = new List<string>()
            {
                "FTE",
                "Contractor",
                "Distributed",
                "Collocated"
            };

            Assert.IsFalse(
                expectedParticipantGroupLegends.Except(participantGroupLegends).Any(),
                $"Legends do not match for widget {StructuralAgilityWidgets.ParticipantGroupDistribution.Title}. Expected: <{string.Join(",", expectedParticipantGroupLegends)}>. Actual: <{string.Join(",", participantGroupLegends)}>");
        }
    }
}
