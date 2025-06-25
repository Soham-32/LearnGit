using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateCardRequest : BaseDto
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
        public CardRequest Card { get; set; }
    }
}