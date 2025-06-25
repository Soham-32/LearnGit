using System;
using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class CustomFieldValueRequest
    {
        [JsonProperty("CustomFieldUid")]
        public Guid CustomFieldUid { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }

        [JsonIgnore]
        public int CompanyId { get; set; }

        public string Name { get; set; }

    }
}