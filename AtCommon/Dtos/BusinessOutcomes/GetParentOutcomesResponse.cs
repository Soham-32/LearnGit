using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class GetParentOutcomesResponse
    {
        public Guid Uid { get; set; }
        public int PrettyId { get; set; }
        public int? TeamId { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public int? BusinessOutcomeCardTypeId { get; set; }
        public Guid? CategoryLabelUid { get; set; }
        public bool IsParent { get; set; }
        public IEnumerable<int> ParentIds { get; set; }
        public int? BudgetCategoryId { get; set; }
        public int? RequestedBudget { get; set; }
        public int? ApprovedBudget { get; set; }
        public int? CalculationMethod { get; set; }
        public string Currency { get; set; }
    }


}