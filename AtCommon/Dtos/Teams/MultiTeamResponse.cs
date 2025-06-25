using System;

namespace AtCommon.Dtos.Teams
{
    public class MultiTeamResponse
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Guid Uid { get; set; }
    }
}