using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class UpdateTeamRequest
    { 
            public string Name { get; set; }
            public string Description { get; set; }
            public string ExternalIdentifier { get; set; }
            public string Department { get; set; }
            public DateTime FormationDate { get; set; }
            public DateTime AgileAdoptionDate { get; set; }
            public string Bio { get; set; }
            public List<TeamTagRequest> TeamTags { get; set; }
    }
}
