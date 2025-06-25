using System.Collections.Generic;

namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class UpdateRoleAllocationTargetsRequest
    {
        public IList<RoleAllocationTargetsRequestDto> RoleAllocationTargets { get; set; }
    }

    public class RoleAllocationTargetsRequestDto
    {
        public int RoleAllocationTargetId { get; set; }
        public int CompanyId { get; set; }
        public int? TeamId { get; set; }
        public string WorkType { get; set; }
        public string Role { get; set; }
        public int Allocation { get; set; }
    }
}
