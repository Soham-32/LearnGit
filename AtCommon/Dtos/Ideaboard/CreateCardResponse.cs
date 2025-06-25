using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class CreateCardResponse : BaseDto
    {
        public int IdeaBoardId { get; set; }
        public string Name { get; set; }
        public Guid AssessmentUid { get; set; }
        public string SignalRUserId { get; set; }
        public CardResponse Card { get; set; }
    }
}