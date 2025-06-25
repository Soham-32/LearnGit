using System;
using System.Collections.Generic;
using AtCommon.Dtos.Teams;

namespace AtCommon.Dtos.Bulk
{
    public class AddTeam
    {
        public AddTeam()
        {
            Tags = new List<TeamTagRequest>();
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ExternalIdentifier { get; set; }
        public string Department { get; set; }
        public DateTime? FormationDate { get; set; }
        public DateTime? AgileAdoptionDate { get; set; }
        public string Bio { get; set; }
        public IList<TeamTagRequest> Tags { get; set; }
        public IList<string> ParentExternalIdentifiers { get; set; }

    }
}