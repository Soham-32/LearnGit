using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects
{
    public class PartnerAdminInfo : AhUser
    {
        public PartnerAdminInfo()
        {
            Skills = new List<string>();
            Certifications = new List<string>();
            Companies = new List<string>();
        }
        public string PreferredLanguage { get; set; }
        public string ImagePath { get; set; }
        public List<string> Skills { get; set; }
        public string LinkedIn { get; set; }
        public List<string> Certifications { get; set; }
        public bool ActiveAhf { get; set; }
        public List<string> Companies { get; set; }
        public bool AhTrainer { get; set; }

    }
}