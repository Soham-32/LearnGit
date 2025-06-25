using System.ComponentModel;

namespace AtCommon.Api.Enums
{
    public enum GrowthItemSegmentType
    {
        [Description("Type")]
        Category = 1,
        [Description("Dimension")]
        Dimension = 2,
        [Description("Sub-Dimension")]
        SubDimension = 3,
        [Description("Competency")]
        Competency = 4,
        [Description("ALL")]
        All = 5
    }
}