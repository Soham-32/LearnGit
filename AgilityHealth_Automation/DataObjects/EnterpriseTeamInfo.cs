using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects
{
    public class EnterpriseTeamInfo
    {
        public string TeamName { get; set; }
        public string TeamType { get; set; }
        public string ExternalIdentifier { get; set; }
        public string Department { get; set; }
        public DateTime DateEstablished { get; set; }
        public DateTime AgileAdoptionDate { get; set; }
        public string Description { get; set; }
        public string TeamBio { get; set; }
        public string ImagePath { get; set; }
        public IList<string> SubTeams { get; set; }
        public IList<TeamMemberInfo> TeamMembers { get; set; }
        public IList<StakeHolderInfo> StakeHolders { get; set; }
    }
}
