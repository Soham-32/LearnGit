namespace AtCommon.Dtos.Companies
{
    public class CurrentUsageResponse
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfTeams { get; set; }
        public int NumberOfMultiTeams { get; set; }
        public int NumberOfEnterpriseTeams { get; set; }
        public int NumberOfTeamAssessments { get; set; }
        public int NumberOfIndividualAssessments { get; set; }
        public string CompanyType { get; set; }
    }
}
