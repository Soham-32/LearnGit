using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Company
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Companies")]
    public class CompanyDashboardTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyCompanyDashboardPageNavigationInProd(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            LoginToProductionEnvironment(env);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\CompanyDashboard.png", 10000);
            Assert.AreEqual("Company Management", companyDashboardPage.GetPageTitleText(), $"Page Title is incorrect after login for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyThatCreateCompanyStepperLoadedSuccessfully(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1CompanyProfilePage = new AddCompany1CompanyProfilePage(Driver, Log);
            var addCompanyHeaderWidgetPage = new AddCompanyHeaderWidget(Driver, Log);

            Log.Info("Navigate to Company dashboard page");
            LoginToProductionEnvironment(env);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Click on the Add Company button and verify the page");
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1CompanyProfilePage.WaitUntilLoaded();
            Assert.IsTrue(addCompanyPage1CompanyProfilePage.IsCompanyNameVisible(), $"Add company name page is not displayed - {env}");

            Log.Info("Click on the Close button from the Add Company page");
            addCompanyHeaderWidgetPage.ClickCloseButton();
            Assert.AreEqual("Company Management", companyDashboardPage.GetPageTitleText(), $"Page Title is incorrect after login for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyThatShowHideColumnSettingsLoadedSuccessfully(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);

            Log.Info("Navigate to Company dashboard page");
            LoginToProductionEnvironment(env);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Click on the Show/Hide columns button and verify the columns name");
            companyDashboardPage.ShowColumnSettings();
            Assert.IsTrue(companyDashboardPage.IsCompanyShowHideColumnsListDisplayed(), $"The Show/Hide columns list not displayed - {env}");

            Log.Info("Click on the Show/Hide columns button");
            companyDashboardPage.HideColumnSettings();
            Assert.IsFalse(companyDashboardPage.IsShowHideColumnsTextDisplayed(), $"The Show/Hide columns text is displayed - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyThatEditCompanyAllSteppersLoadedSuccessfully(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var editCompanyHeaderWidgetPage = new EditCompanyHeaderWidget(Driver, Log);
            var editRadarSelectionPage = new EditRadarSelectionPage(Driver, Log);
            var editSubscriptionPage = new EditCompanySubscriptionPage(Driver, Log);
            var editSecurityPage = new EditCompanySecurityPage(Driver, Log);
            var addCompanyHeaderWidgetPage = new AddCompanyHeaderWidget(Driver, Log);

            Log.Info("Navigate to Company dashboard page");
            LoginToProductionEnvironment(env);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Search a company by name and verify that the Company Profile tab is displayed");
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickEditIconByCompanyName(companyName);
            Assert.IsTrue(addCompanyPage1.IsCompanyNameVisible(), $"Company Profile page is not displayed - {env}");

            Log.Info("Verify that the Radar Selection tab is displayed");
            editCompanyHeaderWidgetPage.ClickOnRadarSelectionTab();
            Assert.AreEqual("Radar Selection", editRadarSelectionPage.GetRadarSelectionHeaderText(), $"The 'Radar Selection' section text is not matched - {env}");

            Log.Info("Verify that the Subscription tab is displayed");
            editCompanyHeaderWidgetPage.ClickOnSubscriptionTab();
            editSubscriptionPage.WaitUntilLoaded();
            Assert.AreEqual("Subscriptions", editSubscriptionPage.GetSubscriptionsSectionText(), $"The 'Subscriptions' section text is not matched - {env}");

            Log.Info("Verify that the Security tab is displayed");
            editCompanyHeaderWidgetPage.ClickOnSecurityTab();
            editSecurityPage.WaitUntilLoaded();
            Assert.AreEqual("Session Timeout (Minutes)", editSecurityPage.GetSessionTimeoutFieldText(), $"The 'Session Timeout (Minutes)' field text is not matched - {env}");
            addCompanyHeaderWidgetPage.ClickCloseButton();
            Assert.AreEqual("Company Management", companyDashboardPage.GetPageTitleText(), $"Page Title is incorrect after login for the client - {env}");
        }
    }
}
