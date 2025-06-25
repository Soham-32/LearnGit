using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments
{
    public class RoleResponse
    {
        public string Key { get; set; }
        public List<TagRoleResponse> Tags { get; set; }
    }
}
