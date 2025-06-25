using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthItemsDashboard"), TestCategory("Dashboard")]
    public class GiDashboardCopyLinkWithAppliedFiltersAndClearFiltersTests : IndividualAssessmentBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51039
        [TestCategory("CompanyAdmin")]
        public void GrowthItemDashboard_Grid_CopyLinkWithAppliedFilters_And_ClearFilters_Verification()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var growthItemsDashboardPage = new GrowthItemsDashboardPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);
            const string categoryFieldValue = "Organizational";
            const string categoryFiledText = "Category";

            Log.Info($"Login as {User.FullName} and Go to 'Growth Item' Dashboard");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.ClickGrowthItemDashBoard();
            growthItemsDashboardPage.ChangeViewWidget(GrowthItemWidget.Grid);

            Log.Info("Verify that 'Copy Link with Applied Filters' and 'Clear Filters' buttons should not be displayed before applied filter");
            Assert.IsFalse(growthItemsDashboardPage.IsCopyLinkWithAppliedFiltersButtonDisplayed(), "'Copy Link with Applied Filters' button is displayed");
            Assert.IsFalse(giDashboardGridView.IsClearFiltersButtonDisplayed(), "'Clear Filters' button is displayed");

            var expectedColumnValues = giDashboardGridView.GetColumnValues(categoryFiledText);

            Log.Info("Select 'Organizational' as category and verify that 'Copy Link with Applied Filters' and 'Clear Filter' buttons should be displayed");
            giDashboardGridView.FilterByCategory(categoryFieldValue);
            Assert.IsTrue(growthItemsDashboardPage.IsCopyLinkWithAppliedFiltersButtonDisplayed(), "'Copy Link with Applied Filters' button is not displayed");
            Assert.IsTrue(giDashboardGridView.IsClearFiltersButtonDisplayed(), "'Clear Filters' button is not displayed");

            Log.Info("Click on the 'Copy Link with Applied Filters' button and navigate to that link");
            growthItemsDashboardPage.ClickOnCopyLinkWithAppliedFiltersButton();
            growthItemsDashboardPage.NavigateToCopiedUrl(CSharpHelpers.GetClipboard());

            Log.Info("Verify that Category, Radar Type and Growth Item data should be correct and 'Copy Link with Applied Filters' & 'Clear Filters' buttons should be displayed");
            Assert.AreEqual(categoryFieldValue, growthItemsDashboardPage.GetSelectedCategoryFilterText());
            Assert.AreEqual("(All)", growthItemsDashboardPage.GetSelectedRadarTypeFilterText());
            Assert.IsTrue(growthItemsDashboardPage.IsCopyLinkWithAppliedFiltersButtonDisplayed(), "'Copy Link with Applied Filters' button is not displayed");
            Assert.IsTrue(giDashboardGridView.IsClearFiltersButtonDisplayed(), "'Clear Filters' button is not displayed");

            Assert.IsTrue(giDashboardGridView.GetColumnValues(categoryFiledText).All(a => a.Equals(categoryFieldValue)), "Filter by Category should working properly");

            Log.Info("Click on the 'Clear Filters' button");
            giDashboardGridView.ClearFilter(false);

            var actualColumnValues = giDashboardGridView.GetColumnValues(categoryFiledText);
            Assert.That.ListsAreEqual(expectedColumnValues, actualColumnValues, "'Clear Filters' button is not working as expected");
        }
    }
}