using System.Collections.Generic;


namespace AtCommon.Dtos.CampaignsV2
{
    public class UpdateAssessmentNameRequest
    {
        public string AssessmentName { get; set; }
        public List<int> TeamIds { get; set; }
    }
}
