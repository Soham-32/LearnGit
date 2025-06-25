using System.Collections.Generic;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class RoleRequest
    {
        public string Key { get; set; }
        public List<TagRoleRequest> Tags { get; set; }
    }
}