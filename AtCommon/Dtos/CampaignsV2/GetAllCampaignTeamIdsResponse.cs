using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class GetAllCampaignTeamIdsResponse
    {
        public List<int> TeamIds { get; set; }

    }

    public class TeamIdsErrorMessage
    {
        public List<string> ParentTeamId { get; set; }

        [JsonProperty("teamIds[0]")]
        public List<string> TeamIds0 { get; set; }

        [JsonProperty("excludeTeamIds[0]")]
        public List<string> ExcludeTeamIds0 { get; set; }
    }
}
