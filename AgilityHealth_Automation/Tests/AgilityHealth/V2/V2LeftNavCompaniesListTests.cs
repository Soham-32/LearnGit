using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.Base;
using AtCommon.Utilities;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
//using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;

namespace AgilityHealth_Automation.Tests.AgilityHealth.V2
{
    [TestClass]
    [TestCategory("V2")]
    public class V2LeftNavCompaniesListTests : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void V2_LeftNav_CompanyList_AscendingOrder()
        {
            var login = new LoginPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var pulseDashboard = new PulseDashboardPage(Driver, Log);
            //var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var insightsDashboard = new InsightsDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            // BO
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            var actualCompanyList = leftNav.GetCompanyList();
            var expectedCompanyList = actualCompanyList.OrderBy(n => n).ToList();
            Assert.That.ListsAreEqual(expectedCompanyList, actualCompanyList, "Company List is not in ascending order on 'Business Outcome' dashboard");

            // Pulse
            pulseDashboard.NavigateToPage(Company.Id);
            actualCompanyList = leftNav.GetCompanyList();
            expectedCompanyList = actualCompanyList.OrderBy(n => n).ToList();
            Assert.That.ListsAreEqual(expectedCompanyList, actualCompanyList, "Company List is not in ascending order on 'Pulse' dashboard");

            // GPV2
            //growthPlanDashboard.NavigateToPage(Company.Id);
            //actualCompanyList = leftNav.GetCompanyList();
            //expectedCompanyList = actualCompanyList.OrderBy(n => n).ToList();
            //Assert.That.ListsAreEqual(expectedCompanyList, actualCompanyList, "Company List is not in ascending order on 'Gpv2' dashboard");

            // Insight
            insightsDashboard.NavigateToPage(Company.Id);
            actualCompanyList = leftNav.GetCompanyList();
            expectedCompanyList = actualCompanyList.OrderBy(n => n).ToList();
            Assert.That.ListsAreEqual(expectedCompanyList, actualCompanyList, "Company List is not in ascending order on 'Insight' dashboard");

        }
    }
}