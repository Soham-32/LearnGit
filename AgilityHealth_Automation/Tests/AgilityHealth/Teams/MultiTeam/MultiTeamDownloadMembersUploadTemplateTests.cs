using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AgilityHealth_Automation.Tests.AgilityHealth.Teams.MultiTeam.MultiTeamDownloadMembersUploadTemplateTests.ColumnSelect;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team")]
    public class MultiTeamDownloadMembersUploadTemplateTests : BaseTest
    {
        private static SetupTeardownApi _setup;
        private static bool _classInitFailed;
        private static int _multiTeamId;
        private const string SheetName = "Sheet1";
        public static class ColumnSelect
        {
            public enum ColumnType
            {
                TeamMembers,
                StakeHolders
            }
            public static List<string> GetExpectedColumns(ColumnType columnType)
            {
                switch (columnType)
                {
                    case ColumnType.TeamMembers:
                        return new List<string>
                        {
                            "FirstName",
                            "LastName",
                            "Email Address",
                            "Role",
                            "Participant Group",
                            "Tags"
                        };
                    case ColumnType.StakeHolders:
                        return new List<string>
                        {
                            "FirstName",
                            "LastName",
                            "Email Address",
                            "Role"
                        };
                    default:
                        throw new ArgumentException("Invalid column type");
                }
            }
        }

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _setup = new SetupTeardownApi(TestEnvironment);
                _multiTeamId = _setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam).TeamId;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_TeamMembers_Download_UploadTemplate()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);

            const string fileName = "AgilityHealth-TeamMembers -Template.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit team members page and click on Download template icon");
            editTeamBasePage.NavigateToPage(_multiTeamId.ToString());
            editTeamBasePage.GoToGrowthTeamTab();

            Log.Info("Click on Download template icon and verify template columns");
            teamMemberCommon.ClickOnDownloadTemplateIcon();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet, SheetName);

            var listOfColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            var actualColumns = listOfColumns.Where(columnName => !columnName.Contains("Column")).ToList();

            Assert.AreEqual(ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.TeamMembers).Count, actualColumns.Count, "column count doesn't match");
            for (var i = 0; i < ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.TeamMembers).Count; i++)
            {
                Log.Info($"Column {i} - Expected='{ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.TeamMembers)[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.TeamMembers)[i], actualColumns[i], $"{i}th column text doesn't match");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_Stakeholders_Download_UploadTemplate()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);

            const string fileName = "AgilityHealth-Stakeholders-Template.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit stakeholders tab and click on Download template icon");
            editTeamBasePage.NavigateToPage(_multiTeamId.ToString());
            editTeamBasePage.GoToStakeHoldersTab();

            Log.Info("Click on Download template icon and verify template columns");
            teamMemberCommon.ClickOnDownloadTemplateIcon();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet, SheetName);

            var listOfColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            var actualColumns = listOfColumns.Where(columnName => !columnName.Contains("Column")).ToList();

            Assert.AreEqual(ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.StakeHolders).Count, actualColumns.Count, "column count doesn't match");
            for (var i = 0; i < ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.StakeHolders).Count; i++)
            {
                Log.Info($"Column {i} - Expected='{ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.StakeHolders)[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(ColumnSelect.GetExpectedColumns(ColumnSelect.ColumnType.StakeHolders)[i], actualColumns[i], $"{i}th column text doesn't match");
            }
        }
    }
}


