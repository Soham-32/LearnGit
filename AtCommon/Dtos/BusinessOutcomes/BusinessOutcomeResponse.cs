using System;
using System.Collections.Generic;
using AtCommon.Api.Enums;
using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeResponse : BaseBusinessOutcomesDto
    {
        public string sourceCategoryName;

        public BusinessOutcomeResponse()
        {
            Labels = new List<BusinessOutcomeLabelResponse>();
            Links = new List<BusinessOutcomeLinkResponse>();
            ChecklistItems = new List<BusinessOutcomeChecklistItemResponse>();
            Stakeholders = new List<BusinessOutcomeStakeholderResponse>();
            Owners = new List<BusinessOutcomeOwnerResponse>();
            Sponsors = new List<BusinessOutcomeSponsorResponse>();
            Dependencies = new List<BusinessOutcomeDependenciesResponse>();
            Obstacles = new List<BusinessOutcomeObstaclesResponse>();
            SupportingTeams = new List<BusinessOutcomeSupportingTeamResponse>();
            ParentCard = new List<GetParentOutcomesResponse>();
            Documents = new List<BusinessOutcomeAttachmentResponse>();
            Financials = new List<BusinessOutcomeFinancialResponse>();
        }

        public string SourceCategoryName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public string TeamName { get; set; }

        public decimal? BusinessValue { get; set; }

        public int? BusinessOutcomeTeamId { get; set; }

        public double EstimatedROI { get; set; }

        public double EstimatedCost { get; set; }

        public int ValuePoints { get; set; }

        public double OverallProgress { get; set; }

        public double TargetProgress { get; set; }

        public double Weight { get; set; }

        public string InvestmentCategory { get; set; }

        public string CardColor { get; set; }

        public int PrettyId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public int? TeamId { get; set; }

        public int? CompanyId { get; set; }

        [JsonProperty("SwimlaneType")]
        public SwimlaneType SwimlaneType { get; set; }

        [JsonProperty("PermissionType")]
        public BusinessOutcomePermissionType PermissionType { get; set; }

        public Guid SwimlaneId { get; set; }

        public Guid OriginalSwimlaneId { get; set; }

        public int SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int HealthId { get; set; }
        public int StatusId { get; set; }

        public string UpdatedBy { get; set; }

        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleteAffirmed { get; set; }

        public string RestoredBy { get; set; }
        public bool? IsRestoreAffirmed { get; set; }
        public int PriorityScore { get; set; }

        public DateTime? RestoredAt { get; set; }

        public bool IsSupportingTeamCard { get; set; }
        public int? BudgetCategoryId { get; set; }
        public int? RequestedBudget { get; set; }
        public int? ApprovedBudget { get; set; }
        public int? CalculationMethod { get; set; }
        public string Currency { get; set; }

        public List<KeyResultResponse> KeyResults { get; set; }

        public List<BusinessOutcomeLabelResponse> Labels { get; set; }

        public List<BusinessOutcomeLinkResponse> Links { get; set; }

        public List<BusinessOutcomeTagResponse> Tags { get; set; }

        public List<CustomFieldValueResponse> CustomFieldValues { get; set; }

        public List<CommentsResponse> Comments { get; set; }

        public List<GetParentOutcomesResponse> ParentCard { get; set; }

        public List<BusinessOutcomeChecklistItemResponse> ChecklistItems { get; set; }
        public IEnumerable<BusinessOutcomeStakeholderResponse> Stakeholders { get; set; }
        public IEnumerable<BusinessOutcomeSponsorResponse> Sponsors { get; set; }
        public IEnumerable<BusinessOutcomeOwnerResponse> Owners { get; set; }
        public IEnumerable<BusinessOutcomeDependenciesResponse> Dependencies { get; set; }
        public List<BusinessOutcomeObstaclesResponse> Obstacles { get; set; }
        public IEnumerable<BusinessOutcomeSupportingTeamResponse> SupportingTeams { get; set; }
        public IEnumerable<BusinessOutcomeAttachmentResponse> Documents { get; set; }
        public List<BusinessOutcomeChildCardResponse> ChildCards { get; set; }
        public int? BusinessOutcomeCardTypeId { get; set; }
        public List<BusinessOutcomeFinancialResponse> Financials { get; set; }
        public string CategoryLabelUid { get; set; }
        public ProgressCalculateType ProgressCalculateType { get; set; }
        public bool IsManualHealthUpdated { get; set; }
    }
    public enum ProgressCalculateType
    {
        Default = 0,
        KeyResults = 1,
        ChildCards = 2,
        CheckList = 3,
    }
}