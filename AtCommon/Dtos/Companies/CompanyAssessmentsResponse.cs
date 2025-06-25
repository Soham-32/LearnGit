using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyAssessmentsResponse
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int FirstRowOnPage { get; set; }
        public int LastRowOnPage { get; set; }
        public int PageCount { get; set; }
        public int RowCount { get; set; }
        public List<TeamAssessmentResponse> Assessments { get; set; }
    }
}
