using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class DeleteCardRequest
    {
        public Guid AssessmentUid { get;set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
        public int ItemId { get; set; }
    }
}