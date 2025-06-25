using System;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class Gpv2DashboardExportToExcelTest : BaseTest
    {
        private static bool _classInitFailed;
        private static int _teamId;
        private static TeamResponse _teamResponse;
        private static AddTeamWithMemberRequest _team;
        private static GrowthPlanItemRequest _growthPlanItemRequest;
        private static RadarQuestionDetailsResponse _radarResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = TeamFactory.GetNormalTeam("Team");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember1.FirstName,
                    LastName = SharedConstants.TeamMember1.LastName,
                    Email = SharedConstants.TeamMember1.Email
                });
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember2.FirstName,
                    LastName = SharedConstants.TeamMember2.LastName,
                    Email = SharedConstants.TeamMember2.Email
                });
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

                var surveyId = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType).Id;
                _radarResponse = setup.GetRadarQuestions(Company.Id, surveyId);
                var competenciesIds = _radarResponse.Dimensions.Select(s => s.Subdimensions).First()
                    .Select(c => c.Competencies).First().Select(i => i.CompetencyId).ToList();

                _growthPlanItemRequest = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, competenciesIds);
                setup.CreateGrowthItem(_growthPlanItemRequest);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_Dashboard_Items_ExportToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);

            const string fileName = "Growth Items.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _teamId);

            //Getting name list of visible column header from grid
            var expectedColumnHeaders = growthPlanDashboard.GetVisibleColumnHeaderNamesFromGrid();

            growthPlanDashboard.ClickOnExportToExcelButton();

            //Read data from spreadsheet
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);
            Assert.AreEqual(tbl.Rows.Count, 1, "More than one raw is present in spreadsheet");

            //Getting name list of visible column header from spreadsheet and verify
            var actualColumnHeaders = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            Assert.AreEqual(expectedColumnHeaders.Count, actualColumnHeaders.Count, "Column count doesn't matched");
            Assert.That.ListsAreEqual(expectedColumnHeaders, actualColumnHeaders, "Column List doesn't matched");

            // Getting the raw values from the grid according to column header
            var expectedRow = expectedColumnHeaders.Select(column => growthPlanDashboard.GetRawValueByColumnHeader(column.Replace("(", "").Replace(")", "").RemoveWhitespace())).ToList();

            // Getting the raw values from spreadsheet
            var actualRow = tbl.Rows.Cast<DataRow>().Select(a => a.ItemArray).First().ToList();

            // Compare the row from the grid to the row from the spreadsheet
            Assert.AreEqual(expectedRow.Count, actualRow.Count, "Raw count doesn't matched");
            Assert.That.ListsAreEqual(expectedRow, actualRow, "Raw list doesn't matched");
        }
    }
}
