namespace AtCommon.Dtos.Reports
{
    public class MostSelectedRecommendationResponse
    {
        public int CompanyId { get; set; }
        public string Competency { get; set; }
        public string Recommendation { get; set; }
        public int RecommendationCount { get; set; }
    }
}
