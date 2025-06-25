using System.Collections.Generic;
using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeObstaclesRequest
    {
        public BusinessOutcomeObstaclesRequest()
        {
           ObstacleOwners = new HashSet<BusinessOutcomeObstacleOwnerRequest>();
        }
        public int BusinessOutcomeObstacleId { get; set; }
        public int Status { get; set; }
        public int SortOrder { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public int ObstacleType { get; set; }
        public int? Roam { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid BusinessOutcomeId { get; set; }

        public IEnumerable<BusinessOutcomeObstacleOwnerRequest> ObstacleOwners { get; set; }
    }
    public class BusinessOutcomeObstacleOwnerRequest
    {
        public int BusinessOutcomeObstacleOwnerId { get; set; }
        public string UserId { get; set; }
    }

}
