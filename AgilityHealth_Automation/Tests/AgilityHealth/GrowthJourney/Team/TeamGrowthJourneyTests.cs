using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthJourney.Team
{
    [TestClass]
    [TestCategory("GrowthJourney")]
    public class TeamGrowthJourneyTests : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static RadarResponse _radar;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            _team = setup.GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.RadarTeam)
                .CheckForNull($"<{SharedConstants.RadarTeam}> was not found in the response.");
            _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
            _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_team.Name).TeamId;
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Team_GrowthJourney_ExportToPDF()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            radarPage.NavigateToGrowthJourney(_team.TeamId, _radar.Id, TeamType.Team);

            var filename = $"{SharedConstants.RadarTeam} {User.CompanyName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(filename);

            radarPage.SelectSurveyType(SharedConstants.TeamAssessmentType);

            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(filename), $"{filename} isn't downloaded successfully");

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Sanity")]
        [TestCategory("CompanyAdmin")]
        public void Team_GrowthJourney_VerifyFilterFunctionality()
        {
            var login = new LoginPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the 'Growth Journey' page of the team - '{_team.Name}'. and verify that all the radars are present in the 'Radar Type' dropdown");
            growthJourneyPage.NavigateToGrowthJourneyPage(_teamId);
            var actualRadarTypes = growthJourneyPage.GetAllRadarTypesFromDropdown();
            var expectedRadarTypes = Constants.RadarTypesListForGrowthJourney;
            Assert.That.ListsAreEqual(expectedRadarTypes, actualRadarTypes, "'Radar Type' list doesn't match");

            Log.Info("Verify that all the assessments are present in the 'Compare Assessments'.");
            growthJourneyPage.SelectRadarFromRadarTypeDropDown(Constants.AtTeamHealth2Radar);
            growthJourneyPage.OpenFilterSidebar();
            var assessmentListFromLeftNav = growthJourneyPage.GetAllAssessmentList();
            var expectedAssessmentList = Constants.AssessmentListForAutomationRadarTeam;
            Assert.That.ListsAreEqual(expectedAssessmentList, assessmentListFromLeftNav, "'Assessment list' doesn't match");

            Log.Info("Verify that 'Assessments' tab is present on the 'left nav' filter bar");
            var topFilterTabList = growthJourneyPage.GetTabsListFromFilter().ListToString();
            Assert.AreEqual(topFilterTabList, "Assessments", "'Assessments' tab is not present in the 'left nav' filter bar");

            var assessmentListFromCompareRadar = growthJourneyPage.GetHeaderList();
            foreach (var assessment in assessmentListFromLeftNav)
            {
                Log.Info($"Verify that '{assessment}' is selected by default and also present in the  'Compare Radar Analysis' column");
                Assert.IsTrue(growthJourneyPage.IsAssessmentCheckboxChecked(assessment), $"Checkbox for the assessment-{assessment} is not checked.");
                Assert.That.ListContains(assessmentListFromCompareRadar, assessment, $"{assessment} is not present in the 'Compare Assessment'");
            }
            Assert.That.ListContains(assessmentListFromCompareRadar, "Points Change", "'Points Change' is not present in the 'Compare radar' table");
            Assert.That.ListContains(assessmentListFromCompareRadar, "Percent Change", "'Percent Change' is not present in the 'Compare radar' table");

            Log.Info($"Verify that unchecked assessment {assessmentListFromLeftNav[0]} is not and checked assessment {assessmentListFromLeftNav[1]} is present on the 'Compare Radar Analysis column'");
            growthJourneyPage.UnCheckAssessmentTypeCheckbox(assessmentListFromLeftNav[0]);
            Assert.IsFalse(growthJourneyPage.IsItemPresentInColumnList(assessmentListFromLeftNav[0]), $"{assessmentListFromLeftNav[0]} is still present in the 'Compare Radar Analysis' column");
            Assert.IsTrue(growthJourneyPage.IsItemPresentInColumnList(assessmentListFromLeftNav[1]), $"{assessmentListFromLeftNav[1]} is not present in the 'Compare Radar Analysis' column");

            Log.Info($"Verify that unchecked assessments {assessmentListFromLeftNav[0]} & {assessmentListFromLeftNav[1]} are not present on the 'Compare Radar Analysis column'");
            growthJourneyPage.UnCheckAssessmentTypeCheckbox(assessmentListFromLeftNav[1]);
            Assert.IsFalse(growthJourneyPage.IsItemPresentInColumnList(assessmentListFromLeftNav[0]), $"{assessmentListFromLeftNav[0]} is not present in the 'Compare Radar Analysis' column");
            Assert.IsFalse(growthJourneyPage.IsItemPresentInColumnList(assessmentListFromLeftNav[1]), $"{assessmentListFromLeftNav[1]} is not present in the 'Compare Radar Analysis' column");
        }
    }
}