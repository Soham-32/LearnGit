using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Accounts
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_MyProfile")]
    public class MyProfileTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]

        public void VerifyMyProfileNavigationInProd(string env)
        {
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var topNavigationPage = new TopNavigation(Driver, Log);
            var header = new TopNavigation(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();

            Log.Info("Log in and click on company name");
            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);

            Log.Info("Verify that the My profile tab");
            header.HoverOnNameRoleSection();
            header.ClickOnMyProfile();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\MyProfile.png");
            Assert.AreEqual("My Profile", accountSettingsPage.GetMyProfileSectionText(), $"My profile section text does not matched after navigating in 'My profile' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyMyNotificationTabNavigationInProd(string env)
        {
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var topNavigationPage = new TopNavigation(Driver, Log);
            var header = new TopNavigation(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();

            Log.Info("Log in and click on company name");
            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);

            Log.Info("Verify that the My Notifications tab");
            header.HoverOnNameRoleSection();
            header.ClickOnMyProfile();
            accountSettingsPage.SelectTab("My Notifications");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\MyProfile.png");
            Assert.AreEqual("Team Name", accountSettingsPage.GetMyTeamNameColumnText(), $"Team Name column text does not matched after navigating in 'My Notification' tab for the client - {env}");
        }
    }
}
