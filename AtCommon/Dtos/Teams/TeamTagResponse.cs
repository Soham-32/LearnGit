using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TeamTagResponse
    {
        public TeamTagResponse()
        {
            Tags = new List<string>();
        }

        public string Category { get; set; }
        public IList<string> Tags { get; set; }
    }
}
