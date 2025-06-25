using System;
using System.Linq;
using OpenQA.Selenium;
using System.Globalization;
using System.Collections.Generic;
using AngleSharp.Common;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AtCommon.Dtos.CampaignsV2;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class SetUpAssessmentsPage : ManageCampaignsCommonPage
    {
        public SetUpAssessmentsPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By ContinueToReviewButton = By.XPath("//button[text()='Continue to review']");
        private readonly By AssessmentNameTextbox = By.Id("name");
        private readonly By AssessmentNameTooltipIcon = By.XPath("//label[@for='name']/parent::div//following-sibling::div/button/*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By TooltipMessage = By.XPath("//div[contains(@id,'mui-')]/div");
        private readonly By AssessmentNameValidationMessage = AutomationId.Equals("name", "+p");
        private readonly By SplitMeetingRadioButton = By.XPath("//input[@value='TwoSeparateMeetings']//parent::span");
        private readonly By OneMeetingRadioButton = By.XPath("//input[@value='SingleCombinedMeeting']//parent::span");
        private readonly By RetroOnlyRadioButton = By.XPath("//input[@value='SingleRetroMeeting']//parent::span");
        private readonly By SplitMeetingTooltipIcon = By.XPath("//strong[contains(text(),'Split Meeting')]//parent::p//parent::div/following-sibling::div//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By OneMeetingTooltipIcon = By.XPath("//strong[contains(text(),'One Meeting')]//parent::p//parent::div/following-sibling::div//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By RetroOnlyTooltipIcon = By.XPath("//strong[contains(text(),'Retro Only')]//parent::p//parent::div/following-sibling::div//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By AssessmentTimelineTooltipIcon = By.XPath("//h4[text()='Assessment Timeline']/parent::div/following-sibling::div//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By CampaignStartsDate = By.XPath("//p[contains(text(),'Campaign Starts')]");
        private readonly By CampaignEndsDate = By.XPath("//p[contains(text(),'Campaign Ends')]");
        private readonly By AssessmentTimelineLabels = By.XPath("//div[contains(@class,'MuiBox-root') and text()]");
        private const string SplitMeetingStakeholderLaunchDate = "twoMeetingsStakeholderLaunchDate";
        private const string SplitMeetingTeamMemberLaunchDate = "twoMeetingsTeamMemberLaunchDate";
        private const string SplitMeetingAssessmentCloseDate = "twoMeetingsCloseDate";
        private const string SplitMeetingRetrospectiveWindowStart = "twoMeetingsRetrospectiveWindowStart";
        private const string SplitMeetingRetrospectiveWindowEnd = "twoMeetingsRetrospectiveWindowEnd";
        private const string OneMeetingStakeholderWindowStart = "singleCombinedMeetingStakeholderWindowStart";
        private const string OneMeetingStakeholderWindowEnd = "singleCombinedMeetingStakeholderWindowEnd";
        private const string OneMeetingRetrospectiveWindowStart = "singleCombinedMeetingRetrospectiveWindowStart";
        private const string OneMeetingTeamMemberLaunchDate = "singleCombinedMeetingTeamMemberLaunchDate";
        private const string OneMeetingRetrospectiveWindowEnd = "singleCombinedMeetingRetrospectiveWindowEnd";
        private const string RetroOnlyAssessmentStartDate = "singleRetroMeetingAssessmentStartDate";
        private const string RetroOnlyAssessmentCloseDate = "singleRetroMeetingAssessmentCloseDate";
        private const string RetroOnlyRetrospectiveWindowStart = "singleRetroMeetingRetrospectiveWindowStart";
        private const string RetroOnlyRetrospectiveWindowEnd = "singleRetroMeetingRetrospectiveWindowEnd";
        private static By AssessmentTimelineDatesTooltipIcon(string option) => By.XPath($"//label[@for='{option}']/parent::div/following-sibling::div//*[local-name()='svg' and @data-icon='question-circle']");
        private static By MeetingDate(string option) => By.XPath($"//div[@automation-id='{option}']//input");

        //Methods
        public void ClickOnContinueToReviewButton()
        {
            Log.Step(GetType().Name, "Click on 'Continue to Review' button");
            Driver.JavaScriptScrollToElement(Wait.UntilAllElementsLocated(ContinueToReviewButton).GetItemByIndex(1)).Click();
            Wait.HardWait(3000);//wait till moved to button
        }

        public bool IsContinueToReviewButtonEnabled()
        {
            return Driver.IsElementEnabled(ContinueToReviewButton);
        }

        public void EnterAssessmentName(string assessmentName)
        {
            Log.Step(GetType().Name, "Enter the 'Assessment Name'");
            Wait.UntilElementVisible(AssessmentNameTextbox).SetText(assessmentName);
        }

        public string GetAssessmentName()
        {
            Log.Step(GetType().Name, "Get the 'Assessment Name'");
            return Wait.UntilElementVisible(AssessmentNameTextbox).GetText();
        }
        public string GetAssessmentNameTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Assessment Name tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentNameTooltipIcon));
            return Wait.UntilElementVisible(TooltipMessage).GetText().Replace("\n", "").Replace("\r", "").Trim();
        }

        public string GetSplitMeetingStakeholderLaunchDateTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Split Meeting Stakeholder Launch Date tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(SplitMeetingStakeholderLaunchDate)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetSplitMeetingTeamMemberLaunchDateTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Split Meeting TeamMember Launch Date tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(SplitMeetingTeamMemberLaunchDate)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetSplitMeetingAssessmentCloseDateTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Split Meeting Assessment Close Date tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(SplitMeetingAssessmentCloseDate)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetSplitMeetingRetrospectiveWindowStartTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Split Meeting Retrospective Window Start tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(SplitMeetingRetrospectiveWindowStart)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetSplitMeetingRetrospectiveWindowEndTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Split Meeting Retrospective Window End tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(SplitMeetingRetrospectiveWindowEnd)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetOneMeetingStakeholderWindowStartTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over One Meeting Stakeholder Window Start tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(OneMeetingStakeholderWindowStart)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetOneMeetingStakeholderWindowEndTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over One Meeting Stakeholder Window End tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(OneMeetingStakeholderWindowEnd)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetOneMeetingRetrospectiveWindowStartTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over One Meeting Retrospective Window Start tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(OneMeetingRetrospectiveWindowStart)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetOneMeetingTeamMemberLaunchDateTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over One Meeting TeamMember Launch Date tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(OneMeetingTeamMemberLaunchDate)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetOneMeetingRetrospectiveWindowEndTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over One Meeting Retrospective Window End tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(OneMeetingRetrospectiveWindowEnd)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetRetroOnlyAssessmentStartDateTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Retro Only Assessment Start Date tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(RetroOnlyAssessmentStartDate)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetRetroOnlyAssessmentCloseDateTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Retro Only Assessment Close Date tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(RetroOnlyAssessmentCloseDate)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetRetroOnlyRetrospectiveWindowStartTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Retro Only Retrospective Window Start tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(RetroOnlyRetrospectiveWindowStart)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetRetroOnlyRetrospectiveWindowEndTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Retro Only Retrospective Window End tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineDatesTooltipIcon(RetroOnlyRetrospectiveWindowEnd)));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetSplitMeetingTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Split Meeting tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(SplitMeetingTooltipIcon));
            Wait.HardWait(2000);
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetOneMeetingTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over One Meeting tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(OneMeetingTooltipIcon));
            Wait.HardWait(2000);
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetRetroOnlyTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Retro Only tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(RetroOnlyTooltipIcon));
            Wait.HardWait(2000);
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetAssessmentTimelineTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Assessment Timeline tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(AssessmentTimelineTooltipIcon));
            Wait.HardWait(2000);
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetAssessmentNameValidationMessage()
        {
            Log.Step(GetType().Name, "Get 'Assessment Name' validation message");
            Wait.UntilElementVisible(AssessmentNameTextbox).SetText("").SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(AssessmentNameValidationMessage).GetText();
        }

        public void ClickOnSplitMeetingRadioButton()
        {
            Log.Step(GetType().Name, "Click on 'Split Meeting' radio button");
            Wait.HardWait(2000);
            Wait.UntilElementClickable(SplitMeetingRadioButton).Click();
        }

        public void ClickOnOneMeetingRadioButton()
        {
            Log.Step(GetType().Name, "Click on 'One Meeting' radio button");
            Wait.UntilElementClickable(OneMeetingRadioButton).Click();
        }

        public void ClickOnRetroOnlyRadioButton()
        {
            Log.Step(GetType().Name, "Click on 'Retro Only' radio button");
            Wait.UntilElementVisible(RetroOnlyRadioButton).Click();
        }

        public string GetCampaignStartsDate()
        {
            Log.Step(GetType().Name, "Get Campaign Starts Date");
            return Wait.UntilElementVisible(CampaignStartsDate).GetText().Replace("Campaign Starts ", "");
        }

        public string GetCampaignEndsDate()
        {
            Log.Step(GetType().Name, "Get Campaign Ends Date");
            return Wait.UntilElementVisible(CampaignEndsDate).GetText().Replace("Campaign Ends ", "");
        }

        public List<string> GetAssessmentTimelineLabelsList()
        {
            Log.Step(GetType().Name, "Get list of all the Assessment Timeline Labels");
            return Driver.GetTextFromAllElements(AssessmentTimelineLabels).ToList();
        }


        public void SetSplitMeetingDates(DateTime stakeholderLaunchDate, DateTime teamMemberLaunchDate, DateTime assessmentCloseDate, DateTime retrospectiveWindowStart, DateTime retrospectiveWindowEnd)
        {
            Log.Step(GetType().Name, "Select values for 'Split Meeting' Dates");
            if (stakeholderLaunchDate.CompareTo(new DateTime()) == 0) return;
            var calendar = new V2Calendar(Driver, SplitMeetingStakeholderLaunchDate);
            calendar.CampaignSetDateAndTime(stakeholderLaunchDate);

            if (teamMemberLaunchDate.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, SplitMeetingTeamMemberLaunchDate);
            calendar.CampaignSetDateAndTime(teamMemberLaunchDate);

            if (assessmentCloseDate.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, SplitMeetingAssessmentCloseDate);
            calendar.CampaignSetDateAndTime(assessmentCloseDate);

            if (retrospectiveWindowStart.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, SplitMeetingRetrospectiveWindowStart);
            calendar.CampaignSetDateAndTime(retrospectiveWindowStart, false);

            if (retrospectiveWindowEnd.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, SplitMeetingRetrospectiveWindowEnd);
            calendar.CampaignSetDateAndTime(retrospectiveWindowEnd, false);
        }

        public void SetOneMeetingDates(DateTime stakeholderWindowStart, DateTime stakeholderWindowEnd, DateTime retrospectiveWindowStart, DateTime teamMemberLaunchDate, DateTime retrospectiveWindowEnd)
        {
            Log.Step(GetType().Name, "Select values for 'Split Meeting' Dates");
            if (stakeholderWindowStart.CompareTo(new DateTime()) == 0) return;
            var calendar = new V2Calendar(Driver, OneMeetingStakeholderWindowStart);
            calendar.CampaignSetDateAndTime(stakeholderWindowStart);

            if (stakeholderWindowEnd.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, OneMeetingStakeholderWindowEnd);
            calendar.CampaignSetDateAndTime(stakeholderWindowEnd);

            if (retrospectiveWindowStart.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, OneMeetingRetrospectiveWindowStart);
            calendar.CampaignSetDateAndTime(retrospectiveWindowStart, false);

            if (teamMemberLaunchDate.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, OneMeetingTeamMemberLaunchDate);
            calendar.CampaignSetDateAndTime(teamMemberLaunchDate);

            if (retrospectiveWindowEnd.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, OneMeetingRetrospectiveWindowEnd);
            calendar.CampaignSetDateAndTime(retrospectiveWindowEnd, false);
        }

        public void SetRetroOnlyDates(DateTime assessmentStartDate, DateTime assessmentCloseDate, DateTime retrospectiveWindowStart, DateTime retrospectiveWindowEnd)
        {
            Log.Step(GetType().Name, "Select values for 'Split Meeting' Dates");
            if (assessmentStartDate.CompareTo(new DateTime()) == 0) return;
            var calendar = new V2Calendar(Driver, RetroOnlyAssessmentStartDate);
            calendar.CampaignSetDateAndTime(assessmentStartDate);

            if (assessmentCloseDate.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, RetroOnlyAssessmentCloseDate);
            calendar.CampaignSetDateAndTime(assessmentCloseDate);

            if (retrospectiveWindowStart.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, RetroOnlyRetrospectiveWindowStart);
            calendar.CampaignSetDateAndTime(retrospectiveWindowStart, false);

            if (retrospectiveWindowEnd.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, RetroOnlyRetrospectiveWindowEnd);
            calendar.CampaignSetDateAndTime(retrospectiveWindowEnd, false);
        }

        public List<DateTime> GetSplitMeetingDates()
        {
            Log.Step(GetType().Name, "Get values for 'Split Meeting' Dates");
            const string format = "MM/dd/yyyy";
            var stakeholderLaunchDate = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(SplitMeetingStakeholderLaunchDate)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var teamMemberLaunchDate = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(SplitMeetingTeamMemberLaunchDate)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var assessmentCloseDate = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(SplitMeetingAssessmentCloseDate)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var retrospectiveWindowStart = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(SplitMeetingRetrospectiveWindowStart)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var retrospectiveWindowEnd = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(SplitMeetingRetrospectiveWindowEnd)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);

            return new List<DateTime>()
            {
               stakeholderLaunchDate,teamMemberLaunchDate,assessmentCloseDate,retrospectiveWindowStart,retrospectiveWindowEnd
            };
        }

        public List<DateTime> GetOneMeetingDates()
        {
            Log.Step(GetType().Name, "Get values for 'One Meeting' Dates");
            const string format = "MM/dd/yyyy";
            var stakeholderWindowStart = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(OneMeetingStakeholderWindowStart)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var stakeholderWindowEnd = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(OneMeetingStakeholderWindowEnd)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var retrospectiveWindowStart = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(OneMeetingRetrospectiveWindowStart)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var teamMemberLaunchDate = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(OneMeetingTeamMemberLaunchDate)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var retrospectiveWindowEnd = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(OneMeetingRetrospectiveWindowEnd)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);

            return new List<DateTime>()
            {
                stakeholderWindowStart,stakeholderWindowEnd,retrospectiveWindowStart,teamMemberLaunchDate,retrospectiveWindowEnd
            };
        }

        public List<DateTime> GetRetroOnlyDates()
        {
            Log.Step(GetType().Name, "Get values for 'Retro Only' Dates");
            const string format = "MM/dd/yyyy";
            var assessmentStartDate = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(RetroOnlyAssessmentStartDate)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var assessmentCloseDate = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(RetroOnlyAssessmentCloseDate)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var retrospectiveWindowStart = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(RetroOnlyRetrospectiveWindowStart)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);
            var retrospectiveWindowEnd = DateTime.ParseExact(Wait.UntilElementVisible(MeetingDate(RetroOnlyRetrospectiveWindowEnd)).GetText().Split(' ')
                .FirstOrDefault(), format, CultureInfo.InvariantCulture);

            return new List<DateTime>()
            {
                assessmentStartDate,assessmentCloseDate,retrospectiveWindowStart,retrospectiveWindowEnd
            };
        }

        public FacilitationApproach GetSelectedFacilitationApproach()
        {
            Log.Step(GetType().Name, "Get selected Facilitation approach radio button");
            return default;
        }

        public void EnterSetupAssessmentsInfo(SetupAssessmentsDetails assessmentDetails)
        {
            Log.Step(GetType().Name, "Enter Campaigns Details information");
            EnterAssessmentName(assessmentDetails.Name);
            switch (assessmentDetails.FacilitationApproach)
            {
                case FacilitationApproach.SplitMeeting:
                    ClickOnSplitMeetingRadioButton();
                    SetSplitMeetingDates(assessmentDetails.StakeholderLaunchDate, assessmentDetails.TeamMemberLaunchDate, assessmentDetails.AssessmentCloseDate, assessmentDetails.RetrospectiveWindowStart, assessmentDetails.RetrospectiveWindowEnd);
                    break;

                case FacilitationApproach.OneMeeting:
                    ClickOnOneMeetingRadioButton();
                    SetOneMeetingDates(assessmentDetails.StakeholderWindowStart, assessmentDetails.StakeholderWindowEnd, assessmentDetails.RetrospectiveWindowStart, assessmentDetails.TeamMemberLaunchDateForOneMeeting, assessmentDetails.RetrospectiveWindowEnd);
                    break;

                case FacilitationApproach.RetroOnly:
                    ClickOnRetroOnlyRadioButton();
                    SetRetroOnlyDates(assessmentDetails.AssessmentStartDate, assessmentDetails.AssessmentCloseDate, assessmentDetails.RetrospectiveWindowStart, assessmentDetails.RetrospectiveWindowEnd);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(assessmentDetails.FacilitationApproach), assessmentDetails.FacilitationApproach, null);
            }
        }

        public SetupAssessmentsDetails GetSetupAssessmentsInfo()
        {
            Log.Step(GetType().Name, "Get Campaigns Details information");
            var approach = GetSelectedFacilitationApproach();
            var previousYearStart = new DateTime(DateTime.Now.Year - 1, 1, 1);
            switch (approach)
            {
                case FacilitationApproach.SplitMeeting:
                    {
                        var meetingDetails = GetSplitMeetingDates();
                        return new SetupAssessmentsDetails
                        {
                            Name = GetAssessmentName(),
                            FacilitationApproach = approach,
                            StakeholderLaunchDate = meetingDetails[0],
                            TeamMemberLaunchDate = meetingDetails[1],
                            AssessmentCloseDate = meetingDetails[2],
                            RetrospectiveWindowStart = meetingDetails[3],
                            RetrospectiveWindowEnd = meetingDetails[4],
                            AssessmentStartDate = previousYearStart,
                            StakeholderWindowEnd = previousYearStart,
                            StakeholderWindowStart = previousYearStart
                        };
                    }
                case FacilitationApproach.OneMeeting:
                    {
                        var meetingDetails = GetOneMeetingDates();

                        return new SetupAssessmentsDetails
                        {
                            Name = GetAssessmentName(),
                            FacilitationApproach = approach,
                            StakeholderWindowStart = meetingDetails[0],
                            StakeholderWindowEnd = meetingDetails[1],
                            RetrospectiveWindowStart = meetingDetails[2],
                            TeamMemberLaunchDate = meetingDetails[3],
                            RetrospectiveWindowEnd = meetingDetails[4],
                            AssessmentCloseDate = previousYearStart,
                            AssessmentStartDate = previousYearStart,
                            StakeholderLaunchDate = previousYearStart
                        };
                    }
                case FacilitationApproach.RetroOnly:
                    {
                        var meetingDetails = GetRetroOnlyDates();

                        return new SetupAssessmentsDetails
                        {
                            Name = GetAssessmentName(),
                            FacilitationApproach = approach,
                            AssessmentStartDate = meetingDetails[0],
                            AssessmentCloseDate = meetingDetails[1],
                            RetrospectiveWindowStart = meetingDetails[2],
                            RetrospectiveWindowEnd = meetingDetails[3],
                            StakeholderLaunchDate = previousYearStart,
                            StakeholderWindowEnd = previousYearStart,
                            StakeholderWindowStart = previousYearStart,
                            TeamMemberLaunchDate = previousYearStart
                        };
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}