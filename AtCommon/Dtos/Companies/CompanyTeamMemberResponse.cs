using AtCommon.Dtos.Teams;
using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyTeamMemberResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; }
        public DateTime? DeletedAt { get; set; }
        public IEnumerable<TagResponse> Tags { get; set; }
        public int TeamId { get; set; }
    }
}
