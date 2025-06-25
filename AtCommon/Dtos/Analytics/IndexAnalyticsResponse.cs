namespace AtCommon.Dtos.Analytics
{
    public class IndexAnalyticsResponse
    {
        public string IndexType { get; set; }
        public decimal ResultPercentage { get; set; }
        public int TeamsCount { get; set; }
        public int PreCrawl { get; set; }
        public int Crawl { get; set; }
        public int Walk { get; set; }
        public int Run { get; set; }
        public int Fly { get; set; }
    }
}
