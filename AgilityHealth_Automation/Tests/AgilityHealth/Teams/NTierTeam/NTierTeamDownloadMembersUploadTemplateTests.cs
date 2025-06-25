using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("NTier")]
    public class NTierTeamDownloadMembersUploadTemplateTests : NTierBaseTest
    {
        private const string SheetName = "Sheet1";
        private static readonly List<string> ExpectedColumns = new List<string>()
        {
            "FirstName",
            "LastName",
            "Email Address",
            "Role",
            "Participant Group",
            "Tags"
        };

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public void NTierTeam_TeamMembers_Download_UploadTemplate()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var nTierTeamName = "N-TierTeam_" + RandomDataUtil.GetTeamName();

            const string fileName = "AgilityHealth-TeamMembers -Template.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();

            //Add N-Tier Team
            Log.Info("Create an N-Tier team & Add team member via template");
            createNTierPage.CreateNTierTeamWithSubTeam(nTierTeamName);
            Log.Info("Click on Download template icon and verify template columns");
            teamMemberCommon.ClickOnDownloadTemplateIcon();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet, SheetName);

            var listOfColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            var actualColumns = listOfColumns.Where(columnName => !columnName.Contains("Column")).ToList();

            Assert.AreEqual(ExpectedColumns.Count, actualColumns.Count, "column count doesn't match");
            for (var i = 0; i < ExpectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{ExpectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(ExpectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

        }
    }
}

