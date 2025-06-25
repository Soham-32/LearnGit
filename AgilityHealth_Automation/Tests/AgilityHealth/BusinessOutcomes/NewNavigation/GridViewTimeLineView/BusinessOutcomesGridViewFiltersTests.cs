using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.GridViewTimeLineView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesGridViewFiltersTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;
        private static MemberResponse _member;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            _member = setup.GetCompanyMember(Company.Id, Constants.TeamMemberEmail1);
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent, 0, null, new List<string> { _member.Uid.ToString() });
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]

        public void BusinessOutcome_GridView_Filters()
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

            Log.Info("Selecting 'Grid View' from the dropdown");
            businessOutcomesViewPage.ClickOnGridViewDropdownOptions("Grid View");

            Log.Info("Clicking on Card Type dropdown and selecting 'Business Outcomes'");
            addBusinessOutcomePage.ClickOnCardTypeDropdown();
            businessOutcomesViewPage.ClickOnCardTypeDropdownOptions("Business Outcomes");

            Log.Info("Clicking on More Menu of Card Title column and verifying filter options");
            businessOutcomesViewPage.ClickOnMoreMenuOfCardTitleColumn();
            var expectedIDcolumnFilterOptions = new List<string> { "Sort Ascending", "Sort Descending", "Filter", "Columns" };
            var actualIDcolumnFilterOptions = businessOutcomesViewPage.GetFilterOptionOfCardTitleColumn();
            Assert.That.ListsAreEqual(expectedIDcolumnFilterOptions, actualIDcolumnFilterOptions);

            Log.Info("Applying filter on Card Title column");
            businessOutcomesViewPage.ClickFilterOptions();
            businessOutcomesViewPage.EnterTextInFilterInput(_response.Title);
            businessOutcomesViewPage.ClickOnFilterButton();

            Log.Info("Verifying filtered Business Outcome details");
            var actualBusinessOutcomeDetails = businessOutcomesViewPage.GetBusinessOutcomeFromGrid();
            Assert.AreEqual(_response.Title, actualBusinessOutcomeDetails.Title, "Title does not match");
            Assert.AreEqual(_response.PrettyId.ToString(), actualBusinessOutcomeDetails.Id, "ID does not match");
        }

    }
}
