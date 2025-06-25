using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TagRequest
    {
        public string Category { get; set; }
        public List<string> Tags { get; set; }
    }
}
