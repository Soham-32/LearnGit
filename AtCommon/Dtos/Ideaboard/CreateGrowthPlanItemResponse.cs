using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class CreateGrowthPlanItemResponse
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRUserId { get; set; }
        public GrowthPlanItemCardResponse Card { get;set; }
    }
}