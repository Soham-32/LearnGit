using Newtonsoft.Json;


namespace AtCommon.Dtos.Reports
{
    public class ReportsById
        {
            [JsonProperty("Team ID")]
            public int TeamId { get; set; }

            [JsonProperty("Team Name")]
            public string TeamName { get; set; }

            [JsonProperty("Company ID")]
            public int CompanyId { get; set; }

            [JsonProperty("Work Type")]
            public string WorkType { get; set; }
            public object Methodology { get; set; }

            [JsonProperty("Number of Assessments")]
            public int NumberOfAssessments { get; set; }

            [JsonProperty("% Completed Of Latest Assessment")]
            public object CompletedOfLatestAssessment { get; set; }

            [JsonProperty("Facilitation Date")]
            public object FacilitationDate { get; set; }

            [JsonProperty("Baseline Date")]
            public string BaselineDate { get; set; }

            [JsonProperty("Date of Last Assessment")]
            public string DateOfLastAssessment { get; set; }

            [JsonProperty("Assessment Type")]
            public string AssessmentType { get; set; }
            public string Status { get; set; }
            public object Tower { get; set; }
            public object Train { get; set; }
            public object Programs { get; set; }

            [JsonProperty("Business Lines")]
            public object BusinessLines { get; set; }

            [JsonProperty("Agile Adoption")]
            public object AgileAdoption { get; set; }

            [JsonProperty("Agile Adoption Date")]
            public object AgileAdoptionDate { get; set; }

            [JsonProperty("Team Formation")]
            public object TeamFormation { get; set; }

            [JsonProperty("Date Established")]
            public string DateEstablished { get; set; }

            [JsonProperty("Number of Team Members")]
            public int NumberOfTeamMembers { get; set; }

            [JsonProperty("Number of Stakeholders")]
            public int NumberOfStakeholders { get; set; }
            public object MultiTeams { get; set; }

            [JsonProperty("Scrum Master Emails")]
            public object ScrumMasterEmails { get; set; }

            [JsonProperty("Scrum Master Names")]
            public object ScrumMasterNames { get; set; }

            [JsonProperty("External ID")]
            public object ExternalId { get; set; }

            [JsonProperty("Product Owner Emails")]
            public object ProductOwnerEmails { get; set; }

            [JsonProperty("Product Owner Names")]
            public object ProductOwnerNames { get; set; }

            [JsonProperty("RTE Emails")]
            public object RteEmails { get; set; }

            [JsonProperty("RTE Names")]
            public object RteNames { get; set; }
            public object Maturity { get; set; }
        }

}

