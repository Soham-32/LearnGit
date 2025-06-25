using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.AssessmentList
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class AssessmentDashboardTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamAssessmentInfo _teamAssessment;
        private static IndividualAssessmentResponse _individualAssessment;
        private static TeamResponse _iaTeam;
        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                // add new GOI team with members
                var teamRequest = TeamFactory.GetGoiTeam("AssessmentDashboard");
                teamRequest.Members.Add(MemberFactory.GetTeamMember());

                var setup = new SetupTeardownApi(TestEnvironment);
                _iaTeam = setup.CreateTeam(teamRequest).GetAwaiter().GetResult();

                // add new Individual assessment
                var assessmentRequest = new CreateIndividualAssessmentRequest
                {
                    AssessmentName = $"AssessmentDashboardTests{Guid.NewGuid()}",
                    PointOfContact = "Test Point of contact",
                    PointOfContactEmail = "pointofcontract@sharklasers.com",
                    CompanyId = Company.Id,
                    TeamUid = _iaTeam.Uid,
                    CompanyName = User.CompanyName,
                    AllowInvite = false,
                    AllowResultView = false,
                    Published = true,
                    ReviewEachOther = false,
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(7),
                    Members = new List<IndividualAssessmentMemberRequest>()
                };

                foreach (var member in _iaTeam.Members)
                {
                    assessmentRequest.Members.Add(new IndividualAssessmentMemberRequest
                    {
                        Uid = member.Uid,
                        FirstName = member.FirstName,
                        LastName = member.LastName,
                        Email = member.Email
                    });
                }

                _individualAssessment = setup.CreateIndividualAssessment(
                    assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    TeamMembers = new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 },
                    StakeHolders = new List<string> { Constants.StakeholderName2, Constants.StakeholderName3 }
                };

                setupUi.AddTeamAssessment(_team.TeamId, _teamAssessment);

                // Complete a survey as a member/stakeholder
                var emailSearch = new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                };
                setupUi.CompleteTeamMemberSurvey(emailSearch, ansValue: 5);
                setupUi.CompleteStakeholderSurvey(_team.Name, SharedConstants.Stakeholder2.Email, _teamAssessment.AssessmentName, ansValue: 6);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void AssessmentDashboard_AssessmentList_Verify_Grid_Export_NewColumns()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            var newColumns = new List<string>
            {
                "Team Members Invited",
                "Team Members Completed",
                "Stakeholders Invited",
                "Stakeholders Completed",
                "Share Results"
            };
            assessmentDashboardListTabPage.AddColumns(newColumns);

            assessmentDashboardListTabPage.FilterBySearchTerm(SharedConstants.AssessmentHideUnHideCommentsRadar);
            assessmentDashboardListTabPage.FilterBySearchTerm(_teamAssessment.AssessmentName); 

            var actualValueOfTeamInvite = assessmentDashboardListTabPage.GetColumnValues("Team Members Invited").First();
            var expectedValueOfTeamInvite = _teamAssessment.TeamMembers.ToList().Count.ToString();
            Assert.AreEqual(expectedValueOfTeamInvite, actualValueOfTeamInvite,
                    "The value of the 'Team Members Invited' does not match.");

            var actualValueOfTeamComplete = assessmentDashboardListTabPage.GetColumnValues("Team Members Completed").First();
            Assert.AreEqual("1", actualValueOfTeamComplete,
                "The value of the 'Team Members Completed' does not match.");

            var actualValueOfStakeholderInvite = assessmentDashboardListTabPage.GetColumnValues("Stakeholders Invited").First();
            var expectedValueOfStakeholderInvite = _teamAssessment.TeamMembers.ToList().Count.ToString();
            Assert.AreEqual(actualValueOfStakeholderInvite, expectedValueOfStakeholderInvite,
                "The value of the 'Stakeholders Invited' does not match.");

            var actualValueOfStakeholderComplete = assessmentDashboardListTabPage.GetColumnValues("Team Members Completed").First();
            Assert.AreEqual("1", actualValueOfStakeholderComplete,
                "The value of the 'Team Members Completed' does not match.");

            var actualValueOfNotSharedResults = assessmentDashboardListTabPage.GetColumnValues("Share Results").First();
            Assert.AreEqual("Not Shared", actualValueOfNotSharedResults,
                "The value of the 'Share Results' column does not match.");

            assessmentDashboardListTabPage.ClickExcelButton();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded("Assessment List.xlsx");

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var expectedColumns = assessmentDashboardListTabPage.GetColumnHeaders();

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Column count doesn't match");
            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"Column header at index {i} text doesn't match");
            }

            var rowCount = assessmentDashboardListTabPage.GetNumberOfGridRows();
            Assert.AreEqual(rowCount, tbl.Rows.Count, "Rows count doesn't match");

            var expectedRows = new List<List<string>>
            {
                assessmentDashboardListTabPage.GetRowValues(rowCount, expectedColumns)
            };

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                // compare them
                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Row count doesn't match");
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"Row {i}, Column {j} value doesn't match");
                }
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48906
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void AssessmentDashboard_AssessmentList_Filtering()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            var expectedColumns = new List<string>
            {
                "Assessment Name",
                "Assessment Type"
            };
            assessmentDashboardListTabPage.AddColumns(expectedColumns);

            assessmentDashboardListTabPage.FilterBySearchTerm(""); // clear any values that are saved
            assessmentDashboardListTabPage.FilterByAssessmentType(_teamAssessment.AssessmentType);

            var actualValues = assessmentDashboardListTabPage.GetColumnValues("Assessment Type");

            for (var i = 0; i < actualValues.Count; i++)
            {
                Assert.AreEqual(_teamAssessment.AssessmentType, actualValues[i], 
                    $"The value of the 'Assessment Type' column at index '{i}' does not match.");
            }

            const string expectedType = SharedConstants.IndividualAssessmentType;
            assessmentDashboardListTabPage.FilterByAssessmentType(expectedType);

            actualValues = assessmentDashboardListTabPage.GetColumnValues("Assessment Type");
            for (var i = 0; i < actualValues.Count; i++)
            {
                Assert.AreEqual(expectedType, actualValues[i], 
                    $"The value of the 'Assessment Type' column at index '{i}' does not match.");
            }

            assessmentDashboardListTabPage.FilterByAssessmentType("All");

            assessmentDashboardListTabPage.FilterBySearchTerm(_teamAssessment.AssessmentName);

            actualValues = assessmentDashboardListTabPage.GetColumnValues("Assessment Name");

            for (var i = 0; i < actualValues.Count; i++)
            {
                Assert.AreEqual(_teamAssessment.AssessmentName, actualValues[i], 
                    $"The value of the 'Assessment Name' column at index '{i}' does not match.");
            }

            assessmentDashboardListTabPage.FilterBySearchTerm(_individualAssessment.AssessmentName);

            actualValues = assessmentDashboardListTabPage.GetColumnValues("Assessment Name");

            // verify all values contain assessment name
            foreach (var assessmentName in actualValues)
            {
                Assert.IsTrue(assessmentName.StartsWith(_individualAssessment.AssessmentName), 
                    $"<{assessmentName}> should start with <{_individualAssessment.AssessmentName}>");
            }
            
            // verify each team member has an entry
            foreach (var participant in _individualAssessment.Participants)
            {
                Assert.That.ListContains(actualValues, $"{_individualAssessment.AssessmentName} - {participant.FullName()}");
            }

        }

        [TestMethod]
        [DoNotParallelize]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void AssessmentDashboard_AssessmentList_ExportToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            FileUtil.DeleteFilesInDownloadFolder("Assessment List.xlsx");

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.ResetAllFilters();

            assessmentDashboardListTabPage.ClickExcelButton();

            var spreadsheet = FileUtil.WaitUntilFileDownloaded("Assessment List.xlsx");

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var expectedColumns = assessmentDashboardListTabPage.GetColumnHeaders();

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();
            Assert.AreEqual(expectedColumns.Count, actualColumns.Count, "Column count doesn't match");
            for (var i = 0; i < expectedColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{expectedColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(expectedColumns[i], actualColumns[i], $"Column header at index {i} text doesn't match");
            }

            var totalRowCount = assessmentDashboardListTabPage.GetNumberOfGridRows();
            //Only verifying first 50 items.
            if (totalRowCount < 50)
            {
                Assert.AreEqual(totalRowCount, tbl.Rows.Count, "Total rows doesn't match");
            }
            else
            {
                totalRowCount = 50;
            }

            var expectedRows = new List<List<string>>();

            for (var i = 1; i <= totalRowCount; i++)
            {
                expectedRows.Add(assessmentDashboardListTabPage.GetRowValues(i, expectedColumns));
            }

            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];

                // get the row from the spreadsheet
                var actualRow = tbl.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                // compare them
                Assert.AreEqual(expectedRow.Count, actualRow.Count, "Row count doesn't match");
                for (var j = 0; j < expectedRow.Count; j++)
                {
                    Log.Info($"Row {i}, Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                    Assert.AreEqual(expectedRow[j], actualRow[j], $"Row {i}, Column {j} value doesn't match");
                }
            }

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void AssessmentDashboard_AssessmentList_EditAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var tAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashboardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnEditAssessment(_teamAssessment.AssessmentName);

            taEditPage.ClickEditDetailButton();

            var editedTeamAssessment = new TeamAssessmentInfo
            {
                AssessmentName = "edited" + _teamAssessment.AssessmentName
            };

            taEditPage.FillDataForAssessmentProfile(editedTeamAssessment);

            taEditPage.EditPopup_ClickUpdateButton();

            taEditPage.ClickReturnToDashboardButton();

            Assert.IsTrue(tAssessmentDashboard.DoesAssessmentExist(editedTeamAssessment.AssessmentName));

            _teamAssessment.AssessmentName = editedTeamAssessment.AssessmentName;
        }

    }
}
