using System.ComponentModel;

namespace AtCommon.Api.Enums
{
    public enum AssessmentPeriod
    {
        [Description("24 Hours")] 
        TwentyFourHours = 1,
        [Description("72 Hours")] 
        SeventyTwoHours = 2,
        [Description("1 Week")] 
        OneWeek = 3,
        [Description("2 Weeks")] 
        TwoWeeks = 4,
        [Description("3 Weeks")] 
        ThreeWeeks = 5,
        [Description("Until the next assessment launch date")] 
        UntilAssessmentLaunch = 6
    }
}
