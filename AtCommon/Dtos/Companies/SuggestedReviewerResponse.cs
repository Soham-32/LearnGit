using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class SuggestedReviewerResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> ReviewerType { get; set; }
        public List<string> Roles { get; set; }
        public int CompanyId { get; set; }
    }
}
