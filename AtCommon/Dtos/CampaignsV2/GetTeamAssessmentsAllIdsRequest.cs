namespace AtCommon.Dtos.CampaignsV2
{
    public class GetTeamAssessmentsAllIdsRequest
    {
        public string SearchText { get; set; }
        public string FacilitatorId { get; set; }
        public string AssessmentStatus { get; set; }
    }
}