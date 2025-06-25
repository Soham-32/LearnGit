using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class GetAllCampaignTeamIdsRequest
    {
        public string SearchTeam { get; set; }
        public string SearchTag { get; set; }
        public string SearchWorkType { get; set; }
        public int ParentTeamId { get; set; }
        public List<int> ExcludeTeamIds { get; set; }
        public bool IsAhf { get; set; }
        public List<int> TeamIds { get; set; }
    }
}
