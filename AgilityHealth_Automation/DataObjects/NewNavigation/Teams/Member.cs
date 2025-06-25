using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects.NewNavigation.Teams
{
    public class Member
    {
        public string ImagePath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; }
        public string PreferredLanguage { get; set; }
        public List<string> Role { get; set; }
        public List<string> ParticipantGroup { get; set; }
        public string AllocationType { get; set; }
        public string Numbers { get; set; }
    }
}
