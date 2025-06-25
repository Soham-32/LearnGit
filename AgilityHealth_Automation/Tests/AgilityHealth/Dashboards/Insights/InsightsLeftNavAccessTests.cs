using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("Dashboard")]
    [TestCategory("Critical")]
    public class InsightsLeftNavAccessTests : BaseTest
    {
        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_SA_PA_CA_LeftNavAccess()
        {
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Assert.IsTrue(leftNavPage.IsCompanyItemSelected(), "Company level should be selected by default");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Insights_BL_OL_TA_LeftNavAccess()
        {
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            v1Header.ClickOnInsightsDashboardLink();

            Assert.IsTrue(leftNavPage.IsFirstTeamItemSelected(), "First team should be selected by default");
            Assert.IsTrue(leftNavPage.IsCompanyItemDisabled(), "Company level should be disabled");
        }
    }
}
