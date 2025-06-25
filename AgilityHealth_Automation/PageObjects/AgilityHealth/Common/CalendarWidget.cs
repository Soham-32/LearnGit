using System;
using System.Globalization;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Common
{
    internal class CalendarWidget : BasePage
    {
        private string CalendarId { get; }
        private string TimeId { get; }

        public CalendarWidget(IWebDriver driver, string calendarId, string timeId = "") : base(driver)
        {
            CalendarId = calendarId;
            TimeId = timeId;
        }

        private By CalendarOpenButton => By.XPath($"//span[@aria-controls = '{CalendarId}']");
        private By CalendarDateItem(DateTime date) => By.XPath($"//div[@id='{CalendarId}']//td/a[contains(@title,'{date:dddd, MMMM dd, yyyy}') or contains(@title,'{date:dddd, MMMM d, yyyy}')]");
        private By CalendarMonthItem(DateTime date) => By.XPath($"//div[@id='{CalendarId}']//a[text() = '{date:MMM}'] | //div[@id='{CalendarId}']//a//font[text() = '{date:MMM}']");
        private By CalendarYearItem(DateTime date) => By.XPath($"//div[@id='{CalendarId}']//*[text() = '{date:yyyy}']");
        private By CalendarPreviousButton => By.CssSelector($"div#{CalendarId} a.k-nav-prev");
        private By CalendarNextButton => By.CssSelector($"div#{CalendarId} a.k-nav-next");
        private By CalendarMonthButton => By.CssSelector($"div#{CalendarId} a.k-nav-fast");

        private By TimeOpenButton => By.XPath($"//span[@aria-controls='{TimeId}']");
        private By CalendarTimeItem(DateTime time) => By.XPath($"//ul[@id = '{TimeId}']//*[text() = '{time:h:mm tt}']");
        private readonly By CalenderIcon = By.XPath("//span[@aria-controls='StartDate_dateview']");

        private DateTime GetCurrentMonth()
        {
            if (!Driver.IsElementDisplayed(CalendarMonthButton))
            {
                for (var i = 0; i < 5; i++)
                {
                    OpenCalendar();
                    if (Driver.IsElementDisplayed(CalendarMonthButton)) break;
                }
            }
            Wait.HardWait(3000); // Wait Until Page Load
            return DateTime.Parse(Wait.UntilElementVisible(CalendarMonthButton).GetText());
        }
        private DateTime GetAssessmentCurrentMonth()
        {
            if (!Driver.IsElementDisplayed(CalendarMonthButton))
            {
                for (var i = 0; i < 5; i++)
                {
                    Wait.UntilElementClickable(CalenderIcon).Click();
                    if (Driver.IsElementDisplayed(CalendarMonthButton)) break;
                }
            }
            Wait.HardWait(3000); // Wait Until Page Load
            return DateTime.Parse(Wait.UntilElementVisible(CalendarMonthButton).GetText());
        }

        private DateTime GetCurrentYear()
        {
            return DateTime.ParseExact(Wait.UntilElementVisible(CalendarMonthButton).GetText(), "yyyy", CultureInfo.InvariantCulture);
        }

        private void OpenCalendar()
        {
            var attempt = 0;
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CalendarOpenButton).Click();
            while ((!Driver.IsElementDisplayed(CalendarNextButton) || (!Wait.UntilElementVisible(CalendarNextButton).Displayed)) && attempt < 5)
            {
                Wait.UntilElementClickable(CalendarOpenButton).Click();
                Wait.UntilJavaScriptReady();
                attempt++;
            }
        }

        internal void SetDate(DateTime date)
        {
            OpenCalendar();

            // set calendar to the correct month/year
            var currentCalendar = GetCurrentMonth();
            if (currentCalendar.Month != date.Month || currentCalendar.Year != date.Year)
            {
                Wait.UntilElementClickable(CalendarMonthButton).Click();
                Wait.HardWait(1000);
                // set correct year
                SelectYear(currentCalendar.Year, date.Year);

                // set the correct month
                Wait.UntilElementClickable(CalendarMonthItem(date)).Click();
                Wait.HardWait(1000);
            }

            // set date
            Wait.UntilElementClickable(CalendarDateItem(date)).Click();
            Wait.HardWait(1000);

        }
        internal void SetDateForAssessment(DateTime date)
        {
            OpenCalendar();

            // set calendar to the correct month/year
            var currentCalendar = GetAssessmentCurrentMonth();
            if (currentCalendar.Month != date.Month || currentCalendar.Year != date.Year)
            {
                Wait.UntilElementClickable(CalendarMonthButton).Click();
                Wait.HardWait(1000);
                // set correct year
                SelectYear(currentCalendar.Year, date.Year);

                // set the correct month
                Wait.UntilElementClickable(CalendarMonthItem(date)).Click();
                Wait.HardWait(1000);
            }

            // set date
            Wait.UntilElementClickable(CalendarDateItem(date)).Click();
            Wait.HardWait(1000);

        }

        internal void SetTime(DateTime time)
        {
            SelectItem(TimeOpenButton, CalendarTimeItem(time));
        }

        internal void SetMonth(DateTime date)
        {
            OpenCalendar();

            // set calendar to the correct month/year
            var currentCalendar = GetCurrentYear();
            if (currentCalendar.Year != date.Year)
            {
                Wait.UntilElementClickable(CalendarMonthButton).Click();
                Wait.UntilJavaScriptReady();
                // set correct year
                SelectYear(currentCalendar.Year, date.Year);
                Wait.UntilElementClickable(CalendarYearItem(date)).Click();
            }

            // set the correct month
            Wait.UntilElementClickable(CalendarMonthItem(date)).Click();
            Wait.UntilJavaScriptReady();

        }

        private void SelectYear(int currentYear, int desiredYear)
        {
            var yearDifference = desiredYear - currentYear;
            for (var i = 0; i < Math.Abs(yearDifference); i++)
            {
                if (yearDifference < 0)
                {
                    Wait.UntilElementClickable(CalendarPreviousButton).Click();
                }
                else
                {
                    Wait.UntilElementClickable(CalendarNextButton).Click();
                }
                Wait.UntilJavaScriptReady();
            }
        }
    }
}
