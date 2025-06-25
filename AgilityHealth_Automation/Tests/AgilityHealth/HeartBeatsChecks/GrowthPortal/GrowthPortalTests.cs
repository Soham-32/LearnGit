using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPortal;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.GrowthPortal
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_GrowthPortal")]
    public class GrowthPortalTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyTeamGrowthPortalPageNavigationInProd(string env)
        {
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            growthPortalPage.NavigateToGrowthPortalPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\GrowthPortal.png");
            Assert.IsTrue(growthPortalPage.IsPageTitleDisplayed(), $"Page title does not matched after navigating in 'Growth portal' for the client - {env}");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void GrowthPortal_ViewContent(string env)
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();

            Log.Info($"Log in and navigate to Growth Portal page for the - {env}");
            LoginToProductionEnvironment(env);
            growthPortalPage.NavigateToGrowthPortalPageForProd(env, companyId);

            Log.Info("Select Assessment, click on comeptency from the tree view and verify the competency sections");
            growthPortalPage.SelectAssessment();
            growthPortalPage.ClickSelectButton();
            growthPortalPage.TreeViewExpandNode();
            growthPortalPage.TreeViewExpandFirstSubNode();
            growthPortalPage.ClickCompetency();
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Health), "The 'Health' section did not display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Videos), "The 'Videos' section did not display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Resources), "The 'Resources' section did not display");
        }
    }
}
