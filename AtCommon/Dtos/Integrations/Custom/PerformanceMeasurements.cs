namespace AtCommon.Dtos.Integrations.Custom
{
    public class PerformanceMeasurements
    {
        public string CalculatedFeatureThroughput { get; set; }
        public string CalculatedFeatureCycleTime { get; set; }
        public string CalculatedDeploymentFrequency { get; set; }
        public string CalculatedDefectRatio { get; set; }
        public string CalculatedPredictability { get; set; }
        public string NormalizedFeatureThroughput { get; set; }
        public string NormalizedFeatureCycleTime { get; set; }
        public string NormalizedDeploymentFrequency { get; set; }
        public string NormalizedDefectRatio { get; set; }
        public string NormalizedPredictability { get; set; }
        public string RecalculatedDefectRatio { get; set; }
        public string RecalculatedPredictability { get; set; }
    }
}
