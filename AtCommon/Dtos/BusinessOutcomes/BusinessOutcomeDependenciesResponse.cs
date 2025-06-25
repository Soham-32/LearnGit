using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeDependenciesResponse 
    {
        public string BusinessOutcomeDependencyId { get; set; }
        public int Status { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public DateTime? DueBy { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public Guid DependencyCardId { get; set; }
        public BusinessOutcomeResponse BusinessOutcome { get; set; }
       public BusinessOutcomeDependencyCardResponse DependencyCard { get; set; }
    }


}