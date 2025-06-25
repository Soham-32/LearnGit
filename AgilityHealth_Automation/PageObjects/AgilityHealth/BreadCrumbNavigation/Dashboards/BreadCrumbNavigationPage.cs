using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BreadCrumbNavigation.Dashboards
{
    public class BreadCrumbNavigationPage : BasePage
    {
        public BreadCrumbNavigationPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By AssessmentBreadCrumbLink = By.CssSelector("div[class='breadcrumbs clear'] a[href$='AssessmentManagementDashboard']");
        private static By CompanyDashboardBreadCrumbLink(string dashboardName) => By.XPath($"//div[contains(@class,'breadcrumbs clear')]//a[text()='{dashboardName}']");
        private static By TeamDashboardBreadcrumbLink(string companyName) => By.XPath($"//a[text()='{companyName}']");

        private readonly By CompanyName = By.XPath("//div[@class='breadcrumbs']/ul/li/a[contains(@href, 'company')][font/font or .]");
        private static By TeamName => By.XPath("//div[@class='breadcrumbs']/ul/li[3]/a[font/font or .]");
        private static By AssessmentName => By.XPath("//i/parent::a[@aria-current='page']");

        public void ClickOnCompanyDashboardBreadCrumbLink(string dashboardName)
        {
            Log.Step(nameof(BreadCrumbNavigationPage), $"Click on '{dashboardName}' breadcrumb link.");
            Wait.UntilElementClickable(CompanyDashboardBreadCrumbLink(dashboardName)).Click();
        }

        public void ClickOnTeamDashboardBreadCrumbLink(string companyName)
        {
            Log.Step(nameof(BreadCrumbNavigationPage), $"Click on '{companyName}' breadcrumb link.");
            Wait.UntilElementClickable(TeamDashboardBreadcrumbLink(companyName)).Click();
        }

        public void ClickOnAssessmentDashboardBreadCrumbLink()
        {
            Log.Step(nameof(BreadCrumbNavigationPage), "Click on 'Assessment Dashboard' breadcrumb link.");
            Wait.UntilElementClickable(AssessmentBreadCrumbLink).Click();
        }
        public string GetCompanyName()
        {
            Log.Step(nameof(BreadCrumbNavigationPage), "Get the Company Name");
            return Wait.UntilElementVisible(CompanyName).GetText().Trim();
        }
        public string GetAssessmentName()
        {
            Log.Step(nameof(BreadCrumbNavigationPage), "Get the Assessment Name");
            Wait.UntilJavaScriptReady();
            return Wait.UntilElementVisible(AssessmentName).GetText().Trim();
        }
        public string GetTeamName()
        {
            Log.Step(nameof(BreadCrumbNavigationPage), "Get the Team Name");
            string teamName = Wait.UntilElementVisible(TeamName).GetText().Trim().Replace("Assessments", "").Trim();
            return teamName;
        }
    }
}
