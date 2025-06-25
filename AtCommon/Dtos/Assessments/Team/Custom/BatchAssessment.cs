using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.Team.Custom
{
    public class BatchAssessment
    {
        public string BatchName { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentType { get; set; }
        public List<TeamAssessmentInfo> TeamAssessments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
