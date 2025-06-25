using System.Collections.Generic;
using AtCommon.Dtos.Reports;

namespace AtCommon.ObjectFactories
{
    public class ReportsFactory
    {
        
        public static List<ReportListResponse> GetReportList()
        {
            var requestBody = new List<ReportListResponse>
            {
                new ReportListResponse
                {
                    Id = 11,
                    Name = "Anthem In Progress"
                },

                new ReportListResponse
                {
                    Id = 15,
                    Name = "Company Teams"
                },
                new ReportListResponse
                {
                    Id = 3,
                    Name = "Future Assessments"
                },
                new ReportListResponse
                {
                    Id = 9,
                    Name = "Overdue Teams"
                },
                new ReportListResponse
                {
                    Id = 5,
                    Name = "Recent Assessments"
                },
                new ReportListResponse
                {
                    Id = 1,
                    Name = "Team"
                },
                new ReportListResponse
                {
                    Id = 4,
                    Name = "Team With Members"
                },
                new ReportListResponse
                {
                    Id = 14,
                    Name = "Work Type Tags - Stabo Imports"
                },
            };

            return requestBody;
        }

        public static ReportsById GetReportById()
        {
            return new ReportsById
            {
                TeamId = 7130,
                TeamName = "Automation Radar Team",
                CompanyId = 79,
                ExternalId = "AH_Team_7130"
            };
        }
    }
}
