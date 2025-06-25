using System;
namespace AtCommon.Dtos.Ideaboard
{
    public class SortCardsRequests : BaseDto
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
    }
}