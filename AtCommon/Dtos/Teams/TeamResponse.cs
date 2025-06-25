using System.Collections.Generic;
using Newtonsoft.Json;

namespace AtCommon.Dtos.Teams
{
    public class TeamResponse : BaseDto
    {
        public TeamResponse()
        {
            Members = new List<MemberResponse>();
            Stakeholders = new List<StakeholderResponse>();
            Subteams = new List<MultiTeamResponse>();
        }
        public string Name { get; set; }
        [JsonIgnore]
        public int TeamId { get; set; }
        [JsonIgnore]
        public int ParentId { get; set; }
        public string Type { get; set; }
        public List<MultiTeamResponse> Subteams { get; set; }
        public List<MemberResponse> Members { get; set; }
        public List<StakeholderResponse> Stakeholders { get; set; }
    }

}