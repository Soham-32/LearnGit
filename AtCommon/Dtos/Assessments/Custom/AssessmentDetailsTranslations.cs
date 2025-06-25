using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.Custom
{
    public class AssessmentDetailsTranslations
    {
        public List<string> HeaderJumpLinkNames { get; set; }
        public List<string> FilterSidebarSection { get; set; }
        public AssessmentDetails AssessmentDetails { get; set; }
        public ActionIconSection ActionIconSection { get; set; }
        public AnalyticsSection AnalyticsSection { get; set; }
        public CommentSection CommentSection { get; set; }
        public GrowthOpportunitiesSection GrowthOpportunitiesSection { get; set; }
        public ImpedimentsSection ImpedimentsSection { get; set; }
        public StrengthsSection StrengthsSection { get; set; }
        public GrowthPlanSection GrowthPlanSection { get; set; }
    }
    public class AssessmentDetails
    {
        public List<string> RadarViewDropDownValues { get; set; }
        public List<BenchmarkingPopupDetail> BenchmarkingPopupDetails { get; set; }
    }
    public class BenchmarkingPopupDetail
    {
        public string BenchmarkingPopupTitle { get; set; }
        public string BenchmarkingPopupViewMessage { get; set; }
        public string BenchmarkingOption { get; set; }
        public List<string> BenchmarkingOptionsDropdownValues { get; set; }
        public string BenchmarkingWorkType { get; set; }
        public List<string> WorkTypeDropdown { get; set; }
        public string BenchmarkingMaturity { get; set; }
        public List<string> MaturityDropDown { get; set; }
        public List<string> BenchmarkingPopupButton { get; set; }
    }
    public class ActionIconSection
    {
        public List<string> ActionIconNames { get; set; }
    }
    public class AnalyticsSection
    {
        public string AnalyticsHeaderTitle { get; set; }
        public List<AnalyticsCard> AnalyticsCards { get; set; }
    }
    public class AnalyticsCard
    {
        public List<TopFiveCompetency> TopFiveCompetencies { get; set; }
        public List<LowestFiveCompetency> LowestFiveCompetencies { get; set; }
        public List<FiveHighestConsensusCompetency> FiveHighestConsensusCompetencies { get; set; }
        public List<FiveLowestConsensusCompetency> FiveLowestConsensusCompetencies { get; set; }
    }
    public class TopFiveCompetency
    {
        public List<string> TopFiveCompetenciesTitle { get; set; }
    }
    public class LowestFiveCompetency
    {
        public List<string> LowestFiveCompetenciesTitle { get; set; }
    }
    public class FiveHighestConsensusCompetency
    {
        public List<string> FiveHighestConsensusCompetenciesTitle { get; set; }
        public TooltipConsensus TooltipConsensus { get; set; }
    }
    public class TooltipConsensus
    {
        public string FiveHighestConsensusTooltipDescription { get; set; }
        public string FiveHighestConsensusTooltipSupportArticleLink { get; set; }
        public string FiveLowestConsensusTooltipDescription { get; set; }
        public string FiveLowestConsensusTooltipSupportArticleLink { get; set; }
    }
    public class FiveLowestConsensusCompetency
    {
        public List<string> FiveLowestConsensusCompetenciesTitle { get; set; }
        public TooltipConsensus TooltipConsensus { get; set; }
    }
    public class CommentSection
    {
        public string TeamCommentsTitle { get; set; }
        public string StakeholderCommentsTitle { get; set; }
        public string HideAllTeamCommentsButton { get; set; }
        public string HideAllStakeholderCommentsButton { get; set; }
        public HideAllTeamCommentsPopup HideAllTeamCommentsPopup { get; set; }
        public string UnHideAllTeamCommentsButton { get; set; }
        public UnHideAllTeamCommentsPopup UnHideAllTeamCommentsPopup { get; set; }
        public HideAllStakeholderCommentsPopup HideAllStakeholderCommentsPopup { get; set; }
        public string UnHideAllStakeholderCommentsButton { get; set; }
        public UnHideAllStakeholderCommentsPopup UnHideAllStakeholderCommentsPopup { get; set; }
        public Buttons Buttons { get; set; }
    }
    public class HideAllTeamCommentsPopup
    {
        public string HideAllTeamCommentsPopupTitle { get; set; }
        public List<string> HideAllTeamCommentsInfo { get; set; }
        public string YesProceedButton { get; set; }
        public string NoCancelButton { get; set; }
    }
    public class UnHideAllTeamCommentsPopup
    {
        public string UnHideAllTeamCommentsPopupTitle { get; set; }
        public List<string> UnHideAllTeamCommentsInfo { get; set; }
        public string YesProceedPopupButton { get; set; }
        public string NoCancelPopupButton { get; set; }
    }
    public class HideAllStakeholderCommentsPopup
    {
        public string HideAllStakeholderCommentsPopupTitle { get; set; }
        public List<string> HideAllStakeholderCommentsInfo { get; set; }
        public string YesProceedButton { get; set; }
        public string NoCancelButton { get; set; }
    }
    public class UnHideAllStakeholderCommentsPopup
    {
        public string UnHideAllStakeholderCommentsPopupTitle { get; set; }
        public List<string> UnHideAllStakeholderCommentsInfo { get; set; }
        public string YesProceedPopupButton { get; set; }
        public string NoCancelPopupButton { get; set; }
    }
    public class Buttons
    {
        public string EditButton { get; set; }
        public UpdateAndCancelButton UpdateAndCancelButton { get; set; }
        public string HideButton { get; set; }
        public string UnHideButton { get; set; }
    }
    public class UpdateAndCancelButton
    {
        public string UpdateButton { get; set; }
        public string CancelButton { get; set; }
    }
    public class GrowthOpportunitiesSection
    {
        public string GrowthOpportunitiesTitle { get; set; }
        public string Description { get; set; }
    }
    public class ImpedimentsSection
    {
        public string ImpedimentsTitle { get; set; }
        public string Description { get; set; }
    }
    public class StrengthsSection
    {
        public string StrengthsTitle { get; set; }
        public string Description { get; set; }
    }
    public class GrowthPlanSection
    {
        public string GrowthPlanTitle { get; set; }
        public List<string> GrowthPlanDescription { get; set; }
        public string AddNewItemButton { get; set; }
    }
}