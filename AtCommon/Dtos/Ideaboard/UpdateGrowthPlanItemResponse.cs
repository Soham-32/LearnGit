using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateGrowthPlanItemResponse
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRUserId { get; set; }
        public UpdateGrowthPlanItemCardResponse Card { get; set; }
    }
}