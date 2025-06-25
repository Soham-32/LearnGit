using System.ComponentModel;

namespace AtCommon.Api.Enums
{

    public enum GrowthItemStatusType
    {
        [Description("Not Started")]
        NotStarted = 1,
        [Description("In Progress")]
        Started = 2,
        [Description("In Progress with Age")]
        StartedWithAge = 3,
        [Description("Done")]
        Finished = 4,
        [Description("Cancelled")]
        Cancelled = 5,
        [Description("On Hold")]
        OnHold = 6,
        [Description("ALL")]
        All = 7,
        [Description("Committed")]
        Committed = 8
    }

    public static class GiStatusExtensions
    {
        public static string AsString(this GrowthItemStatusType status)
        {
            return status switch
            {
                GrowthItemStatusType.NotStarted => "Not Started",
                GrowthItemStatusType.Started => "In Progress",
                GrowthItemStatusType.StartedWithAge => "In Progress",
                GrowthItemStatusType.Finished => "Done",
                GrowthItemStatusType.Cancelled => "Cancelled",
                GrowthItemStatusType.OnHold => "On Hold",
                GrowthItemStatusType.Committed => "Committed",
                _ => string.Empty
            };
        }
    }
}