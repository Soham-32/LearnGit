using System.Collections.Generic;

namespace AtCommon.Dtos.Tags
{
    public class TeamMemberTagRequest
    {
        public string Category { get; set; }
        public List<string> Tags { get; set; }
    }
}