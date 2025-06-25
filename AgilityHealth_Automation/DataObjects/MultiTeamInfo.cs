using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects
{
    public class MultiTeamInfo
    {
        public string TeamName { get; set; }
        public string TeamType { get; set; }
        public string AssessmentType { get; set; }
        public string Department { get; set; }
        public string DateEstablished { get; set; }
        public string AgileAdoptionDate { get; set; }
        public string Description { get; set; }
        public string TeamBio { get; set; }
        public string ImagePath { get; set; }
        public IList<string> SubTeams { get; set; }
        public IList<string> TeamMembers { get; set; }
        public IList<string> StakeHolders { get; set; }
    }
}
