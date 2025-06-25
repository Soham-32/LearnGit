using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit
{
    internal class TeamAssessmentEditPage : BasePage
    {
        public TeamAssessmentEditPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //View
        //Assessment Detail
        private readonly By EditButton = By.Id("edit_details");
        private readonly By PreviewButton = By.Id("preview_link");
        private readonly By DeleteAssessmentButton = By.Id("delete_assessment_link");
        private readonly By AssessmentName = By.XPath("//div[@class='intro']//span[@class='assessment_name_span']");
        private readonly By AssessmentType = By.Id("assessment_type_span");
        private readonly By Facilitator = By.Id("facilitator_span");
        private readonly By FacilitatorEmail = By.Id("facilitator_email_span");
        private readonly By FacilitationDate = By.Id("facilitation_date_span");
        private readonly By StartDate = By.Id("start_date_span");
        private readonly By EndDate = By.Id("end_date_span");
        private readonly By Location = By.Id("Location_span");
        private readonly By Duration = By.Id("duration_time");
        private readonly By LeadershipReadoutDate = By.Id("leadership_readout_date_span");
        private readonly By StartAssessmentResultButton = By.Id("share_assessment_link");
        private readonly By ShareAssessmentConfirmPopupYesButton = By.Id("share_results_assessment");
        private readonly By StopSharingAssessmentResultButton = By.Id("stop_sharing_assessment_link");
        private readonly By StopSharingAssessmentConfirmPopupYesButton = By.Id("stop_share_results_assessment");
        private readonly By TeamMembersSendToAllButton = By.Id("send_all_teammembers");
        private readonly By StakeholdersSendToAllButton = By.Id("send_all_stakeholders");
        private readonly By AccountSetupEmailText = By.XPath("//label[text()='Account Setup E-Mail']/./following-sibling::span");
        private readonly By HeaderTitle = By.CssSelector("div[class='txt fl-lt']");
        private readonly By AssessmentLink = By.Id("btnCopyToClipboard");

        //Delete pop up
        private readonly By RemoveAssessmentButton = By.Id("remove_assessment");

        //Edit pop up
        private readonly By EditAssessmentDetailsPopup = By.Id("edit_details_window");
        private readonly By EditPopUpAssessmentNameTextbox = By.Id("Name");
        private readonly By FacilitatorDropDown = By.XPath("//input[@aria-owns='Facilitator_taglist Facilitator_listbox']");
        private static By FacilitatorListItem(string facilitator) => By.XPath($"//ul[@id = 'Facilitator_listbox']/li[contains(normalize-space(),'{facilitator}')]");
        private const string FacilitationDateCalendarId = "FacilitationDate_dateview";
        private const string FacilitationDateTimeId = "FacilitationDate_timeview";
        private static By FacilitatorDeselectIcon(string facilitatorName) => By.XPath($"*//ul[@id = 'Facilitator_taglist']//span[text() = '{facilitatorName}']//following-sibling::span");
        private const string StartDateCalendarId = "StartDate_dateview";
        private const string StartDateTimeId = "StartDate_timeview";
        private readonly By FacilitationDurationListBox = By.XPath("//select[@id = 'ddlDuration']/preceding-sibling::span");
        private static By FacilitationDurationListItem(string item) => By.XPath($"//ul[@id = 'ddlDuration_listbox']/li[contains(normalize-space(),'{item}')]");
        private const string LeadershipReadOutDateCalendarId = "LeadershipReadoutDate_dateview";
        private const string LeadershipReadOutDateTimeId = "LeadershipReadoutDate_timeview";
        private readonly By LocationTextbox = By.Id("Location");
        private const string EndDateCalendarId = "EndDate_dateview";
        private const string EndDateTimeId = "EndDate_timeview";

        private readonly By EditPopUpUpdateButton = By.Id("update_profile"); private readonly By SendPostRetroFeedbackSurveyCheckbox = By.Id("LaunchAhfSurvey1");
        private readonly By SendPostRetroFeedbackSurveyListBox = By.CssSelector("[aria-owns='AHFSurveySendDateOption1_listbox']");
        private static By SendPostRetroFeedbackSurveyListItem(string item) => By.XPath($"//ul[@id = 'AHFSurveySendDateOption1_listbox']/li[text() = '{item}']");
        private readonly By CampaignDropDown = By.XPath("//select[@id='CampaignId']");
        private static By CampaignListBox(string item) => By.XPath($"//select[@id='CampaignId']//option[text()='{item}']");

        //Results
        private readonly By ResultsButton = By.Id("detail_radar_btn");

        //Team Members
        private readonly By AddTeamMembersButton = By.Id("add_team_members");
        private readonly By SendToAllTeamMemberButton = By.Id("send_all_teammembers");
        private readonly By TeamMembersExportToExcelButton = By.XPath("//div[@id = 'teamMembersGrid']//button[text() = 'Export to Excel']");
        private static By MemberResendInviteLinkButton(string email) => By.XPath($"//div[@id='teamMembersGrid']//td[text()='{email}']/following-sibling::td//img[@title='Resend Invite'] | //div[@id='teamMembersGrid']//td//font[text()='{email}']//..//../following-sibling::td//img[@title='Resend Invite']");
        private static By CompletedActionsText(string memberEmail) => By.XPath($"//td[text()='{memberEmail}']//following-sibling::td//div/span[@class='completed left_td']");
        private static By TeamMemberViewLink(string email) => By.XPath($"//div[@id='teamMembersGrid']//td[text()='{email}']//following-sibling::td//img[@title='Copy to Clipboard'] | //div[@id='teamMembersGrid']//td//font[text()='{email}']//..//..//following-sibling::td//img[@title='Copy to Clipboard']");
        private static By DeleteTeamMemberButton(string email) => By.XPath($"//div[@id='teamMembersGrid']//td[text()='{email}']/following-sibling::td//img[@title='Remove Participant'] | //div[@id='teamMembersGrid']//td//font[text()='{email}']//..//../following-sibling::td//img[@title='Remove Participant']");
        private static By TeamMemberReopenAssessmentIcon(string email) => By.XPath($"//div[@id='teamMembersGrid']//td[text()='{email}']/following-sibling::td//img[@title='Open Assessment']");
        private static By StakeHolderReopenAssessmentIcon(string email) => By.XPath($"//div[@id='stakeHoldersGrid']//td[text()='{email}']/following-sibling::td//img[@title='Open Assessment']");
        private static By TeamMemberCheckbox(string email) => By.XPath($"//div[@id='TeamMembersGrid']/table/tbody//td[text()='{email}']/preceding-sibling::td//input");

        private readonly By TeamMemberEmails = By.XPath("//div[@id='teamMembersGrid']/table/tbody/tr/td[3]");

        // Grid by Email
        private static By FirstNameFromGrid(string email) => By.XPath($"//td[text()='{email}']/preceding-sibling::td[2]");
        private static By LastNameFromGrid(string email) => By.XPath($"//td[text()='{email}']/preceding-sibling::td[1]");
        private static By EmailFromGrid(string email) => By.XPath($"//td[text()='{email}']");
        private static By RoleFromGrid(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td[2]");
        private static By TagsFromGrid(string email) => By.XPath($"//td[text()='{email}']//following-sibling::td[2]");

        //Add pop up
        private readonly By TeamMemberAddPopUpUpdateButton = By.Id("update_contacts");

        //Send Email popup
        private readonly By SendEmailPopupCloseButton = By.XPath("//span[@id='close_email_sent']");

        //Delete confirmation dialog
        private readonly By DeleteConfirmationDialogRemoveButton = By.CssSelector("#delete_contact_dialog #delete");


        //Stakeholders
        private readonly By AddStakeHoldersButton = By.Id("add_stakeholders");
        private readonly By SendToAllStakeholderButton = By.Id("send_all_stakeholders");
        private readonly By StakeholdersExportToExcelButton =
            By.XPath("//div[@id = 'stakeHoldersGrid']//button[text() = 'Export to Excel'] | //div[@id = 'stakeHoldersGrid']//button//font[text() = 'Export to Excel']");
        private static By StakeHolderDeleteButton(string email) => By.XPath(
            $"//div[@id='stakeHoldersGrid']/table/tbody//td[text()='{email}']/following-sibling::td//img[@title='Remove Participant'] | //div[@id='stakeHoldersGrid']/table/tbody//td//font[text()='{email}']//..//../following-sibling::td//img[@title='Remove Participant']");
        private static By StakeholderResendInviteLinkButton(string email) => By.XPath(
            $"//div[@id='stakeHoldersGrid']/table/tbody//td[contains(normalize-space(),'{email}')]/following-sibling::td//img[@title='Resend Invite']");
        private static By StakeholderViewLink(string email) => By.XPath(
            $"//div[@id='stakeHoldersGrid']//td[text()='{email}']//following-sibling::td//img[@title='Copy to Clipboard'] | //div[@id='stakeHoldersGrid']//td//font[text()='{email}']//..//..//following-sibling::td//img[@title='Copy to Clipboard']");
        private static By StakeHolderCheckbox(string email) =>
           By.XPath($"//div[@id='StakeHolderGrid']/table/tbody//td[text()='{email}']/preceding-sibling::td//input");

        private readonly By StakeholderEmails = By.XPath("//div[@id='stakeHoldersGrid']/table/tbody/tr/td[3]");

        //Add pop up
        private readonly By StakeholderAddPopUpUpdateButton = By.Id("update_contacts_stake");
        // Pin Access Popup
        private readonly By PinAccessPopUp = By.Id("dispalyPinInformation");
        private readonly By PinAccessPopupPin = By.XPath("//span[@style='display:inline-flex']/b[contains(text(),'PIN')]/following-sibling::div");
        private readonly By PinAccessPopupUrl = By.XPath("//span[@style='display:inline-flex']/b[contains(text(),'URL')]/following-sibling::div");
        private readonly By PinAccessPopUpCloseButton = By.Id("close_dispalyPinInformation");
        private readonly By TeamMembersDisplayPinInfoButton = By.Id("display_Pin_Info_Teammemeber");
        private readonly By StakeholdersDisplayPinInfoButton = By.Id("display_Pin_Info_stakeHolder");

        //View Link Popup
        private readonly By ViewLinkPopupTitle = By.Id("view_link_dialog_wnd_title");
        private readonly By ViewLinkPopupLink = By.Id("view_link_link");
        private readonly By ViewLinkPopupCloseButton = By.Id("close_view_link");

        //ReOpen Assessment popup
        private readonly By ReopenAssessmentPopupYesButton = By.Id("reopen_yes");
        //Assessments Note
        private readonly By AssessmentNoteTitle = By.Id("assessment_title");
        private readonly By Description = By.CssSelector("body.k-state-active");
        private readonly By DescriptionIframe = By.XPath("//iframe[@class='k-content']");
        private readonly By DescriptionIframeBody = By.CssSelector("body");
        private readonly By SaveAssessmentNoteButton = By.Id("SaveAssessmentNote");
        private readonly By AssessmentChecklistTab = By.Id("ManageTeamMaturity");
        private readonly By ReturnToDashboardButton = By.CssSelector("div.pg-title a.green-btn");
        private readonly By ChecklistSingleDropdown = By.CssSelector("#AssessmentNotesMaturityChecklist-2 .k-dropdown-wrap .k-input");
        private static By ChecklistSingleItem(string item) => By.XPath($"//ul[contains(@id, 'dropdownlist')]//li[text() = '{item}']");
        private readonly By ChecklistMultiDropdown = By.CssSelector("#AssessmentNotesMaturityChecklist-2 .k-multiselect-wrap");
        private static By ChecklistMultiItem(string item) => By.XPath($"//ul[contains(@id, 'multiSelect')]//li[contains(normalize-space(),'{item}')]");
        private readonly By SaveMaturityChecklistButton = By.Id("SaveMaturityChecklist");
        private readonly By MaturityChecklistSavedPopup = By.Id("window_wnd_title");
        //Assessment Details
        public void WaitForEditAssessmentPageToLoad()
        {
            Wait.UntilElementExists(AssessmentName);
        }
        public void ClickOnResultsButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on the 'Results' button.");
            Wait.UntilElementClickable(ResultsButton).Click();
        }
        public void ClickOnPreviewButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on the 'Preview' button");
            Wait.UntilElementClickable(PreviewButton).Click();
        }
        public void StartSharingAssessmentResult()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Start Sharing Assessment Result' button and start sharing");
            Wait.UntilElementClickable(StartAssessmentResultButton).Click();
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(StartAssessmentResultButton));
            Wait.UntilElementClickable(ShareAssessmentConfirmPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public bool IsStartSharingAssessmentButtonDisplayed()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Is 'Start sharing assessment result' button displayed?");
            return Driver.IsElementDisplayed(StartAssessmentResultButton);
        }

        public void StopSharingAssessmentResult()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Stop Sharing Assessment Result' button' and stop sharing");
            Wait.UntilElementClickable(StopSharingAssessmentResultButton).Click();
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(StopSharingAssessmentConfirmPopupYesButton));
            Wait.UntilElementClickable(StopSharingAssessmentConfirmPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnDeleteAssessmentButtonAndChooseRemoveOption()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Delete assessment");
            Wait.UntilElementClickable(DeleteAssessmentButton).Click();
            Wait.UntilJavaScriptReady();
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(RemoveAssessmentButton));
            Wait.UntilElementClickable(RemoveAssessmentButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsDeleteButtonDisplayed()
        {
            return Driver.IsElementDisplayed(DeleteAssessmentButton);
        }
        public void FillDataForAssessmentProfile(TeamAssessmentInfo assessmentInfo)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Enter assessment info");
            if (!string.IsNullOrEmpty(assessmentInfo.AssessmentName))
            {
                Wait.UntilElementVisible(EditPopUpAssessmentNameTextbox).SetText(assessmentInfo.AssessmentName);
            }

            if (!string.IsNullOrEmpty(assessmentInfo.Facilitator))
            {
                SelectItem(FacilitatorDropDown, FacilitatorListItem(assessmentInfo.Facilitator));
            }

            if (assessmentInfo.StartDate.CompareTo(new DateTime()) != 0)
            {
                var startDateCalendar = new CalendarWidget(Driver, StartDateCalendarId, StartDateTimeId);
                // set date
                startDateCalendar.SetDateForAssessment(assessmentInfo.StartDate);
                // set time 
                startDateCalendar.SetTime(assessmentInfo.StartDate);
            }

            if (assessmentInfo.FacilitationDate.CompareTo(new DateTime()) != 0)
            {
                var facilitationDateCal = new CalendarWidget(Driver, FacilitationDateCalendarId, FacilitationDateTimeId);
                // set date
                facilitationDateCal.SetDate(assessmentInfo.FacilitationDate);
                // set time 
                facilitationDateCal.SetTime(assessmentInfo.FacilitationDate);
            }

            if (assessmentInfo.FacilitationDuration != 0 && assessmentInfo.FacilitationDate.CompareTo(new DateTime()) != 0)
            {
                var durationListItemText = assessmentInfo.FacilitationDate.AddHours(assessmentInfo.FacilitationDuration).ToString("hh:mm tt") +
                    $" ({assessmentInfo.FacilitationDuration} hours)";
                SelectItem(FacilitationDurationListBox, FacilitationDurationListItem(durationListItemText));
            }
            if (assessmentInfo.LeadershipReadOutDate.CompareTo(new DateTime()) != 0)
            {
                var leadershipReadOutDateCal = new CalendarWidget(Driver, LeadershipReadOutDateCalendarId, LeadershipReadOutDateTimeId);
                // set date
                leadershipReadOutDateCal.SetDate(assessmentInfo.LeadershipReadOutDate);
                // set time
                leadershipReadOutDateCal.SetTime(assessmentInfo.LeadershipReadOutDate);
            }

            if (!string.IsNullOrEmpty(assessmentInfo.Location))
            {
                Wait.UntilElementClickable(LocationTextbox).SetText(assessmentInfo.Location);
            }

            if (assessmentInfo.EndDate.CompareTo(new DateTime()) != 0)
            {
                var endDateCalendar = new CalendarWidget(Driver, EndDateCalendarId, EndDateTimeId);

                // set date
                endDateCalendar.SetDate(assessmentInfo.EndDate);
                // set time 
                endDateCalendar.SetTime(assessmentInfo.EndDate);
            }

            if (assessmentInfo.SendRetroSurvey)
            {
                Wait.UntilElementClickable(SendPostRetroFeedbackSurveyCheckbox).Check();
                SelectItem(SendPostRetroFeedbackSurveyListBox, SendPostRetroFeedbackSurveyListItem(assessmentInfo.SendRetroSurveyOption));
            }
            else if (!string.IsNullOrEmpty(assessmentInfo.Facilitator))
            {
                Wait.UntilElementClickable(SendPostRetroFeedbackSurveyCheckbox).Check(false);
            }
            if (!string.IsNullOrEmpty(assessmentInfo.Campaign))
            {
                SelectItem(CampaignDropDown, CampaignListBox(assessmentInfo.Campaign));
            }
        }

        public void ClickOnAssessmentLinkCopyIcon()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Assessment Link' copy icon");
            Wait.UntilElementClickable(AssessmentLink).Click();
        }

        public bool IsAssessmentLinkCopyIconDisplayed()
        {
            return Driver.IsElementDisplayed(AssessmentLink);
        }
        public void UpdateEndDate(DateTime date)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Update assessment end date");
            var endDateCalendar = new CalendarWidget(Driver, EndDateCalendarId, EndDateTimeId);
            endDateCalendar.SetDate(date);
        }


        public TeamAssessmentInfo GetAssessmentProfile()
        {
            var timeZoneFormat = CSharpHelpers.GetTimeZone(DateTime.Now);

            var facilitationDateText = Wait.UntilElementVisible(FacilitationDate).GetText().Replace(timeZoneFormat, "").Replace(" at ", " ");
            var startDateText = Wait.UntilElementVisible(StartDate).GetText().Replace(timeZoneFormat, "").Replace(" at ", " ");
            var endDateText = Wait.UntilElementVisible(EndDate).GetText().Replace(timeZoneFormat, "").Replace(" at ", " ");
            var durationText = Wait.UntilElementVisible(Duration).GetText().Replace(" hours", "").Replace(" hour", "");
            var leadershipReadoutDateTimeText = Wait.UntilElementVisible(LeadershipReadoutDate).GetText().Replace(timeZoneFormat, "").Replace(" at ", " ");

            return new TeamAssessmentInfo
            {
                AssessmentName = Wait.UntilElementVisible(AssessmentName).GetText(),
                AssessmentType = Wait.UntilElementVisible(AssessmentType).GetText(),
                Facilitator = Wait.UntilElementVisible(Facilitator).GetText(),
                FacilitatorEmail = Wait.UntilElementVisible(FacilitatorEmail).GetText(),
                FacilitationDate = DateTime.Parse(facilitationDateText),
                StartDate = DateTime.Parse(startDateText),
                EndDate = DateTime.Parse(endDateText),
                Location = Wait.UntilElementVisible(Location).GetText(),
                FacilitationDuration = int.Parse(durationText),
                LeadershipReadOutDate = DateTime.Parse(leadershipReadoutDateTimeText)
            };
        }

        public void ClickEditDetailButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Edit button");
            Wait.UntilElementClickable(EditButton).Click();
        }

        public void ClickMemberResendInviteLinkButton(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'resend invite link' button for the team member");
            Driver.JavaScriptScrollToElement(MemberResendInviteLinkButton(email));
            Wait.UntilElementClickable(MemberResendInviteLinkButton(email)).Click();
            Wait.UntilElementClickable(SendEmailPopupCloseButton).Click();
        }
        public void ClickOnTeamMemberViewLink(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Team Member's View link.");
            Wait.UntilElementClickable(TeamMemberViewLink(email)).Click();
        }
        public void ClickStakeholderResendInviteLinkButton(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'resend invite link' button for the stakeholder");
            Wait.HardWait(1000);//Takes time to load 
            Wait.UntilElementClickable(StakeholderResendInviteLinkButton(email)).Click();
            Wait.UntilElementClickable(SendEmailPopupCloseButton).Click();
        }
        public void ClickOnStakeholderViewLink(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Stakeholder's View link.");
            Wait.UntilElementClickable(StakeholderViewLink(email)).Click();
        }
        public string ViewLinkPopupGetLink()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Get link from 'ViewLink' popup");
            return Wait.UntilElementExists(ViewLinkPopupLink).GetAttribute("value");
        }
        public void ViewLinkPopupClickOnCloseButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on the 'ViewLinkPopupClose' button");
            Wait.UntilElementClickable(ViewLinkPopupCloseButton).Click();
            Wait.UntilElementInvisible(ViewLinkPopupCloseButton);
        }
        public void EditPopup_ClickUpdateButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "On Edit popup, click Update button");
            Wait.UntilElementClickable(EditPopUpUpdateButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public string GetAccountSetupEmailText()
        {
            return Wait.UntilElementVisible(AccountSetupEmailText).GetText();
        }
        public bool IsEditAssessmentDetailsPopupDisplayed()
        {
            return Driver.IsElementDisplayed(EditAssessmentDetailsPopup);
        }

        //Team Members
        public void DeleteTeamMemberByEmail(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Delete team member with email <{email}>");
            Wait.UntilElementClickable(DeleteTeamMemberButton(email)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(DeleteConfirmationDialogRemoveButton).Click();

            Wait.UntilElementInvisible(DeleteConfirmationDialogRemoveButton);
            Wait.UntilJavaScriptReady();
        }

        public bool IsTeamMemberResendInviteIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(MemberResendInviteLinkButton(email));
        }
        public bool IsTeamMemberViewLinkIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(TeamMemberViewLink(email));
        }
        public bool IsTeamMemberRemoveParticipantIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(DeleteTeamMemberButton(email));
        }
        public bool IsTeamMembersReopenAssessmentIconDisplayed(string email)
        {
            Driver.JavaScriptScrollToElement(TeamMembersExportToExcelButton);
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(TeamMemberReopenAssessmentIcon(email), 20);
        }
        public void ClickOnTeamMembersReopenAssessmentIcon(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Team Member's ReopenAssessment icon.");
            Wait.UntilElementClickable(TeamMemberReopenAssessmentIcon(email)).Click();
            Wait.UntilElementVisible(ReopenAssessmentPopupYesButton);
            Wait.UntilElementClickable(ReopenAssessmentPopupYesButton).Click();
        }

        public TeamMemberInfo GetTeamMemberInfoFromGridByEmail(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Get the Team member information from grid for team member {email}");
            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = Wait.UntilElementExists(FirstNameFromGrid(email)).Text,
                LastName = Wait.UntilElementExists(LastNameFromGrid(email)).Text,
                Email = Wait.UntilElementExists(EmailFromGrid(email)).Text,
                Role = Wait.UntilElementExists(RoleFromGrid(email)).Text,
                ParticipantGroup = Wait.UntilElementExists(TagsFromGrid(email)).Text
            };
            return teamMemberInfo;
        }

        public List<string> GetTeamMemberEmails()
        {
            Wait.UntilElementVisible(TeamMemberEmails);
            return Driver.GetTextFromAllElements(TeamMemberEmails).ToList();
        }

        public string GetCompletedActionsText(string memberEmail)
        {
            return Driver.JavaScriptScrollToElement(CompletedActionsText(memberEmail)).GetText();
        }

        public void AddTeamMemberByEmail(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Add a team member with email <{email}>");
            Wait.UntilElementClickable(AddTeamMembersButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamMemberCheckbox(email)).Check();
            Wait.UntilElementClickable(TeamMemberAddPopUpUpdateButton).Click();
            Wait.UntilElementInvisible(TeamMemberAddPopUpUpdateButton);
            Wait.UntilJavaScriptReady();
        }
        public void ClickSendToAllButtonForTeamMembers()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Send To All' button for team member");
            Wait.UntilElementClickable(SendToAllTeamMemberButton).Click();
        }
        public void ClickSendToAllButtonForStakeholders()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Send To All' button for stakeholder");
            Wait.UntilElementClickable(SendToAllStakeholderButton).Click();
        }

        internal void ClickReturnToDashboardButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Return to Dashboard button");
            Wait.UntilElementClickable(ReturnToDashboardButton).Click();
        }

        public bool IsReturnToDashboardButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ReturnToDashboardButton);
        }
        //Stakeholders
        public void DeleteStakeholderByEmail(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Delete a stakeholder with email <{email}>");
            Wait.UntilElementVisible(StakeHolderDeleteButton(email));
            Wait.UntilElementClickable(StakeHolderDeleteButton(email)).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(DeleteConfirmationDialogRemoveButton).Click();
            Wait.UntilElementInvisible(DeleteConfirmationDialogRemoveButton);
            Wait.UntilJavaScriptReady();
        }

        public bool IsStakeholderResendInviteIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(StakeholderResendInviteLinkButton(email));
        }
        public bool IsStakeholderViewLinkIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(StakeholderViewLink(email));
        }
        public bool IsStakeholderRemoveParticipantsIconDisplayed(string email)
        {
            return Driver.IsElementDisplayed(StakeHolderDeleteButton(email));
        }
        public bool IsStakeholderReopenAssessmentIconDisplayed(string email)
        {
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(StakeHolderReopenAssessmentIcon(email));
        }
        public void ClickOnStakeholdersReopenAssessmentIcon(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Stakeholder's ReopenAssessment icon.");
            Wait.UntilElementClickable(StakeHolderReopenAssessmentIcon(email)).Click();
            Wait.UntilElementVisible(ReopenAssessmentPopupYesButton);
            Wait.UntilElementClickable(ReopenAssessmentPopupYesButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetStakeholderEmails()
        {
            return Driver.GetTextFromAllElements(StakeholderEmails).ToList();
        }

        public void AddStakeholderByEmail(string email)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Add a stakeholder with email <{email}>");
            Wait.UntilElementClickable(AddStakeHoldersButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(StakeHolderCheckbox(email)).Click();
            Wait.UntilElementClickable(StakeholderAddPopUpUpdateButton).Click();
            Wait.UntilElementInvisible(StakeholderAddPopUpUpdateButton);
            Wait.UntilJavaScriptReady();
        }


        //Assessment Notes
        public void EnterDataForAssessmentNotesSection(string title, string maturity, string description)
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Enter assessment note info");
            Wait.UntilElementClickable(AssessmentNoteTitle).SetText(title);

            //TODO: Bug on QA environment, hence commenting code for while
            //SelectItem(TeamMaturityDropdown, TeamMaturityListItem(maturity));

            if (!Driver.IsInternetExplorer())
            {
                Driver.SwitchToFrame(DescriptionIframe);
                Wait.UntilElementClickable(DescriptionIframeBody).Click();
                Wait.UntilElementClickable(Description).SetText(description);
                Driver.SwitchTo().DefaultContent();
            }
            else
            {
                Driver.ExecuteJavaScript("document.getElementById('SaveAssessmentNote').focus()");
                AutoIt.EnterAssessmentNoteDescription(description);
            }

            Wait.UntilElementClickable(SaveAssessmentNoteButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetNoteDescription()
        {
            Driver.SwitchToFrame(DescriptionIframe);
            var description = Wait.UntilElementVisible(DescriptionIframeBody).GetAttribute("textContent");
            Driver.SwitchTo().DefaultContent();
            return description.Trim();
        }

        public string GetAssessmentNotes()
        {
            return Wait.UntilElementVisible(AssessmentNoteTitle).GetAttribute("value");
        }

        public bool DoesAssessmentChecklistDisplay()
        {
            return Wait.InCase(AssessmentChecklistTab) != null;
        }

        public void ClickOnAssessmentChecklistTab()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on the Assessment Checklist tab");
            Wait.UntilElementClickable(AssessmentChecklistTab).Click();
        }
        public void DeselectFacilitator(string facilitatorName)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Deselect Facilitator - {facilitatorName}");
            Wait.UntilElementClickable(FacilitatorDeselectIcon(facilitatorName)).Click();
        }

        public bool DoesDisplayPinButtonDisplay()
        {
            return Driver.IsElementDisplayed(TeamMembersDisplayPinInfoButton);
        }
        public void ClickOnDisplayPinInfoButtonForTeamMember()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Display Pin info button");
            Wait.UntilElementClickable(TeamMembersDisplayPinInfoButton).Click();
        }

        public void ClickOnDisplayPinInfoButtonForStakeholder()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Display Pin info button");
            Wait.UntilElementClickable(StakeholdersDisplayPinInfoButton).Click();
        }

        public bool IsPinAccessPopupDisplay()
        {
            Driver.JavaScriptScrollToElement(PinAccessPopUp, false);
            return Driver.IsElementDisplayed(PinAccessPopUp, 10);
        }

        public string GetPinFromPinAccessPopup()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Get the 'PIN' from pin access popup");
            return Wait.UntilElementVisible(PinAccessPopupPin).GetText();
        }
        public string GetUrlFromPinAccessPopup()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Get the 'URL' from pin access popup");
            return Wait.UntilElementVisible(PinAccessPopupUrl).GetText();
        }
        public void ClickOnPinAccessPopupCloseButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Close' button for PIN access popup");
            Wait.UntilElementClickable(PinAccessPopUpCloseButton).Click();
        }

        public void ClickTeamMembersExportToExcelButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Team Member, Export to Excel button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(TeamMembersExportToExcelButton).Click();
        }

        public void ClickStakeholdersExportToExcelButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Stakeholder, Export to Excel button");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(StakeholdersExportToExcelButton).Click();
        }

        public void SelectChecklistItem1(string item)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Selecting item <{item}> in the first checklist item.");
            Wait.HardWait(3000);//Checklist dropdown takes time to open
            SelectItem(ChecklistSingleDropdown, ChecklistSingleItem(item));
        }

        public void SelectChecklistItem2(string item)
        {
            Log.Step(nameof(TeamAssessmentEditPage), $"Selecting item <{item}> in the second checklist item.");
            Wait.HardWait(3000);//Checklist dropdown takes time to open
            SelectItem(ChecklistMultiDropdown, ChecklistMultiItem(item));
        }

        public void ClickSaveMaturityChecklistButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on Save Maturity Checklist button");
            Wait.UntilElementClickable(SaveMaturityChecklistButton).Click();
            Wait.UntilElementVisible(MaturityChecklistSavedPopup);
        }

        public string GetCheckList1Value()
        {
            return Wait.UntilElementVisible(ChecklistSingleDropdown).GetText();
        }

        public string GetCheckList2Value()
        {
            return Wait.UntilElementVisible(ChecklistMultiDropdown).GetText();
        }
        public string GetAssessmentNameFromHeader()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Get Assessment header name");
            return Wait.UntilElementVisible(HeaderTitle).GetText();
        }
        public void ClickOnTeamMembersSendToAllButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Send to all' button");
            Wait.UntilElementClickable(TeamMembersSendToAllButton).Click();
        }

        public void ClickOnStakeholdersSendToAllButton()
        {
            Log.Step(nameof(TeamAssessmentEditPage), "Click on 'Send To All' button for StakeHolders");
            Wait.UntilElementClickable(StakeholdersSendToAllButton).Click();
        }
        public bool IsStakeHolderReopenAssessmentIconDisplayed(string email)
        {
            Driver.JavaScriptScrollToElement(StakeHolderReopenAssessmentIcon(email), false);
            Wait.UntilJavaScriptReady();
            return Driver.IsElementDisplayed(StakeHolderReopenAssessmentIcon(email), 10);
        }
    }
}