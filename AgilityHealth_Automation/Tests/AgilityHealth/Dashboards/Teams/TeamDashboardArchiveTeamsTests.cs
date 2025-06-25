using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard"), TestCategory("Teams")]
    [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
    public class TeamDashboardArchiveTeamsTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static AddTeamWithMemberRequest _goiTeam;
        private static AddTeamWithMemberRequest _multiTeam;
        private static AddTeamWithMemberRequest _enterpriseTeam;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _team = TeamFactory.GetNormalTeam("ArchiveTeam");
            _goiTeam = TeamFactory.GetGoiTeam("ArchiveGoiTeam");
            _multiTeam = TeamFactory.GetMultiTeam("ArchiveMultiTeam");
            _enterpriseTeam = TeamFactory.GetEnterpriseTeam("ArchiveEnterpriseTeam");

            var setup = new SetupTeardownApi(TestEnvironment);

            setup.CreateTeam(_team).GetAwaiter().GetResult();
            setup.CreateTeam(_goiTeam).GetAwaiter().GetResult();
            setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
            setup.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void TeamDashboard_ArchiveTeams()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.GridTeamView();

            Log.Info("Archive all created teams with different reason");
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.DeleteTeam(_team.Name, RemoveTeamReason.ArchiveProjectCompleted);

            dashBoardPage.SearchTeam(_goiTeam.Name);
            dashBoardPage.DeleteTeam(_goiTeam.Name, RemoveTeamReason.ArchiveTeamDisbanded);

            dashBoardPage.SearchTeam(_multiTeam.Name);
            dashBoardPage.DeleteTeam(_multiTeam.Name, RemoveTeamReason.ArchiveNoBudget);

            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            dashBoardPage.DeleteTeam(_enterpriseTeam.Name, RemoveTeamReason.ArchiveOther);

            Log.Info("Verify that all archived teams should not be displayed at active state");
            dashBoardPage.SearchTeam(_team.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_team.Name),$"{_team.Name} team is still displayed at active state");
            dashBoardPage.SearchTeam(_goiTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is still displayed at active state");
            dashBoardPage.SearchTeam(_multiTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is still displayed at active state");
            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is still displayed at active state");
            dashBoardPage.ResetGridView();

            Log.Info("Go to archive state and verify archived teams should be displayed");
            dashBoardPage.FilterTeamStatus("Archived");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is not displayed");

            Log.Info("Verify 'Team Filter' drop down in archive state");
            dashBoardPage.FilterTeamType("Team");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is not displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is displayed");

            dashBoardPage.FilterTeamType("Multi-Team");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is not displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is displayed");

            dashBoardPage.FilterTeamType("Enterprise Team");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is not displayed");

            dashBoardPage.FilterTeamType("All");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is not displayed");

            Log.Info("Verify 'Search Filter' in archive state");
            dashBoardPage.SearchTeam(_team.Name);
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is not displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is displayed");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is displayed");

            Log.Info("Verify 'Reset Filter' in archive state");
            dashBoardPage.ClickOnArchivedResetFilterButton();
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is not displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is not displayed");

            Log.Info("Export excel file from archive state and verify all the data with exported file");
            const string fileName = "TeamList.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            dashBoardPage.ArchivedClickOnExportToExcelButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} not downloaded successfully");
            
            var expectedColumns = dashBoardPage.GetVisibleColumnNamesFromGrid();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            // some times more columns may show up after the export, so you have to get the rows before the export
            var expectedRows = new List<List<string>>();
            for (var i = 1; i <= dashBoardPage.GetNumberOfGridRows(); i++)
            {
                // get the expected values from the grid
                var row = expectedColumns.Select(column => dashBoardPage.GetCellValue(i, column)).ToList();

                expectedRows.Add(row);
            }

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Columns count doesn't match");
            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                // compare the row from the application to the row from the spreadsheet
                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Columns count doesn't match");
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"{j}th row text doesn't match");
                }
            }

            Log.Info("Verify 'Archive Restore' button and select three teams for restore");
            Assert.IsFalse(dashBoardPage.IsArchiveResetButtonDisplayed(), "'Archive Restore' button is displayed");

            Log.Info("Restore multiple teams with 'Archive Restore' button and verify on active state");
            dashBoardPage.SelectArchiveTeam(_team.Name);
            dashBoardPage.SelectArchiveTeam(_goiTeam.Name);
            dashBoardPage.SelectArchiveTeam(_multiTeam.Name);
            Assert.IsTrue(dashBoardPage.IsArchiveResetButtonDisplayed(), "'Archive Reset' button is not displayed");
            
            Log.Info("Click on 'Cancel' button from 'Restore Team' confirmation popup and verify selected teams should not be restored");
            dashBoardPage.ClickOnArchiveRestoreButton();
            dashBoardPage.RestoreTeamConfirmationPopupClickOnCancelButton();
            Assert.IsFalse(dashBoardPage.IsAddTeamButtonDisplayed(), "'Active' state is displayed");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is not displayed at archive state");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is not displayed at archive state");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is not displayed at archive state");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is not displayed at archive state");

            Log.Info("Click on 'Restore' button from 'Restore Team' confirmation popup and verify selected teams should be restored");
            dashBoardPage.ClickOnArchiveRestoreButton();
            dashBoardPage.RestoreTeamConfirmationPopupClickOnRestoreButton();
            Assert.IsTrue(dashBoardPage.IsAddTeamButtonDisplayed(), "'Archive' state is still displayed");
            dashBoardPage.SearchTeam(_team.Name);
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} is not displayed at active state");
            dashBoardPage.SearchTeam(_goiTeam.Name);
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} is not displayed at active state");
            dashBoardPage.SearchTeam(_multiTeam.Name);
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} is not displayed at active state");
            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} is displayed at active state");

            Log.Info("Go back to 'Archive' state and verify restored teams should not be displayed");
            dashBoardPage.FilterTeamStatus("Archived");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_team.Name), $"{_team.Name} team is still displayed at archive state");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_goiTeam.Name), $"{_goiTeam.Name} team is still displayed at archive state");
            Assert.IsFalse(dashBoardPage.DoesTeamDisplay(_multiTeam.Name), $"{_multiTeam.Name} team is still displayed at archive state");
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} team is not displayed at archive state");

            Log.Info("Restore remain team via restore icon and verify at active state");
            dashBoardPage.RestoreTeam(_enterpriseTeam.Name);
            Assert.IsTrue(dashBoardPage.IsAddTeamButtonDisplayed(), "'Archive' state is still displayed");
            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            Assert.IsTrue(dashBoardPage.DoesTeamDisplay(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} is not displayed at active state");

        }
    }
}
