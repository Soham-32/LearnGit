using AtCommon.Dtos;

namespace AgilityHealth_Automation.TestDataGenerator
{
    public class BulkDataDetailsDto
    {
        public int NoOfTeams { get; set; }
        public int NoOfMt { get; set; }
        public int NoOfEt { get; set; }
        public int TeamStartingIndexNumber { get; set; }
        public int MtStartingIndexNumber { get; set; }
        public int EtStartingIndexNumber { get; set; }
        public int NoOfTeamMembersPerTeam { get; set; }
        public User User { get; set; }

        //properties for prefix
        public string TeamPrefix { get; set; }
        public string MultiTeamPrefix { get; set; } 
        public string EnterpriseTeamPrefix { get; set; }

    }
}
