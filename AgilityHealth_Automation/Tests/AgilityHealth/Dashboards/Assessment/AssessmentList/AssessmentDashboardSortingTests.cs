
using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.AssessmentList
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class AssessmentDashboardSortingTests : BaseTest
    {
        public static BatchAssessment BatchAssessment = new BatchAssessment
        {
            BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
            AssessmentName = "assessmentName_" + RandomDataUtil.GetAssessmentName(),
            AssessmentType = SharedConstants.TeamAssessmentType,
            TeamAssessments = new List<TeamAssessmentInfo>
            {
                new TeamAssessmentInfo
                {
                    TeamName = Constants.TeamForBatchAssessment
                }
            },
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today.AddDays(30)
        };

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [Description("Verify that column sorting working properly in Assessment dashboard")]
        public void AssessmentDashboard_ColumnSorting()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var conmanGridPage = new GridPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var percentageColumnList = new List<string>() { "Responses", "Participants", "Reviewers" };

            Log.Info($"Login as {User.FullName} and navigate to the 'Assessment Dashboard' page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.ClickAssessmentDashBoard();

            var expectedColumnsList = DashboardFactory.GetAssessmentDashboardAssessmentListColumnHeaderList();

            Log.Info($"Select columns from {expectedColumnsList}");
            conmanGridPage.AddSelectedColumns(expectedColumnsList, assessmentDashboardListTabPage.AssessmentDashboardVisibleColumns);

            foreach (var column in expectedColumnsList)
            {
                Log.Info($"Click on the {column} and sort in Ascending order");
                conmanGridPage.SortGridColumn(column);
               
                var actualColumnText = conmanGridPage.GetColumnValues(column);
                if (percentageColumnList.Contains(column))
                {
                    actualColumnText = conmanGridPage.GetColumnPercentageValues(column);
                }

                IList<string> expectedColumnText;

                switch (column)
                {
                    case "Responses":
                        expectedColumnText = csharpHelpers.SortListAscending(actualColumnText.Select(int.Parse).ToList());
                        break;
                    case "Participants":
                    case "Reviewers":
                        expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);
                        break;
                    case "Start Date":
                    case "Facilitation Date":
                    case "End Date":
                        expectedColumnText = csharpHelpers.SortListAscending(conmanGridPage.FormatColumnDates(actualColumnText));
                        break;

                    default:
                        expectedColumnText = csharpHelpers.SortListAscending(actualColumnText);
                        break;
                }

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th {column} text doesn't match during ascending");
                }

                Log.Info($"Click on the {column} and sort in Descending order");
                conmanGridPage.SortGridColumn(column);

                actualColumnText = conmanGridPage.GetColumnValues(column);
                if (percentageColumnList.Contains(column))
                {
                    actualColumnText = conmanGridPage.GetColumnPercentageValues(column);
                }

                switch (column)
                {
                    case "Responses":
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText.Select(int.Parse).ToList());
                        break;
                    case "Participants":
                    case "Reviewers":
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);
                        break;
                    case "Start Date":
                    case "Facilitation Date":
                    case "End Date":
                        expectedColumnText = csharpHelpers.SortListDescending(conmanGridPage.FormatColumnDates(actualColumnText));
                        break;
                    default:
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);
                        break;
                }
                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th {column} text doesn't match during descending");
                }
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [Description("Verify that batches column sorting working properly in Assessment dashboard")]
        public void AssessmentDashboard_Batch_ColumnSorting()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var conmanGridPage = new GridPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var csharpHelpers = new CSharpHelpers();

            Log.Info("Create batch assessment and schedule team batch assessment");
            var setup = new SetUpMethods(TestContext, TestEnvironment);
            setup.ScheduleTeamBatchAssessment(BatchAssessment);

            Log.Info($"Login as {User.FullName} and navigate to the 'Assessment Dashboard' page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

            var expectedColumnList = DashboardFactory.GetAssessmentDashboardBatchesColumnHeaderList();
           
            Log.Info($"Select columns from {expectedColumnList}");
            conmanGridPage.SelectCustomColumns(expectedColumnList);

            foreach (var column in expectedColumnList)
            {
                Log.Info($"Click on the {column} and sort in Ascending order");
                conmanGridPage.SortGridColumn(column);

                var actualColumnText = batchesTabPage.GetColumnValues(column);
                var expectedColumnText = column switch
                {
                    "Assessment Date" => csharpHelpers.SortListAscending(
                        conmanGridPage.FormatColumnDates(actualColumnText)),
                    _ => csharpHelpers.SortListAscending(actualColumnText)
                };

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th {column} text doesn't match during ascending");
                }

                Log.Info($"Click on the {column} and sort in Descending order");
                conmanGridPage.SortGridColumn(column);
                actualColumnText = batchesTabPage.GetColumnValues(column);

                switch (column)
                {
                    case "Assessment Date":
                        expectedColumnText = csharpHelpers.SortListDescending(conmanGridPage.FormatColumnDates(actualColumnText));
                        break;
                    default:
                        expectedColumnText = csharpHelpers.SortListDescending(actualColumnText);
                        break;
                }

                for (var i = 0; i < expectedColumnText.Count; i++)
                {
                    Log.Info($"Row {i} - Expected='{expectedColumnText[i]}' Actual='{actualColumnText[i]}'");
                    Assert.AreEqual(expectedColumnText[i], actualColumnText[i], $"{i}th {column} text doesn't match during descending");
                }
            }
        }
    }
}

