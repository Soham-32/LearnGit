using System.Collections.Generic;
namespace AtCommon.ObjectFactories
{
    public static class DashboardFactory
    {
        public static List<string> GetAssessmentDashboardAssessmentListColumnHeaderList()
        {
            return new List<string>
            {
                "Team Name",
                "Work Type",
                "Assessment Name",
                "Location Name",
                "Start Date",
                "Facilitation Date",
                "End Date",
                "Assessment Type",
                "Responses",
                "Team Members Invited",
                "Team Members Completed",
                "Stakeholders Invited",
                "Stakeholders Completed",
                "Facilitator(s)",
                "Team Admin(s)",
                "Participants",
                "Reviewers",
                "Share Results"
            };
        }
        public static List<string> GetAssessmentDashboardBatchesColumnHeaderList()
        {
            return new List<string>{
                "Batch Name",
                "Assessment Name",
                "Assessment Date", 
                "Assessment Type",
                "# of Assessments",
                "Facilitator(s)",
                "Created By",
                "Status"
            };
        }
        public static List<string> GetTeamsDashboardColumnHeaderList()
        {
            return new List<string>{
                "Team Name",
                "Work Type",
                "Number of Team Assessments",
                "Number of Sub Teams",
                "Last Date of Assessment",
                "Number of Team Members",
                "Date Deleted",
                "Team Tags",
                "External ID",
                "Type",
                "Sub Teams",
                "Multi Teams",
                "Enterprise Teams"
            };
        }
        public static List<string> GetGrowthItemDashboardColumnHeaderList()
        {
            var giColumnList = GrowthPlanFactory.GetGrowthPlanColumnNameList();
            giColumnList.Add("External ID");
            //giColumnList.Add("Dimension");
            //giColumnList.Add("Sub-Dimension");
            giColumnList.Add("Target Date");
            giColumnList.Add("Solution");

            return giColumnList;
        }
    }
}