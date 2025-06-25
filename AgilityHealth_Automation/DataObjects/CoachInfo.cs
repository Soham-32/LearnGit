
namespace AgilityHealth_Automation.DataObjects
{
    public class CoachInfo : AhUser
    {
        public string PreferredLanguage { get; set; }
        public string LinkedIn { get; set; }
        public string Bio { get; set; }
        public string ImagePath { get; set; }
        public bool ActiveAhf { get; set; }
        public bool AhTrainer { get; set; }
    }
}
