using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.Team.Custom
{
    public class TeamAssessmentInfo
    {
        public TeamAssessmentInfo()
        {
            TeamMembers = new List<string>();
            StakeHolders = new List<string>();
        }

        public string AssessmentName { get; set; }
        public string AssessmentType { get; set; }
        public string Facilitator { get; set; }
        public string FacilitatorEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FacilitationDate { get; set; }
        public int FacilitationDuration { get; set; }
        public DateTime LeadershipReadOutDate { get; set; }
        public string Location { get; set; }
        public string Campaign { get; set; }
        public bool FindFacilitator { get; set; }
        public DateTime EndDate { get; set; }
        public IList<string> TeamMembers { get; set; }
        public IList<string> StakeHolders { get; set; }
        public string TeamName { get; set; }
        public bool SendRetroSurvey { get; set; }
        public string SendRetroSurveyOption { get; set; }
        
    }
}
