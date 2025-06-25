using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class SaveAsDraftRequest
    {
        public int CampaignId { get; set; }
        public List<int> SelectedTeams { get; set; }
        public List<string> SelectedFacilitators { get; set; }
        public string MatchmakingAssignmentsState { get; set; }
        public AssessmentSettings AssessmentSettings { get; set; }
        public Dictionary<string, string> TeamFacilitatorMap { get; set; }
    }

    public class AssessmentSettings
    {
        public string Name { get; set; }
        public string AssessmentType { get; set; }
        public DateTime? TwoMeetingsStakeholderLaunchDate { get; set; }
        public DateTime? TwoMeetingsTeamMemberLaunchDate { get; set; }
        public DateTime? TwoMeetingsCloseDate { get; set; }
        public DateTime? TwoMeetingsRetrospectiveWindowStart { get; set; }
        public DateTime? TwoMeetingsRetrospectiveWindowEnd { get; set; }
        public DateTime? SingleRetroMeetingAssessmentStartDate { get; set; }
        public DateTime? SingleRetroMeetingAssessmentCloseDate { get; set; }
        public DateTime? SingleRetroMeetingRetrospectiveWindowStart { get; set; }
        public DateTime? SingleRetroMeetingRetrospectiveWindowEnd { get; set; }
        public DateTime? SingleCombinedMeetingStakeholderWindowStart { get; set; }
        public DateTime? SingleCombinedMeetingStakeholderWindowEnd { get; set; }
        public DateTime? SingleCombinedMeetingRetrospectiveWindowStart { get; set; }
        public DateTime? SingleCombinedMeetingTeamMemberLaunchDate { get; set; }
        public DateTime? SingleCombinedMeetingRetrospectiveWindowEnd { get; set; }
    }
}
