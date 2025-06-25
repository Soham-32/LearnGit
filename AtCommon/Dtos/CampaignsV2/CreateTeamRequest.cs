using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
     public class CreateTeamRequest
    {
        public string SearchTeam { get; set; }
        public string SearchWorkType { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<int> TeamIds { get; set; }
        public int ParentTeamId { get; set; }
        public List<int> ExcludeTeamIds { get; set; }
        public string OrderByColumn { get; set; }
        public string OrderByDirection { get; set; }
        public bool IsAhf { get; set; }
        public string SearchTag { get; set; }
    }
}
