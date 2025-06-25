using System;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class PulseAssessmentBatchEmailResponse
    {
        public int? TotalEmailsSent { get; set; }
        public DateTime EmailSentAt { get; set; }
    }
}