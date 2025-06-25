using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create
{
    internal class ReviewAndLaunchPage : BasePage
    {
        public ReviewAndLaunchPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //View

        //Assessment Profile
        private readonly By DeleteButton = By.Id("delete_assessment_link");
        private readonly By AssessmentName = By.XPath("//*[@id='assessment_name_span']");
        private readonly By AssessmentType = By.Id("assessment_type_span");
        private readonly By Facilitator = By.Id("facilitator_span");
        private readonly By FacilitatorEmail = By.Id("facilitator_email_span");
        private readonly By FacilitationDate = By.Id("facilitation_date_span");
        private readonly By Location = By.Id("Location_span");
        private readonly By Duration = By.Id("duration_TimeSpane");

        private readonly By RemoveAssessmentPopUpYes = By.Id("remove_assessment");
        private readonly By AssessmentProfileEditButton = By.XPath("//span[text()='Edit']");
        private readonly By EditPopUpAssessmentNameTextbox = By.Id("Name");
        private readonly By EditAssessmentDetailsPopupTitle = By.XPath("//span[text()='Edit Assessment Details']");

        private readonly By EditAssessmentDetailsPopupCancelButton = By.XPath("//div[@id='edit_details_window']//span[@id='cancel_profile'][text()='Cancel'] | //div[@id='edit_details_window']//span[@id='cancel_profile']//font[text()='Cancel']");
        private readonly By EditAssessmentDetailsPopupUpdateButton = By.XPath("//div[@id='edit_details_window']//span[@id='update_profile'][text()='Update'] | //div[@id='edit_details_window']//span[@id='update_profile']//font[text()='Update']");

        private readonly By DeleteAssessmentButton = By.Id("delete_assessment_link");
        private readonly By RemoveAssessmentButton = By.Id("remove_assessment");
        private readonly By DeleteAssessmentPopupTitle = By.XPath("//span[@id='delete_assessment_dialog_wnd_title']");
        private readonly By DeleteAssessmentPopupCancelButton = By.XPath("//span[@id='cancel_assessment']");

        private readonly By TeamMembersEditButton = By.XPath("//h5[text()='Team Members ']//a[text()='edit'] | //h5//span[text()='Team Members']/following-sibling::a[text()='edit']");
        private readonly By StakeholdersEditButton = By.XPath("//h5[text()='Stakeholders ']//a[text()='edit'] | //h5//span[text()='Stakeholders']/following-sibling::a[text()='edit'] | //h5//span[text()='Stakeholders']/following-sibling::a//font[text()='edit']");

        //Launch Options
        private readonly By PreviewAssessmentButton = By.XPath("//a[text()='Preview Assessment']");
        private readonly By SendToEveryoneRadioButton = By.Id("everyone");
        private readonly By SendToStakeholdersRadioButton = By.Id("stakeholdersOnly");
        private readonly By SendToTeamMembersRadioButton = By.Id("teamMembers");
        private readonly By PublishButton = By.Id("publishBtn");
        private readonly By SaveAsDraftButton = By.Id("draftBn");
        private readonly By FindAFacilitatorCheckbox = By.Id("FoundFacilitator");
        private readonly By SendRetrospectiveSurveyText = By.XPath("//label[contains(text(),'Send post retrospective feedback assessment?')]");
        private readonly By SendRetrospectiveSurveyCheckbox = By.CssSelector("input#LaunchAhfSurvey2");
        private readonly By AccountSetupEmailSettingSection = By.ClassName("settings-onoff");
        private readonly By AllowParticipantsToSelectTheirRolesAndParticipantGroupsCheckbox = By.Id("ParticipantCanSelectRole");
        private readonly By WhenTheParticipantsSubmitTheAssessmentRadioButton = By.Id("TeamMemberLogInAfterSubmitSurvey");
        private readonly By DoNotSendSetupEmailsRadioButton = By.Id("DoNotSendSetupEmailToTeamMembers");

        private const string EndDateCalendarId = "EndDate_dateview";
        private const string EndDateTimeId = "EndDate_timeview";
        private const string StartDateCalendarId = "StartDate_dateview";
        private const string StartDateTimeId = "StartDate_timeview";
        private readonly By StartDateInput = By.Id("StartDate");
        private readonly By EndDateInput = By.Id("EndDate");
        private readonly By LeadershipReadoutDateAndTime = By.XPath("//label[text()='Leadership Readout']//following-sibling::span//input");
        private const string LeadershipReadOutDateCalendarId = "LeadershipReadoutDate_dateview";
        private const string LeadershipReadOutDateTimeId = "LeadershipReadoutDate_timeview";

        private readonly By TeamMemberTableRows =
            By.XPath("//*[contains(@class,'review_section')]/h5/span[text()='Team Members']/../..//tbody/tr");
        private readonly By StakeHolderTableRows =
            By.XPath("//*[contains(@class,'review_section')]/h5/span[text()='Stakeholders']/../..//tbody/tr");

        public void WaitForReviewAndLaunchPageToLoad()
        {
            Wait.UntilElementVisible(AssessmentName);
        }
        //Assessment Profile
        public void ClickOnAssessmentProfileEditButton()
        {
            Log.Step(GetType().Name, "Click on the 'Edit' button of Assessment profile");
            Wait.UntilElementClickable(AssessmentProfileEditButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void FillNameForAssessmentProfile(TeamAssessmentInfo assessmentInfo)
        {
            Log.Step(GetType().Name, "Enter assessment info");
            if (!string.IsNullOrEmpty(assessmentInfo.AssessmentName))
            {
                Wait.UntilElementVisible(EditPopUpAssessmentNameTextbox).SetText(assessmentInfo.AssessmentName);
            }
        }
        public bool IsEditAssessmentDetailsPopupTitleDisplayed()
        {
            return Driver.IsElementDisplayed(EditAssessmentDetailsPopupTitle);
        }
        public void EditAssessmentDetailsPopupClickOnCancelButton()
        {
            Log.Step(GetType().Name, "Click on the 'Cancel' button");
            Wait.UntilElementClickable(EditAssessmentDetailsPopupCancelButton).Click();
        }
        public void EditAssessmentDetailsPopupClickOnUpdateButton()
        {
            Log.Step(GetType().Name, "Click on the 'Update' button");
            Wait.UntilElementClickable(EditAssessmentDetailsPopupUpdateButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnDeleteAssessmentButton()
        {
            Log.Step(GetType().Name, "Click on the 'delete assessment' button");
            Wait.UntilElementClickable(DeleteAssessmentButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnDeleteAssessmentButtonAndChooseRemoveOption()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Delete assessment");
            ClickOnDeleteAssessmentButton();
            Wait.UntilElementClickable(RemoveAssessmentButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsDeleteAssessmentPopupTitleDisplayed()
        {
            return Driver.IsElementDisplayed(DeleteAssessmentPopupTitle);
        }
        public void DeleteAssessmentPopupClickOnCancelButton()
        {
            Log.Step(GetType().Name, "Click on the 'Cancel' button of 'Delete assessment' popup");
            Wait.UntilElementClickable(DeleteAssessmentPopupCancelButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnTeamMembersEditButton()
        {
            Log.Step(GetType().Name, "Click on the 'Edit' button of Team Members");
            Wait.UntilElementClickable(TeamMembersEditButton).Click();
        }
        public void ClickOnStakeholdersEditButton()
        {
            Log.Step(GetType().Name, "Click on the 'Edit' button of Stakeholder");
            Wait.UntilElementClickable(StakeholdersEditButton).Click();
        }

        public string GetAssessmentName()
        {
            return Wait.UntilElementClickable(AssessmentName).GetText();
        }

        public void ClickOnDeleteAssessmentAndClickOnYesWhenAsked()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Click on Delete Assessment and accept delete");
            Wait.UntilElementClickable(DeleteButton).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(RemoveAssessmentPopUpYes);
            Driver.JavaScriptScrollToElement(Wait.UntilElementClickable(RemoveAssessmentPopUpYes)).Click();

            Wait.UntilJavaScriptReady();
        }

        //Lunch Options
        public void SelectSendToTeamMember()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Select Send To Team Members");
            Wait.UntilElementClickable(SendToTeamMembersRadioButton).ClickOn(Driver);
        }
        public bool IsSendToTeamMemberRadioButtonSelected()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Verify That 'SendToTeamMembers' Radio Button Selected Or Not");
            return Driver.IsElementSelected(SendToTeamMembersRadioButton);
        }

        public void SelectSendToStakeHolder()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Select Send To Stakeholder");
            Wait.UntilElementClickable(SendToStakeholdersRadioButton).ClickOn(Driver);
        }
        public bool IsSendToStakeHolderRadioButtonSelected()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Verify That 'SendToStakeholders' Radio Button Selected Or Not");
            return Driver.IsElementSelected(SendToStakeholdersRadioButton);
        }
        public void ClickOnPreviewAssessmentButton()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Click on the 'Preview Assessment' button");
            Driver.JavaScriptScrollToElement(PreviewAssessmentButton, false);
            Wait.UntilElementClickable(PreviewAssessmentButton).Click();
        }
        public void SelectSendToEveryone()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Select Send To Everyone");
            Wait.UntilElementClickable(SendToEveryoneRadioButton).ClickOn(Driver);
        }
        public bool IsSendToEveryoneRadioButtonSelected()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Verify That 'SendToEveryone' Radio Button Selected Or Not");
            return Driver.IsElementSelected(SendToEveryoneRadioButton);
        }

        public void SelectEndAssessmentDate(DateTime date)
        {
            var endDateCal = new CalendarWidget(Driver, EndDateCalendarId, EndDateTimeId);
            // set date
            endDateCal.SetDate(date);
            endDateCal.SetTime(date);
        }

        public void SelectStartAssessmentDate(DateTime date)
        {
            var startDateCal = new CalendarWidget(Driver, StartDateCalendarId, StartDateTimeId);
            startDateCal.SetDate(date);
            startDateCal.SetTime(date);
        }

        public DateTime GetAssessmentStartDate()
        {
            var text = Wait.UntilElementVisible(StartDateInput).GetText();
            return DateTime.Parse(text);
        }

        public DateTime GetAssessmentEndDate()
        {
            var text = Wait.UntilElementVisible(EndDateInput).GetText();
            return DateTime.Parse(text);
        }

        public void UpdateLeadershipReadoutDateTime(TeamAssessmentInfo assessmentInfo)
        {
            Log.Step(nameof(ReviewAndLaunchPage),"Update the Leadership Readout Date");
            if (assessmentInfo.LeadershipReadOutDate.CompareTo(new DateTime()) != 0)
            {
                var leadershipReadOutDateCal = new CalendarWidget(Driver, LeadershipReadOutDateCalendarId, LeadershipReadOutDateTimeId);
                // set date
                leadershipReadOutDateCal.SetDate(assessmentInfo.LeadershipReadOutDate);
                // set time
                leadershipReadOutDateCal.SetTime(assessmentInfo.LeadershipReadOutDate);
            }

        }

        public string GetLeadershipReadoutDateAndTime()
        {
            return Wait.UntilElementVisible(LeadershipReadoutDateAndTime).GetText();
        }

        public void SelectWhenTheParticipantsSubmitTheAssessmentRadioButton()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Select 'When the participants submit the assessment' radio button");
            Wait.UntilElementClickable(WhenTheParticipantsSubmitTheAssessmentRadioButton).ClickOn(Driver);
        }

        public void SelectDoNotSendSetupEmailsRadioButton()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Select 'Do not send setup emails' radio button");
            Wait.UntilElementClickable(DoNotSendSetupEmailsRadioButton).ClickOn(Driver);
        }

        public void ClickOnPublish()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Click on Publish button");
            Wait.UntilElementClickable(PublishButton).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnSaveAsDraft()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Click on Save As Draft button");
            Wait.UntilElementClickable(SaveAsDraftButton).ClickOn(Driver);
        }

        public bool IsSaveAsDraftVisible()
        {
            return Driver.IsElementPresent(SaveAsDraftButton);
        }

        public bool IsSendRetrospectiveSurveyTextPresent()
        {
            return Driver.IsElementPresent(SendRetrospectiveSurveyText);
        }

        public bool IsSendRetrospectiveSurveyCheckboxPresent()
        {
            return Driver.IsElementPresent(SendRetrospectiveSurveyCheckbox);
        }

        public bool IsSendRetroSurveyCheckboxEnabled()
        {
            return Wait.UntilElementExists(SendRetrospectiveSurveyCheckbox).Enabled;
        }

        public bool IsSendRetroSurveyCheckboxChecked()
        {
            return Wait.UntilElementExists(SendRetrospectiveSurveyCheckbox).Selected;
        }

        public bool IsFindAFacilitatorCheckboxVisible()
        {
            return Driver.IsElementPresent(FindAFacilitatorCheckbox) && Wait.UntilElementExists(FindAFacilitatorCheckbox).Displayed;
        }

        public bool IsFindFacilitatorCheckboxEnabled()
        {
            var checkbox = Driver.JavaScriptScrollToElement(Wait.UntilElementExists(FindAFacilitatorCheckbox));
            return checkbox.GetAttribute("disabled") != "true";
        }

        public bool IsFindFacilitatorCheckboxChecked()
        {
            var checkbox = Driver.JavaScriptScrollToElement(Wait.UntilElementExists(FindAFacilitatorCheckbox));
            return checkbox.Selected;
        }

        public bool IsAccountSetupEmailSettingSectionPresent()
        {
            return Driver.IsElementPresent(AccountSetupEmailSettingSection);
        }
        public void SelectAllowParticipantsToSelectTheirRolesAndParticipantGroups(bool check = true)
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Select 'Allow participants to select their roles / participant groups?' checkbox");
            Wait.UntilElementClickable(AllowParticipantsToSelectTheirRolesAndParticipantGroupsCheckbox).Check(check);
        }

        public Dictionary<string, string> GetTeamMembers()
        {
            Wait.UntilJavaScriptReady();
            var teamMemberRows = Wait.UntilAllElementsLocated(TeamMemberTableRows);
            if (teamMemberRows.Count >= 1 &&
                teamMemberRows.First().FindElements(By.CssSelector("td")).Count > 1)
                return teamMemberRows.ToDictionary(
                r => Wait.ForSubElement(r, By.CssSelector("td:nth-of-type(1)")).GetText(),
                r => Wait.ForSubElement(r, By.CssSelector("td:nth-of-type(3)")).GetText());
            return new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetStakeholders()
        {
            Wait.UntilJavaScriptReady();
            var stakeHolderRows = Wait.UntilAllElementsLocated(StakeHolderTableRows);
            if (stakeHolderRows.Count >= 1 &&
                stakeHolderRows.First().FindElements(By.TagName("td")).Count > 1)
                return stakeHolderRows.ToDictionary(
                r => Wait.ForSubElement(r, By.CssSelector("td:nth-of-type(1)")).GetText(),
                r => Wait.ForSubElement(r, By.CssSelector("td:nth-of-type(3)")).GetText());
            return new Dictionary<string, string>();
        }

        public void ClickOnSendRetrospectiveSurveyCheckbox()
        {
            Wait.UntilElementClickable(SendRetrospectiveSurveyCheckbox).Check();
            Wait.UntilJavaScriptReady();
        }

        public void CheckFindFacilitatorCheckbox()
        {
            Log.Step(nameof(ReviewAndLaunchPage), "Check the 'Find a Facilitator' checkbox");
            Wait.UntilElementClickable(FindAFacilitatorCheckbox).Check();
        }

        public TeamAssessmentInfo GetAssessmentProfile()
        {
            var timeZoneFormat = CSharpHelpers.GetTimeZone(DateTime.Now);

            var facilitationDateText = Wait.UntilElementExists(FacilitationDate).GetText().Replace(timeZoneFormat, "").Replace(" at ", " ");
            var durationText = Wait.UntilElementExists(Duration).GetText().Replace(" hours", "").Replace(" hour", "");

            return new TeamAssessmentInfo
            {
                AssessmentName = Wait.UntilElementVisible(AssessmentName).GetText(),
                AssessmentType = Wait.UntilElementVisible(AssessmentType).GetText(),
                Facilitator = Wait.UntilElementExists(Facilitator).GetText(),
                FacilitatorEmail = Wait.UntilElementExists(FacilitatorEmail).GetText(),
                FacilitationDate = DateTime.Parse(facilitationDateText),
                Location = Wait.UntilElementExists(Location).GetText(),
                FacilitationDuration = int.Parse(durationText)
            };
        }
    }
}