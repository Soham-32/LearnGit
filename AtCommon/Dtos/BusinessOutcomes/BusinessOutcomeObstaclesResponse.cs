using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeObstaclesResponse
    {
        public BusinessOutcomeObstaclesResponse()
        {
           ObstacleOwners = new List<BusinessOutcomeObstacleOwnerResponse>();
        }

        public int BusinessOutcomeObstacleId { get; set; }
        public int Status { get; set; }
        public int SortOrder { get; set; }
  
        public string Title { get; set; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public int ObstacleType { get; set; }
        public int Roam { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public BusinessOutcomeResponse BusinessOutcome { get; set; }
        public ICollection<BusinessOutcomeObstacleOwnerResponse> ObstacleOwners { get; set; }
    }


}