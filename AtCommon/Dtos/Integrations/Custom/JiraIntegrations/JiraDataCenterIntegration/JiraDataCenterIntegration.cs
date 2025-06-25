using System.Collections.Generic;

namespace AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraDataCenterIntegration
{
    public class JiraDataCenterIntegration
    {
        public string JiraBoard { get; set; }
        public string JiraProjectName { get; set; }
        public string DynamicJiraBoard { get; set; }
        public Iteration IterationData { get; set; }
        public Release ReleaseData { get; set; }
        public PerformanceMeasurements PerformanceMeasurementsData { get; set; }
        public List<JiraDataCenterCredential> JiraDataCenterCredentials { get; set; }

    }

    public class JiraDataCenterCredential
    {
        public string InstanceName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ServerUrl { get; set; }

    }
}