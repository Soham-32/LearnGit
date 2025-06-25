
namespace AgilityHealth_Automation.DataObjects
{
    public class CompanyAdminInfo : AhUser
    {
        public string PreferredLanguage { get; set; }
        public bool FeatureAdmin { get; set; }
        public string ImagePath { get; set; }
        public bool ActiveAhf { get; set; }
        public bool AhTrainer { get; set; }
    }
}
