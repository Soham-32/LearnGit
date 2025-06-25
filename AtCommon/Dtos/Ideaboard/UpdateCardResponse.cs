using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateCardResponse
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRUserId { get; set; }
        public CardResponse Card { get; set; }
    }
}