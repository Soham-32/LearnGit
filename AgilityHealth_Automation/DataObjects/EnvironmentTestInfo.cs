using System.Collections.Generic;

namespace AgilityHealth_Automation.DataObjects
{
    public class EnvironmentTestInfo
    {
        public List<Environment> Environments { get; set; }
    }

    public class EnterpriseTeam
    {
        public int TeamId { get; set; }
        public int RadarId { get; set; }
        public string PulseName { get; set; }
    }

    public class Environment
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public GoiTeam GoiTeam { get; set; }
        public EnterpriseTeam EnterpriseTeam { get; set; }
        public MultiTeam MultiTeam { get; set; }
        public Team Team { get; set; }
    }

    public class GoiTeam
    {
        public int TeamId { get; set; }
        public int ParticipantRadarId { get; set; }
        public int RollupRadarId { get; set; }
        public string AssessmentUid { get; set; }
    }

    public class MultiTeam
    {
        public int TeamId { get; set; }
        public int RadarId { get; set; }
        public string PulseName { get; set; }
    }

    public class Team
    {
        public string PulseName { get; set; }
        public int PulseTeamId { get; set; }
        public int TeamId { get; set; }
        public string AssessmentUid { get; set; }
        public int RadarId { get; set; }
        public int TeamAssessmentId { get; set; }
        public int PulseToggleId { get; set; }
    }
}
