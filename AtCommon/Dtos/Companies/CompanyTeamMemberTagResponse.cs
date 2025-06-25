using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyTeamMemberTagResponse
    {
        public int MasterTagCategoryId { get; set; }
        public string Name { get; set; }
        public int CompanyTeamMemberCategoryId { get; set; }
        public List<CompanyTeamMemberTags> CompanyTeamMemberTags { get; set; }
    }

    public class CompanyTeamMemberTags
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
