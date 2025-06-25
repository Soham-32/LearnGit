using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class GetPulseAssessmentV2Request
    {
        public string DataRequest { get; set; }
        public bool IsFilterTeamsBasedOnSelectedRoleFilter { get; set; }
        public PulseAssessmentRoleFilterRequest SelectedRoleFilter { get; set; }
        public bool IsSelectedCompetencies { get; set; }
        public List<int> SelectedCompetencyIds { get; set; }
    }
}
