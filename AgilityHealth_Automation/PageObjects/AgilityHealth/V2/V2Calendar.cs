using System;
using System.Globalization;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.V2
{
    internal class V2Calendar : BasePage
    {
        public V2Calendar(IWebDriver driver, string inputId) : base(driver)
        {
            InputId = inputId;
        }

        public string InputId { get; set; }

        // Locators

        private const string CalendarId = "dateTimePickerOpen";
        private readonly By Calendar = AutomationId.Equals(CalendarId);
        private By CalendarOpenButton => By.XPath($"//div[@automation-id='{InputId}']//*[local-name() = 'svg'][@data-testid='CalendarIcon']");

        private readonly By CalendarMonth = By.XPath(
            $"//div[@automation-id = '{CalendarId}']//div[@role='presentation']//div//div");
        private readonly By CalendarPreviousButton = By.XPath(
            $"//div[@automation-id = '{CalendarId}']//button[@title='Previous month']");
        private readonly By CalendarNextButton = By.XPath(
            $"//div[@automation-id = '{CalendarId}']//button[@title='Next month']");

        private static By CalendarDateItem(int day) => By.XPath(
            $"//div[@automation-id = '{CalendarId}']//div[@role='cell']//button[contains(normalize-space(),'{day}')]");

        private static By CalendarTimeItem(string value) => By.XPath(
            $"//div[@automation-id = '{CalendarId}']//span[contains(normalize-space(),'{value}')]");

        private static By CalendarAmPmButton(string aMpM) => By.XPath(
            $"//div[@automation-id = '{CalendarId}']//button//span[contains(normalize-space(),'{aMpM}')]");
        private readonly By ClockTimePointer =
            By.XPath($"//div[@automation-id = '{CalendarId}']//div[@class='css-axpink'] | //div[@automation-id = '{CalendarId}']//div[@class='css-vekjcw'] | //div[@automation-id = '{CalendarId}']//div[@class='css-hehdit']");

        private readonly By PopUpOkButton =
            By.XPath($"//div[contains(@class,'MuiDialog-paperScrollPaper')]//button");

        // Methods

        private DateTime GetCurrentMonth()
        {
            return DateTime.Parse(Wait.UntilElementVisible(CalendarMonth).GetText());
        }


        private void OpenCalendar()
        {
            Wait.UntilJavaScriptReady();
            for (var i = 0; i < 5; i++)
            {
                Wait.UntilElementClickable(CalendarOpenButton).Click();
                Wait.UntilJavaScriptReady();
                if (Wait.UntilElementExists(CalendarNextButton).Displayed) return;
            }

            throw new Exception("The Calendar did not open");

        }

        internal void SetDateAndTime(DateTime date)
        {
            OpenCalendar();

            // set date
            SetDate(date);

            // set time
            SetTime(date);
            Wait.UntilElementNotExist(Calendar);
        }

        internal void CampaignSetDateAndTime(DateTime date, bool setTime = true)
        {
            OpenCalendar();
            // set date
            SetDate(date);

            if (Driver.FindElements(PopUpOkButton).Count > 0)
            {
                Wait.UntilElementClickable(PopUpOkButton).Click();
                Wait.HardWait(2000);
                OpenCalendar();
                SetDate(date);
            }

            if (setTime)
            {
                // set time
                SetTime(date);
            }
            Wait.UntilElementNotExist(Calendar);
        }

        private void SetDate(DateTime date)
        {
            // set calendar to the correct month/year
            var currentCalendar = GetCurrentMonth();
            if (currentCalendar.Month != date.Month || currentCalendar.Year != date.Year)
            {
                var monthDifference = (date.Year - currentCalendar.Year) * 12 + date.Month - currentCalendar.Month;

                for (var i = 0; i < Math.Abs(monthDifference); i++)
                {
                    if (monthDifference > 0)
                    {
                        Wait.UntilElementClickable(CalendarNextButton).Click();
                        Wait.UntilJavaScriptReady();
                    }
                    else if (monthDifference < 0)
                    {
                        Wait.UntilElementClickable(CalendarPreviousButton).Click();
                        Wait.UntilJavaScriptReady();
                    }
                }

            }

            // set date
            Wait.UntilElementClickable(CalendarDateItem(date.Day)).Click();
            Wait.UntilJavaScriptReady();
        }

        private void SetTime(DateTime time)
        {
            Wait.UntilElementClickable(CalendarAmPmButton(
                time.ToString("tt", CultureInfo.InvariantCulture))).Click();
            var actions = new Actions(Driver);
            actions.MoveToElement(Wait.UntilElementClickable(CalendarTimeItem(
                    time.ToString("%h", CultureInfo.InvariantCulture)))).Click().Build().Perform();
            if (time.Minute % 5 != 0)
                throw new Exception($"<{time.Minute}> minute value must be a multiple of 5.");

            var drag = Wait.UntilElementExists(ClockTimePointer);
            var drop = Wait.UntilElementExists(CalendarTimeItem(time.ToString("mm", CultureInfo.InvariantCulture)));
            Driver.DragElementToElement(drag, drop);

        }

    }
}