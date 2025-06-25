using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class AutoMatchmaking
    {
        public string CreateAutoMatchesHeaderTitleMessageText { get; set; }
        public string MatchmakingDescriptionAfterCreateAutoMatchesBackAndForth { get; set; }
        public string TeamContactViewTooltip { get; set; }
        public string FacilitatorViewTooltip { get; set; }
        public string CreateAutoMatchesTooltip { get; set; }
        public string CreateAutoMatchesGridText { get; set; }
    }

    public class CampaignDetail
    {
        public string CreateCampaignHeaderText { get; set; }
        public string SetUpCampaignHeaderText { get; set; }
        public string LetsCreateYourCampaignText { get; set; }
        public List<string> CreateCampaignTooltipMessages { get; set; }
        public List<string> CreateCampaignValidationMessages { get; set; }
    }

    public class ManageCampaigns
    {
        public ManageCampaignsDashboard ManageCampaignsDashboard { get; set; }
        public CampaignDetail CampaignDetail { get; set; }
        public SelectTeams SelectTeams { get; set; }
        public SelectFacilitators SelectFacilitators { get; set; }
        public AutoMatchmaking AutoMatchmaking { get; set; }
        public SetUpAssessment SetUpAssessment { get; set; }
    }

    public class ManageCampaignsDashboard
    {
        public string ManageCampaignHeaderText { get; set; }
    }

    public class ManageCampaignsVerbiage
    {
        public ManageCampaigns ManageCampaigns { get; set; }
    }

    public class CampaignVerbiage
    {
        public ManageCampaignsVerbiage ManageCampaignsVerbiage { get; set; }
    }

    public class SelectFacilitators
    {
        public string SelectFacilitatorsHeaderText { get; set; }
        public string SelectFacilitatorsDescriptionText { get; set; }
    }

    public class SelectTeams
    {
        public string SelectTeamsHeaderText { get; set; }
        public string SelectTeamsDescriptionText { get; set; }
        public string SelectTeamPopupText { get; set; }
        public string TeamContactIsAhfTooltip { get; set; }
    }

    public class SetUpAssessment
    {
        public string SetUpAssessmentHeaderText { get; set; }
        public string SetUpAssessmentDescriptionText { get; set; }
        public string AssessmentNameTooltip { get; set; }
        public string SplitMeetingTooltip { get; set; }
        public string OneMeetingTooltip { get; set; }
        public string RetroMeetingTooltip { get; set; }
        public string AssessmentNameValidationMessage { get; set; }
        public List<string> AssessmentTimelineTooltip { get; set; }
        public List<string> SplitMeetingDatesTooltip { get; set; }
        public List<string> OneMeetingDatesTooltip { get; set; }
        public List<string> RetroMeetingDatesTooltip { get; set; }
    }
}