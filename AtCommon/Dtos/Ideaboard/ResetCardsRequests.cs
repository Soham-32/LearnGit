using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class ResetCardsRequests : BaseDto
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
    }
}