using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.AhTrial;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.AHTrial
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_AhTrial")]
    public class AhTrialTests : BaseTest
    {
        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyAhTrialNavigationInProd(string env)
        {
            var ahTrialSignUpFormPage = new AhTrialSignUpFormPage(Driver, Log);

            Log.Info("Navigate to AhTrial link and verify that the title text displayed");
            ahTrialSignUpFormPage.NavigateToAhTrialPageForProd(env);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\AHTrail.png");
            Assert.IsTrue(ahTrialSignUpFormPage.IsTitleTextDisplayed(), "Ah trial text is not displayed");
        }
    }
}
