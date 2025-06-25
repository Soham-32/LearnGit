using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments
{
    public class AssessmentResponse : BaseDto
    {
        public class Participant
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public bool Completed { get; set; }
            public List<string> TeamMemberTags { get; set; }
            public List<string> StakeHolderTags { get; set; }
        }

        public string AssessmentName { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? FacilitationDate { get; set; }
        public bool Active { get; set; }
        public string SurveyType { get; set; }
        public string SurveyName { get; set; }
        public string Maturity { get; set; }
        public List<Participant> Participants { get; set; }
        public int AssessmentId { get; set; }
        public int SurveyId { get; set; }
    }
}
