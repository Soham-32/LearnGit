using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CampaignAssessment
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

    public class SaveAsDraftResponse
    {
        public int CampaignId { get; set; }
        public List<int> SelectedTeams { get; set; }
        public List<string> SelectedFacilitators { get; set; }
        public Dictionary<string, string> TeamFacilitatorMap { get; set; }
        public string MatchmakingAssignmentsState { get; set; }
        public CampaignAssessment CampaignAssessment { get; set; }
    }

    public class EmptyAndNullErrorList
    {
        public List<string> MatchmakingAssignmentsState { get; set; }

        public List<string> SelectedTeams { get; set; }
        public List<string> SelectedFacilitators { get; set; }

        [JsonProperty("assessmentSettings.name")]
        public List<string> AssessmentSettingsname { get; set; }

        [JsonProperty("assessmentSettings.assessmentType")]
        public List<string> AssessmentSettingsassessmentType { get; set; }

        [JsonProperty("assessmentSettings.twoMeetingsCloseDate")]
        public List<string> AssessmentSettingstwoMeetingsCloseDate { get; set; }

        [JsonProperty("assessmentSettings.twoMeetingsTeamMemberLaunchDate")]
        public List<string> AssessmentSettingstwoMeetingsTeamMemberLaunchDate { get; set; }

        [JsonProperty("assessmentSettings.twoMeetingsStakeholderLaunchDate")]
        public List<string> AssessmentSettingstwoMeetingsStakeholderLaunchDate { get; set; }

        [JsonProperty("assessmentSettings.twoMeetingsRetrospectiveWindowEnd")]
        public List<string> AssessmentSettingstwoMeetingsRetrospectiveWindowEnd { get; set; }

        [JsonProperty("assessmentSettings.twoMeetingsRetrospectiveWindowStart")]
        public List<string> AssessmentSettingstwoMeetingsRetrospectiveWindowStart { get; set; }

        [JsonProperty("assessmentSettings.singleRetroMeetingAssessmentCloseDate")]
        public List<string> AssessmentSettingssingleRetroMeetingAssessmentCloseDate { get; set; }

        [JsonProperty("assessmentSettings.singleRetroMeetingAssessmentStartDate")]
        public List<string> AssessmentSettingssingleRetroMeetingAssessmentStartDate { get; set; }

        [JsonProperty("assessmentSettings.singleRetroMeetingRetrospectiveWindowEnd")]
        public List<string> AssessmentSettingssingleRetroMeetingRetrospectiveWindowEnd { get; set; }

        [JsonProperty("assessmentSettings.singleCombinedMeetingStakeholderWindowEnd")]
        public List<string> AssessmentSettingssingleCombinedMeetingStakeholderWindowEnd { get; set; }

        [JsonProperty("assessmentSettings.singleCombinedMeetingTeamMemberLaunchDate")]
        public List<string> AssessmentSettingssingleCombinedMeetingTeamMemberLaunchDate { get; set; }

        [JsonProperty("assessmentSettings.singleRetroMeetingRetrospectiveWindowStart")]
        public List<string> AssessmentSettingssingleRetroMeetingRetrospectiveWindowStart { get; set; }

        [JsonProperty("assessmentSettings.singleCombinedMeetingRetrospectiveWindowEnd")]
        public List<string> AssessmentSettingssingleCombinedMeetingRetrospectiveWindowEnd { get; set; }

        [JsonProperty("assessmentSettings.singleCombinedMeetingStakeholderWindowStart")]
        public List<string> AssessmentSettingssingleCombinedMeetingStakeholderWindowStart { get; set; }

        [JsonProperty("assessmentSettings.singleCombinedMeetingRetrospectiveWindowStart")]
        public List<string> AssessmentSettingssingleCombinedMeetingRetrospectiveWindowStart { get; set; }
    }
}
