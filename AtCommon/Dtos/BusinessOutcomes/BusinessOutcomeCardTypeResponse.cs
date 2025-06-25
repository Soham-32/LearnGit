namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeCardTypeResponse
    {
        public int BusinessOutcomeCardTypeId { get; set; }
        public int? ParentBusinessOutcomeCardTypeId { get; set; }
        public string Name { get; set; }
        public string MasterName { get; set; }
        public bool IsActive { get; set; }
    }
}