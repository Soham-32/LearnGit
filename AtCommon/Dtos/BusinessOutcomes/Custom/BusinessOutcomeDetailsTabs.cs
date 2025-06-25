using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class BusinessOutcomeDetailsTabs
    {
        public string Description { get; set; }
        public string Formula { get; set; }
        public string Source { get; set; }
        public string Frequency { get; set; }
        public string Direction { get; set; }
        public string Comment { get; set; }
        public string FinalTarget { get; set; }
        public string EndDate { get; set; }

        public List<SubTargetDto> SubTargets { get; set; } = new List<SubTargetDto>();
    }

    public class SubTargetDto
    {
        public string SubTargetValue { get; set; }
        public string ByWhenDate { get; set; }
    }
}
