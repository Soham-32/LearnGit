using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthJourney.MultiTeam
{
    [TestClass]
    [TestCategory("GrowthJourney")]
    public class MultiTeamGrowthJourneyExportTests : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static RadarResponse _radar;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            _team = setup.GetCompanyHierarchy(Company.Id)
                .GetTeamByName(Constants.MultiTeamForGrowthJourney)
                .CheckForNull($"<{Constants.MultiTeamForGrowthJourney}> was not found in the response.");
            _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void MultiTeam_GrowthJourney_ExportToExcel()
        {

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            radarPage.NavigateToGrowthJourney(_team.TeamId, _radar.Id, TeamType.MultiTeam);

            const string fileName = "Analysis.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickOnAnalysisExportToExcel();
            var actualPath = FileUtil.WaitUntilFileDownloaded(fileName);
            var expectedPath =
                $@"{new FileUtil().GetBasePath()}Resources\TestData\GrowthJourney\MultiTeam\Analysis_{TestEnvironment}";

            Assert.That.ExcelAreEqual(expectedPath, actualPath);
        }
    }
}