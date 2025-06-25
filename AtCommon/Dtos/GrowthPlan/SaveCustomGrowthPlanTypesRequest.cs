using System.Collections.Generic;

namespace AtCommon.Dtos.GrowthPlan
{
    public class SaveCustomGrowthPlanTypesRequest
    {
        public int CompanyId { get; set; }
        public IEnumerable<CustomGrowthPlanType> CustomGrowthPlanTypes { get; set; }
    }

    public class CustomGrowthPlanType
    {
        public int CompanyCustomListId { get; set; }
        public string CustomText { get; set; }
    }
}
