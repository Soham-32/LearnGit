using System.Collections.Generic;

namespace AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraCloudIntegration
{
    public class JiraCloudIntegration
    {
        public string JiraBoard { get; set; }
        public string JiraBoardForGiSync { get; set; }
        public string JiraProjectName { get; set; }
        public string JiraProjectForGiSync { get; set; }
        public string DynamicJiraBoard { get; set; }
        public Iteration IterationData { get; set; }
        public Release ReleaseData { get; set; }
        public PerformanceMeasurements PerformanceMeasurementsData { get; set; }

        public List<JiraCloudCredential> JiraCloudCredentials { get; set; }

    }



    public class JiraCloudCredential
    {
        public string InstanceName { get; set; }
        public string ApiToken { get; set; }
        public string Email { get; set; }
        public string ServerUrl { get; set; }
    }


}