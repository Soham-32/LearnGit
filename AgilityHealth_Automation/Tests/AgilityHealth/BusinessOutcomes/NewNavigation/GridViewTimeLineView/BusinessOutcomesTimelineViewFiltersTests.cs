using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.GridViewTimeLineView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesTimelineViewFiltersTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _projectCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var labels = setup.GetBusinessOutcomesAllLabels(Company.Id);
            var projects = labels.First(a => a.Name == BusinessOutcomesCardTypeTags.ProjectsTimeline.GetDescription()).Tags;
            _projectCardResponse = CreateBusinessOutcome(SwimlaneType.Projects, 0, projects);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]

        public void BusinessOutcome_TimeLineView_Filter()
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
            businessOutcomesViewPage.ClickOnGridViewDropdownOptions("Timeline View");

            Log.Info("Clicking on More Menu of the 'Card Title' column to access filter options");
            businessOutcomesViewPage.ClickOnMoreMenuOfCardTitleColumn();

            Log.Info("Verifying the filter options available in 'Card Title' column");
            var expectedIdColumnFilterOptions = new List<string> { "Sort Ascending", "Sort Descending", "Filter", "Columns" };
            var actualIdColumnFilterOptions = businessOutcomesViewPage.GetFilterOptionOfCardTitleColumn();
            Assert.That.ListsAreEqual(expectedIdColumnFilterOptions, actualIdColumnFilterOptions);

            Log.Info($"Applying filter using title: {_projectCardResponse.Title}");
            businessOutcomesViewPage.ClickFilterOptions();
            businessOutcomesViewPage.EnterTextInFilterInput(_projectCardResponse.Title);
            businessOutcomesViewPage.ClickOnFilterButton();

            Log.Info("Verifying filtered Business Outcome details");
            var actualBusinessOutcomeDetails = businessOutcomesViewPage.GetBusinessOutcomesTimelineViewDetails();
            Assert.AreEqual(_projectCardResponse.Title, actualBusinessOutcomeDetails.Title, "Title does not match");
            Assert.AreEqual(_projectCardResponse.PrettyId.ToString(), actualBusinessOutcomeDetails.Id, "ID does not match");
            Assert.IsTrue(businessOutcomesViewPage.IsTimelineViewGraphDetailsDisplayed(), $" Graph is not present");
        }
    }

}
