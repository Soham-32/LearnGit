using System.Collections.Generic;

namespace AtCommon.Dtos.Bulk
{
    public class AddMembers
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string ExternalIdentifier { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> ParticipantGroups { get; set; }
        public bool IsStakeholder { get; set; }
        public string TeamExternalIdentifier { get; set; }
    }
}