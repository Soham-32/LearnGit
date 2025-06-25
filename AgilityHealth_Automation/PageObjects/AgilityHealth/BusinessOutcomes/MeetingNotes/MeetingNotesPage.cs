using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.MeetingNotes
{
    public class MeetingNotesPage : BusinessOutcomeBasePage
    {
        public MeetingNotesPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Locators
        private readonly By AddMeetingNotesButton = By.XPath("//span[contains(normalize-space(),'Add Meeting Note')]/parent::button");
        private readonly By MeetingNotesTitle = By.XPath("//div[@title='Meeting Notes']//following-sibling::div//span[contains(text(),'Meeting Notes')]");
        private readonly By AllMeetingNotesDropdown = By.XPath("//div[@aria-label='Show All Meeting Notes']/span[contains(normalize-space(),'All Meeting Notes')]");
        private readonly By AllMeetingNotesDropdownOptions = By.XPath("//div[@aria-label='Show All Meeting Notes']//following-sibling::ul/li");
        private readonly By AddMeetingNoteHeaderTitles = By.XPath("//div[@title='Meeting Notes']//following-sibling::div//label/span[2]");
        private readonly By MeetingTypeDropdown = By.XPath("//div[@aria-label='Meeting Note Type']/parent::div");
        private readonly By MeetingTypeDropdownOption = By.XPath("//ul[@role='listbox']/li");
        private readonly By MeetingTitle = By.XPath("//section//label[text()='Meeting Title']");
        private readonly By AddAttendeePlusButton = By.XPath("//section//label[text()='Attendees']/parent::div/div//*[local-name()='svg' and @data-icon='plus-circle']");
        private static By AddMeetingNotesSubTabHeaders(string tabName) => By.XPath($"//*[text()='{tabName}']");
        private readonly By AddMeetingNotesAllSubTabHeaders = By.XPath("//div[@title='Meeting Notes']//following-sibling::div//section[2]/div/div");
        private readonly By AddActionItemPlusButton = By.XPath("//div[text()='Add Action Item']");
        private readonly By AddADocumentDropdown = By.XPath("//button[contains(normalize-space(),'Add a Document')]/span[1]");
        private readonly By AddADocumentDropdownValues = By.XPath("//div[@data-testid='sentinelStart']/following-sibling::div//ul[@role='menu']/li");
        private readonly By MeetingNoteTypeDropdown = By.XPath("//input//preceding-sibling::div[@aria-label='Meeting Note Type']");
        private static By MeetingTypeOption(string meetingType) => By.XPath($"//ul[@role='listbox']//li[contains(text(),'{meetingType}')]");
        private readonly By AttendeeSearchTextbox = By.XPath("//input[@placeholder='Search' and @type='text']");
        private readonly By MeetingTitleInput = By.Id("meetingTitle");
        private static By AttendeeOption(string attendeeName) => By.XPath($"//p[text()='{attendeeName}']//preceding-sibling::span");
        private static By MeetingDecisionTextbox => By.XPath("//div[@dir='ltr' and contains(@class,'ProseMirror')]");
        private static readonly By MeetingIframe = By.XPath("//iframe[@class='k-iframe']");
        private static readonly By SaveNoteAndSendEmailButton = By.XPath("//span[text()='Save Note And Send Email']");
        private static readonly By UpdateNoteAndSendEmailButton = By.XPath("//span[text()='Update Note And Send Email']");
        private readonly By AddAttendeeIcon = By.XPath("//label[@for='meetingTitle']/parent::div//div//*[local-name()='svg']");
        private static By RemoveAttendeeIcon => By.XPath("//div[@id='user-autocomplete']//div//*[local-name()='svg']");
        private readonly By ExportToExcelButton = By.XPath("//span[text()='Export to Excel']");
        private static By EditMeetingButton(string meetingTitle) => By.XPath($"//span[text()='{meetingTitle}']/parent::td//following-sibling::td//div[@role='group']//button[text()='Edit']");
        private static By MeetingActionsButton(string meetingTitle) => By.XPath($"//span[text()='{meetingTitle}']/parent::td//following-sibling::td//div[@role='group']//button[@id='project-save-actions']");
        private static By ArchiveMeetingOption => By.XPath("//li[text()='Archive']");
        private static By SearchMeetingTextbox => By.XPath("//input[@placeholder='Search' and @id]");
        private static By MeetingNoteTitleLabel(string title) => By.XPath($"//*[contains(text(),'{title}')]");
        #endregion

        #region Methods
        public string GetTeamNameFormMeetingNotesTitle()
        {
            Log.Step(nameof(MeetingNotesPage), "Get the Team Name on Meeting Note Title");
            return Wait.UntilElementClickable(MeetingNotesTitle).Text.Replace("Meeting Notes - ", "").Trim();
        }

        public bool IsAddMeetingNoteDisplayed()
        {
            Wait.HardWait(1000); // Wait until 'Add Meeting Notes' button is displayed
            return Driver.IsElementDisplayed(AddMeetingNotesButton);
        }

        public void ClickOnAddMeetingNoteButton()
        {
            Log.Step(nameof(MeetingNotesPage), "Click on Add Meeting Note");
            Wait.UntilElementVisible(AddMeetingNotesButton);
            Wait.UntilElementClickable(AddMeetingNotesButton).Click();
            Wait.HardWait(2000);
            for (var i = 0; i < 6; i++)
            {
                if (Driver.IsElementPresent(SaveNoteAndSendEmailButton, 2000))
                    return;
                Wait.UntilElementClickable(AddMeetingNotesButton).Click();
                Wait.HardWait(1000);
            }
        }

        public void ClickOnAllMeetingNotesDropdown()
        {
            Log.Step(nameof(MeetingNotesPage), "Click on All Meeting Notes dropdown");
            Wait.UntilElementClickable(AllMeetingNotesDropdown).Click();
        }

        public List<string> GetMeetingNotesDropdownOptions() =>
            Wait.UntilAllElementsLocated(AllMeetingNotesDropdownOptions).Select(e => e.GetText()).ToList();

        public List<string> GetAddMeetingNoteHeaderTitles() =>
            Wait.UntilAllElementsLocated(AddMeetingNoteHeaderTitles).Select(e => e.GetText()).ToList();

        public void ClickOnMeetingTypeDropdown()
        {
            Log.Step(nameof(MeetingNotesPage), "Click on Meeting Type Dropdown");
            Wait.UntilElementClickable(MeetingTypeDropdown).Click();
        }

        public List<string> GetMeetingTypeDropdownOptions()
        {
            Log.Step(nameof(MeetingNotesPage), "Get dropdown values for Meeting Type dropdown");
            var meetingTypeDropdownOptions = Wait.UntilAllElementsLocated(MeetingTypeDropdownOption);
            return meetingTypeDropdownOptions.Select(e => e.GetText()).ToList();
        }

        public bool IsMeetingTitleDisplayed() => Driver.IsElementDisplayed(MeetingTitle);

        public bool IsAddAttendeeButtonEnable() => Driver.IsElementEnabled(AddAttendeePlusButton);

        public List<string> GetMeetingNoteSubTabs() =>
            Wait.UntilAllElementsLocated(AddMeetingNotesAllSubTabHeaders).Select(e => e.GetText()).Take(3).ToList();

        public void ClickMeetingNoteSubTabHeaders(string tabName)
        {
            Log.Step(nameof(MeetingNotesPage), $"Click Meeting Notes {tabName} tab");
            Driver.ClickOnEscFromKeyboard(); // To close Open Dropdown
            Wait.UntilElementClickable(AddMeetingNotesSubTabHeaders(tabName)).Click();
        }

        public bool IsAddActionItemButtonDisplayed() => Driver.IsElementDisplayed(AddActionItemPlusButton);

        public void ClickOnAddADocumentDropdown()
        {
            Log.Step(nameof(MeetingNotesPage), "Click on Add A Document Tab");
            Wait.UntilElementClickable(AddADocumentDropdown).Click();
        }

        public List<string> GetDocumentDropdownValues()
        {
            Log.Step(nameof(MeetingNotesPage), "Fetching values from 'Add a Document' dropdown");
            return Wait.UntilAllElementsLocated(AddADocumentDropdownValues).Select(e => e.GetText()).ToList();
        }

        public void SelectMeetingType(string meetingType)
        {
            Log.Step(nameof(MeetingNotesPage), "Selecting Meeting Type");
            SelectItem(MeetingNoteTypeDropdown, MeetingTypeOption(meetingType));
        }

        public void SetMeetingTitle(string title)
        {
            Log.Step(nameof(MeetingNotesPage), "Set Meeting Title");
            Wait.UntilElementClickable(MeetingTitleInput).SetText(title);
        }

        private void RemoveExistingAttendee()
        {
            Log.Step(nameof(MeetingNotesPage), "Checking and removing existing attendee (if exists)");
            if (!Driver.IsElementPresent(RemoveAttendeeIcon)) return;
            Wait.HardWait(1000);
            Wait.UntilElementClickable(RemoveAttendeeIcon).Click();
        }

        private void AddAttendee(string attendee)
        {
            Log.Step(nameof(MeetingNotesPage), $"Add Attendee: {attendee}");
            RemoveExistingAttendee();
            Wait.UntilElementClickable(AddAttendeeIcon).Click();
            Wait.UntilElementClickable(AttendeeSearchTextbox).SendKeys(attendee);
            Wait.UntilElementVisible(AttendeeOption(attendee));
            Wait.UntilElementExists(AttendeeOption(attendee)).Check();
            new Actions(Driver).SendKeys(Keys.Escape).Perform();
        }

        public void SetMeetingDecisions(string decisions)
        {
            Log.Step(nameof(MeetingNotesPage), "Setting Meeting Decisions");
            Driver.SwitchToFrame(MeetingIframe);
            Wait.UntilElementClickable(MeetingDecisionTextbox).SetText(decisions);
            Driver.SwitchToDefaultIframe();
        }

        public void EnterMeetingNoteDetails(AtCommon.Dtos.BusinessOutcomes.Custom.MeetingNotes meetingNote)
        {
            Log.Step(nameof(MeetingNotesPage), "Entering Meeting Note Details");
            if (!string.IsNullOrEmpty(meetingNote.MeetingType)) SelectMeetingType(meetingNote.MeetingType);
            if (!string.IsNullOrEmpty(meetingNote.MeetingTitle)) SetMeetingTitle(meetingNote.MeetingTitle);
            if (!string.IsNullOrEmpty(meetingNote.AddAttendees)) AddAttendee(meetingNote.AddAttendees);
            if (!string.IsNullOrEmpty(meetingNote.Decisions)) SetMeetingDecisions(meetingNote.Decisions);
        }

        public void ClickSaveNoteAndSendEmail()
        {
            Log.Step(nameof(MeetingNotesPage), "Clicking 'Save Note and Send Email' Button");
            Wait.UntilElementClickable(SaveNoteAndSendEmailButton).Click();
        }

        public void ClickUpdateNoteAndSendEmail()
        {
            Log.Step(nameof(MeetingNotesPage), "Clicking 'Update Note and Send Email' Button");
            Wait.UntilElementClickable(UpdateNoteAndSendEmailButton).Click();
        }

        public void ClickExportToExcel()
        {
            Log.Step(nameof(MeetingNotesPage), "Clicking 'Export to Excel' button.");
            Wait.UntilElementClickable(ExportToExcelButton).Click();
        }

        public void SearchMeetingNotes(string searchMeetingTitle)
        {
            Log.Step(nameof(MeetingNotesPage), $"Searching for Meeting Note: {searchMeetingTitle}");
            Wait.UntilElementClickable(SearchMeetingTextbox);
            Wait.UntilElementClickable(SearchMeetingTextbox).SetText(searchMeetingTitle, isReact: true);
            Wait.HardWait(1000);
        }

        public bool IsMeetingNotePresent(string title)
        {
            Log.Step(nameof(MeetingNotesPage), $"Checking if Meeting Note is present: {title}");
            Wait.HardWait(2000);
            return Driver.IsElementPresent(MeetingNoteTitleLabel(title));
        }

        public void ClickEditMeetingButton(string meetingTitle)
        {
            Log.Step(nameof(MeetingNotesPage), $"Clicking 'Edit' button for Meeting: {meetingTitle}");
            Wait.UntilElementClickable(EditMeetingButton(meetingTitle)).Click();
        }

        public void ClickArchiveMeetingButton(string meetingTitle)
        {
            Log.Step(nameof(MeetingNotesPage), $"Archiving Meeting: {meetingTitle}");
            Wait.UntilElementClickable(MeetingActionsButton(meetingTitle)).Click();
            Wait.UntilElementClickable(ArchiveMeetingOption).Click();
            Wait.HardWait(2000);
        }
        #endregion
    }
}
