using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.GridViewTimeLineView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesGridAndTimelineViewHeaderTests : BusinessOutcomesBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void VerifyGridViewHeaders()
        {
            VerifyViewHeaders("Grid View");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void VerifyTimelineViewHeaders()
        {
            VerifyViewHeaders("Timeline View");
        }

        private void VerifyViewHeaders(string viewType)
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
                Log.Info("Verifying Grid View header and footer texts");
                Assert.AreEqual("Total Cards", businessOutcomesViewPage.GetGridViewTitleText(), "Grid View title is not matched");
                Assert.AreEqual("items per page", businessOutcomesViewPage.GetGridViewFooterText(), "Grid View footer text is not matched");
            }
            else if (viewType == "Timeline View")
            {
                Log.Info("Verifying Timeline View header and footer texts");
                Assert.AreEqual("Total Cards", businessOutcomesViewPage.GetGridViewTitleText(), "Timeline View title is not matched");
                Assert.IsTrue(businessOutcomesViewPage.IsTimelineViewTimeOptionsDisplayed(), $"{viewType} Time period options are not present on {viewType} page");
            }
        }
    }
}