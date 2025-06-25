using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Ideaboard
{
    public class UpdateCardsRequest : BaseDto
    {
        public Guid AssessmentUid { get; set; }
        public string SignalRGroupName { get; set; }
        public string SignalRUserId { get; set; }
        public List<CardRequest> Cards { get; set; }
    }
}