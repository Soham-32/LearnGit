using System.Collections.Generic;

namespace AtCommon.Dtos.Scim
{
    public class ScimAllUser
    {
        public List<string> Schemas { get; set; }
        public int TotalResults { get; set; }
        public int ItemsPerPage { get; set; }
        public int StartIndex { get; set; }
        public List<ScimUser> Resources { get; set; }
    }
}
