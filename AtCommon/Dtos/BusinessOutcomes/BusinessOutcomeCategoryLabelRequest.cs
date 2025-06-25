using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeCategoryLabelRequest
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public bool KanbanMode { get; set; }
        public int BusinessOutcomeCardTypeId { get; set; }

        public int? CategoryTypeId { get; set; }
        public List<BusinessOutcomeTagRequest> Tags { get; set; }
    }
}