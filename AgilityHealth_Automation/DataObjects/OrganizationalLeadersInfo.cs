
namespace AgilityHealth_Automation.DataObjects
{
    public class OrganizationalLeadersInfo : AhUser
    {
        public string PreferredLanguage { get; set; }
        public bool CanSeeTeamName { get; set; }
        public bool CanViewSubteams { get; set; }
        public string Team { get; set; }
        public string ImagePath { get; set; }
        public bool ActiveAhf { get; set; }
        public bool AhTrainer { get; set; }
    }
}
