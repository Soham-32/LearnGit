using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Radars"), TestCategory("EnterpriseTeam")]
    public class EnterpriseTeamExportAssessmentChecklistTests : BaseTest
    {

        private static TeamHierarchyResponse _team;
        private static RadarResponse _radar;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            _team = setup.GetCompanyHierarchy(Company.Id)
                .GetTeamByName(Constants.EnterpriseTeamForMtEtGrowthItems)
                .CheckForNull($"<{Constants.EnterpriseTeamForMtEtGrowthItems}> was not found in the response.");
            _radar = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
        }

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void EnterpriseTeam_Radar_ExportAssessmentChecklist()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            radarPage.NavigateToPage(_team.TeamId, _radar.Id, TeamType.EnterpriseTeam);

            const string fileName = "AssessmentChecklist.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickExportAssessmentChecklistButton();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            var expectedColumns = new List<string>
                {"Team", "Assessment", "Start Date", "Automation Item 1", "Automation Item 2"};

            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Column count does not match.");
            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns.ElementAt(i)}' Actual='{actualColumns[i]}'");
                Assert.IsTrue(actualColumns[i].Contains(expectedColumns.ElementAt(i)), "Column header text doesn't match");
            }

            Assert.AreEqual(1, tbl.Rows.Count, "Row count does not match.");

            var expectedRows = new List<List<string>>
            {
                new List<string> 
                {
                    Constants.TeamForMtEtGrowthItems, 
                    SharedConstants.AssessmentChecklistRadar,
                    "08/18/2020", 
                    Constants.AssessmentChecklistSingleItem,
                    Constants.AssessmentChecklistMultiItem
                }
            };

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                // compare them
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"Row {i}, Column {j} value doesn't match");
                }
            }
        }
    }
}