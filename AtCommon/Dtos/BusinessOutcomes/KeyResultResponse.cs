using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class KeyResultResponse : BaseBusinessOutcomesDto
    {
        public KeyResultResponse()
        {
            SubTargets = new List<BusinessOutcomeKeyResultSubTargetResponse>();
        }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Formula { get; set; }
        public string KeyResultSource { get; set; }
        public string Comment { get; set; }
        public int? SortOrder { get; set; }
        public bool IsImpact { get; set; }
        public bool IsAligning { get; set; }
        public bool IsContributing { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public int? Team { get; set; }
        public string Start { get; set; }
        public string Target { get; set; }
        public string StretchGoal { get; set; }
        public double Weight { get; set; }
        public double Progress { get; set; }
        public double OverallProgress { get; set; }
        public string TeamName { get; set; }
        public KeyResultRelationshipType RelationshipTypeId { get; set; }
        public int KeyResultFrequency { get; set; }
        public string KeyResultOwnerId { get; set; }
        public int SubTargetsOrder { get; set; }
        public string UpdatedBy { get; set; }
        public BusinessOutcomeMetricResponse Metric { get; set; }
        public BusinessOutcomeResponse BusinessOutcome { get; set; }
        public DateTime? CompletedBy { get; set; }
        public Guid? ParentKeyResultId { get; set; }
        public KeyResultResponse ParentKeyResult { get; set; }
        public ICollection<KeyResultResponse> Links { get; set; }
        public GetParentOutcomesResponse ParentCard { get; set; }
        public BusinessOutcomeUserResponse KeyResultOwner { get; set; }
        public List<BusinessOutcomeKeyResultSubTargetResponse> SubTargets { get; set; }
        public ICollection<KeyResultContributingNotificationResponse> ContributingNotifications { get; set; }
        public KeyResultTargetDetailResponse TargetDetail { get; set; }

    }
    public class KeyResultTargetDetailResponse
    {
        public BusinessOutcomeKeyResultSubTargetResponse SelectedSubTarget { get; set; }
        public double TargetProgress { get; set; }
        public double ProgressBySubTarget { get; set; }
    }

    public class BusinessOutcomeUserResponse
    {
        public string DisplayName { get; set; }
        public string UserId { get; set; }
    }

    public class BusinessOutcomeKeyResultSubTargetResponse 
    {
        public int Id { get; set; }
        public Guid BusinessOutcomeKeyResultId { get; set; }
        public string SubTargetValue { get; set; }
        public DateTime SubTargetAsOfDate { get; set; }
        public double Progress { get; set; }
    }
    public class KeyResultContributingNotificationResponse
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string AuthorName { get; set; }
        public string ChildTitle { get; set; }
        public double ParentValue { get; set; }
        public double SuggestedValue { get; set; }
        public KeyResultContributingNotificationStatus NotificationStatusId { get; set; }
        public KeyResultContributingNotificationType NotificationTypeId { get; set; }
        public Guid ParentKeyResultUid { get; set; }
        public Guid ChildKeyResultUid { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public enum KeyResultContributingNotificationType
    {
        None,

        /// <summary>
        /// Notification when there is one child with the same metric and target as the parent.
        /// </summary>
        SingleChildWithMatchingMetricAndTarget,

        /// <summary>
        /// Notification when the parent value is increasing with a numerical metric and has one or multiple contributing children.
        /// </summary>
        ParentValueIncreasingWithNumericMetric,

        /// <summary>
        /// Notification when the parent value is decreasing with a numerical metric and has one or multiple contributing children.
        /// </summary>
        ParentValueDecreasingWithNumericMetric,

        /// <summary>
        /// Notification when the parent metric is a percentage and has one or multiple contributing children.
        /// </summary>
        ParentMetricAsPercentage,

        /// <summary>
        /// Notification when the parent metric and child metric are different and has one contributing child.
        /// </summary>
        ParentAndChildMetricsDiffer
    }
}