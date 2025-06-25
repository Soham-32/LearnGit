using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects
{
    public class Reviewee
    {
        public Reviewee()
        {
            Reviewers = new List<Reviewer>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string Email { get; set; }
        public string SurveyType { get; set; }
        public List<Reviewer> Reviewers { get; set; }
    }
}
