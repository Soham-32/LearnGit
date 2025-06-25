using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class TeamInfoResponse
    {
        public List<TeamInfo> TeamInfo { get; set; }
        public Paging Paging { get; set; }
    }
    
    public class TeamInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WorkType { get; set; }
        public string TeamTags { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public bool IsFacilitator { get; set; }
        public bool IsAhf { get; set; }
        public DateTime CreateDate { get; set; }
        public int? TeamMembersCount { get; set; }
        
    }
}
