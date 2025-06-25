using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeDependenciesRequest
    {
        public int BusinessOutcomeDependencyId { get; set; }
        public int Status { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public DateTime? DueBy { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public Guid DependencyCardId { get; set; }
        //public BusinessOutcome BusinessOutcome { get; set; }
        //public DependencyCard DependencyCard { get; set; }
    }


}
