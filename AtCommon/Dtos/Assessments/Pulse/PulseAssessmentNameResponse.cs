using System;

namespace AtCommon.Dtos.Assessments.Pulse
{
    public class PulseAssessmentNameResponse
    {
        public int PulseAssessmentId { get; set; }
        public Guid PulseAssessmentUid { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsTemplate { get; set; }
    }
}
