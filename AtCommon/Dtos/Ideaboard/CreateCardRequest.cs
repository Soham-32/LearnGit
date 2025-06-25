using System;

namespace AtCommon.Dtos.Ideaboard
{
    public class CreateCardRequest
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
        public string Name { get; set; }
        public CardRequest Card { get; set; }
    }
}