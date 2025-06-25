using System;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class PulseAssessmentEmailResponse
    {
        public int TotalEmailsSent { get; set; }
        public DateTime EmailSentAt { get; set; }
        public bool IsAssessmentCompleted { get; set; }
    }
}