using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
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
    public class AssessmentListTabTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyManageAssessmentListPageNavigationInProd(string env)
        {
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            assessmentDashboardListTabPage.NavigateToAssessmentListTabPageForProd(env, companyId);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\AssessmentDashboard.png");
            Assert.AreEqual("Manage Assessments", assessmentDashboardListTabPage.GetManageAssessmentsTitle(), $"Assessment list tab title does not matched after navigating in 'Assessment list tab' for the client - {env}");
        }
    }
}
