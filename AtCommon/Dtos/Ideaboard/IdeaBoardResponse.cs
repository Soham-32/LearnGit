using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Ideaboard
{
    public class IdeaBoardResponse : BaseDto
    {
        public string Name { get; set; }
        public Guid AssessmentUid { get; set; }
        public int? VotesAllowed { get; set; }
        public List<CardResponse> Cards { get; set; }
    }
}