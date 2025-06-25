using System;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CampaignDetails
    {
        public string Name { get; set; }
        public string RadarType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FacilitatorsMatchmakingStrategy { get; set; }
        public string TeamsPerFacilitator { get; set; }
        public string ParentTeam { get; set; }
    }

    public class SetupAssessmentsDetails
    {
        public string Name { get; set; }
        public FacilitationApproach FacilitationApproach { get; set; }
        public DateTime StakeholderLaunchDate { get; set; }
        public DateTime TeamMemberLaunchDate { get; set; }
        public DateTime TeamMemberLaunchDateForOneMeeting { get; set; }
        public DateTime AssessmentCloseDate { get; set; }
        public DateTime RetrospectiveWindowStart { get; set; }
        public DateTime RetrospectiveWindowEnd { get; set; }
        public DateTime StakeholderWindowStart { get; set; }
        public DateTime StakeholderWindowEnd { get; set; }
        public DateTime AssessmentStartDate { get; set; }
    }

    public class SelectTeamsDetails
    {
        public string TeamName { get; set; }
        public string Tag { get; set; }
        public string WorkType { get; set; }
        public string TeamContactIsAhf { get; set; }
    }

    public class SelectFacilitatorsDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ParentTeam { get; set; }
        public string Email { get; set; }
    }

    public enum FacilitationApproach
    {
        SplitMeeting,
        OneMeeting,
        RetroOnly
    }
}