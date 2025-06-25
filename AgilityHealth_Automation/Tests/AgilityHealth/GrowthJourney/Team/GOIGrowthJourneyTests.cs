using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthJourney.Team
{
    [TestClass]
    [TestCategory("GrowthJourney")]
    public class GoiGrowthJourneyExportTests : BaseTest
    {
        private static CreateIndividualAssessmentRequest _assessment1;
        private static CreateIndividualAssessmentRequest _assessment2;
        private static AddTeamWithMemberRequest _team;
        private static User _member;
        private const int SurveyResponse1 = 7;
        private const int SurveyResponse2 = 5;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _member = TestEnvironment.UserConfig.GetUserByDescription("member");
            var adminUser = User.IsMember() ? TestEnvironment.UserConfig.GetUserByDescription("user 3") : User;
            // add new GOI team with members
            _team = TeamFactory.GetGoiTeam("IndividualGI");
            _team.Members.Add(
                new AddMemberRequest
                {
                    FirstName = _member.FirstName,
                    LastName = _member.LastName,
                    Email = _member.Username
                }
            );

            var setup = new SetupTeardownApi(TestEnvironment);
            var teamResponse = setup.CreateTeam(_team, adminUser).GetAwaiter().GetResult();

            // add new Individual assessment
            _assessment1 = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                Company.Id, User.CompanyName, teamResponse.Uid, $"IAGrowthJourney{Guid.NewGuid()}");

            foreach (var member in teamResponse.Members)
            {
                _assessment1.Members.Add(new IndividualAssessmentMemberRequest
                {
                    Uid = member.Uid,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.Email
                });
            }

            // add new Individual assessment
            _assessment2 = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                Company.Id, User.CompanyName, teamResponse.Uid, $"IAGrowthJourney{Guid.NewGuid()}");

            foreach (var member in teamResponse.Members)
            {
                _assessment2.Members.Add(new IndividualAssessmentMemberRequest
                {
                    Uid = member.Uid,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.Email
                });
            }

            setup.CreateIndividualAssessment(_assessment1, SharedConstants.IndividualAssessmentType, adminUser)
                .GetAwaiter().GetResult();

            // complete survey
            var setupUi = new SetUpMethods(testContext, TestEnvironment);
            foreach (var member in _assessment1.Members)
            {
                setupUi.CompleteIndividualSurvey(member.Email, _assessment1.PointOfContactEmail, SurveyResponse2);
            }

            setup.CreateIndividualAssessment(_assessment2, SharedConstants.IndividualAssessmentType, adminUser)
                .GetAwaiter().GetResult();

            // complete survey
            foreach (var member in _assessment2.Members)
            {
                setupUi.CompleteIndividualSurvey(member.Email, _assessment2.PointOfContactEmail, SurveyResponse1);
            }

        }

        [TestMethod]
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GOI_GrowthJourney_ExportToPDF()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var iaDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);

            var filename = $"{_team.Name} {User.CompanyName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(filename);

            Driver.NavigateToPage(ApplicationUrl);

            var user = User.IsMember() ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);

            iaDashboardPage.ClickGrowthJourneyTab();

            growthJourneyPage.ClickExportToPdf();
            growthJourneyPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(filename), $"{filename} isn't downloaded successfully");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GOI_GrowthJourney_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var iaDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);

            const string fileName = "Analysis.xlsx";

            Driver.NavigateToPage(ApplicationUrl);

            var user = User.IsMember() ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            Log.Info($"Go to Growth Journey page of team - {_team.Name}");
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            iaDashboardPage.ClickGrowthJourneyTab();

            Log.Info("Download the Excel file of 'Compare Radar Analysis' table and fetch the data");
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            growthJourneyPage.ClickExportToExcel();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Log.Info("Verify that table columns are same in the 'Compare Radar Analysis' table & the downloaded excel file");
            var exportColumns = new List<string>
            {
                "Column1", "Column2", "Competencies", _assessment1.AssessmentName, _assessment2.AssessmentName, "Points Change", "Percent Change"
            };

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            if (exportColumns[3] != actualColumns[3])
            {
                (exportColumns[3], exportColumns[4]) = (exportColumns[4], exportColumns[3]);

            }

            for (var i = 0; i < exportColumns.Count; i++)
            {
                Assert.AreEqual(exportColumns[i], actualColumns[i], "Column header text doesn't match");
            }

            Log.Info("Verify that row values are same in the 'Compare Radar Analysis' table & the downloaded excel file ");
            var rowValues = (growthJourneyPage.GetRowValues()).ToList();
            var expectedRowValues = (rowValues.Select(a => a.Replace(" %", ""))).ToList();
            var actualRowValuesList = (from DataRow item in tbl.Rows select item.ItemArray).ToList();

            var actualRowValues = actualRowValuesList.Select(rowValue => rowValue.Cast<string>().ListToString().Replace(",", "\r\n").Replace("\r\n0", "").Replace("%", "").Trim()).ToList();
            Assert.That.ListsAreEqual(expectedRowValues, actualRowValues, "Excel data and 'Compare Radar Analysis' table data doesn't match");
        }


        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GoiGrowthJourneyIndividualRadar()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var assessmentName1 = _assessment1.AssessmentName;
            var assessmentName2 = _assessment2.AssessmentName;
            var assessmentMember = _assessment1.Members.First();

            Driver.NavigateToPage(ApplicationUrl);
            var user = User.IsMember() ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            Log.Info($"Go to Growth Journey page for the radar - {assessmentName1} - {assessmentMember.FirstName} {assessmentMember.LastName}");
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.GoToTeamAssessmentDashboard(1);
            iAssessmentDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);
            iAssessmentDashboardPage.ClickOnRadar($"{assessmentName1} - {assessmentMember.FirstName} {assessmentMember.LastName}");
            radarPage.ClickGrowthJourneyTab();

            Log.Info("Verify filter functionality of radar via assessments.");
            growthJourneyPage.SwitchToCompareRadarView();
            growthJourneyPage.OpenFilterSidebar();
            var color1 = CSharpHelpers.ConvertRgbToHex(growthJourneyPage.GetFilterAssessmentColor(assessmentName1)).ToLower();
            var color2 = CSharpHelpers.ConvertRgbToHex(growthJourneyPage.GetFilterAssessmentColor(assessmentName2));

            Log.Info($"Uncheck assessment - {assessmentName1} and verify line count,dot count & dot value for assessment - {assessmentName2}");
            growthJourneyPage.SelectFilterItemCheckboxByNameFromFilter(assessmentName1, false);
            Assert.IsFalse(growthJourneyPage.GetLineCountByColor(color1) > 0, $"Radar lines are displayed for color <{color1}>");
            foreach (var comp in Constants.AtIndividualCompentenciesLabels)
            {
                Assert.AreEqual(0, growthJourneyPage.GetDotCountByColor(color1, comp), $"Radar dot is displayed for '{comp}' <{color1}> ");
                Assert.AreEqual(2, growthJourneyPage.GetDotCountByColor(color2, comp), $"Radar dot is not displayed for '{comp}' <{color2}>");
                Assert.AreEqual(SurveyResponse1, int.Parse(growthJourneyPage.GetRadarDotValue(color2, comp).First()), $"Competency: <{comp}> value doesn't match");
            }

            growthJourneyPage.SelectFilterItemCheckboxByNameFromFilter(assessmentName1);

            Log.Info($"Uncheck assessment - {assessmentName2} and verify line count,dot count & dot value for assessment - {assessmentName1}");
            growthJourneyPage.SelectFilterItemCheckboxByNameFromFilter(assessmentName2, false);
            Assert.IsFalse(growthJourneyPage.GetLineCountByColor(color2) > 0, $"Radar lines are displayed for color <{color2}>");
            foreach (var comp in Constants.AtIndividualCompentenciesLabels)
            {
                Assert.AreEqual(0, growthJourneyPage.GetDotCountByColor(color2, comp), $"Radar dot is displayed for '{comp}' <{color2}>");
                Assert.AreEqual(2, growthJourneyPage.GetDotCountByColor(color1, comp), $"Radar dot is not displayed for '{comp}' <{color1}>");
                Assert.AreEqual(SurveyResponse2, int.Parse(growthJourneyPage.GetRadarDotValue(color1, comp).First()), $"Competency: <{comp}> value doesn't match");
            }
            growthJourneyPage.SelectFilterItemCheckboxByNameFromFilter(assessmentName2);

            Log.Info("Verify 'Clear All' Functionality");
            growthJourneyPage.ClickOnClearAllLink();
            Assert.IsFalse(growthJourneyPage.GetLineCountByColor(color1) > 0, $"Radar lines are displayed for color <{color1}>");
            Assert.IsFalse(growthJourneyPage.GetLineCountByColor(color2) > 0, $"Radar lines are displayed for color <{color2}>");
            foreach (var comp in Constants.AtIndividualCompentenciesLabels)
            {
                Assert.AreEqual(0, growthJourneyPage.GetDotCountByColor(color1, comp), $"Radar dot is displayed for '{comp}' <{color1}>");
                Assert.AreEqual(0, growthJourneyPage.GetDotCountByColor(color2, comp), $"Radar dot is displayed for '{comp}' <{color2}>");
            }

            Log.Info("Verify 'Select All' Functionality");
            growthJourneyPage.ClickOnSelectAllLink();
            Assert.IsTrue(growthJourneyPage.GetLineCountByColor(color1) > 0, $"Could not find any radar lines for color <{color1}>");
            Assert.IsTrue(growthJourneyPage.GetLineCountByColor(color2) > 0, $"Could not find any radar lines for color <{color2}>");
            foreach (var comp in Constants.AtIndividualCompentenciesLabels)
            {
                Assert.AreEqual(SurveyResponse2, int.Parse(growthJourneyPage.GetRadarDotValue(color1, comp).First()), $"Competency: <{comp}> value doesn't match");
                Assert.AreEqual(SurveyResponse1, int.Parse(growthJourneyPage.GetRadarDotValue(color2, comp).First()), $"Competency: <{comp}> value doesn't match");
            }
        }

    }
}
