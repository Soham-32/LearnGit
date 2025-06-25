using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeLabelRequest
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }
}