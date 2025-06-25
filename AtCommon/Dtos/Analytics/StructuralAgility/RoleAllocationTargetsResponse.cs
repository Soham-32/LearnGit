using System;

namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class RoleAllocationTargetsResponse
    {
        public Guid Uid { get; set; }

        public int RoleAllocationTargetId { get; set; }

        public int CompanyId { get; set; }

        public int TeamId { get; set; }

        public string WorkType { get; set; }

        public string Role { get; set; }

        public int Allocation { get; set; }
    }
}
