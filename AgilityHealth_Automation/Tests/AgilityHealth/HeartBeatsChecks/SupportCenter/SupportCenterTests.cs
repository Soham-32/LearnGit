using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.SupportCenter
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_SupportCenter")]
    public class SupportCenterTests : BaseTest
    {
        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifySupportCenterPageNavigationInProd(string env)
        {
            var topNavigation = new TopNavigation(Driver, Log);

            LoginToProductionEnvironment(env);

            topNavigation.NavigateToSupportCenterPage();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\SupportCenter.png");
            Assert.IsTrue(topNavigation.IsResourcesLinkDisplayed(), "Resource link is not displayed");
        }
    }
}
