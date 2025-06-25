namespace AtCommon.Dtos.Analytics
{
    public class IndexDimensionsResponse
    {
        public string Dimension { get; set; }
        public decimal ResultPercentage { get; set; }
        public decimal? BenchmarkResultPercentage { get; set; }
    }
}