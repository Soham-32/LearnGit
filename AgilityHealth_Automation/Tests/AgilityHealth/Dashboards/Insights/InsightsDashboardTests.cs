using AgilityHealth_Automation.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.EnterpriseAgility.EnterpriseAgilityPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights
{
    [TestClass]
    [TestCategory("Insights"),TestCategory("Dashboard")]
    [TestCategory("Critical")]
    public class InsightsDashboardTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Insights_StructuralAgility_SmokeTest()
        {
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var structuralAgilityPage = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
                leftNav.SelectCompany(User.CompanyName);
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Log.Info("Navigate to Structural agility dashboard");
            teamAgility.ClickOnStructuralAgilityTab();

            Log.Info("Verify that all the widgets are loaded successfully in Structural agility dashboard");
            foreach (var widget in StructuralAgilityWidgets.GetList())
            {
                if (widget == StructuralAgilityWidgets.AgileAdoption)
                {
                    structuralAgilityPage.SelectTeamCategories("Agile Adoption");
                }
                Assert.IsTrue(structuralAgilityPage.IsWidgetDisplayed(widget), $"<{widget.Title}> is not displayed");
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Insights_EntrepriseAgility_SmokeTest()
        {
            var login = new LoginPage(Driver, Log);
            var v1Header = new TopNavigation(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);
            var structuralAgilityPage = new StructuralAgilityPage(Driver, Log);
            var teamAgility = new TeamAgilityPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to the login page and login with insight user credentials.");
            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                companyDashboard.WaitUntilLoaded();
                v2Header.ClickInsightsButton();
                leftNav.SelectCompany(User.CompanyName);
            }
            else
            {
                v1Header.ClickOnInsightsDashboardLink();
            }

            Log.Info("Navigate to Enterprise agility dashboard");
            teamAgility.ClickOnEnterpriseAgilityTab();

            Log.Info("Verify that all the widgets are loaded successfully in Enterprise agility dashboard");
            GetEnterpriseAgilityWidgetTitles().ForEach(widgetTitle => Assert.IsTrue(structuralAgilityPage.IsWidgetDisplayed(widgetTitle), $"<{widgetTitle}> is not displayed"));
        }
    }
}