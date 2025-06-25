using System.Collections.Generic;

namespace AtCommon.Dtos.GrowthPlan
{
    public class DeleteCustomGrowthPlanTypesRequest
    {
        public int CompanyId { get; set; }
        public IEnumerable<int> CustomGrowthPlanTypeIds { get; set; }
    }
}
