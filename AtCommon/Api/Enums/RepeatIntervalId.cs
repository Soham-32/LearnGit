using System.ComponentModel;

namespace AtCommon.Api.Enums
{
    public enum RepeatIntervalId
    {
        [Description("Not Used")] 
        NotUsed = 0,
        [Description("Does not repeat")]
        Never = 1,
        [Description("Daily")]
        Daily = 2,
        [Description("Weekly on ")]
        Weekly = 3,
        [Description("Bi-monthly on the second and fourth ")]
        BiMonthly = 4,
        [Description("Monthly on the second ")]
        Monthly = 5,
        [Description("Quarterly on the second ")]
        Quarterly = 6
    }
}