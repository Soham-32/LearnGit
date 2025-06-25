using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Analytics.Custom
{
    public class AnalyticsQuarter
    {
        public AnalyticsQuarter(DateTime date)
        {
            QuarterNumber = (date.Month - 1) / 3 + 1;
            FirstDayOfQuarter = new DateTime(date.Year, (QuarterNumber - 1) * 3 + 1, 1);
            LastDayOfQuarter = FirstDayOfQuarter.AddMonths(3).AddDays(-1);
            IsCurrentQuarter = FirstDayOfQuarter.CompareTo(DateTime.UtcNow) <= 0 
                               && LastDayOfQuarter.AddDays(1).CompareTo(DateTime.UtcNow) > 0;
        }

        public int QuarterNumber { get; set; }
        public DateTime FirstDayOfQuarter { get; set; }
        public DateTime LastDayOfQuarter { get; set; }
        public bool IsCurrentQuarter { get; set; }
        public string QuarterName => $"Q{QuarterNumber} {FirstDayOfQuarter.Year}";

        public static List<AnalyticsQuarter> GetValidQuarters(int numberOfQuarters = 13, DateTime? startDate = null)
        {
            var cutOffDate = new DateTime(2020, 10, 1);
            // valid quarters are all quarters from the end date of the first assessment to the end date of the last assessment
            startDate ??= DateTime.Now;

            var quarters = new List<AnalyticsQuarter>();
            var currentQuarter = new AnalyticsQuarter(startDate.Value);
            quarters.Add(currentQuarter);

            // count backwards by 3 months adding a quarter for each until it hits the cutoff
            for (var i = 1; i < numberOfQuarters; i++)
            {
                var newQuarter = new AnalyticsQuarter(currentQuarter.FirstDayOfQuarter.AddMonths(-3 * i));
                if (newQuarter.FirstDayOfQuarter.CompareTo(cutOffDate) < 0) break;
                quarters.Add(newQuarter);
            }
            
            return quarters;
            
        }
    }
}