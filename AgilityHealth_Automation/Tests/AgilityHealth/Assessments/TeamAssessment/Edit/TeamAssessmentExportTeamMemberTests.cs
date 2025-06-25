using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Edit
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentExportTeamMemberTests : BaseTest
    {
        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Edit_ExportTeamMembers()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            var fileName = $"{_team.Name} - {SharedConstants.TeamHealth2Radar} - Team Members.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Edit");

            taEditPage.ClickTeamMembersExportToExcelButton();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var expectedColumns = new List<string>
            {
                "First Name",
                "Last Name",
                "Email",
                "Roles",
                "Status",
                "Link"
            };

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "column count doesn't match");
            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"{i}th column text doesn't match");
            }

            var teamMembers = new List<MemberResponse>
            {
                new MemberResponse
                {
                    FirstName = Constants.TeamMemberName1.Split(' ')[0],
                    LastName = Constants.TeamMemberName1.Split(' ')[1],
                    Email = Constants.TeamMemberEmail1
                },
                new MemberResponse
                {
                    FirstName = Constants.TeamMemberName2.Split(' ')[0],
                    LastName = Constants.TeamMemberName2.Split(' ')[1],
                    Email = Constants.TeamMemberEmail2
                }
            };

            Assert.AreEqual(teamMembers.Count, tbl.Rows.Count, "The number of rows in the Excel doesn't match.'");

            for (var i = 0; i < teamMembers.Count; i++)
            {
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();
                Assert.That.ListContains(teamMembers.Select(a => a.FirstName).ToList(), actualRow[0], "First Name does not match.");
                Assert.That.ListContains(teamMembers.Select(a => a.LastName).ToList(), actualRow[1], "Last Name does not match.");
                Assert.That.ListContains(teamMembers.Select(a => a.Email).ToList(), actualRow[2], "Email does not match.");
                Assert.AreEqual("", actualRow[3], "Role does not match.");
                Assert.AreEqual("Completed", actualRow[4], "Status does not match.");
                Assert.IsTrue(actualRow[5].StartsWith($"https://{TestEnvironment.EnvironmentName}.agilityinsights.ai/survey/assessment/"),
                    "Link does not match.");
            }

            tbl.Dispose();
        }
    }
}
