using System.Collections.Generic;
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
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class AddMeetingNotesTests : BusinessOutcomesBaseTest
    {
        private static readonly MeetingNotes MeetingNotesInfo = BusinessOutcomesFactory.GenerateMeetingNotes();

        private static readonly MeetingNotes EditMeetingNotesInfo = BusinessOutcomesFactory.GenerateUpdatedMeetingNotes();

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void VerifyAddMeetingNoteFormDetails()
        {
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var performancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
            var leftNavigationPage = new LeftNavPage(Driver, Log);

            Log.Info("Login and navigating to Meeting Notes.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();

            var expectedTeamName = performancePage.GetLeftNavigationHierarchyTeamName()[1];
            leftNavigationPage.ClickOnATeam(expectedTeamName);

            Log.Info("Verifying the 'Add Meeting Note' button is displayed.");
            performancePage.ClickOnMeetingNotes();
            Assert.IsTrue(meetingNotesPage.IsAddMeetingNoteDisplayed(), "'Add Meeting Note' button is not displayed");

            Log.Info("Opening 'Add Meeting Note' form and verifying headers.");
            meetingNotesPage.ClickOnAddMeetingNoteButton();
            var expectedHeaderTitles = new List<string> { "Public Note", "Send Email" };
            var actualHeaderTitles = meetingNotesPage.GetAddMeetingNoteHeaderTitles();
            Assert.That.ListsAreEqual(expectedHeaderTitles, actualHeaderTitles, "Header titles do not match");

            Log.Info("Verifying the Meeting Type dropdown values.");
            meetingNotesPage.ClickOnMeetingTypeDropdown();
            var expectedMeetingTypeDropdownValues = BusinessOutcomesFactory.GetMeetingNoteTypeDropdownValues();
            var actualMeetingTypeDropdownValues = meetingNotesPage.GetMeetingTypeDropdownOptions();
            Assert.That.ListsAreEqual(expectedMeetingTypeDropdownValues, actualMeetingTypeDropdownValues,
                "Meeting Type dropdown values do not match");

            Log.Info("Verifying UI elements: Meeting Title field and 'Add Attendee' button.");
            Assert.IsTrue(meetingNotesPage.IsMeetingTitleDisplayed(), "Meeting Title field is not displayed");
            Assert.IsTrue(meetingNotesPage.IsAddAttendeeButtonEnable(), "'Add Attendee' button is not enabled");

            Log.Info("Validating 'Meeting Notes' sub-tabs.");
            var expectedMeetingNotesSubTabs = new List<string> { "Decisions", "Action Item(s)", "Document(s)" };
            var actualMeetingNotesSubTabs = meetingNotesPage.GetMeetingNoteSubTabs();
            Assert.That.ListsAreEqual(expectedMeetingNotesSubTabs, actualMeetingNotesSubTabs, "'Meeting Notes' sub-tabs do not match");

            Log.Info("Clicking on 'Action Item(s)'and 'Document(S)' tab and verifying it is loaded successfully.");
            meetingNotesPage.ClickMeetingNoteSubTabHeaders("Action Item(s)");
            Assert.IsTrue(meetingNotesPage.IsAddActionItemButtonDisplayed(), "'Add Action Item' button is not displayed");

            Log.Info("Clicking on 'Add a Document' dropdown and verifying the options.");
            meetingNotesPage.ClickMeetingNoteSubTabHeaders("Document(s)");
            meetingNotesPage.ClickOnAddADocumentDropdown();
            var expectedDocumentDropdownValues = new List<string> { "Upload a Document", "Add a Link" };
            var actualDocumentDropdownValues = meetingNotesPage.GetDocumentDropdownValues();
            Assert.That.ListsAreEqual(expectedDocumentDropdownValues, actualDocumentDropdownValues, "Document dropdown values do not match");
        }


        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id : 53856
        [TestCategory("CompanyAdmin")]
        public void VerifyUserCanAddMeetingNote()
        {
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var performancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
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

            Log.Info("Validating email receipt for the Meeting Note.");
            Assert.IsTrue(GmailUtil.DoesMeetingNotes("New", SharedConstants.TeamMember1.Email, MeetingNotesInfo.MeetingTitle), "Meeting Note email was not received");

            Log.Info("Deleting the MeetingNote");
            ArchiveMeetingNotes(MeetingNotesInfo.MeetingTitle);
        }
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53856
        [TestCategory("CompanyAdmin")]
        public void VerifyUserCanEditMeetingNote()
        {
            var dashboardPage = new BusinessOutcomesDashboardPage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var performancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
            var leftNavigationPage = new LeftNavPage(Driver, Log);

            Log.Info("Login and navigating to Meeting Notes.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            dashboardPage.NavigateToPage(Company.Id);
            dashboardPage.WaitTillBoPageLoadedCompletely();
            var expectedTeamName = performancePage.GetLeftNavigationHierarchyTeamName()[1];
            leftNavigationPage.ClickOnATeam(expectedTeamName);
            performancePage.ClickOnMeetingNotes();

            Log.Info("Adding a new Meeting Note.");
            meetingNotesPage.ClickOnAddMeetingNoteButton();
            meetingNotesPage.EnterMeetingNoteDetails(MeetingNotesInfo);
            meetingNotesPage.ClickSaveNoteAndSendEmail();

            Log.Info($"Editing the Meeting Note: {MeetingNotesInfo.MeetingTitle} and Verifying updated meeting note");
            meetingNotesPage.ClickEditMeetingButton(MeetingNotesInfo.MeetingTitle);
            meetingNotesPage.EnterMeetingNoteDetails(EditMeetingNotesInfo);
            meetingNotesPage.ClickUpdateNoteAndSendEmail();
            Assert.IsTrue(meetingNotesPage.IsMeetingNotePresent(EditMeetingNotesInfo.MeetingTitle), $"Meeting Note Title '{EditMeetingNotesInfo.MeetingTitle}' is not present");

            Log.Info("Validating email receipt for the updated Meeting Note.");
            Assert.IsTrue(GmailUtil.DoesMeetingNotes("Updated", SharedConstants.TeamMember2.Email, EditMeetingNotesInfo.MeetingTitle), "Updated Meeting Note email was not received");

            Log.Info("Archiving the updated Meeting Note.");
            ArchiveMeetingNotes(EditMeetingNotesInfo.MeetingTitle);
        }

        public void ArchiveMeetingNotes(string meetingNoteTitle)
        {
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            Log.Info($"Archiving the Meeting Note: {meetingNoteTitle} and Verify that meeting not is not displayed");
            meetingNotesPage.ClickArchiveMeetingButton(meetingNoteTitle);
            Assert.IsFalse(meetingNotesPage.IsMeetingNotePresent(meetingNoteTitle), $"Archived Meeting Note Title '{meetingNoteTitle}' is still present");
        }
    }
}