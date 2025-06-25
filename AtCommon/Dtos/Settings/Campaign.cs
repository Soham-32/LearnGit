using System;

namespace AtCommon.Dtos.Settings
{
    public class Campaign
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime SatrtDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}