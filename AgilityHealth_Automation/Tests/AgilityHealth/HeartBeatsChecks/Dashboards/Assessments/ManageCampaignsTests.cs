using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AtCommon.Api;
using System.Linq;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards.Assessments
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Dashboard")]
    public class ManageCampaignsTests : BaseTest
    {
        public EnvironmentTestInfo JsonResponse = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyManageCampaignsPageNavigationInProd(string env)
        {
            var manageCampaignsTabPage = new ManageCampaignsTabPage(Driver, Log);

            var companyId = JsonResponse.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            manageCampaignsTabPage.NavigateToManageCampaignsTabForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\ManageCampaigns.png");
            Assert.AreEqual("Manage Campaigns", manageCampaignsTabPage.GetManageCampaignHeaderTitleText(), $"Manage Campaigns tab title does not matched after navigating in 'Manage Campaigns tab' for the client - {env}");
        }
    }
}