using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyMemberResponse
    {
        public List<CompanyTeamMemberResponse> MemberResponse { get; set; }
        public Paging Paging { get; set; }
    }

    public class Paging
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int FirstRowOnPage { get; set; }
        public int LastRowOnPage { get; set; }
    }
}
