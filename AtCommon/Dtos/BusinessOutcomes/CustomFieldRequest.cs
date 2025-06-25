using System;
using Newtonsoft.Json;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class CustomFieldRequest
    {
        
        public Guid Uid { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public int CompanyId { get; set; }
        [JsonIgnore]
        public int Order { get; set; }
    }
}