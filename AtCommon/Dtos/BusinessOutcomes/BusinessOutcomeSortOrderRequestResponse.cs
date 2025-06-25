using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeSortOrderRequestResponse
    {
        public Guid BusinessOutcomeUid { get; set; }
        public int SortOrder { get; set; }
    }
}