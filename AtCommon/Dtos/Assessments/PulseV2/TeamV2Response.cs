using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class TeamV2Response
    {
        public bool IsSelected { get; set; }
        public int TeamId { get; set; }
        public string TeamType { get; set; }
        public string Name { get; set; }
        public bool IsAssessmentCompleted { get; set; }
        public int TotalTeamMembersThatCompletedPulseAssessment { get; set; }
        public int TotalTeamMembers { get; set; }
        public List<TeamMemberV2Response> SelectedParticipants { get; set; }
        public Guid Uid { get; set; }
    }
}