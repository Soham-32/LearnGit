using System.Collections.Generic;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.GridViewTimeLineView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"),TestCategory("NewNavigation")]
    public class BusinessOutcomesGridAndTimelineViewColumnsTests : BusinessOutcomesBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void VerifyGridViewColumn()
        {
            VerifyColumnsForView("Grid View");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void VerifyTimeLineViewColumn()
        {
            VerifyColumnsForView("Timeline View");
        }

        private void VerifyColumnsForView(string viewType)
        {
            Log.Info("Initializing required pages");
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomesViewPage = new BusinessOutcomesViewPage(Driver, Log);

            Log.Info("Navigating to Login Page and Logging into the application");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to Business Outcomes Dashboard and Clicking on Grid View tab");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            addBusinessOutcomePage.ClickOnGridViewTab();

            Log.Info("Verifying Grid View dropdown options");
            var expectedGridViewDropdownValues = new List<string> { "Grid View", "Timeline View" };
            var actualGridViewDropdownValues = businessOutcomesViewPage.GetGridViewDropdownOptions();
            Assert.That.ListsAreEqual(expectedGridViewDropdownValues, actualGridViewDropdownValues);

            Log.Info($"Selecting '{viewType}' from the dropdown");
            businessOutcomesViewPage.ClickOnGridViewDropdownOptions(viewType);

            if (viewType == "Grid View")
            {
                Log.Info("Selecting 'Business Outcomes' from Card Type dropdown");
                addBusinessOutcomePage.ClickOnCardTypeDropdown();
                businessOutcomesViewPage.ClickOnCardTypeDropdownOptions(BusinessOutcomesCardType.BusinessOutcomes.GetDescription());
            }

            Log.Info("Clicking on More Menu of Card Title column and verifying filter options");
            businessOutcomesViewPage.ClickOnMoreMenuOfCardTitleColumn();
            var expectedIdColumnFilterOptions = new List<string> { "Sort Ascending", "Sort Descending", "Filter", "Columns" };
            var actualIdColumnFilterOptions = businessOutcomesViewPage.GetFilterOptionOfCardTitleColumn();
            Assert.That.ListsAreEqual(expectedIdColumnFilterOptions, actualIdColumnFilterOptions);

            businessOutcomesViewPage.ClickColumnOptionFromMoreMenu();

            if (viewType == "Grid View")
            {
                Log.Info("Verifying column headers for Grid View");
                var expectedGridViewColumnOptions = new List<string> { "ID", "Card Title", "Progress", "Team", "Target", "Actual", "Comment", "Updated By" };
                var actualColumnOptions = businessOutcomesViewPage.GetColumnsValueFromTable();
                Assert.That.ListsAreEqual(expectedGridViewColumnOptions, actualColumnOptions, $"{viewType} column headers do not match expected values.");
            }
            else
            {
                Log.Info("Verifying column headers for Timeline View");
                var expectedTimelineViewColumnOptions = new List<string> { "ID", "Card Title", "Owners", "Start Date", "End Date", "Progress" };
                var actualTimelineViewColumnOptions = businessOutcomesViewPage.GetColumnsValueFromTable();
                Assert.That.ListsAreEqual(expectedTimelineViewColumnOptions, actualTimelineViewColumnOptions, $"{viewType} column headers do not match expected values.");
            }
        }
    }
}


