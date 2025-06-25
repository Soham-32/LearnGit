using System.Collections.Generic;
namespace AtCommon.ObjectFactories.NewNavigation
{
    public static class TeamDashboardFactory
    {
        public static List<string> GetTeamsDashboardColumnHeaderList()
        {
            return new List<string>{
                "Team Name",
                "Work Type",
                "No of Team Assessments",
                "No of Sub-Teams",
                "Last Date of Assessment",
                "No of Team Members",
                "Date Deleted",
                "Team Tags",
                "External ID",
                "Type",
                "Sub-Teams",
                "Multi Teams",
                "Enterprise Teams"
            };
        }
    }
}