using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageCustomTypes;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Settings
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_SettingsV2")]
    public class SettingsV2Tests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsPageNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Settings", settingsV2Page.GetSettingsTitle(), $"Settings Title does not matched after navigating in 'Settings' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsAuditLogsNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Audit Logs");
            settingsV2Page.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Audit Trails", settingsV2Page.GetPageTitle(), $"'Audit Trail' Title does not matched after navigating in 'Manage Audit Logs' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsBusinessOutcomesNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Business Outcomes");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Business Outcome Settings", settingsV2Page.GetPageTitle(), $"'Business outcomes' Title does not matched after navigating in 'Manage Settings' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsCustomSettingsNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);
            var manageCustomTypesPage = new ManageCustomTypesPage(Driver, Log);
            var customGrowthPlanSettingsPage = new CustomGrowthPlanSettingsPage(Driver, Log);
                
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Custom Settings");
            customGrowthPlanSettingsPage.ClickOnManageGrowthItemTypesButton();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Custom Growth Item Types", manageCustomTypesPage.GetCustomGrowthItemTypesTitle(), $"'Custom Growth Item Types' Title does not matched after navigating in 'Manage Custom Types' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsManageFeaturesNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Features");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Manage Features", settingsV2Page.GetPageTitle(), $"'Manage Features' Title does not matched after navigating in 'Manage Features' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsGrowthPortalNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Growth Portal");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("AgilityHealth Growth Portal", settingsV2Page.GetPageTitle(), $"'AgilityHealth Growth Portal' title does not matched after navigating in 'Growth Portal' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsIntegrationsNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Integrations");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Add Connection", settingsV2Page.GetPageTitle(), $"'Add Connection' Title does not matched after navigating in 'Manage Integrations' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsKiosksNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            LoginToProductionEnvironment(env);
            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Kiosks");
            settingsV2Page.SelectCompanyFromSettings(companyName);
            Driver.SwitchToLastWindow();

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Kiosk Settings", settingsV2Page.GetPageTitle(), $"'Kiosk Settings' Title does not matched after navigating in 'Manage Kiosks' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsMaturityModelAndAssessmentChecklistNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Maturity Model");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Custom Maturity Model And Assessment Checklist", settingsV2Page.GetPageTitle(), $"'Custom Maturity Model and Assessment Checklist' Title does not matched after navigating in 'Manage Model & Checklist' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsMyProfileNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("My Profile");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Account Settings", settingsV2Page.GetPageTitle(), $"'Account Settings' Title does not matched after navigating in 'Manage Profile\"' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsRadarNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Radar");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Manage Radars", settingsV2Page.GetPageTitle(), $"'Manage Radars' Title does not matched after navigating in 'Manage Radars' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsTeamTagsNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Team Tags");
            settingsV2Page.SelectCompanyFromSettings(companyName);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual($"Tags For {companyName}", settingsV2Page.GetPageTitle(), $"'Tags for {companyName}' Title does not matched after navigating in 'Manage Team Tags' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsSingleSgnOnNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Single Sign On");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Manage SSO For Custom Domain", settingsV2Page.GetPageTitle(), $"'Manage SSO for custom domain' Title does not matched after navigating in 'Manage SSO' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsUsersNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();

            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Users");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Manage Users", settingsV2Page.GetPageTitle(), $"'Manage Users' Title does not matched after navigating in 'Manage Users' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsCampaignsNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            
            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Campaigns");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Manage Campaigns", settingsV2Page.GetPageTitle(), $"'Manage Campaigns' Title does not matched after navigating in 'View Campaigns' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsLanguagesNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("Languages");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("Manage Languages", settingsV2Page.GetPageTitle(), $"'Manage Languages' Title does not matched after navigating in 'Manage Languages' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyV2SettingsExternalLinksNavigationInProd(string env)
        {
            var settingsV2Page = new SettingsV2Page(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            settingsV2Page.NavigateToSettingsPageForV2ForProd(env, companyId);
            settingsV2Page.SelectSettingsOption("External Links");

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SettingsV2.png");
            Assert.AreEqual("External Links", settingsV2Page.GetPageTitle(), $"'External Links' Title does not matched after navigating in 'View Links' for the client - {env}");
        }
    }
}
