
namespace AgilityHealth_Automation.DataObjects
{
    public class BlAdminInfo : AhUser
    {
        public string PreferredLanguage { get; set; }
        public string Team { get; set; }
        public string Tag { get; set; }
        public string ImagePath { get; set; }
        public bool ActiveAhf { get; set; }
        public bool AhTrainer { get; set; }
    }
}
