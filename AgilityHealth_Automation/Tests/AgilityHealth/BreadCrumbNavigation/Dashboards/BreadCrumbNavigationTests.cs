using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Dashboards;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BreadCrumbNavigation.Dashboards;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentDashboardBasePage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BreadCrumbNavigation.Dashboards
{
    [TestClass]
    [TestCategory("Dashboards")]
    public class BreadcrumbsNavigationTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Sanity")]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"),
         TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public void V1_Dashboards_BreadCrumbNavigation()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var assessmentDashboardBasePage = new AssessmentDashboardBasePage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var breadCrumbNavigationPage = new BreadCrumbNavigationPage(Driver, Log);
            const string companyDashboardTitle = "Company Management";
            const string dashboardName = "Company Dashboard";

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Verify breadcrumb navigation for v1 dashboards.");
            dashBoardPage.NavigateToPage(Company.Id);

            foreach (DashboardTypes dashboardType in System.Enum.GetValues(typeof(DashboardTypes)))
            {
                switch (dashboardType)
                {
                    case DashboardTypes.TeamDashboard:
                        Log.Info("Navigate to Team Dashboard & Verify breadcrumb navigation.");
                        dashBoardPage.ClickOnTeamDashboard();
                        break;

                    case DashboardTypes.GrowthItems:
                        Log.Info("Navigate to Growth Items Dashboard & Verify breadcrumb navigation.");
                        dashBoardPage.ClickGrowthItemDashBoard();
                        break;

                    case DashboardTypes.FacilitatorDashboard:
                        Log.Info("Navigate to Facilitator Dashboard & Verify breadcrumb navigation.");
                        dashBoardPage.ClickFacilitatorDashboard();
                        break;

                    case DashboardTypes.AssessmentDashboard:
                        Log.Info("Navigate to Assessment Dashboard & Verify breadcrumb navigation.");
                        dashBoardPage.ClickAssessmentDashBoard();
                        var assessmentDashboardTabs = assessmentDashboardBasePage.GetAllTabs().ToList();
                        foreach (var tab in assessmentDashboardTabs)
                        {
                            TabSelection subTab = assessmentDashboardBasePage.AllTabs.FirstOrDefault(subTab => subTab.Value.Contains(tab.Substring(0, 3).ToLower())).Key;
                            assessmentDashboardBasePage.SelectSubTab(subTab);

                            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
                            {
                                breadCrumbNavigationPage.ClickOnCompanyDashboardBreadCrumbLink(dashboardName);
                                Assert.AreEqual(companyDashboardTitle, companyDashboardPage.GetPageTitleText(), "On Company dashboard page, Page title doesn't match.");
                                Driver.Back();
                            }
                            breadCrumbNavigationPage.ClickOnTeamDashboardBreadCrumbLink(User.CompanyName);
                            Assert.IsTrue(dashBoardPage.IsAddTeamButtonDisplayed(), "On Team dashboard page, 'Add A Team' button is not displayed");

                            Driver.Back();
                            breadCrumbNavigationPage.ClickOnAssessmentDashboardBreadCrumbLink();
                            Assert.AreEqual("Manage Assessments", assessmentDashboardListTabPage.GetSubTabName(), "On Assessment List dashboard page, Page title doesn't match.");
                        }
                        break;
                }

                if (User.IsSiteAdmin() || User.IsPartnerAdmin())
                {
                    breadCrumbNavigationPage.ClickOnCompanyDashboardBreadCrumbLink(dashboardName);
                    Assert.AreEqual(companyDashboardTitle, companyDashboardPage.GetPageTitleText(), "On Company dashboard page, Page title doesn't match.");
                    Driver.Back();
                }
                breadCrumbNavigationPage.ClickOnTeamDashboardBreadCrumbLink(User.CompanyName);
                Assert.IsTrue(dashBoardPage.IsAddTeamButtonDisplayed(), "On Team dashboard page, 'Add A Team' button is not displayed");
            }
        }
    }
}
