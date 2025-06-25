using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TeamTagRequest
    {
        public string Category { get; set; }
        public List<string> Tags { get; set; }
    }
}
