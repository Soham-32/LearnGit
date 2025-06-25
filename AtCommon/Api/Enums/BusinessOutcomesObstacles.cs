using System.ComponentModel;

namespace AgilityHealth_Automation.Enum.BusinessOutcomes
{
    public enum BusinessOutcomesObstacles
    {
        Obstacle = 1,
        Risk = 2
    }

    public enum RoamType
    {
        Resolve = 1,
        Own = 2,
        Accept = 3,
        Mitigate = 4
    }

    public enum ImpactLevel
    {
        Critical = 1,
        High = 2,   
        Medium = 3,
        Low = 4
    }

    public enum StatusType
    {
        [Description("Not Started")]
        NotStarted = 1,
        Completed = 2,
        [Description("In Progress")]
        InProgress = 3,
        [Description("On Hold")]
        OnHold = 4
    }

}