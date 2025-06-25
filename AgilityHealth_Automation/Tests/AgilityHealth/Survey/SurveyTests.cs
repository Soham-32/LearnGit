using System;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Survey
{
    [TestClass]
    [TestCategory("Survey")]
    public class SurveyTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical"), TestCategory("Survey"), TestCategory("OrgLeader"), TestCategory("Rocket")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", @"Resources\TestData\surveys.xml", "row", DataAccessMethod.Sequential)]
        public void Survey_Questions()
        {
            Log.Info($"Starting test: Survey Questions for {TestContext.DataRow["Description"]}");
            var surveyPage = new SurveyPage(Driver, Log);

            Driver.NavigateToPage(TestContext.DataRow[$"{TestEnvironment.EnvironmentName}_SurveyLink"].ToString());

            var actualIds = surveyPage.GetQuestionIds();
            foreach (var item in actualIds)
            {
                Console.WriteLine(item);
            }

            var expectedIds = FileUtil.TextFileToList($"Resources\\TestData\\SurveyQuestions\\{TestEnvironment.EnvironmentName}_{TestContext.DataRow["QuestionIdFile"]}");

            var missing = expectedIds.Except(actualIds).ToList();

            var extra = actualIds.Except(expectedIds).ToList();

            if (missing.Any() || extra.Any())
            {
                var failMessage = $"Survey Question Validation failed for {TestContext.DataRow["Description"]}. ";
                if (missing.Any()) { failMessage += $"Missing questions: <{string.Join(",", missing)}>. "; }
                if (extra.Any()) { failMessage += $"Extra questions: <{string.Join(",", extra)}>. "; }
                failMessage += $"Survey link: {TestContext.DataRow[$"{TestEnvironment.EnvironmentName}_SurveyLink"]}";

                Assert.Fail(failMessage);
            }

        }

    }
}
