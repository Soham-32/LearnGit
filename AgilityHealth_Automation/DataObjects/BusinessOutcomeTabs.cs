using Newtonsoft.Json;
using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects
{
    public class BusinessOutcome
    {
        [JsonProperty("Business Outcomes")]
        public List<string> BusinessOutcomes { get; set; }
        public List<string> Initiatives { get; set; }

        public List<string> Projects { get; set; }

        public List<string> Deliverables { get; set; }
        public List<string> Stories { get; set; }
    }

    public class BusinessOutcomeTabs
    {
        public BusinessOutcome BusinessOutcome { get; set; }
    }
}