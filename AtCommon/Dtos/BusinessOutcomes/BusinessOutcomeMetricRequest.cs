namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeMetricRequest
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public int? TeamId { get; set; }
        public int Order { get; set; }
        public int CompanyId { get; set; }
    }


    public enum MetricType
    {
        Percent = 1,
        Number = 2,
        Money = 3
    }
}