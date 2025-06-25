using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects.NewNavigation.Teams
{
    public class Team
    {
        public string TeamName { get; set; }
        public string WorkType { get; set; }
        public string PreferredLanguage { get; set; }
        public string Methodology { get; set; }
        public string DepartmentAndGroup { get; set; }
        public string ExternalIdentifier { get; set; }
        public string DateEstablished { get; set; }
        public string TeamBioOrBackground { get; set; }
        public List<KeyValuePair<string, string>> Tags { get; set; }
        public string ImagePath { get; set; }

    }
}
