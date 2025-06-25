using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Radars.MultiTeam
{
    [TestClass]
    [TestCategory("Radars"), TestCategory("MultiTeam")]
    public class MultiTeamRadarCommentsTests : BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static int _teamId;
        private static AddTeamWithMemberRequest _multiTeam;
        private static int _multiTeamId;
        private static TeamAssessmentInfo _teamAssessment;
        private static readonly List<DimensionNote> Notes1 = new List<DimensionNote>
        {
            new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = Guid.NewGuid().ToString() }
        };
        private static readonly List<DimensionNote> Notes2 = new List<DimensionNote>
        {
            new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = Guid.NewGuid().ToString() },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = Guid.NewGuid().ToString() }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            // add normal team
            _team = TeamFactory.GetNormalTeam("TeamComments");
            _team.Members.Add(MemberFactory.GetTeamMember());
            _team.Members.Add(MemberFactory.GetTeamMember());
            var teamResponse = setupApi.CreateTeam(_team).GetAwaiter().GetResult();

            // add multi-team
            _multiTeam = TeamFactory.GetMultiTeam("MultiTeamComments");
            var multiTeamResponse = setupApi.CreateTeam(_multiTeam).GetAwaiter().GetResult();

            // add normal team as subteam to multi-team
            setupApi.AddSubteams(multiTeamResponse.Uid, new List<Guid> { teamResponse.Uid }).GetAwaiter().GetResult();

            // get the teamIds
            var teams = setupApi.GetCompanyHierarchy(Company.Id);
            _teamId = teams.GetTeamByName(_team.Name).TeamId;
            _multiTeamId = teams.GetTeamByName(_multiTeam.Name).TeamId;

            // add team assessment
            var setupUi = new SetUpMethods(testContext, TestEnvironment);

            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = $"TeamComments{Guid.NewGuid()}",
                TeamMembers = new List<string>(new[] { _team.Members[0].FullName(), _team.Members[1].FullName() })
            };

            setupUi.AddTeamAssessment(_teamId, _teamAssessment);
            // complete surveys
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                To = _team.Members[0].Email,
                Labels = new List<string> { "inbox" }
            };
            setupUi.CompleteTeamMemberSurvey(emailSearch, Notes1);
            emailSearch.To = _team.Members[1].Email;
            setupUi.CompleteTeamMemberSurvey(emailSearch, Notes2);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void MultiTeam_Radar_TeamComments_ExportToExcel()
        {
            _multiTeam.CheckForNull($"<{nameof(_multiTeam)}> is null. Aborting test.");

            var login = new LoginPage(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var fileName = $"{_multiTeam.Name} Team Comments Report.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            mtetDashboardPage.NavigateToPage(_multiTeamId);

            mtetDashboardPage.ClickOnRadar(_teamAssessment.AssessmentType);

            radarPage.ClickTeamCommentsExcelButton();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var expectedColumns = new Dictionary<string, int>
            {
                { "Dimension Name", 0 },
                { "SubDimension Name", 1 },
                { "Role", 2 },
                { "Note", 3 },
                { "Filters", 4 },
                { "Company Team Tag Name", 5}
            };

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns.Keys.ElementAt(i)}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns.Keys.ElementAt(i), actualColumns[i], "Column header text doesn't match");
            }

            Notes1.AddRange(Notes2);

            var expectedTags = string.Join("; ", _team.Tags.Select(t => t.Tags.First()).OrderBy(t => t).ToList());

            for (var i = 0; i < Notes1.Count; i++)
            {

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                // get expected values from list
                var note = actualRow[expectedColumns["Note"]];
                var dimensionName = Notes1.Single(n => n.Note == note).Dimension;
                var subdimensionName = Notes1.Single(n => n.Note == note).SubDimension;

                Assert.AreEqual("TeamMember", actualRow[expectedColumns["Role"]],
                    $"Row <{i}>, Column <Role> value doesn't match.");
                Assert.AreEqual(dimensionName, actualRow[expectedColumns["Dimension Name"]],
                    $"Row <{i}>, Column <Dimension Name> value doesn't match.");
                Assert.AreEqual(subdimensionName, actualRow[expectedColumns["SubDimension Name"]],
                    $"Row <{i}>, Column <Subdimension Name> value doesn't match.");
                Assert.AreEqual("All", actualRow[expectedColumns["Filters"]],
                    $"Row <{i}>, Column <Filters> value doesn't match.");
                Assert.AreEqual(expectedTags, actualRow[expectedColumns["Company Team Tag Name"]],
                    $"Row <{i}>, Column <Company Team Tag Name> value doesn't match.");
            }


        }
    }
}
