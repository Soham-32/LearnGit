using System;

namespace AtCommon.Dtos.Analytics
{
    public class CompanyAssessmentQuartersResponse
    {
        public string QuarterName { get; set; }
        public DateTime LastDayOfQuarter { get; set; }
        public bool IsCurrentQuarter { get; set; }

    }
}