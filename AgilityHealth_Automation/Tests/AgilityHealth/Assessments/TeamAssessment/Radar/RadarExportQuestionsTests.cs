using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class RadarExportQuestionsTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")]
        [TestCategory("KnownDefectAsBL")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void Radar_TA_ExportQuestionsToExcel()
        {
            var team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.RadarTeam)
                .CheckForNull($"{SharedConstants.RadarTeam} was not found in the response.");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);

            const string filename = "Assessment Results.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(filename);

            radarPage.ClickExportQuestionsButton();
            var actualPath = FileUtil.WaitUntilFileDownloaded(filename);
            var expectedPath = $@"{FileUtil.GetBasePath()}Resources\TestData\SurveyQuestions\{filename}";

            Assert.That.ExcelAreEqual(expectedPath, actualPath);

        }
    }
}