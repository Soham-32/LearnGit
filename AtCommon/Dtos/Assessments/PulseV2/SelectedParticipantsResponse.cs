using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class SelectedParticipantsResponse
    {
        public int PulseAssessmentId { get; set; }
        public int TeamId { get; set; }
        public List<TeamMemberV2Response> SelectedParticipants { get; set; }
    }
}
