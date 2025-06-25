using System.ComponentModel;

namespace AtCommon.Api.Enums
{
    public enum SwimlaneType
    {
        [Description("3 Year Outcomes")]
        StrategicIntent, // 0 - 3 Year Outcomes
        [Description("1 Year Outcome")]
        StrategicTheme, // 1 - 1 Year Outcomes
        [Description("Quarterly Outcomes")]
        QuarterlyObjective, // 2 - Quarterly Outcomes
        [Description("Initiatives")]
        Initiatives, // 3
        [Description("Projects")]
        Projects, // 4
        [Description("Deliverables")]
        DeliveryColumn, // 5
        [Description("Stories")]
        Stories, // 6
    }
}