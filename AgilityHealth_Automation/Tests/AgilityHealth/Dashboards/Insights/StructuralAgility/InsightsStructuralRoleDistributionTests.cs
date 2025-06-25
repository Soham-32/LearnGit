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
    public class InsightsStructuralRoleDistributionTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_RoleDistribution_Validation()
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            
            teamAgility.NavigateToPage(Company.InsightsId);
            
            teamAgility.ClickOnStructuralAgilityTab();

            structuralAgility.SelectPeopleFilter("Software Delivery");

            var peopleDistributionYAxisValues = structuralAgility.GetWidgetYAxisValues(StructuralAgilityWidgets.PeopleDistribution).ToList();
            var expectedPeopleDistributionYAxisValues = new List<string> { "0", "1", "2", "3" };
            Assert.That.ListsAreEqual(expectedPeopleDistributionYAxisValues, peopleDistributionYAxisValues, 
                $"Y Axis for {StructuralAgilityWidgets.PeopleDistribution.Title} Widget should display correctly.");

            var peopleDistributionTitle = structuralAgility.GetWidgetTitle(StructuralAgilityWidgets.PeopleDistribution);
            Assert.AreEqual(StructuralAgilityWidgets.PeopleDistribution.Title, peopleDistributionTitle, "People Distribution widget title displays incorrectly");

            var peopleDistributionSubtitle = structuralAgility.GetWidgetSubtitle(StructuralAgilityWidgets.PeopleDistribution);
            Assert.AreEqual("How many roles are assigned for each team?", peopleDistributionSubtitle, "Roles Distribution widget subtitle displays incorrectly");
        }
    }
}
