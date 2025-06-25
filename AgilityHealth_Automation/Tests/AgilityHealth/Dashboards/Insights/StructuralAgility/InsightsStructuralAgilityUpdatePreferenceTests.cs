using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard")]
    public class InsightsStructuralAgilityUpdatePreferenceTests : BaseTest
    {

        [TestMethod]
        [DoNotParallelize]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_StructuralAgility_UpdatePreferences()
        {
            var login = new LoginPage(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var insightsDashboard = new InsightsDashboardPage(Driver, Log);
            var structuralAgilityTab = new StructuralAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            insightsDashboard.NavigateToPage(Company.InsightsId);

            insightsDashboard.ClickOnStructuralAgilityTab();

            var newWorkType = (structuralAgilityTab.GetAllocationByRoleWorkType() == "Software Delivery")
                ? "Business Operations"
                : "Software Delivery";
            structuralAgilityTab.SelectAllocationByRoleWorkType(newWorkType);

            var newPeopleFilter = (structuralAgilityTab.GetPeopleFilter() == "Software Delivery")
                ? "Business Operations"
                : "Software Delivery";
            structuralAgilityTab.SelectPeopleFilter(newPeopleFilter);

            var newCategoryFilter = (structuralAgilityTab.GetTeamCategoriesFilter() == "Agile Adoption")
                ? "Work Type"
                : "Agile Adoption";
            structuralAgilityTab.SelectTeamCategories(newCategoryFilter);

            v2Header.SignOut();

            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            insightsDashboard.NavigateToPage(Company.InsightsId);

            insightsDashboard.ClickOnStructuralAgilityTab();

            Log.Info("Verify new vales are retained.");
            Assert.AreEqual(newWorkType, structuralAgilityTab.GetAllocationByRoleWorkType(),
                "Allocation by Role filter does not match");
            Assert.AreEqual(newPeopleFilter, structuralAgilityTab.GetPeopleFilter(),
                "People By Role filter does not match.");
            Assert.AreEqual(newCategoryFilter, structuralAgilityTab.GetTeamCategoriesFilter(),
                "Team Category filter does not match.");
        }
    }
}