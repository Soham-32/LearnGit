using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Scheduler
{
    public class CreateDraftAssessmentPopupPage : SchedulerTabPage
    {
        public CreateDraftAssessmentPopupPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AssessmentTypeListbox = By.CssSelector("span[aria-owns='ddlSurveyType_listbox']");

        private static By AssessmentTypeListItem(string item) =>
            By.XPath($"//ul[@id = 'ddlSurveyType_listbox']/li[text() = '{item}'] | //ul[@id = 'ddlSurveyType_listbox']/li//font[text() = '{item}']");

        private readonly By AssessmentNameTextbox = By.Id("Title");
        private readonly By TeamListbox = By.CssSelector("span[aria-owns='ddlTeams_listbox']");
        private static By TeamListItem(string item) => By.XPath($"//ul[@id = 'ddlTeams_listbox']/li[text() = '{item}'] | //ul[@id = 'ddlTeams_listbox']/li//font[text() = '{item}']");
        private readonly By FacilitatorListbox = By.XPath("//ul[@id = 'ddlFacilitator_taglist']/parent::div");
        private static By FacilitatorsListIem(string item) => By.XPath($"//ul[@id = 'ddlFacilitator_listbox']/li[text() = '{item}'] | //ul[@id = 'ddlFacilitator_listbox']/li//font[text() = '{item}']");
        private readonly By FindFacilitatorCheckbox = By.Id("foundFacilitator");
        private const string FacilitationDate = "startDate_dateview";
        private const string StartTime = "start_timeview";

        private readonly By FacilitationDurationListbox =
            By.CssSelector("span[aria-owns='ddlDurationScheduler_listbox']");
        private static By FacilitationDurationListItem(string item) => By.XPath($"//ul[@id = 'ddlDurationScheduler_listbox']/li[text() = '{item}']");
        private readonly By LocationTextbox = By.Id("Location");
        private readonly By CreateDraftButton = By.CssSelector("a.k-scheduler-update");

        public void EnterAssessmentInfo(TeamAssessmentInfo assessment)
        {
            Log.Step(nameof(CreateDraftAssessmentPopupPage), "Enter assessment info");
            Wait.UntilJavaScriptReady();
            SelectItem(AssessmentTypeListbox, AssessmentTypeListItem(assessment.AssessmentType));
            Wait.UntilElementClickable(AssessmentNameTextbox).SetText(assessment.AssessmentName);
            SelectItem(TeamListbox, TeamListItem(assessment.TeamName));
            
            if (!string.IsNullOrWhiteSpace(assessment.Facilitator))
            {
                SelectItem(FacilitatorListbox, FacilitatorsListIem(assessment.Facilitator));
            }

            if (assessment.FindFacilitator)
            {
                Wait.UntilElementClickable(FindFacilitatorCheckbox).Check();
            }

            if (assessment.FacilitationDate.CompareTo(new DateTime()) != 0)
            {
                var startDateCalendar = new CalendarWidget(Driver, FacilitationDate, StartTime);
                // set date
                startDateCalendar.SetDate(assessment.FacilitationDate);
                // set time 
                startDateCalendar.SetTime(assessment.FacilitationDate);
            }

            if (assessment.FacilitationDuration != 0 && assessment.FacilitationDate.CompareTo(new DateTime()) != 0)
            {
                var durationListItemText = assessment.FacilitationDate.AddHours(assessment.FacilitationDuration).ToString("hh:mm tt") +
                                           $" ({assessment.FacilitationDuration} hours)";
                SelectItem(FacilitationDurationListbox, FacilitationDurationListItem(durationListItemText));
            }

            if (!string.IsNullOrEmpty(assessment.Location))
            {
                Wait.UntilElementClickable(LocationTextbox).SetText(assessment.Location);
            }
        }

        public void ClickCreateDraftAssessmentButton()
        {
            Log.Step(nameof(CreateDraftAssessmentPopupPage), "Click on Create Draft Assessment button");
            Wait.UntilElementClickable(CreateDraftButton).Click();
            Wait.UntilElementNotExist(CreateDraftButton);
            Wait.UntilJavaScriptReady();
        }
    }
}
