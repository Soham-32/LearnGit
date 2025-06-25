using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base
{
    public class PulseCheckBasePage : BasePage
    {
        public PulseCheckBasePage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators 
        //Headers
        private readonly By CreatePulseCheckTitle = By.XPath("//button[@automation-id='stepNext']//parent::div//parent::div//preceding-sibling::div//p[contains(normalize-space(),'Create Pulse Check')]");

        //Assessment Locators
        private readonly By AssessmentNameTextBox = By.Id("assessmentName");
        private readonly By AssessmentNameErrorMessage = By.XPath("//*[@automation-id = 'assessmentName']/following-sibling::p");

        //Scheduler Locators
        private const string StartDateTimePickerInputId = "startDateTime";
        private readonly By StartDateTimePickerInput = AutomationId.Equals("startDateTime", "input");
        private readonly By AssessmentPeriodDropDown = AutomationId.Equals("periodName", "div");
        private static By AssessmentPeriodItem(string item) => By.XPath($"//div[@id = 'menu-periodName']//li[text() = '{item}']");
        private readonly By AssessmentPeriodItemsList = By.XPath("//div[@id = 'menu-periodName']//li");
        private readonly By RepeatIntervalDropDown = AutomationId.Equals("repeatInterval", "div");
        private static By RepeatIntervalDropDownItem(string item) => By.XPath($"//div[@id = 'menu-repeatInterval']//li[contains(normalize-space(), '{item}')]");
        private readonly By RepeatIntervalDropDownItemsList = By.XPath("//div[@id = 'menu-repeatInterval']//li");
        private readonly By RepeatIntervalEndsNeverRadioButton = By.XPath("//span[text()='Never']");
        private readonly By RepeatIntervalEndsOnRadioButton = By.XPath("//span[text()='On']");
        private readonly By RepeatIntervalEndsOnDateTextBox = By.XPath("//input[@name='repeatEnds']/following::input[contains(@id,'mui')]");
        private readonly By RepeatIntervalEndsAfterRadioButton = By.XPath("//span[text()='After']");
        private static By RepeatIntervalEndsRadioButton(string interval) => By.XPath($"//span[text()='{interval}']//preceding-sibling::span//input");
        private readonly By RepeatIntervalEndsAfterOccurrencesTextBox = By.Id("repeatIntervalOccurrenceNumber");
        private readonly By EndsSection = By.XPath("//label[text() = 'Ends']");

        //Methods
        //Headers
        public bool IsCreatePulseCheckTitleDisplayed()
        {
            return Driver.IsElementDisplayed(CreatePulseCheckTitle);
        }

        //Assessment
        public string GetAssessmentNameErrorMessage()
        {
            Log.Step(GetType().Name, "Get Assessment name 'Error Message'");
            return Wait.InCase(AssessmentNameErrorMessage)?.GetText() ?? "";
        }

        public string GetAssessmentName()
        {
            Log.Step(GetType().Name, "Get Assessment name");
            return Wait.UntilElementVisible(AssessmentNameTextBox).GetText();
        }

        public void AddAssessmentName(string assessmentName)
        {
            Log.Step(GetType().Name, $"Add pulse assessment name'{assessmentName}'");
            Wait.UntilElementVisible(AssessmentNameTextBox).SetText(assessmentName).SendKeys(Keys.Tab);
        }

        //Schedulers
        public void FillSchedulerInfo(SavePulseAssessmentV2Request request)
        {
            Log.Step(GetType().Name, "Select values for 'Start Date/Time', 'Assessment Period', 'Repeat Interval'");
            SetStartDate(request.StartDate);
            SelectPeriod(((AssessmentPeriod)request.PeriodId).GetDescription());
            SelectRepeatInterval((RepeatIntervalId)request.RepeatIntervalId, request.StartDate);

            SetRepeatIntervalEnd(request);
        }

        // TODO use the one that it takes a SavePulseAssessmentRequest as a parameter
        public void FillSchedulerInfo(AtCommon.Dtos.Assessments.PulseV2.Custom.PulseAssessmentV2 assessmentInfo)
        {
            Log.Step(GetType().Name, "Select values for 'Start Date/Time', 'Assessment Period', 'Repeat Interval'");
            // SetStartDate(assessmentInfo.StartDate); commenting this line because we can not enter time of past.
            SelectRepeatInterval(assessmentInfo.RepeatInterval.Type);
            SelectPeriod(assessmentInfo.Period);

            if (assessmentInfo.RepeatInterval.Type.Equals("Does not repeat")) return;

            switch (assessmentInfo.RepeatInterval.Ends)
            {
                case End.Never:
                    Wait.UntilElementClickable(RepeatIntervalEndsNeverRadioButton).Click();
                    break;
                case End.OnDate:
                    Wait.UntilElementClickable(RepeatIntervalEndsOnRadioButton).Click();
                    Wait.UntilElementClickable(RepeatIntervalEndsOnDateTextBox).SetText(assessmentInfo.EndDate.ToString("MM/dd/yyyy"), isReact: true);
                    break;
                case End.AfterOccurrences:
                    Driver.JavaScriptScrollToElement(RepeatIntervalEndsAfterRadioButton);
                    Wait.UntilElementClickable(RepeatIntervalEndsAfterRadioButton).Click();
                    for (var i = 0; i < assessmentInfo.NumberOfOccurrences - 1; i++)
                    {
                        Wait.UntilElementClickable(RepeatIntervalEndsAfterOccurrencesTextBox).KeyUp();
                    }
                    break;
                case End.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetStartDate(DateTime startDate)
        {
            Log.Step(GetType().Name, "Select values for 'Start Date/Time'");
            IsStartDateTimeFieldEnabled();
            if (startDate.CompareTo(new DateTime()) == 0) return;
            var calendar = new V2Calendar(Driver, StartDateTimePickerInputId);
            calendar.SetDateAndTime(startDate);
        }

        public void SelectRepeatInterval(string repeatInterval)
        {
            Log.Step(GetType().Name, "Select values for 'Repeat Interval'");
            if (GetRepeatInterval() == repeatInterval) return;
            SelectItem(RepeatIntervalDropDown, RepeatIntervalDropDownItem(repeatInterval));
        }

        public void SelectRepeatInterval(RepeatIntervalId repeatIntervalId, DateTime startDate)
        {
            Log.Step(GetType().Name, "Select values for 'Repeat Interval' with 'Start date'");
            var interval = repeatIntervalId.GetDescription();
            if (repeatIntervalId == RepeatIntervalId.BiMonthly || repeatIntervalId == RepeatIntervalId.Monthly ||
                repeatIntervalId == RepeatIntervalId.Quarterly || repeatIntervalId == RepeatIntervalId.Weekly)
                interval += startDate.ToString("dddd");
            SelectRepeatInterval(interval);
        }

        public void SelectPeriod(string period)
        {
            Log.Step(GetType().Name, "Select values for 'Assessment Period'");
            if (GetPeriod() == period) return;
            SelectItem(AssessmentPeriodDropDown, AssessmentPeriodItem(period));
        }

        public void SetRepeatIntervalEnd(SavePulseAssessmentV2Request request)
        {
            Log.Step(GetType().Name, "Select values for 'Repeat interval end'");
            if (request.RepeatIntervalId == (int)RepeatIntervalId.Never) return;
            var ends = (End)request.RepeatEndStrategyId;

            switch (ends)
            {
                case End.Never:
                    Wait.UntilElementClickable(RepeatIntervalEndsNeverRadioButton).Click();
                    break;
                case End.OnDate:
                    Wait.UntilElementClickable(RepeatIntervalEndsOnRadioButton).Click();
                    Wait.UntilElementClickable(RepeatIntervalEndsOnDateTextBox).SetText(request.EndDate?.ToString("MM/dd/yyyy"), isReact: true);
                    break;
                case End.AfterOccurrences:
                    Wait.UntilElementClickable(RepeatIntervalEndsAfterRadioButton).Click();
                    var startOccurrences = Wait.UntilElementVisible(RepeatIntervalEndsAfterOccurrencesTextBox).GetText();
                    var keystrokes = request.RepeatOccurrenceNumber - startOccurrences.ToInt();
                    for (var i = 0; i < keystrokes; i++)
                    {
                        Wait.UntilElementClickable(RepeatIntervalEndsAfterOccurrencesTextBox).KeyUp();
                    }
                    break;
                case End.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public AtCommon.Dtos.Assessments.PulseV2.Custom.PulseAssessmentV2 GetScheduleSectionData()
        {
            Log.Step(GetType().Name, "Get 'Start Date/Time', 'Assessment Period', 'Repeat Interval' values");
            var startDateTime = DateTime.Parse(Wait.UntilElementVisible(StartDateTimePickerInput).GetText());
            var assessmentPeriod = GetPeriod();
            var assessmentRepeatIntervalType = GetRepeatInterval();


            var endsDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            var occurrences = "0";
            var endsType = End.Never;

            if (assessmentRepeatIntervalType == "Does not repeat")
                return new AtCommon.Dtos.Assessments.PulseV2.Custom.PulseAssessmentV2
                {
                    StartDate = startDateTime,
                    Period = assessmentPeriod,
                    RepeatInterval = new RepeatIntervals()
                    {
                        Type = assessmentRepeatIntervalType,
                        Ends = endsType
                    },
                    EndDate = DateTime.Parse(endsDate),
                    NumberOfOccurrences = int.Parse(occurrences)
                };
            if (Wait.UntilElementExists(RepeatIntervalEndsRadioButton("Never")).Selected)
            {
                endsType = End.Never;
            }
            else if (Wait.UntilElementExists(RepeatIntervalEndsRadioButton("On")).Selected)
            {
                endsType = End.OnDate;
                endsDate = Wait.UntilElementVisible(RepeatIntervalEndsOnDateTextBox).GetText();
            }
            else if (Wait.UntilElementExists(RepeatIntervalEndsRadioButton("After")).Selected)
            {
                endsType = End.AfterOccurrences;
                occurrences = Wait.UntilElementVisible(RepeatIntervalEndsAfterOccurrencesTextBox).GetText();
            }

            return new AtCommon.Dtos.Assessments.PulseV2.Custom.PulseAssessmentV2
            {
                StartDate = startDateTime,
                Period = assessmentPeriod,
                RepeatInterval = new RepeatIntervals()
                {
                    Type = assessmentRepeatIntervalType,
                    Ends = endsType
                },
                EndDate = DateTime.Parse(endsDate),
                NumberOfOccurrences = int.Parse(occurrences)
            };
        }

        public string GetPeriod()
        {
            Log.Step(GetType().Name, "Get 'Assessment Period' values");
            return Wait.UntilElementVisible(AssessmentPeriodDropDown).GetText();
        }

        public string GetRepeatInterval()
        {
            Log.Step(GetType().Name, "Get 'Repeat Interval' values");
            return Wait.UntilElementVisible(RepeatIntervalDropDown).GetText();
        }

        public bool IsStartDateTimeFieldEnabled()
        {
            return Driver.IsElementEnabled(StartDateTimePickerInput);
        }

        public bool IsEndsSectionDisplayed()
        {
            return Driver.IsElementDisplayed(EndsSection);
        }

        public IList<string> GetAllAssessmentPeriodListItems()
        {
            Log.Step(GetType().Name, "Get list of 'AssessmentPeriod' Items");
            Wait.UntilElementClickable(AssessmentPeriodDropDown).Click();
            Wait.HardWait(2000);// Need to give sometime to get data
            var list = Driver.GetTextFromAllElements(AssessmentPeriodItemsList);
            Wait.UntilAllElementsLocated(AssessmentPeriodItemsList)[0].Click();
            return list;
        }

        public IList<string> GetAllRepeatIntervalListItems()
        {
            Log.Step(GetType().Name, "Get list of 'RepeatInterval' Items");
            Wait.UntilElementClickable(RepeatIntervalDropDown).Click();
            Wait.HardWait(2000);// Need to give sometime to get data
            var list = Driver.GetTextFromAllElements(RepeatIntervalDropDownItemsList);
            Wait.UntilAllElementsLocated(RepeatIntervalDropDownItemsList)[0].Click();
            return list;
        }

    }
}