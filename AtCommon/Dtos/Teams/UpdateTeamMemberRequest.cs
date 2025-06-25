using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class UpdateTeamMemberRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; } 
        public List<TagRequest> Tags { get; set; }
    }
}
