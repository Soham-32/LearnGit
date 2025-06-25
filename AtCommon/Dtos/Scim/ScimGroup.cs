using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtCommon.Dtos.Scim
{
    public class ScimGroup
    {
        public List<string> Schemas { get; set; }
        public int TotalResults { get; set; }
        public int ItemsPerPage { get; set; }
        public int StartIndex { get; set; }
        public List<Resource> Resources { get; set; }

        public class Member
        {
            public string Type { get; set; }
            public bool Primary { get; set; }
            public string Display { get; set; }
            public string Value { get; set; }

            [JsonProperty("$ref")]
            public string Ref { get; set; }
        }

        public class Resource
        {
            public string DisplayName { get; set; }
            public List<Member> Members { get; set; }
            public string UId { get; set; }
            public string Schemas { get; set; }
            public string Meta { get; set; }
        }
    }
}
