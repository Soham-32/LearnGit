using System.ComponentModel;

namespace AgilityHealth_Automation.Enum.BusinessOutcomes
{
    public enum BusinessOutcomesCardType
    {
        [Description("Business Outcomes")]
        BusinessOutcomes,
        [Description("Initiatives")]
        AnnualView,
        [Description("Projects")]
        ProjectsTimeline,
        [Description("Deliverables")]
        DeliverablesTimeline,
        [Description("Stories")]
        Monthly,
    }

    public enum BusinessOutcomesCardTypeTags
    {
        [Description("Annual View")]
        AnnualView,
        [Description("Projects Timeline")]
        ProjectsTimeline,
        [Description("Deliverables Timeline")]
        DeliverablesTimeline,
        [Description("Annually")]
        Annually,
        [Description("Monthly")]
        Monthly,
    }
}