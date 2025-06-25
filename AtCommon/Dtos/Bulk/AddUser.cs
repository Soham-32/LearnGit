using System.Collections.Generic;
using AtCommon.Dtos.Teams;

namespace AtCommon.Dtos.Bulk
{
    public class AddUser
    {
        public AddUser()
        {
            Tags = new List<TeamTagRequest>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public IEnumerable<string> Teams { get; set; }
        public IEnumerable<TeamTagRequest> Tags { get; set; }
    }
}