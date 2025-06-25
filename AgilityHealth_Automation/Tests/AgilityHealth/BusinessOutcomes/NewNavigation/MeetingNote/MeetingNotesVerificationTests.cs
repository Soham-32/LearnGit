
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.MeetingNotes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.MeetingNote
{

    [TestClass]
    [TestCategory("BusinessOutcomes")]
    [TestCategory("NewNavigation")]
    public class MeetingNotesVerificationTests : BusinessOutcomesBaseTest
    {
        private static readonly MeetingNotes MeetingNotesInfo = BusinessOutcomesFactory.GenerateMeetingNotes();
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyMeetingNotesInformationDisplay()
        {
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var performancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);

            Log.Info("Login and navigating to Meeting Notes.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();
            meetingNotesPage.ClickOnMeetingNotes();

            Log.Info("Navigating to Meeting Notes and verifying the displayed team name.");
            var expectedTeamName = performancePage.GetLeftNavigationHierarchyTeamName()[0];
            var actualTeamName = meetingNotesPage.GetTeamNameFormMeetingNotesTitle();
            Assert.AreEqual(expectedTeamName, actualTeamName, "Meeting Notes Title is incorrect");

            Log.Info("Verifying 'Add Meeting Note' button is not displayed.");
            Assert.IsFalse(meetingNotesPage.IsAddMeetingNoteDisplayed(), "'Add Meeting Note' button should not be displayed");

            Log.Info("Checking 'All Meeting Notes' dropdown values.");
            meetingNotesPage.ClickOnAllMeetingNotesDropdown();
            var expectedDropdownValues = BusinessOutcomesFactory.GetAllMeetingNotesDropdownValues();
            var actualDropdownValues = meetingNotesPage.GetMeetingNotesDropdownOptions();
            Assert.That.ListsAreEqual(expectedDropdownValues, actualDropdownValues, "Meeting Notes Dropdown Values are incorrect");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyMeetingNotesExportToExcel()
        {
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
            var performancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var leftNavigationPage = new LeftNavPage(Driver, Log);

            Log.Info("Login and navigating to Meeting Notes.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();

            var expectedTeamName = performancePage.GetLeftNavigationHierarchyTeamName()[1];
            leftNavigationPage.ClickOnATeam(expectedTeamName);
            performancePage.ClickOnMeetingNotes();

            Log.Info("Adding and verifying the new Meeting Note.");
            meetingNotesPage.ClickOnAddMeetingNoteButton();
            meetingNotesPage.EnterMeetingNoteDetails(MeetingNotesInfo);
            meetingNotesPage.ClickSaveNoteAndSendEmail();

            Assert.IsTrue(meetingNotesPage.IsMeetingNotePresent(MeetingNotesInfo.MeetingTitle), $"Meeting Note Title '{MeetingNotesInfo.MeetingTitle}' is not present");
            Log.Info("Deleted the previously downloaded file and navigated to the meeting notes.");

            var exportFileName = $"MeetingNotes_{User.CompanyName}_{DateTime.Today:M_d_yyyy}.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(exportFileName);

            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();
            meetingNotesPage.ClickOnMeetingNotes();

            Log.Info($"Clicking on Export To Excel button and Check file '{exportFileName}' has been downloaded.");
            meetingNotesPage.ClickExportToExcel();
            Assert.IsTrue(FileUtil.IsFileDownloaded(exportFileName), $"<{exportFileName}> file not downloaded successfully");

            Log.Info("Loading and validating exported Excel file.");
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(exportFileName);
            ExcelUtil.ExcelColumnAutoAdjustExcel(spreadsheet);
            var excelDataTable = ExcelUtil.GetExcelData(spreadsheet);

            Log.Info("Validating column headers and meeting note data.");
            var exportColumns = new List<string> { "Company", "Team", "Author", "Title", "Scheduled Date" };
            var actualColumns = (from DataColumn item in excelDataTable.Columns select item.ColumnName).ToList();
            actualColumns = actualColumns.Where(c => !c.StartsWith("Column")).ToList();
            for (var i = 0; i < exportColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{exportColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(exportColumns[i], actualColumns[i], "Column header text doesn't match");
            }
            var titles = excelDataTable.AsEnumerable().Select(row => row["Title"].ToString()).ToList();
            Assert.IsTrue(titles.Contains(MeetingNotesInfo.MeetingTitle), $"The created card with title '{MeetingNotesInfo.MeetingTitle}' is not displayed in the exported Excel file.");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyMeetingNotesSearchFunctionality()
        {
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
            var performancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var leftNavigationPage = new LeftNavPage(Driver, Log);

            Log.Info("Login and navigating to Meeting Notes.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();

            var expectedTeamName = performancePage.GetLeftNavigationHierarchyTeamName()[1];
            leftNavigationPage.ClickOnATeam(expectedTeamName);
            performancePage.ClickOnMeetingNotes();

            Log.Info("Adding and verifying the new Meeting Note.");
            meetingNotesPage.ClickOnAddMeetingNoteButton();
            meetingNotesPage.EnterMeetingNoteDetails(MeetingNotesInfo);
            meetingNotesPage.ClickSaveNoteAndSendEmail();
            Assert.IsTrue(meetingNotesPage.IsMeetingNotePresent(MeetingNotesInfo.MeetingTitle), $"Meeting Note Title '{MeetingNotesInfo.MeetingTitle}' is not present");

            Log.Step(nameof(VerifyMeetingNotesSearchFunctionality), $"navigating and Searching for Meeting Note: {MeetingNotesInfo.MeetingTitle}");
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();
            meetingNotesPage.ClickOnMeetingNotes();
            meetingNotesPage.SearchMeetingNotes(MeetingNotesInfo.MeetingTitle);
            Assert.IsTrue(meetingNotesPage.IsMeetingNotePresent(MeetingNotesInfo.MeetingTitle), $"Meeting Note '{MeetingNotesInfo.MeetingTitle}' not found.");

        }

    }

}

