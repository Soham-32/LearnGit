using System.Collections.Generic;
using System;

namespace AtCommon.Dtos.BusinessOutcomes
{

    public class BusinessOutcomeDependencyCardResponse
    {
        public BusinessOutcomeDependencyCardResponse()
        {
            Owners = new HashSet<BusinessOutcomeOwnerResponse>();
        }

        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public double OverallProgress { get; set; }
        public IEnumerable<BusinessOutcomeOwnerResponse> Owners { get; set; }
    }
}


