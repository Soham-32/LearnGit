using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes.BusinessOutcomesCardView
{
    [TestClass]
    [TestCategory("HeartBeatChecks")]
    public class BusinessOutcomesCardViewSubHeadersTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DataRow("hhc")]
        [DataRow("srca")]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBusinessOutcomesSubHeaders(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomeDashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            if (env == "srca")
            {
                addBusinessOutcomePage.NavigateToBusinessOutcomesPageForSaDomain(env, companyId);
            }

            else
            {
                addBusinessOutcomePage.NavigateToBusinessOutcomesPageForProd(env, companyId);
            }

            addBusinessOutcomePage.WaitUntilBusinessOutcomesPageLoaded();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesSubHeaders.png", 10000);
            Assert.IsTrue(businessOutcomeDashboardPage.AreBusinessOutcomeSubHeadersDisplayed(),"Business Outcomes sub-headers are not displayed");

            

        }

    }
}
