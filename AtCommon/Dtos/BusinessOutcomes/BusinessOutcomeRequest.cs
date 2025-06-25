using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Api.Enums;
using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeRequest
    {
        public BusinessOutcomeRequest()
        {
            ChecklistItems = new HashSet<BusinessOutcomeChecklistItemRequest>();
            Stakeholders = new List<BusinessOutcomesStakeholderRequest>();
            Owners = new List<BusinessOutcomeOwnerRequest>();
            Sponsors = new List<BusinessOutcomeSponsorRequest>();
            Dependencies = new List<BusinessOutcomeDependenciesRequest>();
            Obstacles = new List<BusinessOutcomeObstaclesRequest>();
            SupportingTeams = new List<BusinessOutcomeSupportingTeamRequest>();
            Links = new List<BusinessOutcomesLinkRequest>();
        }
        public string SourceCategoryName { get; set; }
        public Guid Uid { get; set; }
        public Guid ParentUid { get; set; }
        public bool IsDeleted { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long BusinessValue { get; set; }
        public int BusinessOutcomesTeamId { get; set; }
        public long EstimatedROI { get; set; }
        public long EstimatedCost { get; set; }
        public int ValuePoints { get; set; }
        public int PriorityScore { get; set; }
        public long OverallProgress { get; set; }
        public double Weight { get; set; }
        public string InvestmentCategory { get; set; }
        public int? PrettyId { get; set; }
        public int? TeamId { get; set; }
        public int CompanyId { get; set; }
        public SwimlaneType SwimlaneType { get; set; }
        public Guid SwimlaneId { get; set; }
        public Guid OriginalSwimlaneId { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CardColor { get; set; }
        public List<DeliverableRequest> Deliverables { get; set; }
        public List<KeyResultRequest> KeyResults { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<Guid> NewLinkedParentsCardUid { get; set; }
        public IEnumerable<BusinessOutcomeChecklistItemRequest> ChecklistItems { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int HealthId { get; set; }
        public int StatusId { get; set; }
        public string DomainName { get; set; } = null;
        public int? CardType { get; set; }
        public int? BudgetCategoryId { get; set; }
        public int? RequestedBudget { get; set; }
        public int? ApprovedBudget { get; set; }
        public int? CalculationMethod { get; set; }
        public string Currency { get; set; }

        [JsonProperty("BusinessOutcomeLabel")]
        public List<BusinessOutcomeLabelRequest> BusinessOutcomeRequestLabel { get; set; }

        public List<BusinessOutcomeChildCardRequest> ChildCards { get; set; }
        public List<BusinessOutcomeTagRequest> Tags { get; set; }
        public List<CustomFieldValueRequest> CustomFieldValues { get; set; }
        public List<CommentRequest> Comments { get; set; }
        public List<BusinessOutcomeFinancialRequest> Financials { get; set; }
        public IEnumerable<BusinessOutcomesStakeholderRequest> Stakeholders { get; set; }
        public IEnumerable<BusinessOutcomeSponsorRequest> Sponsors { get; set; }
        public IEnumerable<BusinessOutcomeOwnerRequest> Owners { get; set; }
        public IEnumerable<BusinessOutcomeDependenciesRequest> Dependencies { get; set; }
        public List<BusinessOutcomeObstaclesRequest> Obstacles { get; set; }
        public IEnumerable<BusinessOutcomeSupportingTeamRequest> SupportingTeams { get; set; }
        public ICollection<BusinessOutcomeAttachmentRequest> Documents { get; set; }
        public List<BusinessOutcomesLinkRequest> Links { get; set; }

        public void SetOwner(string firstname, string lastname)
        {
            Owner = $"{firstname} {lastname}";
        }

        public void HydrateUpdatedByAndComments(string userId)
        {
            UpdatedBy = userId;

            if (Comments.Any())
            {
                foreach (var comment in Comments)
                {
                    if (comment.Uid == Guid.Empty)
                    {
                        comment.Owner = userId;
                    }
                }
            }
        }
    }
}