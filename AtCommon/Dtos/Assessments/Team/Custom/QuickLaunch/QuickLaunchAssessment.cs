namespace AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch
{
    public class QuickLaunchAssessment
    {
        public string RadarName { get; set; }
        public string ExistingTeamName { get; set; }
        public bool CreateNewTeam { get; set; }
        public string NewTeamName { get; set; }
    }
}