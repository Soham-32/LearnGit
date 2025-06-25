using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class CustomBusinessOutcomeRequest : BaseBusinessOutcomesDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<KeyResultRequest> KeyResults { get; set; }
        public float BusinessValue { get; set; }
        public string InvestmentCategory { get; set; }
        public double OverallProgress { get; set; }
        public int TeamId { get; set; }
        public int CompanyId { get; set; }
        public int PrettyId { get; set; }
        public int SwimlaneType { get; set; }
        public Guid SwimlaneId { get; set; }
        public string CardColor { get; set; }
        public int SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<BusinessOutcomeLabelRequest> BusinessOutcomeLabel { get; set; }
        public List<BusinessOutcomeTagRequest> Tags { get; set; }
        public List<CommentRequest> Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public List<CustomFieldValueRequest> CustomFieldValues { get; set; }
        public string SourceCategoryName { get; set; }
        public List<DeliverableRequest> Deliverables { get; set; }
        public List<BusinessOutcomeChecklistItemRequest> CheckListItems { get; set; }
        
        public List<BusinessOutcomeAttachmentRequest> Documents { get; set; }
        public List<BusinessOutcomeChildCardRequest> ChildCards { get; set; }
        public List<BusinessOutcomeFinancialRequest> Financials { get; set; }
    }
}