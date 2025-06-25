using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class AddTeamWithMemberRequest
    {
        public AddTeamWithMemberRequest()
        {
            Type = "Team";
            Tags = new List<TeamTagRequest>();
            Members = new List<AddMemberRequest>();
            Stakeholders = new List<AddStakeholderRequest>();
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ExternalIdentifier { get; set; }
        public string Department { get; set; }
        public DateTime FormationDate { get; set; }
        public DateTime AgileAdoptionDate { get; set; }
        public string Bio { get; set; }
        public List<TeamTagRequest> Tags { get; set; }
        public List<AddMemberRequest> Members { get; set; }
        public List<AddStakeholderRequest> Stakeholders { get; set; }
    }
}
