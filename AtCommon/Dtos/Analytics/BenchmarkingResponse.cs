namespace AtCommon.Dtos.Analytics
{
    public class BenchmarkingResponse
    {
        public int? BenchmarkAssessmentCount { get; set; }
        public int AssessmentCount { get; set; }
        public decimal? BenchmarkResultPercentage { get; set; }
        public decimal ResultPercentage { get; set; }
    }
}