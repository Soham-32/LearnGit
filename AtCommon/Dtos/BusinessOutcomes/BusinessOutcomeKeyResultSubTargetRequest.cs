using System;


namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeKeyResultSubTargetRequest
    {
        public int Id { get; set; }
        public Guid BusinessOutcomeKeyResultId { get; set; }
        public string SubTargetValue { get; set; }
        public DateTime? SubTargetAsOfDate { get; set; }
    }
}
