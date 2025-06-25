using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Settings
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_SettingsV1")]
    public class SettingsV1Tests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageNavigationInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Radars", settingsPage.GetManageRadarsButtonTitle(), $"Manage radars Title does not matched after navigating in 'V1 settings' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageRadarsInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Radars");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Radars", settingsPage.GetPageTitle(), $"'Manage Radars' Title does not matched after navigating in 'Manage Radars' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageTagsInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Tags");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual($"Tags For {companyName}", settingsPage.GetPageTitle(), $"'Tags For {companyName}' Title does not matched after navigating in 'Manage Tags' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageUsersInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Users");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Users", settingsPage.GetPageTitle(), $"'Manage Users' Title does not matched after navigating in 'Manage Users' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageFinancialsInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Financials");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Financials", settingsPage.GetPageTitle(), $"'Manage Financials' Title does not matched after navigating in 'Manage Financials' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageFeaturesInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Features");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Features", settingsPage.GetPageTitle(), $"'Manage Features' Title does not matched after navigating in 'Manage Features' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageCampaignsInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Campaigns");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Campaigns", settingsPage.GetPageTitle(), $"'Manage Campaigns' Title does not matched after navigating in 'Manage Campaigns' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageIntegrationsInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Integrations");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Add Connection", settingsPage.GetPageTitle(), $"'Manage Integrations' Title does not matched after navigating in 'Manage Integrations' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageMyProfileInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage My Profile");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Account Settings", settingsPage.GetPageTitle(), $"'Account Settings' Title does not matched after navigating in 'Manage My Profile' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageAuditLogsInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Audit Logs");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Audit Trails", settingsPage.GetPageTitle(), $"'Audit Trails' Title does not matched after navigating in 'Audit Logs' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageSingleSignOnInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Single SignOn");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage SSO For Custom Domain", settingsPage.GetPageTitle(), $"'Manage SSO for custom domain' Title does not matched after navigating in 'Manage Single SignOn' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageGrowthPortalInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            
            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Growth Portal");
            settingsPage.ClickOnGoButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Growth Portal Content", settingsPage.GetPageTitle(), $"'Manage Growth Portal Content' Title does not matched after navigating in 'Manage Growth Portal' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageCustomMaturityModelAndAssessmentChecklistInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Custom Maturity Model and Assessment Checklist");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Custom Maturity Model And Assessment Checklist", settingsPage.GetPageTitle(), $"'Custom Maturity Model And Assessment Checklist' Title does not matched after navigating in 'Manage Custom Maturity Model and Assessment Checklist' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageKiosksInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Kiosks");
            settingsPage.SelectCompanyFromSettings(companyName);
            Driver.SwitchToLastWindow();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Kiosk Settings", settingsPage.GetPageTitle(), $"'Kiosk Settings' Title does not matched after navigating in 'Manage Kiosks' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageExternalLinksInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage External Links");
            settingsPage.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("External Links", settingsPage.GetPageTitle(), $"'External Links' Title does not matched after navigating in 'Manage External Links' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV1SettingsPageManageLanguagesInProd(string env)
        {
            var settingsPage = new SettingsPage(Driver, Log);

            LoginToProductionEnvironment(env);

            settingsPage.NavigateToSettingsPageForV1ForProd(env);
            settingsPage.SelectSettingsOption("Manage Languages");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV1.png");
            Assert.AreEqual("Manage Languages", settingsPage.GetPageTitle(), $"'Manage Languages' Title does not matched after navigating in 'Manage Languages' for the client - {env}");
        }
    }
}
