using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create
{
    internal class AssessmentProfilePage : BasePage
    {
        public AssessmentProfilePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AssessmentName = By.Id("Name");
        private readonly By AssessmentTypeDropDown = By.CssSelector("span[aria-owns='SurveyId_listbox']");
        private static By DynamicAssessmentType(string assessmentType) =>
            By.XPath($"//ul[@id = 'SurveyId_listbox']/li//font[text()='{assessmentType}'] | //ul[@id = 'SurveyId_listbox']/li[text() = '{assessmentType}']");

        private readonly By FacilitatorDropDown = By.CssSelector("input[aria-owns='Facilitator_taglist Facilitator_listbox']");
        private static By FacilitatorListItem(string facilitator) => By.XPath($"//ul[@id = 'Facilitator_listbox']/li[text() = '{facilitator}'] | //ul[@id='Facilitator_listbox']//li[contains(text(),'{facilitator}')]");

        private const string FacilitationDateCalendarId = "FacilitationDate_dateview";
        private const string FacilitationDateTimeId = "FacilitationDate_timeview";
        private readonly By FacilitationDurationListBox = By.XPath("//select[@id = 'ddlDuration']/preceding-sibling::span");
        private By FacilitationDurationListItem(string item) => By.XPath($"//ul[@id = 'ddlDuration_listbox']/li[text() = '{item}']");

        private const string LeadershipReadOutDateCalendarId = "LeadershipReadoutDate_dateview";
        private const string LeadershipReadOutDateTimeId = "LeadershipReadoutDate_timeview";
        private readonly By LocationTextbox = By.Id("Location");
        private readonly By NextSelectTeamMemberButton = By.CssSelector("input[type='submit']");
        private readonly By CampaignDropDown = By.CssSelector("span[aria-owns='CampaignId_listbox']");
        private By CampaignListBox(string item) => By.XPath($"//ul[@id='CampaignId_listbox']/li[contains(normalize-space(),'{item}')]");
        internal readonly By AssessmentCampaignFieldValidationMessage = By.Id("campaign_error_message");

        public void WaitForAssessmentProfilePageToLoad()
        {
            Wait.UntilElementVisible(AssessmentTypeDropDown);
        }
        public void FillDataForAssessmentProfile(string assessmentType, string name)
        {
            Log.Step(nameof(AssessmentProfilePage), $"Fill assessment type and name for assessment profile type = <{assessmentType}> and name = <{name}>");
            Wait.UntilElementVisible(AssessmentName).SetText(name);
            SelectItem(AssessmentTypeDropDown, DynamicAssessmentType(assessmentType));
            if (Driver.IsElementDisplayed(CampaignDropDown))
            {
                SelectItem(CampaignDropDown, CampaignListBox(Constants.AtCampaign));
            }
        }

        public void FillDataForAssessmentProfile(TeamAssessmentInfo assessmentInfo)
        {
            Log.Step(nameof(AssessmentProfilePage), "Fill data for assessment detail");
            Wait.HardWait(3000);//Assessment dropdown takes time to load
            SelectItem(AssessmentTypeDropDown, DynamicAssessmentType(assessmentInfo.AssessmentType));

            Wait.UntilElementVisible(AssessmentName).SetText(assessmentInfo.AssessmentName);

            if (!string.IsNullOrEmpty(assessmentInfo.Facilitator))
            {
                Wait.HardWait(3000);//Time to load dropdown
                SelectItem(FacilitatorDropDown, FacilitatorListItem(assessmentInfo.Facilitator));
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
                string durationListItemText = assessmentInfo.FacilitationDate.AddHours(assessmentInfo.FacilitationDuration).ToString("hh:mm tt") +
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

            if (Driver.IsElementDisplayed(CampaignDropDown))
            {
                if (!string.IsNullOrEmpty(assessmentInfo.Campaign))
                {
                    SelectItem(CampaignDropDown, CampaignListBox(assessmentInfo.Campaign));
                }
            }

        }
        public void ClickOnNextSelectTeamMemberButton()
        {
            Log.Step(nameof(AssessmentProfilePage), "Click on Next, Select Team Member button");
            Wait.UntilElementClickable(NextSelectTeamMemberButton).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }

        public bool DoesCampaignFieldDisplay()
        {
            var campaign = Wait.InCase(CampaignDropDown);
            if (campaign != null)
            {
                return campaign.Displayed;
            }

            return false;
        }
        public string GetAssessmentCampaignFieldValidationMessage()
        {
            return Wait.UntilElementVisible(AssessmentCampaignFieldValidationMessage).GetText();
        }
        public void NavigateToPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/{teamId}/assessments/create", AssessmentName);
        }
    }
}