using AtCommon.Dtos.Assessments;
using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyAssessmentsResultsResponse
    {
        public List<AssessmentsResultsResponse> AssessmentResults { get; set; }
        public Paging Paging { get; set; }
    }
}