using System.ComponentModel;

namespace AtCommon.Api.Enums
{
    public enum GrowthItemDetailStatusType
    {
        [Description("Not Started")]
        InBacklog = 1,
        [Description("In Progress")]
        Started = 2,
        [Description("In Progress < 3 months old")]
        StartedLess3 = 3,
        [Description("In Progress 3+ months old")]
        StartedMore3 = 4,
        [Description("Cancelled")]
        Cancelled = 5,
        [Description("On Hold")]
        OnHold = 6,
        [Description("Done")]
        Finished = 7,
        [Description("Committed")]
        Committed = 8
    }

    public static class GiDetailStatusExtensions
    {
        public static string AsString(this GrowthItemDetailStatusType status)
        {
            return status switch
            {
                GrowthItemDetailStatusType.StartedLess3 => "In Progress",
                GrowthItemDetailStatusType.StartedMore3 => "In Progress",
                _ => status.GetDescription()
            };
        }
    }
}