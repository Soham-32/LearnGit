using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class KeyResultRequest : BaseBusinessOutcomesDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Formula { get; set; }

        public string KeyResultSource { get; set; }

        public string Comment { get; set; }

        [JsonProperty("isImpact")]
        public bool IsImpact { get; set; }

        [JsonProperty("RelationshipTypeId")]
        public KeyResultRelationshipType RelationshipTypeId { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        [JsonProperty("BusinessOutcomeId")]
        public Guid BusinessOutcomeId { get; set; }

        [JsonProperty("ParentKeyResultId")]
        public Guid? ParentKeyResultId { get; set; }

        [JsonProperty("Team")]
        public int? Team { get; set; }

        [JsonProperty("Start")]
        public string Start { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }

        [JsonProperty("StretchGoal")]
        public string StretchGoal { get; set; }

        [JsonProperty("Weight")]
        public double Weight { get; set; }

        [JsonProperty("Progress")]
        public double Progress { get; set; }

        [JsonProperty("TeamName")]
        public string TeamName { get; set; }

        [JsonProperty("Metric")]
        public BusinessOutcomeMetricRequest Metric { get; set; }
        public DateTime? CompletedBy { get; set; }
        public int? KeyResultFrequency { get; set; }
        public string KeyResultOwnerId { get; set; }
        public int? SubTargetsOrder { get; set; }

        public List<KeyResultRequest> Links { get; set; }
        public List<BusinessOutcomeKeyResultSubTargetRequest> SubTargets { get; set; } = new List<BusinessOutcomeKeyResultSubTargetRequest>();
        public List<KeyResultContributingNotificationRequest> ContributingNotifications { get; set; }
    }
    public enum KeyResultRelationshipType
    {
        Aligning = 0,
        Contributing = 1
    }
}