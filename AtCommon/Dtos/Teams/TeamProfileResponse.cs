using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TeamProfileResponse : BaseDto
    {
        public int CompanyId { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExternalIdentifier { get; set; }
        public string Department { get; set; }
        public DateTime? FormationDate { get; set; }
        public DateTime? AgileAdoptionDate { get; set; }
        public string Bio { get; set; }
        public string Type { get; set; }
        public int TeamArchiveStatusId { get; set; }
        public string TeamArchiveStatus { get; set; }
        public List<TeamTagResponse> TeamTags { get; set; }
        public List<Guid> Subteams { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
