using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class CreateGrowthPlanItemRequest
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
        public GrowthPlanItemCardRequest Card { get; set; }
    }
}