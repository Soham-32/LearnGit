using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class GetParticipantsRequest
    {
        public PulseAssessmentRoleFilterRequest SelectedRoleFilter { get; set; }
        public bool IsSelectedCompetencies { get; set; }
        public List<int> SelectedCompetencyIds { get; set; }
    }
}
