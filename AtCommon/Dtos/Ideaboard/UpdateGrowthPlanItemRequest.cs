using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateGrowthPlanItemRequest
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
        public UpdateGrowthPlanItemCardRequest Card { get; set; }
    }
}