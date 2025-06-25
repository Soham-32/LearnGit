using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.IdeaBoard
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Ideaboard")]
    public class IndividualAssessmentIdeaboardTests : BaseTest
    {
        public EnvironmentTestInfo JsonResponse = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyTeamAssessmentIdeaboardPageNavigationInProd(string env)
        {
            if (env == "truist" || env == "citi" || env == "prudential" || env == "nscorp" || env == "7eleven" || env == "nasa" || env == "usaf") { return; }
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            LoginToProductionEnvironment(env);

            var companyId = JsonResponse.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList()
                .FirstOrDefault();
            var assessmentUid = JsonResponse.Environments.Where(a => a.Name.Equals(env)).Select(a => a.GoiTeam.AssessmentUid).ToList()
                .FirstOrDefault();
            ideaboardPage.NavigateToTeamAssessmentIdeaboardPageForProd(env, companyId, assessmentUid);

            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\IdeaBoard.png", 10000);
            Assert.IsTrue(ideaboardPage.IsIdeaBoardTitleDisplayed(), "Ideaboard title is not displayed");
        }
    }
}
