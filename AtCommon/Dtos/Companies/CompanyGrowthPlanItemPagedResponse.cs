using System.Collections.Generic;
using AtCommon.Dtos.GrowthPlan;

namespace AtCommon.Dtos.Companies
{
    public class CompanyGrowthPlanItemPagedResponse
    {
        public Paging Paging { get; set; }

        public List<GrowthPlanItemResponse> GrowthPlanItems { get; set; }
    }
}