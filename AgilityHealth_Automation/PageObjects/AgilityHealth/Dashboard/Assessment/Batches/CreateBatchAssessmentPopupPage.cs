using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches
{
    internal class CreateBatchAssessmentPopupPage : BatchesTabPage
    {
        public CreateBatchAssessmentPopupPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By BatchNameTextBox = By.Id("BatchName");
        private readonly By AssessmentNameTextBox = By.Id("Assessment");
        private readonly By AssessmentTypeDropdown = By.CssSelector("span[aria-owns='ddlSurveyTypeForBatch_listbox']");
        private const string StartDateCalendarId = "StartDateForBatch_dateview";
        private readonly By StartDateTimeListbox = By.CssSelector("span[aria-controls = 'startTimeForBatch_timeview']");
        private static By StartDateTimeListItem(string item) => By.XPath($"//ul[@id = 'startTimeForBatch_timeview']/li[text() = '{item}']");
        private const string EndDateCalendarId = "EndDateForBatch_dateview";
        private readonly By ScheduleButton = By.Id("saveBatchAssessmentSchedule");
        private readonly By SendAndLaunchNowButton = By.Id("saveBatchAssessment");
        private readonly By ScheduleConfirmationPopup = By.Id("showFilterKanbanViewSchdule");
        private readonly By SendConfirmationPopup = By.Id("showFilterKanbanView");
        private readonly By ScheduleConfirmationPopupSurveyType = By.Id("surveyNameSchedule");
        private readonly By SendConfirmationPopupSurveyType = By.Id("surveyName");
        private readonly By ScheduleConfirmationPopupSelectedTeam = By.Id("selectedTeamCountSchedule");
        private readonly By SendConfirmationPopupSelectedTeam = By.Id("selectedTeamCount");
        private readonly By ConfirmationPopupStartDate = By.Id("DateSchedule");
        private readonly By ScheduleYesProceedButton = By.Id("do_SaveAssessment_schdule");
        private readonly By SendYesProceedButton = By.Id("do_SaveAssessment");
        private readonly By SuccessPopupText = By.CssSelector("div#finalConfirmation>div:first-of-type");
        private readonly By SuccessPopupOkButton = By.Id("do_final_ok_reload");
        private readonly By TeamMembersRadioButton = By.CssSelector("label[for = 'sendTo_TeamMember']");
        private readonly By StakeholdersRadioButton = By.CssSelector("label[for = 'sendTo_Stakeholder']");
        private readonly By TeamMembersAndStakeholdersRadioButton = By.CssSelector("label[for = 'sendTo_TeamMemberAndStakeholder']");

        private static By TeamNameCheckBox(string teamName) => By.XPath($"//div[@id='teamsDetails']//*[text()='{teamName}']//ancestor::tr/td/input | //font[text()='{teamName}']//ancestor::tr/td/input");

        private static By AssessmentTypeItem(string assessmentType) => By.XPath($"//body/div/div[@id='ddlSurveyTypeForBatch-list']//ul[@id='ddlSurveyTypeForBatch_listbox']/li[text()='{assessmentType}'] | //body/div/div[@id='ddlSurveyTypeForBatch-list']//ul[@id='ddlSurveyTypeForBatch_listbox']/li//font[text()='{assessmentType}']");

        //Create or Edit Batch Assessment popup
        private readonly By CreateEditBatchAssessmentPopupColumnHeaders = By.XPath("//div[@id='showCreateEditBatchAssessment']//th[@data-role='columnsorter']/a");
        private static By CreateEditBatchAssessmentPopupColumnValueList(string columnName) => By.XPath($"//div[@id='showCreateEditBatchAssessment']//div[@id='gridWrapper1']//tbody//tr/td[count(//th[@data-role='columnsorter'][@data-title='{columnName}']//preceding-sibling::th) + 1]");
        private readonly By CreateEditBatchAssessmentPopupWorkTypeDropdownValueList = By.XPath("//ul[@id='ddlTeamsForBatchPopUp_listbox']//parent::div[@style]//li");
        private readonly By CreateEditBatchAssessmentPopupWorkTypeDropdown = By.XPath("//span[@aria-owns='ddlTeamsForBatchPopUp_listbox']//span[contains(@class,'k-input')]");
        private readonly By CreateEditBatchAssessmentPopupYesProceedButton = By.Id("do_SaveAssessment");
        private readonly By CreateEditBatchAssessmentPopupAssessmentTypeDropdownValueList = By.XPath("//ul[@id='ddlSurveyTypeForBatch_listbox']/li");

        public void ClickScheduleButton()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Click on Schedule button");
            Wait.UntilElementClickable(ScheduleButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectTeamName(string team, bool check = true)
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), $"Select '{team}' team");
            Driver.JavaScriptScrollToElement(TeamNameCheckBox(team));
            Wait.HardWait(3000);//Need to wait till team is visible
            Wait.UntilElementVisible(TeamNameCheckBox(team)).Check(check);
            if (check)
            {
                if (!Driver.IsElementSelected(TeamNameCheckBox(team)))
                {
                    Wait.UntilElementVisible(TeamNameCheckBox(team)).Check();
                }
            }
            else
            {
                if (Driver.IsElementSelected(TeamNameCheckBox(team)))
                {
                    Wait.UntilElementVisible(TeamNameCheckBox(team)).Check(false);
                }
            }
        }

        public void EnterBatchAssessment(BatchAssessment batchAssessment)
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Enter batch assessment");
            Wait.HardWait(3000); // Need to wait until batch pop up loaded
            Wait.UntilElementVisible(BatchNameTextBox).SetText(batchAssessment.BatchName);
            Wait.UntilElementVisible(AssessmentNameTextBox).SetText(batchAssessment.AssessmentName);
            SelectItem(AssessmentTypeDropdown, AssessmentTypeItem(batchAssessment.AssessmentType));
            foreach (var teamAssessment in batchAssessment.TeamAssessments)
            {
                SelectTeamName(teamAssessment.TeamName);
            }

            if (batchAssessment.StartDate.CompareTo(new DateTime()) != 0)
            {
                var startCal = new CalendarWidget(Driver, StartDateCalendarId);
                startCal.SetDate(batchAssessment.StartDate);

                SelectItem(StartDateTimeListbox, StartDateTimeListItem(batchAssessment.StartDate.ToString("h:mm tt")));

            }
            if (batchAssessment.EndDate.CompareTo(new DateTime()) != 0 && batchAssessment.StartDate.CompareTo(new DateTime()) != 0)
            {
                var endCal = new CalendarWidget(Driver, EndDateCalendarId);
                endCal.SetDate(batchAssessment.EndDate);
            }
        }

        public bool IsScheduleConfirmationPopupDisplayed()
        {
            var popup = Wait.InCase(ScheduleConfirmationPopup);
            return popup?.Displayed ?? false;

        }

        public bool IsSendConfirmationPopupDisplayed()
        {
            var popup = Wait.InCase(SendConfirmationPopup);
            return popup?.Displayed ?? false;
        }


        public string GetSchedulePopupSurveyType()
        {
            return Wait.UntilElementExists(ScheduleConfirmationPopupSurveyType).GetText();
        }

        public string GetSendPopupSurveyType()
        {
            return Wait.UntilElementExists(SendConfirmationPopupSurveyType).GetText();
        }

        public string GetPopupStartDate()
        {
            return Wait.UntilElementExists(ConfirmationPopupStartDate).GetText();
        }

        public int GetSchedulePopupSelectedTeam()
        {
            return Convert.ToInt32(Wait.UntilElementExists(ScheduleConfirmationPopupSelectedTeam).GetText());
        }

        public int GetSendPopupSelectedTeam()
        {
            return Convert.ToInt32(Wait.UntilElementExists(SendConfirmationPopupSelectedTeam).GetText());
        }

        public void ClickScheduleYesProceedButton()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Click on Schedule, Yes Proceed button");
            Wait.UntilElementClickable(ScheduleYesProceedButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickSendYesProceedButton()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Click on Send, Yes Proceed button");
            Wait.UntilElementClickable(SendYesProceedButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetSuccessPopupText()
        {
            return Wait.UntilElementVisible(SuccessPopupText).GetText();
        }

        public void ClickOkButton()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Click on Ok button");
            Wait.UntilElementClickable(SuccessPopupOkButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickSendAndLaunchNowButton()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Click on Send and Launch Now button");
            Wait.UntilElementClickable(SendAndLaunchNowButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SendTo(string option)
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), $"Click on Send to {option}");
            switch (option)
            {
                case "Team Members":
                    Wait.UntilElementClickable(TeamMembersRadioButton).Click();
                    break;
                case "Stakeholders":
                    Wait.UntilElementClickable(StakeholdersRadioButton).Click();
                    break;
                case "Team Members and Stakeholders":
                    Wait.UntilElementClickable(TeamMembersAndStakeholdersRadioButton).Click();
                    break;
                default:
                    throw new Exception($"<{option}> is not valid. Please use 'Team Members', 'Stakeholders', or 'Team Members and Stakeholders'.");
            }
        }

        public List<string> GetCreateEditBatchAssessmentPopupColumnHeadersValueList()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Get all Batch Popup ColumnHeader value List");
            return Wait.UntilAllElementsLocated(CreateEditBatchAssessmentPopupColumnHeaders).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        public List<string> GetCreateEditBatchAssessmentPopupColumnValueList(string columnName)
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Get all the value of Column List");
            return Wait.UntilAllElementsLocated(CreateEditBatchAssessmentPopupColumnValueList(columnName)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        public List<string> GetCreateEditBatchAssessmentPopupWorkTypeValueList()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Get all 'WorkType' value List from Work Type dropdown");
            Wait.UntilElementClickable(CreateEditBatchAssessmentPopupWorkTypeDropdown).Click();
            var getWorkTypeList = Driver.GetTextFromAllElements(CreateEditBatchAssessmentPopupWorkTypeDropdownValueList).ToList();
            Wait.UntilJavaScriptReady();
            return getWorkTypeList;
        }

        public void ClickOnCreateEditBatchAssessmentPopupYesProceedButton()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Click on 'Yes,Proceed' button");
            Wait.UntilElementClickable(CreateEditBatchAssessmentPopupYesProceedButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<string> GetCreateEditBatchAssessmentPopupAssessmentTypeValueList()
        {
            Log.Step(nameof(CreateBatchAssessmentPopupPage), "Get all 'AssessmentType' value from Assessment Type dropdown");
            Wait.UntilElementClickable(AssessmentTypeDropdown).Click();
            var getAssessmentTypeList = Driver.GetTextFromAllElements(CreateEditBatchAssessmentPopupAssessmentTypeDropdownValueList).ToList();
            Wait.UntilJavaScriptReady();
            return getAssessmentTypeList;
        }
    }
}
